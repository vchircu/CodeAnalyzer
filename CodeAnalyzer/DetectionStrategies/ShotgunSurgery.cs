namespace CodeAnalyzer.DetectionStrategies
{
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
                            Name = "Shotgun Surgery", Severity = cm, SourceFile = m.ParentType.SourceFile(), Source = m
                        });
            }

            return Maybe<DesignSmell>.None;
        }
    }
}