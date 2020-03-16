namespace CodeAnalyzer.App.Output
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using CodeAnalyzer.StructuralRelations;

    using CsvHelper;
    using CsvHelper.Configuration.Attributes;

    public static class StructuralRelationCsvWriter
    {
        public static void Export(IEnumerable<StructuralRelation> structuralRelations, string pathToOutputFile)
        {
            IEnumerable<StructuralRelationRecord> records = structuralRelations.Select(
                r => new StructuralRelationRecord
                         {
                             ExternalCalls = r.NumberOfCallsToMethods,
                             ExternalData = r.NumberOfCallsToPropertiesOrFields,
                             Hierarchy = r.IsTargetPartOfSourcesHierarchy ? 1 : 0,
                             Source = r.Source,
                             Target = r.Target
                         });

            FileHelper.EnsureDirectoriesExist(pathToOutputFile);
            using (var writer = new StreamWriter(pathToOutputFile))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
            }
        }
    }

    public class StructuralRelationRecord
    {
        [Index(0)]
        [Name("source")]
        public string Source { get; set; }

        [Index(1)]
        [Name("target")]
        public string Target { get; set; }

        [Index(2)]
        [Name("extCalls")]
        public int ExternalCalls { get; set; }

        [Index(3)]
        [Name("extData")]
        public int ExternalData { get; set; }

        [Index(4)]
        [Name("hierarchy")]
        public int Hierarchy { get; set; }
    }
}