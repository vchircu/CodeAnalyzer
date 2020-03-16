namespace CodeAnalyzer.DetectionStrategies
{
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
                            Severity = cint,
                            SourceFile = m.ParentType.SourceFile(),
                            Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}