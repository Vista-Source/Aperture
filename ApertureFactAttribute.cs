using Xunit;
using Xunit.v3;

namespace Aperture;

public class ApertureFactAttribute : FactAttribute, ITraitAttribute
{
    /// <summary> The name of the module to load during this test. </summary>
    public string ModuleName { get; }

    /// <summary> Initializes a new instance of the <see cref="ApertureFactAttribute"/> class. </summary>
    public ApertureFactAttribute(string moduleName) : base()
    {
        ModuleName = moduleName;
    }
    
    /// <inheritdoc/>
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        return new[]
        {
            new KeyValuePair<string, string>("ModuleName", ModuleName)
        };
    }
}
