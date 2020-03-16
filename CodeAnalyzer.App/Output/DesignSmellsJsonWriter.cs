namespace CodeAnalyzer.App.Output
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using CodeAnalyzer.DetectionStrategies;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class DesignSmellsJsonWriter
    {
        public static void Export(IEnumerable<DesignSmell> designSmells, string designSmellsOutputFile)
        {
            List<DesignFlaw> designFlaws = designSmells.Select(DesignFlaw.From).ToList();
            WriteToJson(designFlaws, designSmellsOutputFile);
        }

        private static void WriteToJson(IReadOnlyCollection<DesignFlaw> designFlaws, string output)
        {
            FileHelper.EnsureDirectoriesExist(output);

            var serializerSettings =
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            var serializeObject = JsonConvert.SerializeObject(designFlaws, serializerSettings);

            File.WriteAllText(output, serializeObject);
        }
    }

    internal class DesignFlaw
    {
        public string File { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Value { get; set; }

        public static DesignFlaw From(DesignSmell smell)
        {
            return new DesignFlaw
                       {
                           File = smell.SourceFile,
                           Name = smell.Name,
                           Category = GetCategory(smell.Name),
                           Value = smell.Severity.ToString(CultureInfo.InvariantCulture)
                       };
        }

        private static string GetCategory(string name)
        {
            const string encapsulation = "Encapsulation";
            const string inheritance = "Inheritance Relations";
            const string coupling = "Coupling";
            var designSmellToCategory = new Dictionary<string, string>
                                            {
                                                { "God Class", encapsulation },
                                                { "Feature Envy", encapsulation },
                                                { "Data Class", encapsulation },
                                                { "Intensive Coupling", coupling },
                                                { "Dispersed Coupling", coupling },
                                                { "Shotgun Surgery", coupling },
                                                { "Refused Parent Bequest", inheritance },
                                                { "Tradition Breaker", inheritance },
                                                { "Blob Method", "Complexity" }
                                            };

            if (designSmellToCategory.ContainsKey(name))
            {
                return designSmellToCategory[name];
            }

            return name;
        }
    }
}