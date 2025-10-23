using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;
using DescritazedTable = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;


namespace CSAI_Lab2.DecisionTreeSystem
{
    internal class DecisionTreeGenerator
    {
        public static DecisionTreeNode GetTree(Table table) => GetTree(table.Descritaze());

        private static DecisionTreeNode GetTree(DescritazedTable table)
        {
            string[] keys = table.GetKeys();
            string[] results = table.GetResultColumn();
            string[] uniques = GetUniques(results);

            if (uniques.Length == 1)
            {
                DecisionTreeNode leaf = new("leaf", []);
                leaf.SetLeaf(uniques[0]);
                return leaf;
            }
            else if (keys.Length == 1)
            {
                float[] numbers = CalculateValues(results);
                UnevenSelector unevenSelector = new UnevenSelector(uniques, numbers);
                DecisionTreeNode leaf = new("leaf", []);
                leaf.SetLeaf(unevenSelector.GetRandom());
                return leaf;
            }

            string targetKey = keys.OrderBy(k => GetInfoGein(table, k)).Last();
            string[] targetValues = table.GetColumn(targetKey);
            string[] uniqueValues = GetUniques(targetValues);
            DecisionTreeNode node = new DecisionTreeNode(targetKey, uniqueValues);

            for (int i = 0; i < uniqueValues.Length; i++)
            {
                DescritazedTable subtable = table.GetSubtable(targetKey, uniqueValues[i]);
                DecisionTreeNode nextNode = GetTree(subtable);
                node.SetNextNode(uniqueValues[i], nextNode);
            }

            return node;
        }

        private static float[] CalculateValues(string[] values)
        {
            Dictionary<string, float> results = new Dictionary<string, float>();

            for (int i = 0; i < values.Length; i++)
            {
                string v = values[i];

                if (results.TryGetValue(v, out float count))
                    results[v] = count + 1;
                else
                    results[v] = 1;
            }

            return results.Values.ToArray();
        }

        private static string[] GetUniques(string[] values)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < values.Length; i++)
            {
                string v = values[i];

                if (!result.Contains(v))
                    result.Add(v);
            }

            return result.ToArray();
        }

        private static float GetEntropy(float[] values)
        {
            float entropy = 0;
            float valuesSum = values.Sum();

            for (int i = 0; i < values.Length; ++i)
            {
                float val = values[i];

                if (val > 0)
                {
                    float p_c = val / valuesSum;
                    entropy += p_c * MathF.Log2(p_c);
                }
            }

            return -entropy;
        }

        private static float GetInfoGein(DescritazedTable table, string key)
        {
            if (table.GetKeys().Last() == key)
                return 0;

            string[] results = table.GetResultColumn();
            float[] counts = CalculateValues(results);
            float before = GetEntropy(counts);

            string[] values = table.GetColumn(key);
            string[] uniqValues = GetUniques(values);
            float sum = counts.Sum();
            float after = 0;

            for (int i = 0; i < uniqValues.Length; i++)
            {
                DescritazedTable subtable = table.GetSubtable(key, uniqValues[i]);
                string[] subResults = subtable.GetResultColumn();
                float[] subCounts = CalculateValues(subResults);
                float weight = subCounts.Sum() / sum;
                after += weight * GetEntropy(subCounts);
            }

            return before - after;
        } 
    }
}