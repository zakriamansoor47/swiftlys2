#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeFileSystem
{
    private static int _MainThreadID;

    private unsafe static delegate* unmanaged<byte*, byte*, int, int, int> _GetSearchPath;

    public unsafe static string GetSearchPath(string pathId, int searchPathType, int searchPathsToGet)
    {
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* pathIdBufferPtr = pathIdBuffer)
        {
            var ret = _GetSearchPath(null, pathIdBufferPtr, searchPathType, searchPathsToGet);
            var retBuffer = new byte[ret + 1];
            fixed (byte* retBufferPtr = retBuffer)
            {
                ret = _GetSearchPath(retBufferPtr, pathIdBufferPtr, searchPathType, searchPathsToGet);
                return Encoding.UTF8.GetString(retBufferPtr, ret);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, int, int, void> _AddSearchPath;

    public unsafe static void AddSearchPath(string path, string pathId, int searchPathAdd, int searchPathPriority)
    {
        byte[] pathBuffer = Encoding.UTF8.GetBytes(path + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* pathBufferPtr = pathBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                _AddSearchPath(pathBufferPtr, pathIdBufferPtr, searchPathAdd, searchPathPriority);
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte> _RemoveSearchPath;

    public unsafe static bool RemoveSearchPath(string path, string pathId)
    {
        byte[] pathBuffer = Encoding.UTF8.GetBytes(path + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* pathBufferPtr = pathBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _RemoveSearchPath(pathBufferPtr, pathIdBufferPtr);
                return ret == 1;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte> _FileExists;

    public unsafe static bool FileExists(string fileName, string pathId)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _FileExists(fileNameBufferPtr, pathIdBufferPtr);
                return ret == 1;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte> _IsDirectory;

    public unsafe static bool IsDirectory(string path, string pathId)
    {
        byte[] pathBuffer = Encoding.UTF8.GetBytes(path + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* pathBufferPtr = pathBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _IsDirectory(pathBufferPtr, pathIdBufferPtr);
                return ret == 1;
            }
        }
    }

    private unsafe static delegate* unmanaged<void> _PrintSearchPaths;

    public unsafe static void PrintSearchPaths()
    {
        _PrintSearchPaths();
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte*, int> _ReadFile;

    public unsafe static string ReadFile(string fileName, string pathId)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _ReadFile(null, fileNameBufferPtr, pathIdBufferPtr);
                var retBuffer = new byte[ret + 1];
                fixed (byte* retBufferPtr = retBuffer)
                {
                    ret = _ReadFile(retBufferPtr, fileNameBufferPtr, pathIdBufferPtr);
                    return Encoding.UTF8.GetString(retBufferPtr, ret);
                }
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte*, byte> _WriteFile;

    public unsafe static bool WriteFile(string fileName, string pathId, string content)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        byte[] contentBuffer = Encoding.UTF8.GetBytes(content + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                fixed (byte* contentBufferPtr = contentBuffer)
                {
                    var ret = _WriteFile(fileNameBufferPtr, pathIdBufferPtr, contentBufferPtr);
                    return ret == 1;
                }
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, uint> _GetFileSize;

    public unsafe static uint GetFileSize(string fileName, string pathId)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _GetFileSize(fileNameBufferPtr, pathIdBufferPtr);
                return ret;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte> _PrecacheFile;

    public unsafe static bool PrecacheFile(string fileName, string pathId)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _PrecacheFile(fileNameBufferPtr, pathIdBufferPtr);
                return ret == 1;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte> _IsFileWritable;

    public unsafe static bool IsFileWritable(string fileName, string pathId)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _IsFileWritable(fileNameBufferPtr, pathIdBufferPtr);
                return ret == 1;
            }
        }
    }

    private unsafe static delegate* unmanaged<byte*, byte*, byte, byte> _SetFileWritable;

    public unsafe static bool SetFileWritable(string fileName, string pathId, bool writable)
    {
        byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName + "\0");
        byte[] pathIdBuffer = Encoding.UTF8.GetBytes(pathId + "\0");
        fixed (byte* fileNameBufferPtr = fileNameBuffer)
        {
            fixed (byte* pathIdBufferPtr = pathIdBuffer)
            {
                var ret = _SetFileWritable(fileNameBufferPtr, pathIdBufferPtr, writable ? (byte)1 : (byte)0);
                return ret == 1;
            }
        }
    }
}