namespace CodeAnalyzer.Metrics
{
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
    }
}