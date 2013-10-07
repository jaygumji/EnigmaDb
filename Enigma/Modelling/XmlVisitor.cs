using System.Xml;
namespace Enigma.Modelling
{
    public class XmlVisitor
    {

        private readonly XmlDictionaryReader _xmlReader;
        private bool _isAutoMoved;

        public XmlVisitor(XmlDictionaryReader xmlReader)
        {
            _xmlReader = xmlReader;
        }

        public string Name { get { return _xmlReader.Name; } }
        public XmlNodeType NodeType { get { return _xmlReader.NodeType; } }
        public bool IsEmptyElement { get { return _xmlReader.IsEmptyElement; } }

        public bool VisitNext()
        {
            if (_isAutoMoved)
            {
                _isAutoMoved = false;
                return true;
            }
            return _xmlReader.Read();
        }

        public string GetElementContentAsString()
        {
            _isAutoMoved = true;
            return _xmlReader.ReadElementContentAsString();
        }

        public int GetElementContentAsInt32()
        {
            _isAutoMoved = true;
            return _xmlReader.ReadElementContentAsInt();
        }

    }
}
