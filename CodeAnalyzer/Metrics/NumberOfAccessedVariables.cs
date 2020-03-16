namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class NumberOfAccessedVariables
    {
        public static int Value(IMethod method)
        {
            return OwnAttributesAccessed(method).Count() + method.NbVariables.GetValueOrDefault() + method.NbParameters;
        }

        private static IEnumerable<IMember> OwnAttributesAccessed(IMethod method)
        {
            return method.MembersUsed.Where(
                member => member.IsPropertyOrField() && member.ParentType == method.ParentType);
        }
    }
}