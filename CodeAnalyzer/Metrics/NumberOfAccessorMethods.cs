namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public static class NumberOfAccessorMethods
    {
        public static IEnumerable<IMember> Accessors(this IType type)
        {
            return type.Methods.Where(m => m.IsPublic && m.IsProperty() && !m.IsAbstract && !m.IsStatic);
        }
    }
}