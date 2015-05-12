using System.IO;
using Enigma.IO;
using Enigma.Serialization;
using Enigma.Serialization.PackedBinary;
using Enigma.Serialization.Reflection;
using Enigma.Serialization.Reflection.Emit;
using Enigma.Test.Serialization.Fakes;
using Enigma.Test.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    public class SerializationTestContext
    {
        private readonly DynamicTravellerContext _travellerContext;

        public SerializationTestContext()
        {
            _travellerContext = new DynamicTravellerContext(new SerializableTypeProvider(new SerializationReflectionInspector()), true);
        }

        public IGraphTraveller<T> CreateTraveller<T>()
        {
            var traveller = _travellerContext.GetInstance<T>();
            return traveller;
        }

        public byte[] Pack<T>(T graph)
        {
            var stream = new MemoryStream();
            var writer = new BinaryDataWriter(stream);
            var visitor = new PackedDataWriteVisitor(writer);

            var traveller = CreateTraveller<T>();
            traveller.Travel(visitor, graph);
            return stream.ToArray();
        }

        public void AssertWriteSingleProperty<T>(T graph)
        {
            AssertWrite(1, graph);
        }

        public void AssertReadSingleProperty<T>() where T : new()
        {
            AssertRead<T>(1);
        }

        public void AssertWrite<T>(int expectedValueWriteCount, T graph)
        {
            var visitor = new FakeWriteVisitor();
            var traveller = CreateTraveller<T>();
            traveller.Travel(visitor, graph);

            visitor.AssertHiearchy();
            Assert.AreEqual(expectedValueWriteCount, visitor.VisitValueCount);
        }

        public void AssertRead<T>(int expectedValueReadCount) where T : new()
        {
            var visitor = new FakeReadVisitor();
            var traveller = CreateTraveller<T>();

            var graph = new T();
            traveller.Travel(visitor, graph);

            visitor.AssertHiearchy();
            Assert.AreEqual(expectedValueReadCount, visitor.VisitValueCount);
        }
    }
}
