namespace CodeAnalyzer.App
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeAnalyzer.App.Output;
    using CodeAnalyzer.DetectionStrategies;
    using CodeAnalyzer.StructuralRelations;

    using NDepend.CodeModel;

    using Serilog;

    internal static class Analyzers
    {
        public static void Handle(string outputDirectory, string outputFileName, ICodeBase codeBase, ILogger logger)
        {
            HandleStructuralRelations(codeBase, outputDirectory, outputFileName, logger);

            HandleDesignSmells(codeBase, outputDirectory, outputFileName, logger);
        }

        private static void HandleDesignSmells(
            ICodeBase codeBase,
            string outputDirectory,
            string outputFileName,
            ILogger logger)
        {
            List<DesignSmell> designSmells = new DesignSmellsDetection(logger).Detect(codeBase).ToList();

            var designSmellsOutputFile = FileNameConventions.BuildDesignSmellsOutputFile(outputDirectory, outputFileName);
            DesignSmellsJsonWriter.Export(designSmells, designSmellsOutputFile);
        }

        private static void HandleStructuralRelations(
            ICodeBase analysisResultCodeBase,
            string outputDirectory,
            string outputFileName,
            ILogger logger)
        {
            var relations = new StructuralRelationsBuilder(logger).GetStructuralRelations(analysisResultCodeBase);

            var structuralRelationsOutputFile = FileNameConventions.BuildStructuralRelationsOutputFile(outputDirectory, outputFileName);
            StructuralRelationCsvWriter.Export(relations, structuralRelationsOutputFile);
        }
    }
}