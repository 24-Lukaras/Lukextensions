using System.Text.RegularExpressions;

namespace Lukextensions.Shared
{
    public static class TextHelper
    {
        public static string PluralizeListName(string listName)
        {
            var iesRegex = new Regex(@"^.*[^aeiouy]y$");
            if (iesRegex.IsMatch(listName))
            {
                return $"{listName.Substring(0, listName.Length - 1)}ies";
            }

            string[] esSufixEndings = new string[] { "s", "ss", "sh", "ch", "x", "z" };
            foreach (var sufix in esSufixEndings)
            {
                if (listName.EndsWith(sufix))
                    return $"{listName}es";
            }

            return $"{listName}s";
        }
    }
}
