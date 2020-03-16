namespace CodeAnalyzer
{
    using System.Collections.Generic;
    using System.Linq;

    using NDepend.CodeModel;
    using NDepend.Helpers;

    public static class TypeExtensions
    {
        public static IEnumerable<IType> HierarchyIncludingImplementedInterfaces(this IType type)
        {
            return type.Hierarchy()
                .Concat(type.InterfacesImplemented);
        }

        public static IEnumerable<IType> Hierarchy(this IType type)
        {
            return ExtensionMethodsEnumerable.Append(type.BaseClasses, type).ToHashSetEx();
        }

        public static string SourceFile(this IType type)
        {
            return type.SourceFileDeclAvailable
                       ? type.SourceDecls.FirstOrDefault().SourceFile.FilePathString
                       : string.Empty;
        }
    }
}