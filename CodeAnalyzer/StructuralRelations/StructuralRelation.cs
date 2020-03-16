namespace CodeAnalyzer.StructuralRelations
{
    using System.Linq;

    using CodeAnalyzer;
    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class StructuralRelation
    {
        public bool IsTargetPartOfSourcesHierarchy { get; private set; }
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

            var isHierarchy = source.HierarchyIncludingImplementedInterfaces().Contains(target);
            var externalCalls = isHierarchy ? 0 : target.Methods.Count(m => !m.IsPropertyOrField() && source.IsUsingMethod(m));
            var externalData = isHierarchy ? 0 : target.Members.Count(m => m.IsPropertyOrField() && source.IsUsing(m));

            if (!isHierarchy && externalCalls == 0 && externalData == 0)
            {
                return Maybe<StructuralRelation>.None;
            }

            return Maybe<StructuralRelation>.From(new StructuralRelation
            {
                IsTargetPartOfSourcesHierarchy = isHierarchy,
                NumberOfCallsToMethods = externalCalls,
                NumberOfCallsToPropertiesOrFields = externalData,
                Source = sourceSourceFile,
                Target = targetSourceFile
            });
        }
    }
}