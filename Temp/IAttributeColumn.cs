interface IAttributeColumn
{
    public bool isDiscrete { get; }
    public string name { get; }
    public string[] values { get; }
    public int valuesCount { get; }

    public int GetWordsMaxLength()
    {
        int lines = values.Length;
        string[] vls = values;
        int max = 0;

        for (int j = 0; j < lines; j++)
        {
            int l = vls[j].Length;
            if (l > max) max = l;
        }

        int nameSize = name.Length;
        return nameSize > max ? nameSize : max;
    }

    public string[] GetValues(params int[] rows)
    {
        string[] result = new string[rows.Length];

        for (int i = 0; i < result.Length; i++)
            result[i] = values[rows[i]];

        return result;
    }
}

struct StringAttributeColumn : IAttributeColumn
{
    private string _name;
    private string[] _values;

    public bool isDiscrete => true;
    public string name => _name;
    public string[] values => _values;
    public int valuesCount => _values.Length;

    public StringAttributeColumn(string name, params string[] values)
    {
        _name = name;
        _values = values;
    }

    public override string ToString()
    {
        string result = $"{_name}: ";

        for (int i = 0; i < _values.Length; i++)
            result += $"{_values[i]}; ";

        return result;
    }
}


struct NumberAttributeColumn : IAttributeColumn
{
    private string _name;
    private float[] _values;

    public bool isDiscrete => false;
    public string name => _name;
    public string[] values => _values.Select(v => v.ToString()).ToArray();
    public int valuesCount => _values.Length;
    public float[] numbers => _values;

    public NumberAttributeColumn(string name, params float[] values)
    {
        _name = name;
        _values = values;
    }

    public override string ToString()
    {
        string result = $"{_name}: ";

        for (int i = 0; i < _values.Length; i++)
            result += $"{_values[i]}; ";

        return result;
    }
}