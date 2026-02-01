# Aperture
An xUnit extension for testing parity between C# and C++ Source Engine modules.

> [!WARNING]
> **This is a low-level library.** Abstractions are in place, but a solid understanding of vtables, delegates, and Source Engine internals will help.

## Requirements
- Valid install of **Steam**
- Valid install of the target branch via Steam (By default: Source SDK Base 2013 Multiplayer)

## Purpose
Aperture allows Vista Source to test it's C# implementations of Source Engine modules against the original C++ implementations. This allows us to ensure Vista modules remain behaviourally consistent with the SDK.

## Usage
Aperture provides multiple different ways of interfacing with Source Engine modules. All tests that use Aperture must be marked with the `[ApertureFact]` attribute.

### Interfaces
Source Engine modules expose [Interfaces](https://developer.valvesoftware.com/wiki/Category:Interfaces) that allow running functionality from said module from the outside. The vast majority of Source logic is contained within an interface, and as such they're vital to parity testing. To test interface logic, we use the `SourceInterface` class.

```csharp
public class UnitTest1
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void AttachToWindowDelegate(IntPtr thisPtr, IntPtr window);

    [ApertureFact("inputsystem")]
    public void Test1()
    {
        var input = new SourceInterface("InputSystemVersion001");

        var attachToWindow = input.VTable.GetFunction<AttachToWindowDelegate>(5);
        IntPtr curWindow = Process.GetCurrentProcess().MainWindowHandle;

        attachToWindow(input.Handle, curWindow);
    }
}
```

There are also type-safe versions of some common interfaces, such as `IFileSystem`.
```csharp
public class UnitTest1
{
    [ApertureFact("FileSystem_Stdio")]
    public void Test1()
    {
        var fileSystem = new IFileSystem("VFileSystem022");

        Assert.Equal(1, fileSystem.Init());

        Assert.Equal(IFileSystem.FilesystemMountRetval.FILESYSTEM_MOUNT_OK, fileSystem.MountSteamContent(243750));

        fileSystem.Shutdown();
    }
}
```
