namespace CodeAnalyzer.DetectionStrategies
{
    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public interface IDetectMethodDesignSmell
    {
        Maybe<DesignSmell> Detect(IMethod m);
    }
}