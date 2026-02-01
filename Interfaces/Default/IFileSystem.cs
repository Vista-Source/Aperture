
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
    delegate void RemoveSearchPathsDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string pathID);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void MarkPathIDByRequestOnlyDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string pathID, [MarshalAs(UnmanagedType.I1)] bool requestOnly);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate IntPtr RelativePathToFullPathDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string fileName, [MarshalAs(UnmanagedType.LPStr)] string pathID, StringBuilder dest, int maxLen, int pathFilter, IntPtr pathTypePtr);

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate int GetSearchPathDelegate(IntPtr thisPtr, [MarshalAs(UnmanagedType.LPStr)] string pathID, [MarshalAs(UnmanagedType.I1)] bool getPackFiles, StringBuilder dest, int maxLen);


    #endregion

    /// <inheritdoc/>
    public IFileSystem(string interfaceName) : base(interfaceName) { }

    private const int INIT_INDEX = 3;
    private const int MOUNT_INDEX = 6;
    private const int ADD_SEARCH_PATH_INDEX = 7;
    private const int RELATIVE_TO_FULL_INDEX = 12;


    public int Init() => VTable.GetFunction<InitDelegate>(INIT_INDEX)(Handle);

    public void Shutdown() => VTable.GetFunction<ShutdownDelegate>(INIT_INDEX + 1)(Handle);

    public bool IsSteam() => VTable.GetFunction<IsSteamDelegate>(INIT_INDEX + 2)(Handle);

    public FilesystemMountRetval MountSteamContent(int extraAppID = -1) =>
        (FilesystemMountRetval)VTable.GetFunction<MountSteamContentDelegate>(MOUNT_INDEX)(Handle, extraAppID);

    public void AddSearchPath(string path, string pathID, SearchPathAdd addType = SearchPathAdd.PATH_ADD_TO_TAIL) =>
        VTable.GetFunction<AddSearchPathDelegate>(ADD_SEARCH_PATH_INDEX)(Handle, path, pathID, (int)addType);

    public void RemoveSearchPath(string path, string pathID) =>
        VTable.GetFunction<RemoveSearchPathDelegate>(ADD_SEARCH_PATH_INDEX + 1)(Handle, path, pathID);

    public void RemoveSearchPaths(string pathID) =>
        VTable.GetFunction<RemoveSearchPathsDelegate>(ADD_SEARCH_PATH_INDEX + 2)(Handle, pathID);

    public void MarkPathIDByRequestOnly(string pathID, bool requestOnly) =>
        VTable.GetFunction<MarkPathIDByRequestOnlyDelegate>(ADD_SEARCH_PATH_INDEX + 3)(Handle, pathID, requestOnly);

    public string RelativePathToFullPath(string fileName, string pathID, int maxLen = 1024, int pathFilter = 0)
    {
        var sb = new StringBuilder(maxLen);
        var func = VTable.GetFunction<RelativePathToFullPathDelegate>(RELATIVE_TO_FULL_INDEX);
        func(Handle, fileName, pathID, sb, sb.Capacity, pathFilter, IntPtr.Zero);
        return sb.ToString();
    }

    public string GetSearchPath(string pathID, bool getPackFiles, int maxLen = 1024)
    {
        var sb = new StringBuilder(maxLen);
        var func = VTable.GetFunction<GetSearchPathDelegate>(RELATIVE_TO_FULL_INDEX + 1);
        func(Handle, pathID, getPackFiles, sb, sb.Capacity);
        return sb.ToString();
    }
}
