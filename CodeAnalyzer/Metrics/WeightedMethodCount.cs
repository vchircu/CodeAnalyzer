namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class WeightedMethodCount
    {
        public static int Value(IType type)
        {
            return type.MethodsAndConstructors.Select(m => (int)m.CyclomaticComplexity.GetValueOrDefault()).Sum();
        }
    }
}