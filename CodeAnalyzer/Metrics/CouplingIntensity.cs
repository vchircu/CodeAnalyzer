namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class CouplingIntensity
    {
        public static int Value(IMethod method)
        {
            return method.CoupledMethods().Count();
        }
    }
}