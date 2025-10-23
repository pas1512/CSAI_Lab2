static class DataSetTools
{
    private static bool TryUpgrade(IAttributeColumn[] arrays, ref int[] indexes, int index = -1)
    {
        if (index == -1)
            index = indexes.Length - 1;

        indexes[index]++;

        if (indexes[index] >= arrays[index].valuesCount)
        {
            if (index > 0)
            {
                indexes[index] = 0;
                return TryUpgrade(arrays, ref indexes, index - 1);
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static DataSet Generate(params IAttributeColumn[] variations)
    {
        Random rd = new Random();
        int size = variations.Length;
        List<string>[] set = new List<string>[size];
        int[] indexes = new int[size];

        for (int i = 0; i < size; i++)
            set[i] = new List<string>(size);

        do
        {
            for (int i = 0; i < size; i++)
            {
                if (variations[i].isDiscrete)
                {
                    set[i].Add(variations[i].values[indexes[i]]);
                }
                else
                {
                    float min = float.Parse(variations[i].values[0]);
                    float max = float.Parse(variations[i].values[1]);
                    float random = rd.NextSingle() * (max + min) - min;
                    set[i].Add(random.ToString("F2"));
                }
            }
        } while (TryUpgrade(variations, ref indexes));

        IAttributeColumn[] result = new IAttributeColumn[size];

        for (int i = 0; i < size; i++)
        {
            if (variations[i].isDiscrete)
                result[i] = new StringAttributeColumn(variations[i].name, set[i].ToArray());
            else
                result[i] = new NumberAttributeColumn(variations[i].name, set[i].Select(s => float.Parse(s)).ToArray());
        }

        return new DataSet(result);
    }

    public static string GetSpace(int number)
    {
        string res = "";

        for (int i = 0; i < number; i++)
            res += " ";

        return res;
    }

    public static float GetEntropy(float[] values)
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

    public static float[] CalculateValues(string[] values)
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

    public static void Save(string path, DataSet data)
    {
        string strData = data.ToString();
        File.WriteAllText(path, strData);
    }

    public static bool Load(string path, out DataSet data)
    {
        string strData = File.ReadAllText(path);
        bool result = DataSet.TryParce(strData, out data);
        return result;
    }

    public static DataSet PATROL_ROBOT_RAW_SET => Generate(
        new StringAttributeColumn("Озброєння", "Невиявлено", "Неактивне", "Мала загроза", "Серьозна загроза"),
        new StringAttributeColumn("Поведiнка", "Спокiйна", "Переляк", "Агресивна", "Небезпечна"),
        new StringAttributeColumn("Вiк", "Неповнолітній", "Повнолітній", "Похилого віку"),
        new NumberAttributeColumn("Час виявлення", 0, 60, 0, 0, 0),
        new StringAttributeColumn("Реакцiя на вимоги", "Послух", "Iгнор"),
        new StringAttributeColumn("Рiшення", "Рiшення")
    );
}