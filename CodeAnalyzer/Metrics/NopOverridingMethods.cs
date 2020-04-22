namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class NopOverridingMethods
    {
        public static int Value(IType type)
        {
            IEnumerable<IMethod> nopOverridingMethods = type.Methods.Where(m => m.IsPublic && m.OverriddensBase.Any() && m.NbLinesOfCode == 0);

            return nopOverridingMethods.Count();
        }
    }
}