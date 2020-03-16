namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class DataClass : IDetectTypeDesignSmell
    {
        public Maybe<DesignSmell> Detect(IType t)
        {
            var woc = WeightOfAClass.Value(t);
            var wmc = WeightedMethodCount.Value(t);
            var nopa = NumberOfPublicAttributes.Value(t);
            var noam = NumberOfAccessorMethods.Value(t);

            if (woc < CommonFractionThreshold.OneThird
                && (nopa + noam > 5 && wmc < 31
                    || nopa + noam > 8 && wmc < 47))
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Data Class", Severity = nopa + noam, SourceFile = t.SourceFile(), Source = t
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}