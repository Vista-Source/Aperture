using Xunit;

namespace Aperture;

/// <summary>
/// Attribute that is applied to a method that is a parity-based fact that should be run by the test runner.
/// </summary>
public class ApertureFactAttribute : FactAttribute
{
    /// <summary> The name of the C++ module this tests against. </summary>
    public string CPPModule { get; }

    public ApertureFactAttribute(string cppModule)
    {
        CPPModule = cppModule;
    }
}
