namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class BaseClassOverridingRatio
    {
        public static double Value(IType type)
        {
            IEnumerable<IMethod> overridingMethods = type.OverridingMethods();
            var bovr = (double)overridingMethods.Count() / type.NbMethods.GetValueOrDefault();

            return bovr;
        }
    }
}