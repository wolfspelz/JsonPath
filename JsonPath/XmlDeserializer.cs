using System.Text;
using System.Xml;

namespace JsonPath
{
    public class Xml
    {
        public const string Name = "Name";
        public const string Attributes = "Attributes";
        public const string Text = "Text";
        public const string Children = "Children";
    }

    public class XmlDeserializerOptions
    {
        public string Name = Xml.Name;
        public string Attributes = Xml.Attributes;
        public string Text = Xml.Text;
        public string Children = Xml.Children;
        public bool FlattenAttributes = false;
        public bool TextNodesAsChildren = true;
    }

    public class XmlDeserializer
    {
        readonly XmlDeserializerOptions Options;

        public XmlDeserializer(XmlDeserializerOptions? options = null)
        {
            Options = options ?? new XmlDeserializerOptions();
        }

        public Node Parse(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var root = doc.DocumentElement;
            if (root != null) {
                return GetNode(root);
            }

            return new Node(Node.Type.Dictionary);
        }

        private Node GetNode(XmlNode xmlNode)
        {
            var node = new Node(Node.Type.Dictionary).AsDictionary;
            node.Add(Options.Name, xmlNode.Name);

            if (Options.FlattenAttributes) {
                if (xmlNode.Attributes != null) {
                    foreach (XmlAttribute attr in xmlNode.Attributes) {
                        node.Add(attr.Name, attr.Value);
                    }
                }
            } else {
                var attributesDict = new Node(Node.Type.Dictionary).AsDictionary;
                if (xmlNode.Attributes != null) {
                    foreach (XmlAttribute attr in xmlNode.Attributes) {
                        attributesDict.Add(attr.Name, attr.Value);
                    }
                    node.Add(Options.Attributes, attributesDict);
                }
            }

            var childList = new Node(Node.Type.Dictionary).AsList;
            var textBuilder = new StringBuilder();
            foreach (XmlNode child in xmlNode.ChildNodes) {
                if (child.NodeType == XmlNodeType.Element) {
                    var childNode = GetNode(child);
                    childList.Add(childNode);
                } else if (child.NodeType == XmlNodeType.Text) {
                    if (Options.TextNodesAsChildren) {
                        if (child.Value != null) {
                            childList.Add(child.Value);
                        }
                    }
                    textBuilder.Append(child.Value);
                } else if (child.NodeType == XmlNodeType.CDATA) {
                    if (Options.TextNodesAsChildren) {
                        if (child.Value != null) {
                            childList.Add(child.InnerText);
                        }
                    }
                    textBuilder.Append(child.InnerText);
                }
            }
            node.Add(Options.Children, childList);
            node.Add(Options.Text, textBuilder.ToString());

            return node;
        }
    }
}
