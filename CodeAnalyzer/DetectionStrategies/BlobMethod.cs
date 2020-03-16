namespace CodeAnalyzer.DetectionStrategies
{
    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public class BlobMethod : IDetectMethodDesignSmell
    {
        public Maybe<DesignSmell> Detect(IMethod m)
        {
            var Several = 3;
            var Many = 7;
            var cycloPerMethodHigh = 3.1;
            var locPerClassHigh = 100;

            var loc = m.NbLinesOfCode.GetValueOrDefault();
            var cyclo = m.CyclomaticComplexity.GetValueOrDefault();
            var maxNesting = m.ILNestingDepth.GetValueOrDefault();
            var noav = NumberOfAccessedVariables.Value(m);

            if (loc > locPerClassHigh / 2 && cyclo >= cycloPerMethodHigh && maxNesting >= Several && noav > Many)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Blob Method", Severity = cyclo, SourceFile = m.ParentType.SourceFile(), Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}