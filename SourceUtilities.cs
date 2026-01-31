using System.Runtime.InteropServices;

namespace Aperture;

internal static class SourceUtilities
{
    // TODO: Does this work on Linux?
    /// <summary> Gets the full path to the Steam installation directory or null if Steam is not found. </summary>
    public static string? GetSteamPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam") ?? Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");

            return key?.GetValue("InstallPath")?.ToString() ?? null;
        }

        // Linux
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        string[] candidates =
        {
            Path.Combine(home, ".steam", "steam"),
            Path.Combine(home, ".local", "share", "Steam"),
            Path.Combine(home, ".var", "app", "com.valvesoftware.Steam", ".local", "share", "Steam")
        };

        return candidates.FirstOrDefault(Directory.Exists) ?? null;
    }

    public static List<string> GetSteamLibraries(string steamPath)
    {
        var libraries = new List<string>
        {
            Path.Combine(steamPath, "steamapps")
        };

        var vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(vdfPath))
            return libraries;

        var text = File.ReadAllText(vdfPath);

        foreach (var line in text.Split('\n'))
        {
            if (line.Contains("\"path\""))
            {
                var path = line.Split('"')[3];

                if (OperatingSystem.IsWindows())
                    path = path.Replace(@"\\", @"\");

                libraries.Add(Path.Combine(path, "steamapps"));
            }
        }

        return libraries;
    }

    /// <summary> Gets the full path to a Source Engine module (binary) from it's name. </summary>
    /// <param name="name"> The name of the Source Engine module (i.e. "inputsystem"). </param>
    /// <param name="appID"> The application ID of the Source Engine branch. </param>
    public static string GetModulePath(string name, int appID)
    {
        // Programmatically find the app ID path
        var steamPath = GetSteamPath();
        if (steamPath == null)
            throw new Exception("Steam not found.");

        var libraries = GetSteamLibraries(steamPath);

        foreach (var library in GetSteamLibraries(steamPath))
        {
            var manifest = Path.Combine(library, $"appmanifest_{appID}.acf");
            if (!File.Exists(manifest))
                continue;

            var installDir = File.ReadLines(manifest).FirstOrDefault(l => l.Contains("\"installdir\""))?.Split('"')[3];

            if (installDir == null)
                continue;

            var basePath = Path.Combine(library, "common", installDir);

            // Linux binaries
            var linuxBin = Path.Combine(basePath, "bin", "linux64", $"lib{name}.so");
            if (File.Exists(linuxBin))
                return linuxBin;

            // Windows binaries
            var winBin = Path.Combine(basePath, "bin", $"{name}.dll");
            if (File.Exists(winBin))
                return winBin;
        }

        return string.Empty;
    }
}
