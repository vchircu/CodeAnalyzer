namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeAnalyzer;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public static class ForeignDataExtensions
    {
        public static IEnumerable<IMember> ForeignData(this IType type)
        {
            return type.Methods.Where(m => !m.IsAbstract).SelectMany(ForeignData).ToHashSetEx();
        }

        public static IEnumerable<IMember> ForeignData(this IMethod method)
        {
            return method.MembersUsed.Where(
                memberUsed => !method.ParentType.Hierarchy().Contains(memberUsed.ParentType)
                              && memberUsed.IsPropertyOrField() &&
                              // Ignore Nullable
                              !"Nullable<T>".ToNameLikePredicate()(memberUsed.ParentType) &&
                              // Ignore Async related classes
                              !"System.Runtime.CompilerServices".ToNameLikePredicate()(memberUsed.ParentType)
                              && !memberUsed.ParentType.IsEnumeration).ToHashSetEx();
        }

        public static IEnumerable<IType> ForeignDataProviders(this IMethod method)
        {
            return method.ForeignData().Select(member => member.ParentType).ExceptThirdParty().ToHashSetEx();
        }
    }
}