namespace CodeAnalyzer.App
{
    using System.IO;

    internal static class FileNameConventions
    {
        public static string BuildOutputFileName(string topLevelFolder, string slnFile)
        {
            return slnFile.Replace(topLevelFolder, string.Empty).Replace(".sln", string.Empty).Replace('\\', '_')
                .Replace('.', '_');
        }

        public static string BuildStructuralRelationsOutputFile(string outputDirectory, string outputFileName)
        {
            return Path.Combine(outputDirectory, "Results", outputFileName + "_StructuralRelations.csv");
        }

        public static string BuildDesignSmellsOutputFile(string outputDirectory, string outputFileName)
        {
            return Path.Combine(outputDirectory, "Results", outputFileName + "_DesignSmells.json");
        }

        public static string BuildNDependProjectFilePath(string outputDirectory, string outputFileName)
        {
            return Path.Combine(outputDirectory, "NDependProjects", outputFileName + ".ndproj");
        }
    }
}