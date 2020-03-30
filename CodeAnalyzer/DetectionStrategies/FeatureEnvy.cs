namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class FeatureEnvy : IDetectMethodDesignSmell
    {
        private const int Few = 4;

        public Maybe<DesignSmell> Detect(IMethod m)
        {
            var atfd = AccessToForeignDataForMethod.Value(m);
            var laa = LocalityOfAttributeAccess.Value(m);
            var fdp = ForeignDataProviders.Value(m);

            if (atfd > Few && laa < CommonFractionThreshold.OneThird && fdp > 0 && fdp <= 3)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Feature Envy", Severity = CalculateSeverity(atfd), SourceFile = m.ParentType.SourceFile(), Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(int atfd)
        {
            return LinearNormalization.WithMeasurementRange(Few, 20).ValueFor(atfd);
        }
    }
}