using System.Runtime.InteropServices;
using Xunit;

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
    public SourceInterface(string interfaceName)
    {
        var context = TestContext.Current;
        var methodMetadata = context?.Test?.TestCase?.TestMethod;

        if (methodMetadata == null)
            throw new Exception("SourceInterface must be instantiated within an active Aperture test.");

        InterfaceName = interfaceName;
        ModuleName = methodMetadata.Traits["ModuleName"].First();
        if (string.IsNullOrEmpty(ModuleName))
            throw new Exception("SourceInterface must be instantiated within an active Aperture test.");

        CreateInstance();

        VTable = VTable.GetVTable(Handle);
    }

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
