namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public static class AccessToForeignDataForType
    {
        public static int Value(IType type)
        {
            return type.ForeignData().Count();
        }
    }
}