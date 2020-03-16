namespace CodeAnalyzer.App.Output
{
    using System.IO;

    public static class FileHelper
    {
        public static void EnsureDirectoriesExist(string output)
        {
            FileInfo file = new FileInfo(output);
            file.Directory.Create();
        }
    }
}