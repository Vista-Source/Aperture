using System.Runtime.InteropServices;

namespace Aperture.Interfaces;

/// <summary>
/// A Source-Engine C++ interface.
/// </summary>
public class SourceInterface
{
    public string InterfaceName { get; }
    public string ModuleName { get; }

    // The handle to the C++ instance
    private IntPtr handle;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr CreateInterfaceDelegate(string name, int returnCode);

    /// <summary> Initializes a new instance of the <see cref="SourceInterface"/> class. </summary>
    public SourceInterface(string interfaceName, string moduleName)
    {
        InterfaceName = interfaceName;
        ModuleName = moduleName;

        CreateInstance();
    }
    
    private void CreateInstance()
    {
        IntPtr lib = NativeLibrary.Load(ModuleName);
        IntPtr func = NativeLibrary.GetExport(lib, "CreateInterface");

        var createInterface = Marshal.GetDelegateForFunctionPointer<CreateInterfaceDelegate>(func);
        handle = createInterface(InterfaceName, 0);
    }
}
