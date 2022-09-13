namespace CodeAnalyzer.StructuralRelations
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public class StructuralRelation
    {
        public int HierarchyRelations { get; private set; }

        public string Source { get; private set; }

        public string Target { get; private set; }

        public int NumberOfCallsToMethods { get; private set; }

        public int NumberOfCallsToPropertiesOrFields { get; private set; }

        public static StructuralRelation Aggregate(IEnumerable<StructuralRelation> structuralRelations)
        {
            List<StructuralRelation> relations = structuralRelations.ToList();
            return new StructuralRelation
                       {
                           Source = relations.First().Source,
                           Target = relations.First().Target,
                           HierarchyRelations = relations.Sum(r => r.HierarchyRelations),
                           NumberOfCallsToMethods = relations.Sum(r => r.NumberOfCallsToMethods),
                           NumberOfCallsToPropertiesOrFields = relations.Sum(r => r.NumberOfCallsToPropertiesOrFields)
                       };
        }

        public static Maybe<StructuralRelation> From(IType source, IType target)
        {
            if (source == target)
            {
                return Maybe<StructuralRelation>.None;
            }

            var sourceSourceFile = source.SourceFile();
            var targetSourceFile = target.SourceFile();

            if (string.IsNullOrWhiteSpace(sourceSourceFile) || string.IsNullOrWhiteSpace(targetSourceFile) || sourceSourceFile == targetSourceFile)
            {
                return Maybe<StructuralRelation>.None;
            }

            var isInnerClass = source.ChildTypes.Contains(target);

            HashSet<IMethod> hierarchySpecificRelatedMethods = HierarchySpecificRelatedMethods(source, target);
            var externalCalls = isInnerClass
                                    ? 0
                                    : target.Methods.Count(
                                        m => m.IsFunctional() && source.IsUsingMethod(m)
                                                              && !hierarchySpecificRelatedMethods.Contains(m));

            HashSet<IMember> hierarchyDataAccess = HierarchyDataAccess(source, target);
            var externalData = isInnerClass
                                   ? 0
                                   : target.Members.Count(
                                       m => m.IsPropertyOrField() && source.IsUsing(m)
                                                                  && !hierarchyDataAccess.Contains(m));

            var hierarchySpecificRelations = HierarchySpecificRelations(
                source,
                target,
                hierarchySpecificRelatedMethods,
                hierarchyDataAccess);

            if (hierarchySpecificRelations == 0 && externalCalls == 0 && externalData == 0)
            {
                return Maybe<StructuralRelation>.None;
            }

            return Maybe<StructuralRelation>.From(
                new StructuralRelation
                    {
                        HierarchyRelations = hierarchySpecificRelations,
                        NumberOfCallsToMethods = externalCalls,
                        NumberOfCallsToPropertiesOrFields = externalData,
                        Source = sourceSourceFile,
                        Target = targetSourceFile
                    });
        }

        private static int HierarchySpecificRelations(
            IType source,
            IType target,
            HashSet<IMethod> hierarchySpecificRelatedMethods,
            HashSet<IMember> hierarchyDataAccess)
        {
            var isBaseClass = source.BaseClasses.Contains(target);
            var isImplementedInterface = source.InterfacesImplemented.Contains(target);
            var hierarchySpecificRelations = (isBaseClass ? 1 : 0) + (isImplementedInterface ? 1 : 0)
                                                                   + hierarchySpecificRelatedMethods.Count
                                                                   + hierarchyDataAccess.Count;
            return hierarchySpecificRelations;
        }

        private static HashSet<IMethod> HierarchySpecificRelatedMethods(IType source, IType target)
        {
            return AllBaseOverriddenMethods(source, target).Concat(HierarchyMethodCalls(source, target)).ToHashSetEx();
        }

        private static HashSet<IMember> HierarchyDataAccess(IType source, IType target)
        {
            return source.Methods.SelectMany(m => m.MembersUsed)
                .Where(m => m.ParentType == target && m.IsPropertyOrField() && m.IsProtected).ToHashSetEx();
        }

        private static HashSet<IMethod> HierarchyMethodCalls(IType source, IType target)
        {
            return source.Methods.SelectMany(m => m.MethodsCalled)
                .Where(m => m.ParentType == target && m.IsProtected && m.IsFunctional()).ToHashSetEx();
        }

        private static HashSet<IMethod> AllBaseOverriddenMethods(IType source, IType target)
        {
            return source.Methods.SelectMany(m => m.OverriddensBase).Where(m => m.ParentType == target).ToHashSetEx();
        }
    }
}