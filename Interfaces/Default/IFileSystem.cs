using System.Runtime.InteropServices;
using System.Text;

namespace Aperture.Interfaces.Default;

public class IFileSystem : SourceInterface
{
    public enum FilesystemMountRetval
    {
        FILESYSTEM_MOUNT_OK = 0,
        FILESYSTEM_MOUNT_FAILED
    }

    public enum SearchPathAdd
    {
        PATH_ADD_TO_HEAD,
        PATH_ADD_TO_TAIL
    }

    #region Delegates

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate int InitDelegate(IntPtr thisPtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void ShutdownDelegate(IntPtr thisPtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate bool IsSteamDelegate(IntPtr thisPtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate int MountSteamContentDelegate(IntPtr thisPtr, int extraAppID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void AddSearchPathDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string path, [MarshalAs(UnmanagedType.LPStr)] string pathID, int addType);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void RemoveSearchPathDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string path, [MarshalAs(UnmanagedType.LPStr)] string pathID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void RemoveAllSearchPathsDelegate(IntPtr thisPtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void RemoveSearchPathsDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string pathID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void MarkPathIDByRequestOnlyDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string pathID, [MarshalAs(UnmanagedType.I1)] bool requestOnly);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate IntPtr RelativePathToFullPathDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string fileName, [MarshalAs(UnmanagedType.LPStr)] string pathID, StringBuilder dest, int maxLen, int pathFilter, IntPtr pathTypePtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate int GetSearchPathDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string pathID, [MarshalAs(UnmanagedType.I1)] bool getPackFiles, StringBuilder dest, int maxLen);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate bool AddPackFileDelegate([MarshalAs(UnmanagedType.LPStr)] string fullPath, [MarshalAs(UnmanagedType.LPStr)] string pathID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void RemoveFileDelegate([MarshalAs(UnmanagedType.LPStr)] string relativePath, [MarshalAs(UnmanagedType.LPStr)] string pathID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void RenameFileDelegate([MarshalAs(UnmanagedType.LPStr)] string oldPath, [MarshalAs(UnmanagedType.LPStr)] string newPath, [MarshalAs(UnmanagedType.LPStr)] string pathID);


    #endregion

    /// <inheritdoc/>
    public IFileSystem(string interfaceName) : base(interfaceName) { }

    private enum VTableIndex
    {
        Init = 3,
        Shutdown,
        IsSteam,
        MountSteamContent = 6,
        AddSearchPath,
        RemoveSearchPath,
        RemoveAllSearchPaths,
        RemoveSearchPaths,
        MarkPathIDByRequestOnly,
        RelativePathToFullPath,
        GetSearchPath,
        AddPackFile,
        RemoveFile,
        RenameFile
    }

    public int Init() => VTable.GetFunction<InitDelegate>((int)VTableIndex.Init)(Handle);

    public void Shutdown() => VTable.GetFunction<ShutdownDelegate>((int)VTableIndex.Shutdown)(Handle);

    public bool IsSteam() => VTable.GetFunction<IsSteamDelegate>((int)VTableIndex.IsSteam)(Handle);

    public FilesystemMountRetval MountSteamContent(int extraAppID = -1) => (FilesystemMountRetval)VTable.GetFunction<MountSteamContentDelegate>((int)VTableIndex.MountSteamContent)(Handle, extraAppID);

    public void AddSearchPath(string path, string pathID, SearchPathAdd addType = SearchPathAdd.PATH_ADD_TO_TAIL) => VTable.GetFunction<AddSearchPathDelegate>((int)VTableIndex.AddSearchPath)(Handle, path, pathID, (int)addType);

    public void RemoveSearchPath(string path, string pathID) => VTable.GetFunction<RemoveSearchPathDelegate>((int)VTableIndex.RemoveSearchPath)(Handle, path, pathID);

    public void RemoveAllSearchPaths() => VTable.GetFunction<RemoveAllSearchPathsDelegate>((int)VTableIndex.RemoveAllSearchPaths)(Handle);

    public void RemoveSearchPaths(string pathID) => VTable.GetFunction<RemoveSearchPathsDelegate>((int)VTableIndex.RemoveSearchPaths)(Handle, pathID);

    public void MarkPathIDByRequestOnly(string pathID, bool requestOnly) => VTable.GetFunction<MarkPathIDByRequestOnlyDelegate>((int)VTableIndex.MarkPathIDByRequestOnly)(Handle, pathID, requestOnly);

    public string RelativePathToFullPath(string fileName, string pathID, int maxLen = 1024, int pathFilter = 0)
    {
        var sb = new StringBuilder(maxLen);
        var func = VTable.GetFunction<RelativePathToFullPathDelegate>((int)VTableIndex.RelativePathToFullPath);
        func(Handle, fileName, pathID, sb, sb.Capacity, pathFilter, IntPtr.Zero);
        return sb.ToString();
    }

    public string GetSearchPath(string pathID, bool getPackFiles, int maxLen = 1024)
    {
        var sb = new StringBuilder(maxLen);
        var func = VTable.GetFunction<GetSearchPathDelegate>((int)VTableIndex.GetSearchPath);
        func(Handle, pathID, getPackFiles, sb, sb.Capacity);
        return sb.ToString();
    }

    public bool AddPackFile(string fullPath, string pathID) => VTable.GetFunction<AddPackFileDelegate>((int)VTableIndex.AddPackFile)(fullPath, pathID);

    public void RemoveFile(string relativePath, string pathID) => VTable.GetFunction<RemoveFileDelegate>((int)VTableIndex.RemoveFile)(relativePath, pathID);

    public void RenameFile(string oldPath, string newPath, string pathID) => VTable.GetFunction<RenameFileDelegate>((int)VTableIndex.RenameFile)(oldPath, newPath, pathID);
}
