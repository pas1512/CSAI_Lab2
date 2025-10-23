using System;
using System.Data;
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;


namespace CSAI_Lab2
{
    internal static class Utilities
    {
        public static string GetSpace(int number)
        {
            string res = "";

            for (int i = 0; i < number; i++)
                res += " ";

            return res;
        }

        public static bool IsNumericType(this object value)
        {
            if (value == null) return false;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static int GetWordsMaxLength(Table table, string key)
        {
            int lines = table.Count;
            string[] vls = [..table.Select(row => row[key].ToString())];
            int max = 0;

            for (int j = 0; j < lines; j++)
            {
                int l = vls[j].Length;
                if (l > max) max = l;
            }

            int nameSize = key.Length;
            return nameSize > max ? nameSize : max;
        }

        public static string ParceToString(this Table table)
        {
            string[] keys = table[0].Keys.ToArray();
            int columnsCount = keys.Length;
            int linesCount = table.Count;

            int[] spaceSizes = new int[columnsCount];
            string result = "|id    ";

            for (int i = 0; i < columnsCount; i++)
            {
                string key = keys[i];
                int size = GetWordsMaxLength(table, key);
                spaceSizes[i] = size;
                result += $"|{key} " + GetSpace(size - key.Length);
            }

            for (int i = 0; i < linesCount; i++)
            {
                string number = i.ToString();
                result += $"\n|{number}" + GetSpace(6 - number.ToString().Length);

                for (int j = 0; j < columnsCount; j++)
                {
                    string[] values = [..table.Select(row => row[keys[j]].ToString())];
                    result += $"|{values[i]} " + GetSpace(spaceSizes[j] - values[i].Length);
                }
            }

            return result;
        }

        public static Table ParceToTable(this string data)
        {
            string[] lines = data.Split('\n');
            string[] keys = lines[0].Split('|').Select(s => s.Trim()).ToArray();
            string[][] matrix = new string[keys.Length - 2][];
            Table result = new Table(lines.Length);

            for (int i = 0; i < keys.Length - 2; i++)
                matrix[i] = new string[lines.Length - 1];

            for (int l = 1; l < lines.Length; l++)
            {
                result.Add(new Dictionary<string, object>(keys.Length - 2));
                string[] rowData = lines[l].Split('|');

                for (int k = 2; k < keys.Length; k++)
                    result[l - 1].Add(keys[k], rowData[k].Trim());
            }

            return result;
        }

        public static void Save(string path, Table table)
        {
            string data = table.ParceToString();
            File.WriteAllText(path, data);
        }

        public static Table Load(string path)
        {
            string strData = File.ReadAllText(path); 
            return strData.ParceToTable();
        }
    
        public static float[] GetMids(float[] values)
        {
            float[] result = new float[values.Length - 1];

            for(int i = 0;i < values.Length - 1;i++)
                result[i] = (values[i] + values[i + 1]) /2;

            return result;
        }

        public static string SelectRange(this object value, string[] ranges)
        {
            double val = Convert.ToDouble(value);

            for (int i = 0; i < ranges.Length; i++)
            {
                string[] minMax = ranges[i].Split(';');
                double min = double.Parse(minMax[0].Trim(' ', '[', ']'));
                double max = double.Parse(minMax[1].Trim(' ', '[', ']'));

                if (min < val && val <= max)
                    return ranges[i];
            }

            return "";
        }

    }
}
