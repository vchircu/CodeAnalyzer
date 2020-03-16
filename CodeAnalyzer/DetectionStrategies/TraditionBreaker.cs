namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class TraditionBreaker : ClassificationDesignSmellDetectionStrategy
    {
        protected override Maybe<DesignSmell> DetectCore(IType t)
        {
            var amwAverage = 2;
            var wmcVeryHigh = 47;
            var nomAverage = 7;
            var nomHigh = 10;

            var parent = t.BaseClass;

            var nas = NumberOfAddedServices.Value(t);
            var pnas = PercentageOfNewlyAddedServices.Value(t);

            var wmc = WeightedMethodCount.Value(t);
            var wmcParent = WeightedMethodCount.Value(parent);

            var nom = t.NbMethods;
            var nomParent = parent.NbMethods;

            var amw = AverageMethodWeight.Value(t);
            var amwParent = AverageMethodWeight.Value(parent);

            var excessiveIncreaseOfChildClassInterface = nas > nomAverage && pnas > CommonFractionThreshold.TwoThirds;
            var childClassHasSubstantialSizeAndComplexity = (amw > amwAverage || wmc >= wmcVeryHigh) && nom >= nomHigh;
            var parentClassIsNeitherSmallNorDumb =
                amwParent > amwAverage && nomParent > nomHigh / 2 && wmcParent >= wmcVeryHigh / 2;


            if (excessiveIncreaseOfChildClassInterface &&
                childClassHasSubstantialSizeAndComplexity &&
                parentClassIsNeitherSmallNorDumb)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Tradition Breaker",
                            Severity = nas,
                            SourceFile = t.SourceFile(),
                            Source = t
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}