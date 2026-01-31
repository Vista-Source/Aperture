# Aperture
An xUnit extension for testing parity between C# and C++ Source Engine modules.

> [!WARNING]
> **This is a low-level library.** Abstractions are in place, but a solid understanding of vtables, delegates, and Source Engine internals will help.

## Purpose
Aperture allows Vista Source to test it's C# implementations of Source Engine modules against the original C++ implementations. This allows us to ensure Vista modules remain behaviourally consistent with the SDK.

## Usage
Aperture provides multiple different ways of interfacing with Source Engine modules:

### Interfaces
Source Engine modules expose [Interfaces](https://developer.valvesoftware.com/wiki/Category:Interfaces) that allow running functionality from said module from the outside. The vast majority of Source logic is contained within an interface, and as such they're vital to parity testing. To test interface logic, we use the `SourceInterface` class.

```csharp
public class UnitTest1
{
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    delegate void AttachToWindowDelegate(IntPtr thisPtr, IntPtr window);

    [Fact]
    public void Test1()
    {
        var input = new SourceInterface("InputSystemVersion001", @"D:\SteamLibrary\steamapps\common\Source SDK Base 2013 Multiplayer\bin\inputsystem.dll");

        var attachToWindow = input.VTable.GetFunction<AttachToWindowDelegate>(5);
        IntPtr curWindow = Process.GetCurrentProcess().MainWindowHandle;

        attachToWindow(input.Handle, curWindow);
    }
}
```
