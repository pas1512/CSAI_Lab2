public class UnevenSelector
{
    private static Random _random = new Random();
    private string[] _selectables;
    private float _totalWeight;
    private float[] _prefix;

    public UnevenSelector(string[] selectables, float[] values)
    {
        _selectables = selectables;
        _prefix = new float[selectables.Length];

        for (int i = 0; i < selectables.Length; i++)
        {
            _totalWeight += values[i];
            _prefix[i] = _totalWeight;
        }
    }

    public string GetRandom()
    {
        float randomValue = _random.NextSingle() * _totalWeight;
        int min = 0;
        int max = _selectables.Length - 1;

        while (min < max)
        {
            int mid = (max + min) / 2;

            if (randomValue < _prefix[mid])
                max = mid;
            else
                min = mid + 1;
        }

        return _selectables[min];
    }
}
