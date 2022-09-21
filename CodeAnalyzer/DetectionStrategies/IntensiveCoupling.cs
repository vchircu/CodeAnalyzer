namespace CodeAnalyzer.DetectionStrategies
{
    using System.Collections.Generic;

    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class IntensiveCoupling : IDetectMethodDesignSmell
    {
        public Maybe<DesignSmell> Detect(IMethod m)
        {
            var Shallow = 1;
            var Few = 5;
            var ShortMemoryCap = 7;

            var maxNesting = m.ILNestingDepth.GetValueOrDefault();
            var cint = CouplingIntensity.Value(m);
            var cdisp = CouplingDispersion.Value(m);

            if ((cint > ShortMemoryCap && cdisp < CommonFractionThreshold.Half
                 || cint > Few && cdisp < CommonFractionThreshold.AQuarter) && maxNesting > Shallow)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Intensive Coupling",
                            Severity = CalculateSeverity(cint, cdisp),
                            SourceFile = m.ParentType.SourceFile(),
                            Source = m,
                            Metrics = new Dictionary<string, double>
                                          {
                                              { "cint", cint }, { "cdisp", cdisp }, { "maxNesting", maxNesting }
                                          }
                        });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(int cint, double cdisp)
        {
            var severityCdisp = LinearNormalization.WithMeasurementRange(3, 9).ValueFor(cdisp);
            var severityCint = LinearNormalization.WithMeasurementRange(7, 21).ValueFor(cint);

            return (3 * severityCdisp + severityCint) / 4;
        }
    }
}