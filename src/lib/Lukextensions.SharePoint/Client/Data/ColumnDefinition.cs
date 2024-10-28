using System.Collections.Generic;

namespace Lukextensions.SharePoint
{
    public static class ColumnDefinition
    {
        public static Dictionary<string, object> BoolColumn(string name, bool required = false)
        {
            return Column(name, "boolean", new Dictionary<string, object>(), required);
        }

        public static Dictionary<string, object> TextColumn(string name, bool required = false)
        {
            return Column(name, "text", new Dictionary<string, object>(), required);
        }

        public static Dictionary<string, object> MultilineTextColumn(string name, bool required = false)
        {
            var columnProperties = new Dictionary<string, object>()
            {
                { "allowMultipleLines", true }
            };
            return Column(name, "text", columnProperties, required);
        }

        public static Dictionary<string, object> IntColumn(string name, bool required = false)
        {
            var columnProperties = new Dictionary<string, object>()
            {
                { "decimalPlaces", "none" }
            };
            return Column(name, "number", columnProperties, required);
        }

        public static Dictionary<string, object> NumberColumn(string name, bool required = false)
        {
            return Column(name, "number", new Dictionary<string, object>(), required);
        }

        public static Dictionary<string, object> DateTimeColumn(string name, bool dateOnly = false, bool required = false)
        {
            var columnProperties = new Dictionary<string, object>()
            {
                { "format", dateOnly ? "dateOnly" : "dateTime" }
            };
            return Column(name, "dateTime", columnProperties, required);
        }

        private static Dictionary<string, object> Column(string name, string columnType, Dictionary<string, object> columnProperties, bool required)
        {
            var result = new Dictionary<string, object> {
                { "name", name },
                { columnType, columnProperties }
            };
            if (required)
            {
                result.Add("required", true);
            }
            return result;
        }
    }
}
