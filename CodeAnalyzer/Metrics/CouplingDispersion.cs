namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class CouplingDispersion
    {
        public static double Value(IMethod method)
        {
            var cint = CouplingIntensity.Value(method);

            if (cint == 0)
            {
                return 0;
            }

            return (double)method.ProvidersForCoupledMethods().Count() / cint;
        }
    }
}