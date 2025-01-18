using System.Xml.Linq;

namespace SecretSwitcher.Common;

internal static class ProjectManager
{
    public static List<ProjectInfo> TraverseProjectsAndRetrieveSecrets(string baseAddress)
    {
        var projectInfos = new List<ProjectInfo>();

        try
        {
            foreach (string filePath in Directory.EnumerateFiles(baseAddress, "*.csproj", SearchOption.AllDirectories))
            {
                string projectName = Path.GetFileNameWithoutExtension(filePath);

                var projectType = DetermineProjectType(filePath);
                if (projectType == ProjectType.ClassLibrary || projectType == ProjectType.Test)
                {
                    Console.WriteLine($"Skipping project: {projectName} ({projectType})");
                    continue;
                }

                string? secretId = GetUserSecretsIdFromProject(filePath);
                projectInfos.Add(new ProjectInfo
                {
                    Name = projectName,
                    SecretId = secretId
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error traversing directories: {ex.Message}");
        }

        return projectInfos;
    }

    private static ProjectType DetermineProjectType(string projectFilePath)
    {
        try
        {
            XDocument xmlDoc = XDocument.Load(projectFilePath);
            var sdk = xmlDoc.Root?.Attribute("Sdk")?.Value;
            if (sdk != null && sdk.Equals("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase))
                return ProjectType.WebApp;

            foreach (var group in xmlDoc.Descendants("PropertyGroup"))
            {
                var outputType = group.Element("OutputType")?.Value;
                if (outputType?.Equals("Exe", StringComparison.OrdinalIgnoreCase) == true)
                    return ProjectType.Executable;
                if (outputType?.Equals("Library", StringComparison.OrdinalIgnoreCase) == true)
                    return ProjectType.ClassLibrary;
            }

            foreach (var package in xmlDoc.Descendants("PackageReference"))
            {
                var packageName = package.Attribute("Include")?.Value;
                if (packageName != null &&
                    (packageName.Contains("xunit", StringComparison.OrdinalIgnoreCase) ||
                     packageName.Contains("nunit", StringComparison.OrdinalIgnoreCase) ||
                     packageName.Equals("Microsoft.NET.Test.Sdk", StringComparison.OrdinalIgnoreCase)))
                {
                    return ProjectType.Test;
                }
            }
        }
        catch (Exception)
        {
            // Fall through to default
        }

        return ProjectType.ClassLibrary;
    }

    private static string? GetUserSecretsIdFromProject(string projectFilePath)
    {
        try
        {
            XDocument xmlDoc = XDocument.Load(projectFilePath);
            foreach (var group in xmlDoc.Descendants("PropertyGroup"))
            {
                var userSecretsId = group.Element("UserSecretsId");
                if (userSecretsId != null)
                    return userSecretsId.Value;
            }
        }
        catch (Exception)
        {
            // Ignore and return null
        }

        return null;
    }
}

internal enum ProjectType
{
    Executable,
    WebApp,
    ClassLibrary,
    Test
}

internal class ProjectInfo
{
    public string Name { get; set; } = null!;
    public string? SecretId { get; set; }
}