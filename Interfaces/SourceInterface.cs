using System.Runtime.InteropServices;

namespace Aperture.Interfaces;

/// <summary>
/// A Source-Engine C++ interface.
/// </summary>
public class SourceInterface : IDisposable
{
    public string InterfaceName { get; }
    public string ModuleName { get; }
    public IntPtr Handle { get; private set; }
    public VTable VTable { get; }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr CreateInterfaceDelegate([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr returnCode);

    /// <summary> Initializes a new instance of the <see cref="SourceInterface"/> class. </summary>
    public SourceInterface(string interfaceName, string moduleName)
    {
        InterfaceName = interfaceName;
        ModuleName = moduleName;

        CreateInstance();

        VTable = VTable.GetVTable(Handle);
    }

    /// <inheritdoc/>
    public void Dispose() => NativeLibrary.Free(Handle);
    
    private void CreateInstance()
    {
        IntPtr lib = NativeLibrary.Load(ModuleName);
        IntPtr func = NativeLibrary.GetExport(lib, "CreateInterface");

        var createInterface = Marshal.GetDelegateForFunctionPointer<CreateInterfaceDelegate>(func);
        Handle = createInterface(InterfaceName, 0);
    }
}
