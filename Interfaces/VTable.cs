
using System.Runtime.InteropServices;

namespace Aperture.Interfaces;

/// <summary>
/// Low-Level access into a C++ instance's VTable.
/// </summary>
public unsafe class VTable
{
    /// <summary> The address of the VTable. </summary>
    public void* Handle { get; }

    /// <summary> Initializes a new instance of the <see cref="VTable"/> class. </summary>
    public VTable(void* handle) => Handle = handle;

    /// <summary> Gets a function from the VTable. </summary>
    public TDelegate GetFunction<TDelegate>(int index) => Marshal.GetDelegateForFunctionPointer<TDelegate>((nint)(*(void***)Handle)[index]);

    /// <summary> Gets the VTable from a C++ instance. </summary>
    public static VTable GetVTable(void* instance) => new VTable(**(void***)instance);

    /// <summary> Gets the VTable from a C++ instance. </summary>
    public static VTable GetVTable(nint instance) => new VTable(**(void***)instance);
}
