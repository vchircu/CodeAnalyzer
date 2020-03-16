namespace CodeAnalyzer.App
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CSharpFunctionalExtensions;

    using NDepend;
    using NDepend.Analysis;
    using NDepend.Path;
    using NDepend.Project;

    using Serilog;

    internal static class NDependProjectHandler
    {
        public static Maybe<IProject> GetOrCreateProjectFromVisualStudioSolution(string slnAbsolutePath, string outputDirectory, string outputFileName)
        {
            var ndependProjectPath = FileNameConventions.BuildNDependProjectFilePath(outputDirectory, outputFileName);
            if (File.Exists(ndependProjectPath))
            {
                return Maybe<IProject>.From(GetProjectFromNDependFile(ndependProjectPath));
            }

            var ndependServicesProvider = new NDependServicesProvider();
            List<IAbsoluteFilePath> assemblies = GetAssembliesFromSolution(slnAbsolutePath, ndependServicesProvider);

            if (!assemblies.Any())
            {
                return Maybe<IProject>.None;
            }

            var projectManager = ndependServicesProvider.ProjectManager;
            var project = projectManager.CreateBlankProject(
                ndependProjectPath.ToAbsoluteFilePath(),
                outputFileName);
            project.CodeToAnalyze.SetApplicationAssemblies(assemblies);
            projectManager.SaveProject(project);

            return Maybe<IProject>.From(project);
        }

        private static IProject GetProjectFromNDependFile(string pathToNDependProject)
        {
            var projectManager = new NDependServicesProvider().ProjectManager;
            var project = projectManager.LoadProject(pathToNDependProject.ToAbsoluteFilePath());
            return project;
        }

        private static IProject CreateTemporaryProjectFrom(string slnAbsolutePath)
        {
            var ndependServicesProvider = new NDependServicesProvider();
           
            List<IAbsoluteFilePath> assemblies = GetAssembliesFromSolution(slnAbsolutePath, ndependServicesProvider);

            var projectManager = ndependServicesProvider.ProjectManager;
            var project = projectManager.CreateTemporaryProject(assemblies, TemporaryProjectMode.TemporaryNewer);

            return project;
        }

        private static List<IAbsoluteFilePath> GetAssembliesFromSolution(string slnAbsolutePath, NDependServicesProvider ndependServicesProvider)
        {
            var visualStudioManager = ndependServicesProvider.VisualStudioManager;

            ICollection<IAbsoluteFilePath> solutions = new List<IAbsoluteFilePath>();
            solutions.Add(slnAbsolutePath.ToAbsoluteFilePath());

            var assemblies = new List<IAbsoluteFilePath>();
            foreach (var solution in solutions)
            {
                assemblies.AddRange(visualStudioManager.GetAssembliesFromVisualStudioSolutionOrProject(solution));
            }

            return assemblies;
        }

        public static IAnalysisResult GetAnalysisResult(IProject getProject, ILogger logger)
        {
            if (getProject.TryGetMostRecentAnalysisResultRef(out var analysisResultRef))
            {
                logger.Information("\tProject already analyzed");
                var recentAnalysisResult = analysisResultRef.Load();
                return recentAnalysisResult;
            }

            var analysisResult = getProject.RunAnalysis();
            return analysisResult;
        }
    }
}