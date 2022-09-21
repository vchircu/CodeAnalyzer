namespace CodeAnalyzer.DetectionStrategies
{
    using System.Collections.Generic;
    using System.Linq;

    using CodeAnalyzer.Metrics;
    using CodeAnalyzer.Thresholds;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public class DataClass : IDetectTypeDesignSmell
    {
        private const int Few = 4;

        public Maybe<DesignSmell> Detect(IType t)
        {
            var woc = WeightOfAClass.Value(t);
            var wmc = WeightedMethodCount.Value(t);
            IList<IMember> publicAttributes = t.PublicAttributes().ToList();
            var nopa = publicAttributes.Count;
            IList<IMember> accessors = t.Accessors().ToList();
            var noam = accessors.Count;

            if (woc < CommonFractionThreshold.OneThird
                && (nopa + noam > Few && wmc < 31 || nopa + noam > 8 && wmc < 47))
            {
                return Maybe<DesignSmell>.From(
                    new DesignSmell
                    {
                        Name = "Data Class",
                        Severity = CalculateSeverity(publicAttributes, accessors, t),
                        SourceFile = t.SourceFile(),
                        Source = t,
                        Metrics = new Dictionary<string, double>
                                          {
                                              { "woc", woc }, { "wmc", wmc }, { "nopa", nopa }, { "noam", noam }
                                          }
                    });
            }

            return Maybe<DesignSmell>.None;
        }

        private static double CalculateSeverity(IList<IMember> publicAttributes, IList<IMember> accessors, IType type)
        {
            var severityExploit = SeverityExploit(publicAttributes, accessors, type);

            var severityExposure = SeverityExposure(publicAttributes, accessors);

            return (2 * severityExploit + severityExposure) / 3;
        }

        private static double SeverityExposure(IList<IMember> publicAttributes, IList<IMember> accessors)
        {
            return LinearNormalization.WithMeasurementRange(Few, 20).ValueFor(publicAttributes.Count + accessors.Count);
        }

        private static double SeverityExploit(IList<IMember> publicAttributes, IList<IMember> accessors, IType type)
        {
            HashSet<IMethod> methodsUsingPublicAttributes = publicAttributes.Select(pa => pa.AsField)
                .SelectMany(f => f.MethodsUsingMe).ToHashSetEx();
            HashSet<IMethod> methodsUsingAccessors =
                accessors.Select(a => a.AsMethod).SelectMany(m => m.MethodsCallingMe).ToHashSetEx();

            var methodsUsingData = methodsUsingAccessors.Concat(methodsUsingPublicAttributes);

            HashSet<IType> classesUsingPublicData = methodsUsingData.Select(m => m.ParentType).ToHashSetEx();
            classesUsingPublicData.Remove(type);

            return LinearNormalization.WithMeasurementRange(2, 10).ValueFor(classesUsingPublicData.Count);
        }
    }
}