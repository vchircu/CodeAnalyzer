namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class NumberOfAccessorMethods
    {
        public static int Value(IType type)
        {
            return AccessorsFor(type).Count();
        }

        private static IEnumerable<IMember> AccessorsFor(IType type)
        {
            return type.Methods.Where(m => m.IsPublic && m.IsProperty() && !m.IsAbstract && !m.IsStatic);
        }
    }
}