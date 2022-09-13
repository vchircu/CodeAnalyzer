namespace CodeAnalyzer.StructuralRelations
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using CSharpFunctionalExtensions;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    using Serilog;

    public class StructuralRelationsBuilder
    {
        private readonly ILogger _logger;

        public StructuralRelationsBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<StructuralRelation> GetStructuralRelations(ICodeBase codeBase)
        {
            _logger.Information("\tBuilding Structural Relations");
            var stopWatch = Stopwatch.StartNew();

            HashSet<IType> types = codeBase.Application.Types.ToHashSetEx();
            
            List<Maybe<StructuralRelation>> relations =
                (from source in types from target in types select StructuralRelation.From(source, target)).ToList();

            IEnumerable<StructuralRelation> structuralRelations = relations.Where(r => r.HasValue).Select(r => r.Value).ToList();

            structuralRelations = GroupByFiles(structuralRelations);
            stopWatch.Stop();
            _logger.Information("\tBuilt Structural Relations in {Elapsed:000} ms", stopWatch.ElapsedMilliseconds);

            return structuralRelations;
        }

        private static IEnumerable<StructuralRelation> GroupByFiles(IEnumerable<StructuralRelation> structuralRelations)
        {
            return structuralRelations.GroupBy(r => r.Source + r.Target).Select(
                g => StructuralRelation.Aggregate(g.ToList()));
        }
    }
}