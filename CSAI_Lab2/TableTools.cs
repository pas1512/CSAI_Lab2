using System.Data;
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;
using DescritazedTable = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;

namespace CSAI_Lab2
{
    internal static class TableTools
    {
        public static DescritazedTable Descritaze(this Table rawTable)
        {
            if (rawTable == null || rawTable.Count == 0)
                return null;

            DescritazedTable result = new DescritazedTable(rawTable.Count);

            string[] results = rawTable.Select(r => (string)r.Last().Value).ToArray();
            string[] keys = rawTable[0].Keys.ToArray();
            int rows = rawTable.Count;

            for (int i = 0; i < rows; i++)
                result.Add(new Dictionary<string, string>(keys.Length));

            for (int k = 0; k < keys.Length; k++)
            {
                if (rawTable[0][keys[k]].IsNumericType())
                {
                    (object, string)[] valResPairs = new (object, string)[rows];

                    for (int i = 0; i < rows; i++)
                        valResPairs[i] = (rawTable[i][keys[k]], results[i]);

                    string[] ranges = DescritazeValues(valResPairs);

                    for (int i = 0; i < rows; i++)
                        result[i].Add(keys[k], GetRangeOf(ranges, valResPairs[i].Item1));
                }
                else
                {
                    for (int i = 0; i < rows; i++)
                        result[i].Add(keys[k], (string) rawTable[i][keys[k]]);
                }
            }

            return result;
        }

        private static string[] DescritazeValues((object value, string result)[] dict)
        {
            var result = new List<string>();
            var ordered = dict.OrderBy(d => Convert.ToDouble(d.value)).ToList();
            double[] values = ordered.Select(o => Convert.ToDouble(o.value)).ToArray();
            string[] results = ordered.Select(o => o.result).ToArray();

            double[] mid = new double[values.Length - 1];

            for (int i = 0; i < mid.Length; i++)
                mid[i] = (values[i] + values[i + 1]) / 2;

            string prevClass = results[0];
            double start = mid[0];
            result.Add($"[{double.NegativeInfinity};{start:F2}]");

            for (int i = 0; i < mid.Length; i++)
            {
                if (results[i + 1] != prevClass)
                {
                    result.Add($"[{start:F2};{mid[i]:F2}]");
                    start = mid[i];
                    prevClass = results[i + 1];
                }
            }

            result.Add($"[{start:F2};{double.PositiveInfinity}]");
            return result.ToArray();
        }

        public static string GetRangeOf(string[] ranges, object value)
        {
            double val = Convert.ToDouble(value);

            for (int i = 0; i < ranges.Length; i++)
            {
                string[] minMax = ranges[i].Split(';');
                double min = double.Parse(minMax[0].Trim(' ', '[', ']'));
                double max = double.Parse(minMax[1].Trim(' ', '[', ']'));

                if(min < val && val <= max)
                    return ranges[i];
            }

            return "";
        }

        public static DescritazedTable GetSubtable(this DescritazedTable origin, string key, object value)
        {
            return origin
                .Where(row => row.ContainsKey(key) && row[key].Equals(value))
                .Select(row =>
                {
                    var newRow = new Dictionary<string, string>(row);
                    newRow.Remove(key);
                    return newRow;
                })
                .ToList();
        }

        public static string[] GetKeys(this DescritazedTable table)
        {
            return table[0].Select(row => row.Key).ToArray();
        }

        public static string[] GetColumn(this DescritazedTable table, string key)
        {
            return table.Where(row => row.ContainsKey(key)).Select(row => row[key]).ToArray();
        }

        public static string[] GetResultColumn(this DescritazedTable table)
        {
            return table.Select(row =>
            {
                var values = row.Values.ToArray();
                int valuesCount = values.Length;
                return values[valuesCount - 1];
            }).ToArray();
        }

        public static string[] GetResultColumn(this DescritazedTable table, string key, string value)
        {
            return table
                .Where(r => r.ContainsKey(key) && r[key] == value)
                .Select(r => r.Last().Value)
                .ToArray();
        }
    }
}