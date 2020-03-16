namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public class LocalityOfAttributeAccess
    {
        public static double Value(IMethod method)
        {
            return (double)OwnMembersUsedBy(method).Count() / AllMembersUsedBy(method).Count();
        }

        private static IEnumerable<IMember> AllMembersUsedBy(IMethod method)
        {
            return method.MembersUsed.Where(memberUsed => memberUsed.IsPropertyOrField()).ToHashSetEx();
        }

        private static IEnumerable<IMember> OwnMembersUsedBy(IMethod method)
        {
            return AllMembersUsedBy(method).Where(memberUsed => memberUsed.ParentType == method.ParentType)
                .ToHashSetEx();
        }
    }
}