namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class WeightOfAClass
    {
        public static double Value(IType type)
        {
            return (double)AllPublicFunctionalMembersFor(type).Count() / AllPublicMembersFor(type).Count();
        }

        private static IEnumerable<IMember> AllPublicFunctionalMembersFor(IType type)
        {
            return type.Methods.Where(m => m.IsPublic && !m.IsProperty() && !m.IsAbstract);
        }

        private static IEnumerable<IMember> AllPublicMembersFor(IType type)
        {
            return type.Members.Where(
                m => m.IsPublic && (!m.IsMethod || !m.AsMethod.IsClassConstructor && !m.AsMethod.IsConstructor));
        }
    }
}