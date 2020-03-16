namespace CodeAnalyzer.DetectionStrategies
{
    using NDepend.CodeModel;

    public class DesignSmell
    {
        public string SourceFile { get; set; }

        public string Name { get; set; }

        public double Severity { get; set; }

        public ICodeElement Source { get; set; }
    }
}