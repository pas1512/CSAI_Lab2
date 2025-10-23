namespace CSAI_Lab2.DecisionTreeSystem
{
    internal class DecisionTreeNode
    {
        public string name => _name;

        private bool _isNumber;
        private string _name;
        private Dictionary<string, DecisionTreeNode> _nexts;

        public DecisionTreeNode(string value)
        {
            _name = value;
            _nexts = [];
        }

        public DecisionTreeNode(string key, string[] values)
        {
            _name = key;
            _nexts = new Dictionary<string, DecisionTreeNode>(values.Length);

            for (int i = 0; i < values.Length; i++)
                _nexts.Add(values[i], null);
        }

        public void SetNextNode(string value, DecisionTreeNode next)
        {
            if (_nexts != null && _nexts.ContainsKey(value))
                _nexts[value] = next;
        }

        public bool IsLeaf(out string value)
        {
            value = _name;
            return _nexts == null || _nexts.Count == 0;
        }

        public string Test(string name, params (string attribute, object value)[] attributes)
        {
            string evaluated = Evaluate(attributes);
            string result = name + "\n";

            for (int i = 0; i < attributes.Length; i++) 
                result += $"{attributes[i].attribute}: {attributes[i].value};\n";

            result += $"Рішення: {evaluated}\n";
            return result;
        }

        public string Evaluate(params (string attribute, object value)[] attributes)
        {
            if (IsLeaf(out var result))
                return result;

            var current = attributes.FirstOrDefault(attr => attr.attribute == _name);

            if(current == default)
                return string.Empty;

            string value = string.Empty;

            if (current.value.IsNumericType())
                value = current.value.SelectRange(_nexts.Keys.ToArray());
            else
                value = (string)current.value;

            return _nexts[value].Evaluate(attributes);
        }

        public static string ParceToString(DecisionTreeNode node, string label = "", string tab = "")
        {
            if (node.IsLeaf(out var val))
                return $"{tab}{label}: {val}\n";

            string result = "";

            if (string.IsNullOrEmpty(label))
                result = tab + node._name + "=>\n";
            else
                result = $"{tab}{label}: {node._name}=>\n";

            foreach (var next in node._nexts)
                result += ParceToString(next.Value, next.Key, tab + "\t");

            return result;
        }

        public override string ToString() => ParceToString(this);
    }
}
