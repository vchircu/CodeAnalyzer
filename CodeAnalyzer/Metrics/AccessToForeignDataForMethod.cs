namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public static class AccessToForeignDataForMethod
    {
        public static int Value(IMethod method)
        {
            return method.ForeignData().Count();
        }
    }
}