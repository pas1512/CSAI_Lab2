struct DataSet
{
    private IAttributeColumn[] _attributes;

    public DataSet(params IAttributeColumn[] attributes)
    {
        _attributes = attributes;
    }

    public string[] GetValuesByPath(Dictionary<string, string> path)
    {
        IAttributeColumn target = _attributes[_attributes.Length - 1];

        if (path == null || path.Count == 0)
            return target.values;

        List<string> result = new List<string>(target.valuesCount);
        var values = target.values;

        for (int i = 0; i < values.Length; i++)
        {
            bool isSearched = true;

            for (int j = 0; j < _attributes.Length; j++)
            {
                string name = _attributes[j].name;

                if (path.ContainsKey(name) && path[name] != _attributes[j].values[i])
                {
                    isSearched = false;
                    break;
                }
            }

            if (isSearched) result.Add(values[i]);
        }

        return result.ToArray();
    }

    public float GetInfoGein(Dictionary<string, string> path = null)
    {
        string[] values = GetValuesByPath(path);
        float[] counts = DataSetTools.CalculateValues(values);
        float before = DataSetTools.GetEntropy(counts);
        float sum = counts.Sum();

        for (int i = 0; i < 5; i++)
        {

        }

        return 0;
    }

    public static bool TryParce(string data, out DataSet result)
    {
        string[] lines = data.Split('\n');
        string[] names = lines[0].Split('|').Select(s => s.Trim()).ToArray();
        string[][] matrix = new string[names.Length - 2][];

        for (int i = 0; i < names.Length - 2; i++)
            matrix[i] = new string[lines.Length - 1];

        for (int l = 1; l < lines.Length; l++)
        {
            string[] rowData = lines[l].Split('|');

            for (int i = 2; i < names.Length; i++)
                matrix[i - 2][l - 1] = rowData[i].Trim();
        }

        string[] line1 = lines[1].Split('|').Select(s => s.Trim()).ToArray();
        IAttributeColumn[] attributes = new IAttributeColumn[names.Length - 2];

        for (int i = 0; i < line1.Length - 2; i++)
        {
            if (float.TryParse(line1[i + 2], out float number))
                attributes[i] = new NumberAttributeColumn(names[i + 2], matrix[i].Select(float.Parse).ToArray());
            else
                attributes[i] = new StringAttributeColumn(names[i + 2], matrix[i]);
        }

        result = new DataSet(attributes);
        return true;
    }

    public override string ToString()
    {
        int columnsCount = _attributes.Length;
        int linesCount = _attributes[0].valuesCount;
        int[] spaceSizes = new int[_attributes.Length];
        string result = "|id    ";

        for (int i = 0; i < columnsCount; i++)
        {
            int size = _attributes[i].GetWordsMaxLength();
            string name = _attributes[i].name;
            spaceSizes[i] = size;
            result += $"|{name} " + DataSetTools.GetSpace(size - name.Length);
        }

        for (int i = 0; i < linesCount; i++)
        {
            string number = i.ToString();
            result += $"\n|{number}" + DataSetTools.GetSpace(6 - number.ToString().Length);

            for (int j = 0; j < columnsCount; j++)
            {
                string[] values = _attributes[j].values;
                result += $"|{values[i]} " + DataSetTools.GetSpace(spaceSizes[j] - values[i].Length);
            }
        }

        return result;
    }
}