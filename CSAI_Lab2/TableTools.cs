using System.Data;
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;
using AttrRes = (object value, object result);

namespace CSAI_Lab2
{
    internal static class TableTools
    {
        public static AttrRes[] GetAttributeResultSamples(this Table table, string key)
        {
            object[] values = table.GetColumn(key).ToArray();
            object[] results = table.GetResultColumn();
            AttrRes[] input = new AttrRes[results.Length];

            for (int i = 0; i < input.Length; ++i)
                input[i] = new(values[i], results[i]);

            return input;
        }

        public static Table GetSubtable(this Table origin, string key, object value)
        {
            return origin
                .Where(row => row.ContainsKey(key) && row[key].Equals(value))
                .Select(row =>
                {
                    var newRow = new Dictionary<string, object>(row);
                    newRow.Remove(key);
                    return newRow;
                })
                .ToList();
        }

        public static Table GetSubtable(this Table origin, string key, Predicate<object> pred)
        {
            return origin
                .Where(row => row.ContainsKey(key) && pred(row[key]))
                .Select(row =>
                {
                    var newRow = new Dictionary<string, object>(row);
                    newRow.Remove(key);
                    return newRow;
                })
                .ToList();
        }

        public static string[] GetKeys(this Table table)
        {
            return table[0].Select(row => row.Key).ToArray();
        }

        public static object[] GetColumn(this Table table, string key)
        {
            return table.Where(row => row.ContainsKey(key)).Select(row => row[key]).ToArray();
        }

        public static object[] GetResultColumn(this Table table)
        {
            return table.Select(row =>
            {
                var values = row.Values.ToArray();
                int valuesCount = values.Length;
                return values[valuesCount - 1];
            }).ToArray();
        }

        public static object[] GetResultColumn(this Table table, string key, string value)
        {
            return table
                .Where(r => r.ContainsKey(key) && r[key] == value)
                .Select(r => r.Last().Value)
                .ToArray();
        }
    }
}