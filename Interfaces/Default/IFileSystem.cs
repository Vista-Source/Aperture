
using System.Runtime.InteropServices;

namespace Aperture.Interfaces.Default;

public class IFileSystem : SourceInterface
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate bool IsSteamDelegate(IntPtr thisPtr);

    /// <inheritdoc/>
    public IFileSystem(string interfaceName) : base(interfaceName) { }

    public bool IsSteam() => VTable.GetFunction<IsSteamDelegate>(22)(Handle);
}
