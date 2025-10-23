using AttributeInfo = (string attribute, bool isNumber, float cutPoint);
using BrancheInfo = (string value, System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>> subtable);
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;

namespace CSAI_Lab2.DecisionTreeSystem
{
    internal class DecisionTreeGenerator
    {
        public static DecisionTreeNode GetTree(Table table)
        {
            string[] keys = table.GetKeys();
            object[] results = table.GetResultColumn();
            object[] uniques = GetUniques(results);

            if (uniques.Length == 1)
            {
                return new((string)uniques[0], []);
            }
            else if (keys.Length == 1)
            {
                float[] numbers = CalculateValues(results);
                UnevenSelector unevenSelector = new UnevenSelector(uniques, numbers);
                return new((string)unevenSelector.GetRandom(), []);
            }

            AttributeInfo selected = GetBestAttribute(table);
            BrancheInfo[] branches = GetBranches(table, selected);
            DecisionTreeNode node = new DecisionTreeNode(selected.attribute, branches.Select(b => b.value).ToArray());

            for (int i = 0; i < branches.Length; i++)
            {
                DecisionTreeNode next = GetTree(branches[i].subtable);
                node.SetNextNode(branches[i].value, next);
            }

            return node;
        }

        private static float[] CalculateValues(object[] values)
        {
            Dictionary<object, float> results = new Dictionary<object, float>();

            for (int i = 0; i < values.Length; i++)
            {
                object v = values[i];

                if (results.TryGetValue(v, out float count))
                    results[v] = count + 1;
                else
                    results[v] = 1;
            }

            return results.Values.ToArray();
        }

        private static object[] GetUniques(object[] values)
        {
            List<object> result = new List<object>();

            for (int i = 0; i < values.Length; i++)
            {
                object v = values[i];

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

        private static float GetWeightedEntropyPart(Table table, string key, Predicate<float> predicate)
        {
            Table subtable = table.GetSubtable(key, predicate);
            object[] results = subtable.GetResultColumn();
            float[] counts = CalculateValues(results);
            return counts.Sum() / table.Count * GetEntropy(counts);
        }

        private static float GetInfoGein(Table table, string key, out float bestCutPoint)
        {
            var attrRes = table.GetAttributeResultSamples(key);
            var orderedAttrRes = attrRes.OrderBy(ar => (float)ar.value).ToArray();
            float[] cutPoints = Utilities.GetMids(orderedAttrRes.Select(o => (float)o.value).ToArray());

            object[] results = table.GetResultColumn();
            float[] counts = CalculateValues(results);
            float entropyBefore = GetEntropy(counts);
            float maxInfoGain = 0;
            bestCutPoint = 0;

            foreach (var cutPoint in cutPoints)
            {
                float leftEntrpyPart = GetWeightedEntropyPart(table, key, v => v <= cutPoint);
                float rightEntrpyPart = GetWeightedEntropyPart(table, key, v => v > cutPoint);
                float entropyAfter = leftEntrpyPart + rightEntrpyPart;
                float currentInfoGain = entropyBefore - entropyAfter;

                if (currentInfoGain > maxInfoGain)
                {
                    maxInfoGain = currentInfoGain;
                    bestCutPoint = cutPoint;
                }
            }

            return maxInfoGain;
        }

        private static float GetInfoGein(Table table, string key)
        {
/*            if (table.GetKeys().Last() == key)
                return -100;*/

            object[] results = table.GetResultColumn();
            float[] counts = CalculateValues(results);
            float before = GetEntropy(counts);

            object[] values = table.GetColumn(key);
            object[] uniqValues = GetUniques(values);
            float sum = counts.Sum();
            float after = 0;

            for (int i = 0; i < uniqValues.Length; i++)
            {
                Table subtable = table.GetSubtable(key, uniqValues[i]);
                object[] subResults = subtable.GetResultColumn();
                float[] subCounts = CalculateValues(subResults);
                float weight = subCounts.Sum() / sum;
                after += weight * GetEntropy(subCounts);
            }

            return before - after;
        }

        private static AttributeInfo GetBestAttribute(Table table)
        {
            string[] keys = table.GetKeys();
            float maxGain = -1;
            string currentKey = "";
            float currentCutPoint = 0;
            bool currentIsNumber = false;

            for(int i = 0; i < keys.Length - 1; i++)
            {
                bool isNumber = table[0][keys[i]].IsNumericType();
                float currentGain = 0;
                float cutPoint = 0;

                if (isNumber)
                    currentGain = GetInfoGein(table, keys[i], out cutPoint);
                else
                    currentGain = GetInfoGein(table, keys[i]);

                if(currentGain > maxGain)
                {
                    maxGain = currentGain;
                    currentKey = keys[i];
                    currentIsNumber = isNumber;
                    currentCutPoint = cutPoint;
                }
            }

            return new (currentKey, currentIsNumber, currentCutPoint);
        }
    
        private static BrancheInfo[] GetBranches(Table table, AttributeInfo selected)
        {
            if (selected.isNumber)
            {
                string left = $"[{float.NegativeInfinity}; {selected.cutPoint:F2}]";
                string right = $"[{selected.cutPoint:F2}; {float.PositiveInfinity}]";
                return [(left, table.GetSubtable(selected.attribute, v => (float)v <= selected.cutPoint)), 
                    (right, table.GetSubtable(selected.attribute, v => (float)v > selected.cutPoint))];
            }
            else
            {
                object[] targetValues = table.GetColumn(selected.attribute);
                object[] uniqueValues = GetUniques(targetValues);
                BrancheInfo[] branches = new BrancheInfo[uniqueValues.Length];

                for (int i = 0; i < uniqueValues.Length; i++)
                {
                    Table subtable = table.GetSubtable(selected.attribute, uniqueValues[i]);
                    branches[i] = new BrancheInfo((string)uniqueValues[i], subtable);
                }

                return branches;
            }
        }
    }
}