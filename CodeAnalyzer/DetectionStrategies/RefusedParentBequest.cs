namespace CodeAnalyzer.DetectionStrategies
{
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
                            Severity = CalculateSeverity(bur, bovr),
                            SourceFile = t.SourceFile(),
                            Source = t
                        });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(double bur, double bovr)
        {
            var severityBur = LinearNormalization.WithMeasurementRange(0.75, 1).ValueFor(1 - bur);
            var severityBovr = LinearNormalization.WithMeasurementRange(0.75, 1).ValueFor(1 - bovr);

            return (severityBur + severityBovr) / 2;
        }
    }
}