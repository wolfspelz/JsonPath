namespace JsonPath
{
    public class KeyValueLfDeserializerOptions
    {
        public char[] LineSeparators = new char[] { '\r', '\n' };
        public char[] FieldSeparators = new char[] { '=' };
    }

    public class KeyValueLfDeserializer
    {
        readonly KeyValueLfDeserializerOptions Options;

        public KeyValueLfDeserializer(KeyValueLfDeserializerOptions? options = null)
        {
            Options = options ?? new KeyValueLfDeserializerOptions();
        }

        public Node Parse(string data)
        {
            var root = new Node(Node.Type.Dictionary).AsDictionary;

            var lines = data.Split(Options.LineSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                var parts = line.Trim().Split(Options.FieldSeparators, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0) {
                    if (parts.Length == 1) {
                        root.Add(parts[0].Trim(), "");
                    } else {
                        root.Add(parts[0].Trim(), parts[1].Trim());
                    }
                }
            }

            return root;
        }
    }
}
