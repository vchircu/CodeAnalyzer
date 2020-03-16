namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;

    public class NumberOfAddedServices
    {
        public static int Value(IType type)
        {
            IEnumerable<IMethod> newlyAddedMethods = type.NewlyAddedMethods();
            return newlyAddedMethods.Count();
        }
    }
}