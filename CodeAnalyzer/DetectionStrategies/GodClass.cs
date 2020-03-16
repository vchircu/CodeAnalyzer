namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class GodClass : IDetectTypeDesignSmell
    {
        public Maybe<DesignSmell> Detect(IType t)
        {
            var atfd = AccessToForeignDataForType.Value(t);
            var wmc = WeightedMethodCount.Value(t);
            var tcc = TotalClassCohesion.Value(t);

            if (atfd > 5 && wmc >= 47 && tcc < CommonFractionThreshold.OneThird)
            {
                return Maybe<DesignSmell>.From(new DesignSmell
                                                   {
                                                       Name = "God Class", Severity = atfd, SourceFile = t.SourceFile(), Source = t
                                                   });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}