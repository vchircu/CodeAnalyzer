namespace CodeAnalyzer.DetectionStrategies
{
    using System;

    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class RefusedParentBequest : ClassificationDesignSmellDetectionStrategy
    {
        protected override Maybe<DesignSmell> DetectCore(IType t)
        {
            var Few = 3;
            var amwAverage = 2;
            var wmcAverage = 14;
            var nomAverage = 7;

            var bur = BaseClassUsageRatio.Value(t);
            var bovr = BaseClassOverridingRatio.Value(t);
            var nprotm = NumberOfProtectedMembers.Value(t);
            var wmc = WeightedMethodCount.Value(t);
            var nom = t.NbMethods;
            var amw = AverageMethodWeight.Value(t);

            var childClassIgnoresBequest = nprotm > Few && bur < CommonFractionThreshold.OneThird
                                           || bovr < CommonFractionThreshold.OneThird;
            var childClassIsNotTooSmallAndSimple = (amw > amwAverage || wmc > wmcAverage) && nom > nomAverage;

            if (nprotm > 0 && childClassIgnoresBequest && childClassIsNotTooSmallAndSimple)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Refused Parent Bequest",
                            Severity = Math.Min((double)bur, bovr) * 100,
                            SourceFile = t.SourceFile(),
                            Source = t
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}