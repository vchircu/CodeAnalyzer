namespace CodeAnalyzer.DetectionStrategies
{
    using System.Collections.Generic;

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
                            Name = "Blob Method", Severity = CalculateSeverity(loc, cyclo, maxNesting), SourceFile = m.ParentType.SourceFile(), Source = m,
                            Metrics = new Dictionary<string, double>
                                          {
                                              { "loc", loc },
                                              { "cyclo", cyclo },
                                              { "noav", noav },
                                              { "maxNesting", maxNesting }
                                          }
                    });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(uint loc, ushort cyclo, ushort maxNesting)
        {
            var severityCyclo = LinearNormalization.WithMeasurementRange(3, 15).ValueFor(cyclo);
            var severityNesting = LinearNormalization.WithMeasurementRange(3, 7).ValueFor(maxNesting);
            var severityLoc = LinearNormalization.WithMeasurementRange(100, 500).ValueFor(loc);

            return (2 * severityCyclo + 2 * severityNesting + severityLoc) / 5;
        }
    }
}