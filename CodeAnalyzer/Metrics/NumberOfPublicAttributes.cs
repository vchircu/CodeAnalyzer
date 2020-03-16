namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class NumberOfPublicAttributes
    {
        public static int Value(IType type)
        {
            return PublicAttributesFor(type).Count();
        }

        private static IEnumerable<IMember> PublicAttributesFor(IType type)
        {
            return type.Fields.Where(f => f.IsPublic && !f.IsInitOnly && !f.IsStatic);
        }
    }
}