import os
from pathlib import Path

PARAM_TYPE_MAP = {
    "int16": "short",
    "uint16": "ushort", 
    "int32": "int",
    "float": "float",
    "double": "double",
    "bool": "bool",
    "byte": "byte",
    "int64": "long",
    "uint32": "uint",
    "uint64": "ulong",
    "ptr": "nint",
    "string": "string",
    "void": "void",
    "vector2": "Vector2D",
    "vector": "Vector",
    "vector4": "Vector4D", 
    "qangle": "QAngle",
    "color": "Color",
    "bytes": "byte[]",
    "cutlstringtoken": "CUtlStringToken"
}

DELEGATE_PARAM_TYPE_MAP = {
    **PARAM_TYPE_MAP,
    "string": "byte*",
    "bytes": "byte*",
    "bool": "byte",
}

DELEGATE_RETURN_TYPE_MAP = {
    **PARAM_TYPE_MAP,
    "string": "int",
    "bytes": "int",
    "bool": "byte",
}

RETURN_TYPE_MAP = {
    **PARAM_TYPE_MAP,
    "string": "string",
    "bytes": "byte[]",
}

class CodeWriter:
    def __init__(self):
        self.lines = []
        self.indent_level = 0
    
    def add_line(self, text: str = ""):
        if text.strip():
            self.lines.append("    " * self.indent_level + text)
        else:
            self.lines.append("")
    
    def indent(self):
        self.indent_level += 1
    
    def dedent(self):
        self.indent_level = max(0, self.indent_level - 1)
    
    def add_block(self, header: str, content_func):
        self.add_line(header)
        self.add_line("{")
        self.indent()
        content_func()
        self.dedent()
        self.add_line("}")
    
    def get_code(self) -> str:
        return "\r\n".join(self.lines)


def split_by_last_dot(value: str):
    idx = value.rfind(".")
    if idx == -1:
        return "", value
    return value[:idx], value[idx + 1:]

def is_buffer_return(return_type: str) -> bool:
    return return_type in ("string", "bytes")

def parse_native(lines: list[str]):
    namespace_line, *native_lines = lines
    namespace_content = namespace_line.split(" ")[1].strip()
    namespace_prefix, class_name = split_by_last_dot(namespace_content)
    print(class_name)

    out_path = Path("../../managed/src/SwiftlyS2.Generated/Natives/") / f"{class_name}.cs"
    out_path.parent.mkdir(parents=True, exist_ok=True)

    writer = CodeWriter()
    writer.add_line("#pragma warning disable CS0649")
    writer.add_line("#pragma warning disable CS0169")
    writer.add_line()
    writer.add_line("using System.Text;")
    writer.add_line("using System.Threading;")
    writer.add_line("using SwiftlyS2.Shared.Natives;")
    writer.add_line()
    writer.add_line("namespace SwiftlyS2.Core.Natives;")
    writer.add_line()

    def write_class_content():
        writer.add_line("private static int _MainThreadID;")
        
        for raw_line in native_lines:
            if raw_line.strip() == "":
                continue
                
            left, right = raw_line.split("=", 1)
            
            is_marked_sync = False
            if "sync " in left:
                is_marked_sync = True
                left = left.replace("sync ", "")
            
            return_type, function_name = left.split(" ", 1)
            function_name = function_name.strip()
            params_and_comment = right.split("//", 1)
            native_params_raw = params_and_comment[0] if params_and_comment else ""
            comment = params_and_comment[1] if len(params_and_comment) > 1 else ""
            trimmed_params = native_params_raw.strip()
            native_params_list = [] if trimmed_params in ("", "void") else [p for p in trimmed_params.split(",")]

            native_param_types = []
            for p in native_params_list:
                t = p.strip().split(" ", 1)[0]
                if t == "bytes":
                    native_param_types.extend([DELEGATE_PARAM_TYPE_MAP[t], "int"])
                else:
                    native_param_types.append(DELEGATE_PARAM_TYPE_MAP[t])
            
            param_signatures = []
            for p in native_params_list:
                t, n = p.strip().split(" ", 1)
                param_signatures.append((PARAM_TYPE_MAP[t], n))

            if is_buffer_return(return_type):
                native_param_types_with_buffer = ["byte*"] + native_param_types
            else:
                native_param_types_with_buffer = native_param_types
            
            delegate_generic = ", ".join(native_param_types_with_buffer + [DELEGATE_RETURN_TYPE_MAP[return_type]])
            writer.add_line()
            writer.add_line(f"private unsafe static delegate* unmanaged<{delegate_generic}> _{function_name};")
            writer.add_line()

            if comment and comment.strip():
                writer.add_line("/// <summary>")
                writer.add_line(f"/// {comment.strip()}")
                writer.add_line("/// </summary>")

            method_signature = ", ".join([f"{t} {n}" for t, n in param_signatures])
            
            def write_method_content():
                if is_marked_sync:
                    writer.add_block("if (Thread.CurrentThread.ManagedThreadId != _MainThreadID)", lambda: writer.add_line('throw new InvalidOperationException("This method can only be called from the main thread.");'))

                string_params = [n for t, n in param_signatures if t == "string"]
                bytes_params = [n for t, n in param_signatures if t == "byte[]"]

                for param in bytes_params:
                    writer.add_line(f"var {param}Length = {param}.Length;")

                if not string_params:
                    fixed_blocks = []
                    for param in bytes_params:
                        fixed_blocks.append(f"fixed (byte* {param}BufferPtr = {param})")
                    
                    def write_simple_call():
                        call_args = []
                        for t, n in param_signatures:
                            if t == "byte[]":
                                call_args.extend([f"{n}BufferPtr", f"{n}Length"])
                            elif t == "bool":
                                call_args.append(f"{n} ? (byte)1 : (byte)0")
                            else:
                                call_args.append(n)
                        
                        if is_buffer_return(return_type):
                            first_call_args = ["null"] + call_args
                            writer.add_line(f"var ret = _{function_name}({', '.join(first_call_args)});")
                            if return_type == "string":
                                writer.add_line("var retBuffer = new byte[ret + 1];")
                            else:
                                writer.add_line("var retBuffer = new byte[ret];")
                            
                            def write_ret_fixed():
                                second_call_args = ["retBufferPtr"] + call_args
                                writer.add_line(f"ret = _{function_name}({', '.join(second_call_args)});")
                                if return_type == "string":
                                    writer.add_line("return Encoding.UTF8.GetString(retBufferPtr, ret);")
                                else:
                                    writer.add_line("var retBytes = new byte[ret];")
                                    writer.add_line("for (int i = 0; i < ret; i++) retBytes[i] = retBufferPtr[i];")
                                    writer.add_line("return retBytes;")
                            
                            writer.add_block("fixed (byte* retBufferPtr = retBuffer)", write_ret_fixed)
                        else:
                            if return_type == "void":
                                writer.add_line(f"_{function_name}({', '.join(call_args)});")
                            else:
                                writer.add_line(f"var ret = _{function_name}({', '.join(call_args)});")
                                writer.add_line("return ret == 1;" if return_type == "bool" else "return ret;")
                    
                    def write_with_fixed_blocks(blocks, index=0):
                        if index < len(blocks):
                            writer.add_block(blocks[index], lambda: write_with_fixed_blocks(blocks, index + 1))
                        else:
                            write_simple_call()
                    
                    if fixed_blocks:
                        write_with_fixed_blocks(fixed_blocks)
                    else:
                        write_simple_call()
                    return

                for param in string_params:
                    writer.add_line(f"byte[] {param}Buffer = Encoding.UTF8.GetBytes({param} + \"\\0\");")
                
                fixed_blocks = []
                for param in string_params:
                    fixed_blocks.append(f"fixed (byte* {param}BufferPtr = {param}Buffer)")
                for param in bytes_params:
                    fixed_blocks.append(f"fixed (byte* {param}BufferPtr = {param})")
                
                def write_native_call():
                    call_args = []
                    for t, n in param_signatures:
                        if t == "string":
                            call_args.append(f"{n}BufferPtr")
                        elif t == "byte[]":
                            call_args.extend([f"{n}BufferPtr", f"{n}Length"])
                        elif t == "bool":
                            call_args.append(f"{n} ? (byte)1 : (byte)0")
                        else:
                            call_args.append(n)
                    
                    if is_buffer_return(return_type):
                        first_call_args = ["null"] + call_args
                        writer.add_line(f"var ret = _{function_name}({', '.join(first_call_args)});")
                        if return_type == "string":
                            writer.add_line("var retBuffer = new byte[ret + 1];")
                        else:
                            writer.add_line("var retBuffer = new byte[ret];")
                        
                        def write_ret_fixed():
                            second_call_args = ["retBufferPtr"] + call_args
                            writer.add_line(f"ret = _{function_name}({', '.join(second_call_args)});")
                            if return_type == "string":
                                writer.add_line("return Encoding.UTF8.GetString(retBufferPtr, ret);")
                            else:
                                writer.add_line("var retBytes = new byte[ret];")
                                writer.add_line("for (int i = 0; i < ret; i++) retBytes[i] = retBufferPtr[i];")
                                writer.add_line("return retBytes;")
                        
                        writer.add_block("fixed (byte* retBufferPtr = retBuffer)", write_ret_fixed)
                    else:
                        if return_type == "void":
                            writer.add_line(f"_{function_name}({', '.join(call_args)});")
                        else:
                            writer.add_line(f"var ret = _{function_name}({', '.join(call_args)});")
                            writer.add_line("return ret == 1;" if return_type == "bool" else "return ret;")
                
                def write_with_fixed_blocks(blocks, index=0):
                    if index < len(blocks):
                        writer.add_block(blocks[index], lambda: write_with_fixed_blocks(blocks, index + 1))
                    else:
                        write_native_call()
                
                if fixed_blocks:
                    write_with_fixed_blocks(fixed_blocks)
                else:
                    write_native_call()
            
            writer.add_block(f"public unsafe static {RETURN_TYPE_MAP[return_type]} {function_name}({method_signature})", write_method_content)

    # Benchmark class should be public for external access
    access_modifier = "public" if class_name == "Benchmark" else "internal"
    writer.add_block(f"{access_modifier} static class Native{class_name}", write_class_content)

    with open(out_path, "w", encoding="utf-8", newline="") as f:
        f.write(writer.get_code())

def main():
    out_dir = Path("../../managed/src/SwiftlyS2.Generated/Natives/")
    out_dir.mkdir(parents=True, exist_ok=True)
    definitions_dir = Path("../../natives")
    for file_path in definitions_dir.rglob("*.native"):
        with open(file_path, "r", encoding="utf-8") as f:
            parse_native(f.readlines())

if __name__ == "__main__":
    main()