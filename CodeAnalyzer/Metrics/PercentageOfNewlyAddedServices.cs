namespace CodeAnalyzer.Metrics
{
    using System.Linq;

    using NDepend.CodeModel;

    public class PercentageOfNewlyAddedServices
    {
        public static double Value(IType type)
        {
            var nas = NumberOfAddedServices.Value(type);
            var publicMethodsCount = type.PublicMethods().Count();

            var pnas = publicMethodsCount == 0 ? 0 : (double) nas / publicMethodsCount;
            return pnas;
        }
    }
}