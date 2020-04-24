namespace CodeAnalyzer.DetectionStrategies
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;

    using Serilog;

    public class DesignSmellsDetection
    {
        private readonly IList<IDetectTypeDesignSmell> _typeDesignSmellDetectionStrategies =
            new List<IDetectTypeDesignSmell>
                {
                    new GodClass(), new DataClass(), new RefusedParentBequest(), new TraditionBreaker()
                };

        private readonly IList<IDetectMethodDesignSmell> _methodDesignSmellDetectionStrategies =
            new List<IDetectMethodDesignSmell>
                {
                    new FeatureEnvy(),
                    new BlobMethod(),
                    new IntensiveCoupling(),
                    new DispersedCoupling(),
                    new ShotgunSurgery()
                };

        private readonly ILogger _logger;

        public DesignSmellsDetection(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<DesignSmell> Detect(ICodeBase codeBase)
        {
            IEnumerable<DesignSmell> typeDesignSmells = DetectTypeDesignSmells(codeBase);

            IEnumerable<DesignSmell> methodDesignSmells = DetectMethodDesignSmells(codeBase);

            return typeDesignSmells.Concat(methodDesignSmells);
        }

        private static IEnumerable<DesignSmell> GroupByDesignSmellAndSourceFile(List<DesignSmell> methodDesignSmells)
        {
            return methodDesignSmells.GroupBy(s => s.SourceFile + s.Name).Select(
                g => new DesignSmell
                         {
                             SourceFile = g.First().SourceFile,
                             Name = g.First().Name,
                             Severity = g.Sum(d => d.Severity),
                             Source = g.First().Source
                         });
        }

        private static bool IsDefaultConstructorGeneratedByCompiler(IMethod m)
        {
            return m.IsConstructor && m.NbParameters == 0 && (m.IsPublic || m.IsProtected && m.ParentType.IsAbstract)
                   && !m.SourceFileDeclAvailable;
        }

        private static bool IsGenerated(ICodeBase codeBase, IType type)
        {
            var generatedCodeAttributeType = codeBase.ThirdParty.Types
                .WithFullName("System.CodeDom.Compiler.GeneratedCodeAttribute").SingleOrDefault();
            return generatedCodeAttributeType != null && type.HasAttribute(generatedCodeAttributeType);
        }

        private IEnumerable<DesignSmell> DetectMethodDesignSmells(ICodeBase codeBase)
        {
            var methodDesignSmells = new List<DesignSmell>();
            var stopWatch = Stopwatch.StartNew();
            _logger.Information("\tDetecting Design Smells in methods");
            foreach (var m in codeBase.Application.Methods)
            {
                _logger.Debug("\t\t" + m.FullName);

                if (string.IsNullOrWhiteSpace(m.ParentType.SourceFile()))
                {
                    continue;
                }

                if (IsGenerated(codeBase, m.ParentType))
                {
                    continue;
                }

                if (IsDefaultConstructorGeneratedByCompiler(m))
                {
                    continue;
                }

                foreach (var methodDesignSmellDetectionStrategy in _methodDesignSmellDetectionStrategies)
                {
                    Maybe<DesignSmell> designSmell = methodDesignSmellDetectionStrategy.Detect(m);
                    if (designSmell.HasValue)
                    {
                        methodDesignSmells.Add(designSmell.Value);
                    }
                }
            }

            stopWatch.Stop();
            _logger.Information(
                "\tDetected Design Smells in methods in {Elapsed:000} ms",
                stopWatch.ElapsedMilliseconds);

            IEnumerable<DesignSmell> groupedDesignSmells = GroupByDesignSmellAndSourceFile(methodDesignSmells);
            return groupedDesignSmells;
        }

        private IEnumerable<DesignSmell> DetectTypeDesignSmells(ICodeBase codeBase)
        {
            _logger.Information("\tDetecting Design Smells in types");

            var stopWatch = Stopwatch.StartNew();

            var typeDesignSmells = new List<DesignSmell>();
            foreach (var t in codeBase.Application.Types)
            {
                _logger.Debug("\t\t" + t.FullName);

                if (string.IsNullOrWhiteSpace(t.SourceFile()))
                {
                    continue;
                }

                if (IsGenerated(codeBase, t))
                {
                    continue;
                }

                foreach (var typeDesignSmellDetectionStrategy in _typeDesignSmellDetectionStrategies)
                {
                    Maybe<DesignSmell> designSmell = typeDesignSmellDetectionStrategy.Detect(t);
                    if (designSmell.HasValue)
                    {
                        typeDesignSmells.Add(designSmell.Value);
                    }
                }
            }

            stopWatch.Stop();

            _logger.Information("\tDetected Design Smells in types in {Elapsed:000} ms", stopWatch.ElapsedMilliseconds);
            return typeDesignSmells;
        }
    }
}