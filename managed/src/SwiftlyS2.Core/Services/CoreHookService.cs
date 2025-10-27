using System.Runtime.CompilerServices;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Core.Events;
using SwiftlyS2.Core.Extensions;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Memory;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.SchemaDefinitions;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Services;

internal class CoreHookService : IDisposable
{
  private ILogger<CoreHookService> _Logger { get; init; }
  private ISwiftlyCore _Core { get; init; }

  public CoreHookService(ILogger<CoreHookService> logger, ISwiftlyCore core)
  {
    _Logger = logger;
    _Core = core;

    HookCanAcquire();
    HookCommandExecute();
    HookICVarFindConCommand();
  }

  private delegate int CanAcquireDelegate(nint pItemServices, nint pEconItemView, nint acquireMethod, nint unk1);
  /*
    Original function in engine2.dll: __int64 sub_1C0CD0(__int64 a1, int a2, unsigned int a3, ...)
    This is a variadic function, but we only need the first two variable arguments (v55, v57)

    __int64 sub_1C0CD0(__int64 a1, int a2, unsigned int a3, ...)
    {
        ...
        
        va_list va; // [rsp+D28h] [rbp+D28h]
        __int64 v55; // [rsp+E28h] [rbp+D28h] BYREF
        va_list va1; // [rsp+E28h] [rbp+D28h]

        ...

        va_start(va1, a3);
        va_start(va, a3);
        v55 = va_arg(va1, _QWORD);
        v57 = va_arg(va1, _QWORD);

        ...
    }

    So we model it as a fixed 5-parameter function for interop purposes
  */
  private delegate nint ExecuteCommandDelegate(nint a1, int a2, uint a3, nint a4, nint a5);
  private IUnmanagedFunction<ExecuteCommandDelegate>? _ExecuteCommand;
  private Guid _ExecuteCommandGuid;
  private IUnmanagedFunction<CanAcquireDelegate>? _CanAcquire;
  private Guid _CanAcquireGuid;

  private void HookCanAcquire()
  {

    var address = _Core.GameData.GetSignature("CCSPlayer_ItemServices::CanAcquire");

    _Logger.LogInformation("Hooking CCSPlayer_ItemServices::CanAcquire at {Address}", address);

    _CanAcquire = _Core.Memory.GetUnmanagedFunctionByAddress<CanAcquireDelegate>(address);
    _CanAcquireGuid = _CanAcquire.AddHook(next =>
    {

      return (pItemServices, pEconItemView, acquireMethod, unk1) =>
      {
        var result = next()(pItemServices, pEconItemView, acquireMethod, unk1);

        var itemServices = _Core.Memory.ToSchemaClass<CCSPlayer_ItemServices>(pItemServices);
        var econItemView = _Core.Memory.ToSchemaClass<CEconItemView>(pEconItemView);

        var @event = new OnItemServicesCanAcquireHookEvent
        {
          ItemServices = itemServices,
          EconItemView = econItemView,
          AcquireMethod = (AcquireMethod)acquireMethod,
          OriginalResult = (AcquireResult)result
        };

        EventPublisher.InvokeOnCanAcquireHook(@event);

        if (@event.Intercepted)
        {
          // original result is modified here.
          return (int)@event.OriginalResult;
        }

        return result;
      };
    });
  }

  private void HookCommandExecute()
  {

    var address = _Core.GameData.GetSignature("Cmd_ExecuteCommand");

    _Logger.LogInformation("Hooking Cmd_ExecuteCommand at {Address}", address);

    _ExecuteCommand = _Core.Memory.GetUnmanagedFunctionByAddress<ExecuteCommandDelegate>(address);
    _ExecuteCommandGuid = _ExecuteCommand.AddHook((next) =>
    {
      return (a1, a2, a3, a4, a5) =>
      {
        unsafe
        {
          if (a5 != nint.Zero)
          {
            ref var command = ref Unsafe.AsRef<CCommand>((void*)a5);
            
            var @eventPre = new OnCommandExecuteHookEvent(ref command, HookMode.Pre);
            EventPublisher.InvokeOnCommandExecuteHook(@eventPre);

            var result = next()(a1, a2, a3, a4, a5);

            var @eventPost = new OnCommandExecuteHookEvent(ref command, HookMode.Post);
            EventPublisher.InvokeOnCommandExecuteHook(@eventPost);

            return result;
          }
          return next()(a1, a2, a3, a4, a5);
        }
      };
    });
  }

  private delegate nint FindConCommandDelegate(nint pICvar, nint pRet, nint pConCommandName, int unk1);
  private delegate nint FindConCommandDelegateLinux(nint pICvar, nint pConCommandName, int unk1);

  private IUnmanagedFunction<FindConCommandDelegate>? _FindConCommandWindows;
  private IUnmanagedFunction<FindConCommandDelegateLinux>? _FindConCommandLinux;
  private Guid _FindConCommandGuid;

  private void HookICVarFindConCommand()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      var offset = _Core.GameData.GetOffset("ICvar::FindConCommand");
      _FindConCommandLinux = _Core.Memory.GetUnmanagedFunctionByVTable<FindConCommandDelegateLinux>(_Core.Memory.GetVTableAddress("tier0", "CCvar")!.Value, offset);

      _Logger.LogInformation("Hooking ICvar::FindConCommand at {Address}", _FindConCommandLinux.Address);

      _FindConCommandGuid = _FindConCommandLinux.AddHook((next) =>
      {
        return (pICvar, pConCommandName, unk1) =>
        {
          var commandName = Marshal.PtrToStringAnsi(pConCommandName)!;
          if (commandName.StartsWith("^wb^"))
          {
            commandName = commandName.Substring(4);
            var bytes = Encoding.UTF8.GetBytes(commandName);
            unsafe
            {
              var pStr = (nint)NativeMemory.AllocZeroed((nuint)bytes.Length);
              pStr.CopyFrom(bytes);
              var result = next()(pICvar, pStr, unk1);
              NativeMemory.Free((void*)pStr);
              return result;
            }
          }
          return next()(pICvar, pConCommandName, unk1);
        };
      });
    }
    else
    {
      var offset = _Core.GameData.GetOffset("ICvar::FindConCommand");
      _FindConCommandWindows = _Core.Memory.GetUnmanagedFunctionByVTable<FindConCommandDelegate>(_Core.Memory.GetVTableAddress("tier0", "CCvar")!.Value, offset);

      _Logger.LogInformation("Hooking ICvar::FindConCommand at {Address}", _FindConCommandWindows.Address);

      _FindConCommandGuid = _FindConCommandWindows.AddHook((next) =>
      {
        return (pICvar, pRet, pConCommandName, unk1) =>
        {
          var commandName = Marshal.PtrToStringAnsi(pConCommandName)!;
          if (commandName.StartsWith("^wb^"))
          {
            commandName = commandName.Substring(4);
            var bytes = Encoding.UTF8.GetBytes(commandName);
            unsafe
            {
              var pStr = (nint)NativeMemory.AllocZeroed((nuint)bytes.Length);
              pStr.CopyFrom(bytes);
              var result = next()(pICvar, pRet, pStr, unk1);
              NativeMemory.Free((void*)pStr);
              return result;
            }
          }
          return next()(pICvar, pRet, pConCommandName, unk1);
        };
      });
    }
  }

  public void Dispose()
  {
    _CanAcquire!.RemoveHook(_CanAcquireGuid);
    _ExecuteCommand!.RemoveHook(_ExecuteCommandGuid);
  }
}