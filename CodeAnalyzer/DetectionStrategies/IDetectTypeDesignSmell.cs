namespace CodeAnalyzer.DetectionStrategies
{
    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    public interface IDetectTypeDesignSmell
    {
        Maybe<DesignSmell> Detect(IType t);
    }
}