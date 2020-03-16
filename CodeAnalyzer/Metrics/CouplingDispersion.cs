namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class CouplingDispersion
    {
        public static double Value(IMethod method)
        {
            return (double)method.ProvidersForCoupledMethods().Count() / CouplingIntensity.Value(method);
        }
    }
}