namespace CodeAnalyzer.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public class TotalClassCohesion
    {
        public static double Value(IType t)
        {
            return (double)CohesivePairsFrom(PairsFrom(MethodsToConsiderFor(t))).Count()
                   / NumberOfPairsFor(MethodsToConsiderFor(t).Count());
        }

        private static HashSet<IMember> FieldsUsedFromParentClass(IMethod method)
        {
            return method.MembersUsed.Where(
                    memberUsed => memberUsed.ParentType == method.ParentType && memberUsed.IsPropertyOrField())
                .ToHashSetEx();
        }

        private static IEnumerable<IMethod> MethodsToConsiderFor(IType type)
        {
            return type.Methods.Where(m => !m.IsAbstract);
        }

        private static IEnumerable<HashSet<IMethod>> PairsFrom(IEnumerable<IMethod> methods)
        {
            var methodList = methods.ToList();
            return methodList.Select(
                    (m1, i) => methodList.Where((m2, j) => j > i).Select(m2 => new HashSet<IMethod> { m1, m2 }))
                .SelectMany(_ => _);
        }

        private static IEnumerable<HashSet<IMethod>> CohesivePairsFrom(IEnumerable<HashSet<IMethod>> pairs)
        {
            return pairs.Where(p => FieldsUsedFromParentClass(p.First()).Overlaps(FieldsUsedFromParentClass(p.Last())));
        }

        private static int NumberOfPairsFor(int n)
        {
            return n * (n - 1) / 2;
        }
    }
}