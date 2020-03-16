namespace CodeAnalyzer.App
{
    using PowerArgs;

    public class Configuration
    {
        [ArgRequired, ArgPosition(0), ArgExistingDirectory,
         ArgDescription("This is the top level folder that contains all Visual Studio solution files (.sln).")]
        public string TopLevelInputFolder { get; set; }

        [ArgPosition(1),
         ArgDescription(
             "Folder where the output is generated. The program will generate one file per solution per analyzer."),
         ArgDefaultValue(".\\output\\")]
        public string OutputFolder { get; set; }
    }
}