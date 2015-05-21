using Enigma.Test.Serialization.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Serialization
{
    [TestClass]
    public class ReadValuePropertyTests
    {

        [TestMethod]
        public void ReadInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<Int16Graph>();
        }

        [TestMethod]
        public void ReadInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<Int32Graph>();
        }

        [TestMethod]
        public void ReadInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<Int64Graph>();
        }

        [TestMethod]
        public void ReadUInt16Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<UInt16Graph>();
        }

        [TestMethod]
        public void ReadUInt32Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<UInt32Graph>();
        }

        [TestMethod]
        public void ReadUInt64Test()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<UInt64Graph>();
        }

        [TestMethod]
        public void ReadBooleanTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<BooleanGraph>();
        }

        [TestMethod]
        public void ReadSingleTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<SingleGraph>();
        }

        [TestMethod]
        public void ReadDoubleTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<DoubleGraph>();
        }

        [TestMethod]
        public void ReadDecimalTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<DecimalGraph>();
        }

        [TestMethod]
        public void ReadTimeSpanTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<TimeSpanGraph>();
        }

        [TestMethod]
        public void ReadDateTimeTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<DateTimeGraph>();
        }

        [TestMethod]
        public void ReadStringTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<StringGraph>();
        }

        [TestMethod]
        public void ReadGuidTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<GuidGraph>();
        }

        [TestMethod]
        public void ReadBlobTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<BlobGraph>();
        }

        [TestMethod]
        public void ReadEnumTest()
        {
            var context = new SerializationTestContext();
            context.AssertReadSingleProperty<EnumGraph>();
        }

        [TestMethod]
        public void ReadComplexTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<ComplexGraph>(4);
        }

        [TestMethod]
        public void ReadDictionaryTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryGraph>(3);
        }

        [TestMethod]
        public void ReadDictionaryWithComplexValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithComplexValueGraph>(5);
        }

        [TestMethod]
        public void ReadDictionaryWithComplexKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithComplexKeyAndValueGraph>(5);
        }

        [TestMethod]
        public void ReadDictionaryCount3WithComplexKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithComplexKeyAndValueGraph>(15, 3);
        }

        [TestMethod]
        public void ReadDictionaryWithComplexKeyTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithComplexKeyGraph>(3);
        }

        [TestMethod]
        public void ReadDictionaryWithDictionaryKeyTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithDictionaryKeyGraph>(4);
        }

        [TestMethod]
        public void ReadDictionaryWithDictionaryValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithDictionaryValueGraph>(5);
        }

        [TestMethod]
        public void ReadDictionaryWithDictionaryKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithDictionaryKeyAndValueGraph>(6);
        }

        [TestMethod]
        public void ReadDictionaryWithCollectionKeyTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithCollectionKeyGraph>(3);
        }

        [TestMethod]
        public void ReadDictionaryWithCollectionValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithCollectionValueGraph>(4);
        }

        [TestMethod]
        public void ReadDictionaryWithCollectionKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<DictionaryWithCollectionKeyAndValueGraph>(4);
        }

        [TestMethod]
        public void ReadCollectionTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<CollectionGraph>(2);
        }

        [TestMethod]
        public void ReadCollectionOfComplexTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<CollectionOfComplexGraph>(4);
        }

        [TestMethod]
        public void ReadCollectionOfDictionaryTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<CollectionOfDictionaryGraph>(3);
        }

        [TestMethod]
        public void ReadCollectionOfCollectionTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<CollectionOfCollectionGraph>(2);
        }

        [TestMethod]
        public void ReadJaggedArrayTest()
        {
            var context = new SerializationTestContext();
            context.AssertRead<JaggedArrayGraph>(2);
        }

    }
}