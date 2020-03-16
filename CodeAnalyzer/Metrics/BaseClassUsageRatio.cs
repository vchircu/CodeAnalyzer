namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class BaseClassUsageRatio
    {
        public static double Value(IType type)
        {
            var bur = (double)type.ProtectedMembersUsed().Count() / type.ProtectedMembers().Count();
            return bur;
        }
    }
}