namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public static class ForeignDataProviders
    {
        public static int Value(IMethod method)
        {
            return method.ForeignDataProviders().Count();
        }
    }
}