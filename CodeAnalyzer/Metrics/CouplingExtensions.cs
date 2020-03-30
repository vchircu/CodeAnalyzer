namespace CodeAnalyzer.Metrics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public static class CouplingExtensions
    {
        public static IEnumerable<IMethod> CoupledMethods(this IMethod method)
        {
            return method.MethodsCalled.Where(m => m.ParentType != method.ParentType && !m.IsProperty())
                .ExceptThirdParty();
        }

        public static IEnumerable<IType> ProvidersForCoupledMethods(this IMethod method)
        {
            return method.CoupledMethods().Select(m => m.ParentType).ToHashSetEx();
        }

        public static IList<Tuple<IType, IList<IMethod>>> CouplingIntensityPerProvider(this IMethod method)
        {
            var result = method.CoupledMethods().GroupBy(m => m.ParentType, m => m, (key, g) => new Tuple<IType, IList<IMethod>>(key, g.ToList())).ToList();

            return result;
        }
    }
}