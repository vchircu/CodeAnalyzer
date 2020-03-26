namespace CodeAnalyzer.StructuralRelations
{
    using System.Linq;

    using CodeAnalyzer;
    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class StructuralRelation
    {
        public int HierarchyRelations { get; private set; }
        public string Source { get; private set; }
        public string Target { get; private set; }
        public int NumberOfCallsToMethods { get; private set; }
        public int NumberOfCallsToPropertiesOrFields { get; private set; }

        public static Maybe<StructuralRelation> From(IType source, IType target)
        {
            if (source == target)
            {
                return Maybe<StructuralRelation>.None;
            }

            var sourceSourceFile = source.SourceFile();
            var targetSourceFile = target.SourceFile();

            if (string.IsNullOrWhiteSpace(sourceSourceFile) || string.IsNullOrWhiteSpace(targetSourceFile))
            {
                return Maybe<StructuralRelation>.None;
            }

            int hierarchySpecificRelations = ComputeHierarchyRelations(source, target);
            var externalCalls = hierarchySpecificRelations > 0 ? 0 : target.Methods.Count(m => !m.IsPropertyOrField() && source.IsUsingMethod(m));
            var externalData = hierarchySpecificRelations > 0 ? 0 : target.Members.Count(m => m.IsPropertyOrField() && source.IsUsing(m));

            if (hierarchySpecificRelations == 0 && externalCalls == 0 && externalData == 0)
            {
                return Maybe<StructuralRelation>.None;
            }

            return Maybe<StructuralRelation>.From(new StructuralRelation
            {
                HierarchyRelations = hierarchySpecificRelations,
                NumberOfCallsToMethods = externalCalls,
                NumberOfCallsToPropertiesOrFields = externalData,
                Source = sourceSourceFile,
                Target = targetSourceFile
            });
        }

        private static int ComputeHierarchyRelations(IType source, IType target)
        {
            return source.HierarchyIncludingImplementedInterfaces().Contains(target) ? 0 : 1;
        }
    }
}