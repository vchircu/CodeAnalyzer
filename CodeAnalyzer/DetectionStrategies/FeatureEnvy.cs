namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class FeatureEnvy : IDetectMethodDesignSmell
    {
        public Maybe<DesignSmell> Detect(IMethod m)
        {
            var atfd = AccessToForeignDataForMethod.Value(m);
            var laa = LocalityOfAttributeAccess.Value(m);
            var fdp = ForeignDataProviders.Value(m);

            if (atfd > 5 && laa < 0.33 && fdp > 0 && fdp <= 3)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Feature Envy", Severity = atfd, SourceFile = m.ParentType.SourceFile(), Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}