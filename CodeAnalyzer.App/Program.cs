namespace CodeAnalyzer.App
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    using NDepend.Analysis;
    using NDepend.Path;

    using PowerArgs;

    using Serilog;

    class Program
    {
        private static readonly ILogger Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console()
            .WriteTo.File("logs\\log.txt").CreateLogger();

        private static readonly AssemblyResolver AssemblyResolver = new AssemblyResolver(@".\..\..\..\..\Lib");

        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.AssemblyResolveHandler;

            try
            {
                var parsed = Args.Parse<Configuration>(args);

                MainSub(parsed.TopLevelInputFolder, parsed.OutputFolder);
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<Configuration>());
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void MainSub(string topLevelFolder, string outputDirectory)
        {
            outputDirectory = EnsureAbsolutePath(outputDirectory);

            var slnFiles = GetAllVisualStudioSolutionsInFolder(topLevelFolder);
            foreach (var slnFile in slnFiles)
            {
                var outputFileName = FileNameConventions.BuildOutputFileName(topLevelFolder, slnFile);

                HandleProject(slnFile, outputDirectory, outputFileName);
            }

            Console.WriteLine("Finished running the analysis. Press any key to exit...");
            Console.ReadKey();
        }

        private static string EnsureAbsolutePath(string outputDirectory)
        {
            if (outputDirectory.IsValidRelativeDirectoryPath())
            {
                outputDirectory = Path.GetFullPath(outputDirectory);
            }

            return outputDirectory;
        }

        private static IEnumerable<string> GetAllVisualStudioSolutionsInFolder(string topFolder)
        {
            return Directory.EnumerateFiles(topFolder, "*.sln", SearchOption.AllDirectories);
        }

        private static void HandleProject(string slnAbsolutePath, string outputDirectory, string outputFileName)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Information("**** Analyzing solution {solution} *** ", slnAbsolutePath);
            Logger.Information("\tBuilding NDepend Project");

            var result = NDependProjectHandler.GetOrCreateProjectFromVisualStudioSolution(
                slnAbsolutePath,
                outputDirectory,
                outputFileName);

            if (result.IsFailure)
            {
                Logger.Information(" \t Could not create an NDepend project for the solution. Reason:{error}", result.Error);
                return;
            }

            stopwatch.Stop();
            Logger.Information("\tBuilt NDepend Project in {Elapsed:000} ms", stopwatch.ElapsedMilliseconds);

            stopwatch = Stopwatch.StartNew();
            Logger.Information("\tRunning NDepend Analysis");
            IAnalysisResult analysisResult = NDependProjectHandler.GetAnalysisResult(result.Value, Logger);

            stopwatch.Stop();
            Logger.Information("\tAnalyzed NDepend Project in {Elapsed:000} ms", stopwatch.ElapsedMilliseconds);

            var codeBase = analysisResult.CodeBase;

            Analyzers.Handle(outputDirectory, outputFileName, codeBase, Logger);
        }
    }
}