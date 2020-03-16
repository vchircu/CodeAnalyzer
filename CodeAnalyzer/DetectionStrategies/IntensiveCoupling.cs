namespace CodeAnalyzer.DetectionStrategies
{
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

            var maxNesting = m.ILNestingDepth;
            var cint = CouplingIntensity.Value(m);
            var cdisp = CouplingDispersion.Value(m);

            if ((cint > ShortMemoryCap && cdisp < CommonFractionThreshold.Half
                 || cint > Few && cdisp < CommonFractionThreshold.AQuarter) && maxNesting > Shallow)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Intensive Coupling",
                            Severity = cint,
                            SourceFile = m.ParentType.SourceFile(),
                            Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}