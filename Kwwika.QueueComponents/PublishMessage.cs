using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections;

namespace Kwwika.QueueComponents
{
    [Serializable]
    public class PublishMessage
    {
        private Dictionary<string, string> _values = new Dictionary<string, string>();
        
        [XmlElement(ElementName="TopicName")]
        public string TopicName { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> Values { get { return _values; } set { _values = value; } }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlArray(ElementName="Values")]
        [XmlArrayItem(ElementName="Add", Type=typeof(StringPair))]
        public StringPairList ValuesProxy
        {
            get { return new StringPairList(Values); }
        }

        public PublishMessage() { }

        public PublishMessage(string topicName)
        {
            this.TopicName = topicName;
        }
    }

    [Serializable]
    public sealed class StringPairList : IEnumerable<StringPair>
    {
        private readonly IDictionary<string, string> parent;
        public StringPairList(IDictionary<string, string> parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            this.parent = parent;
        }
        public void Add(StringPair item)
        {
            parent.Add(item.Key, item.Value);
        }
        public IEnumerator<StringPair> GetEnumerator()
        {
            foreach (var pair in parent)
            {
                yield return new StringPair
                {
                    Key = pair.Key,
                    Value = pair.Value
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    [Serializable]
    public sealed class StringPair
    {
        [XmlAttribute("key")]
        public string Key { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
