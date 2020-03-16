namespace CodeAnalyzer.DetectionStrategies
{
    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public abstract class ClassificationDesignSmellDetectionStrategy : IDetectTypeDesignSmell
    {
        public Maybe<DesignSmell> Detect(IType t)
        {
            return ShouldSkip(t) ? Maybe<DesignSmell>.None : DetectCore(t);
        }

        protected abstract Maybe<DesignSmell> DetectCore(IType type);

        private static bool ShouldSkip(IType type)
        {
            if (!type.IsClass)
            {
                return true;
            }

            return !HasParent(type);
        }

        private static bool HasParent(IType t)
        {
            return t.DepthOfInheritance.HasValue && t.DepthOfInheritance > 1;
        }
    }
}