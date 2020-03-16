namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class NumberOfProtectedMembers
    {
        public static int Value(IType type)
        {
            return type.ProtectedMembers().Count();
        }
    }
}