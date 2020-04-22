namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class TraditionBreaker : ClassificationDesignSmellDetectionStrategy
    {
        protected override Maybe<DesignSmell> DetectCore(IType t)
        {
            var nopOverridingMethods = NopOverridingMethods.Value(t);

            if (nopOverridingMethods > 0)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Tradition Breaker",
                            Severity = CalculateSeverity(nopOverridingMethods),
                            SourceFile = t.SourceFile(),
                            Source = t
                        });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(int nopOverridingMethods)
        {
            return LinearNormalization.WithMeasurementRange(1, 10).ValueFor(2 * nopOverridingMethods);
        }
    }
}