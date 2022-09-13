namespace CodeAnalyzer.DetectionStrategies
{
    using System.Collections.Generic;

    using NDepend.CodeModel;

    public class DesignSmell
    {
        public string SourceFile { get; set; }

        public string Name { get; set; }

        public double Severity { get; set; }

        public IDictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>();

        public ICodeElement Source { get; set; }
    }
}