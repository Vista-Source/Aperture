using Xunit;
using Xunit.v3;

namespace Aperture;

public class ApertureFactAttribute : FactAttribute, ITraitAttribute
{
    /// <summary> The name of the module to load during this test. </summary>
    public string ModuleName { get; }

    /// <summary> The full path to the module to load during this test. </summary>
    public string ModulePath => SourceUtilities.GetModulePath(ModuleName, AppID);

    /// <summary> The application ID of the Source Engine branch to test against. </summary>
    public int AppID { get; }

    /// <summary> Initializes a new instance of the <see cref="ApertureFactAttribute"/> class. </summary>
    /// <param name="moduleName"> The name of the Source Engine module to load during this test. </param>
    public ApertureFactAttribute(string moduleName, int appID = 243750) : base()
    {
        ModuleName = moduleName;
        AppID = appID;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        return
        [
            new KeyValuePair<string, string>("ModuleName", ModuleName),
            new KeyValuePair<string, string>("ModulePath", ModulePath)
        ];
    }
}
