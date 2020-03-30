﻿namespace CodeAnalyzer.DetectionStrategies
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
                                                       Name = "God Class", Severity = CalculateSeverity(atfd), SourceFile = t.SourceFile(), Source = t
                                                   });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(int atfd)
        {
            var linearNormalization = LinearNormalization.WithMeasurementRange(Few, 20);
            return linearNormalization.ValueFor(atfd);
        }
    }
}