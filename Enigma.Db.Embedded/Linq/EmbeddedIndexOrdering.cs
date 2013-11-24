using Enigma.Db.Linq;
using Enigma.Store.Indexes;

namespace Enigma.Db.Embedded.Linq
{
    internal class EmbeddedIndexOrdering
    {
        private readonly PropertyPath _path;
        private readonly OrderingDirection _direction;

        public EmbeddedIndexOrdering(PropertyPath path, OrderingDirection direction)
        {
            _path = path;
            _direction = direction;
        }

        public PropertyPath Path
        {
            get { return _path; }
        }

        public OrderingDirection Direction
        {
            get { return _direction; }
        }
    }
}