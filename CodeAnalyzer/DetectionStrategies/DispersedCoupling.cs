namespace CodeAnalyzer.DetectionStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class DispersedCoupling : IDetectMethodDesignSmell
    {
        public Maybe<DesignSmell> Detect(IMethod m)
        {
            var Shallow = 1;
            var ShortMemoryCap = 7;

            var maxNesting = m.ILNestingDepth;
            var cint = CouplingIntensity.Value(m);
            var cdisp = CouplingDispersion.Value(m);

            if (cint > ShortMemoryCap && cdisp >= CommonFractionThreshold.Half && maxNesting > Shallow)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Dispersed Coupling",
                            Severity = CalculateSeverity(m),
                            SourceFile = m.ParentType.SourceFile(),
                            Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(IMethod method)
        {
            List<Tuple<IType, IList<IMethod>>> relevantCouplingIntensityPerProvider =
                method.CouplingIntensityPerProvider().Where(t => t.Item2.Count >= 7).ToList();

            var relevantOcio = relevantCouplingIntensityPerProvider.Sum(g => g.Item2.Count);
            var severityRelevantOcio = LinearNormalization.WithMeasurementRange(7, 21).ValueFor(relevantOcio);

            var relevantOcdo = relevantCouplingIntensityPerProvider.Count;
            var severityRelevantOcdo = LinearNormalization.WithMeasurementRange(1, 4).ValueFor(relevantOcdo);

            return (2 * severityRelevantOcio + severityRelevantOcdo) / 3;
        }
    }
}