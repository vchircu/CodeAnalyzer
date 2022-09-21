namespace CodeAnalyzer.DetectionStrategies
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeAnalyzer.Metrics;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public class ShotgunSurgery : IDetectMethodDesignSmell
    {
        public Maybe<DesignSmell> Detect(IMethod m)
        {
            var ShortMemoryCap = 7;
            var Many = 10;

            var methodsCallingMe = m.MethodsCallingMe.Where(method => !method.IsProperty()).ToHashSetEx();
            var changingClasses = methodsCallingMe.Select(method => method.ParentType).ToHashSetEx();

            var cm = methodsCallingMe.Count;
            var cc = changingClasses.Count;

            if (cm > ShortMemoryCap && cc > Many)
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                        {
                            Name = "Shotgun Surgery", Severity = CalculateSeverity(cm, cc, m), SourceFile = m.ParentType.SourceFile(), Source = m,
                            Metrics = new Dictionary<string, double>
                                          {
                                              { "cm", cm },
                                              { "cc", cc }
                                          }
                    });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(int icio, int icdo, IMethod method)
        {
            var severityIcio = LinearNormalization.WithMeasurementRange(7, 21).ValueFor(icio);
            var severityIcdo = LinearNormalization.WithMeasurementRange(3, 9).ValueFor(icdo);

            var ocio = CouplingIntensity.Value(method);
            var severityOcio = LinearNormalization.WithMeasurementRange(7, 21).ValueFor(ocio);
            
            var ocdo = CouplingDispersion.Value(method);
            var severityOcdo = LinearNormalization.WithMeasurementRange(3, 9).ValueFor(ocdo);

            return (2 * severityIcio + 2 * severityIcdo + severityOcio + severityOcdo) / 6;
        }
    }
}