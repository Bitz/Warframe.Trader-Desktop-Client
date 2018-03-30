namespace WFTDC
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public static class Extensions
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        public static bool Match<T>(this IEnumerable<T> Container)
        {
            if (!Container.Any())
            {
                return true;
            }

            return false;
        }
    }
}
