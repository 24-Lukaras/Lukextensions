using System.Collections.Generic;

namespace Lukextensions.SharePoint
{
    public static class ColumnDefinition
    {
        public static Dictionary<string, object> BoolColumn(string name)
        {
            return new Dictionary<string, object> {
                { "name", name },
                { "boolean", new Dictionary<string, object>() }
            };
        }

        public static Dictionary<string, object> TextColumn(string name)
        {
            return new Dictionary<string, object> { 
                { "name", name },
                { "text", new Dictionary<string, object>() }
            };
        }

        public static Dictionary<string, object> IntColumn(string name)
        {
            return new Dictionary<string, object> {
                { "name", name },
                { "number",
                    new Dictionary<string, object>() 
                    {
                        { "decimalPlace", "none" }
                    }
                }
            };
        }

        public static Dictionary<string, object> NumberColumn(string name)
        {
            return new Dictionary<string, object> { { "name", name }, { "number", new Dictionary<string, object>() } };
        }

        public static Dictionary<string, object> DateColumn(string name)
        {
            return new Dictionary<string, object> {
                { "name", name },
                { "dateTime",
                    new Dictionary<string, object>()
                    {
                        { "format", "dateOnly" }
                    }
                }
            };
        }

        public static Dictionary<string, object> DateTimeColumn(string name)
        {
            return new Dictionary<string, object> {
                { "name", name },
                { "dateTime",
                    new Dictionary<string, object>()
                    {
                        { "format", "dateTime" }
                    }
                }
            };
        }
    }
}
