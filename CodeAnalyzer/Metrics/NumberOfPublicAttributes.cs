namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public static class NumberOfPublicAttributes
    {
        public static IEnumerable<IMember> PublicAttributes(this IType type)
        {
            return type.Fields.Where(f => f.IsPublic && !f.IsInitOnly && !f.IsStatic);
        }
    }
}