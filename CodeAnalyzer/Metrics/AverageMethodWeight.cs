namespace CodeAnalyzer.Metrics
{
    using NDepend.CodeModel;

    public static class AverageMethodWeight
    {
        public static double Value(IType type)
        {
            return (double)type.CyclomaticComplexity.GetValueOrDefault() / type.NbMethods.GetValueOrDefault();
        }
    }
}