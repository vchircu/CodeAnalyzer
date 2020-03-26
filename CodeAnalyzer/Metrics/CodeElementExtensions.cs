namespace CodeAnalyzer.Metrics
{
    using NDepend.CodeModel;

    public static class CodeElementExtensions
    {
        public static bool IsProperty(this ICodeElement codeElement)
        {
            if (!codeElement.IsMethod)
            {
                return false;
            }

            var method = codeElement.AsMethod;
            return method.IsPropertyGetter || method.IsPropertySetter;
        }

        public static bool IsFunctional(this ICodeElement codeElement)
        {
            if (!codeElement.IsMethod)
            {
                return false;
            }

            var method = codeElement.AsMethod;

            return !method.IsProperty() && !method.IsConstructor;
        }

        public static bool IsPropertyOrField(this ICodeElement codeElement)
        {
            return codeElement.IsProperty() || codeElement.IsField;
        }
    }
}