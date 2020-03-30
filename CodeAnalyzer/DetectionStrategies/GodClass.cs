namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class GodClass : IDetectTypeDesignSmell
    {
        private const int Few = 4;

        public Maybe<DesignSmell> Detect(IType t)
        {
            var wmcVeryHigh = 47;

            var atfd = AccessToForeignDataForType.Value(t);
            var wmc = WeightedMethodCount.Value(t);
            var tcc = TotalClassCohesion.Value(t);

            if (atfd > Few && wmc >= wmcVeryHigh && tcc < CommonFractionThreshold.OneThird)
            {
                return Maybe<DesignSmell>.From(new DesignSmell
                                                   {
                                                       Name = "God Class", Severity = SeverityFor(atfd), SourceFile = t.SourceFile(), Source = t
                                                   });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double SeverityFor(int atfd)
        {
            var linearNormalization = LinearNormalization.WithMeasurementAndDesiredRange(Few, 20, 1, 10);
            return linearNormalization.ValueFor(atfd);
        }
    }
}