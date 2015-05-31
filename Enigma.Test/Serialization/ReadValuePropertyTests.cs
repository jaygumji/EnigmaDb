using System.Collections.Generic;
using Enigma.Serialization;
using Enigma.Testing.Fakes.Graphs;
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
            var stats = context.AssertRead<ComplexGraph>(4);
            stats.AssertVisitOrderExact(LevelType.Single, LevelType.Value, LevelType.Value, LevelType.Value, LevelType.Value);
        }

        [TestMethod]
        public void ReadDictionaryTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryGraph>(3);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey, LevelType.DictionaryValue,
                LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithComplexValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithComplexValueGraph>(5);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey, LevelType.DictionaryValue,
                LevelType.Value, LevelType.Value, LevelType.Value, LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithComplexKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithComplexKeyAndValueGraph>(5);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey, LevelType.Value, LevelType.Value,
                LevelType.DictionaryValue, LevelType.Value, LevelType.Value, LevelType.Value,
                LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryCount3WithComplexKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithComplexKeyAndValueGraph>(15, 3);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey, LevelType.Value, LevelType.Value,
                LevelType.DictionaryValue, LevelType.Value, LevelType.Value, LevelType.Value,
                LevelType.DictionaryKey, LevelType.Value, LevelType.Value,
                LevelType.DictionaryValue, LevelType.Value, LevelType.Value, LevelType.Value,
                LevelType.DictionaryKey, LevelType.Value, LevelType.Value,
                LevelType.DictionaryValue, LevelType.Value, LevelType.Value, LevelType.Value,
                LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithComplexKeyTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithComplexKeyGraph>(3);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey, LevelType.Value, LevelType.Value,
                LevelType.DictionaryValue, LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithDictionaryKeyTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithDictionaryKeyGraph>(4);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryInDictionaryKey,
                LevelType.DictionaryKey, LevelType.DictionaryValue, LevelType.DictionaryKey, LevelType.DictionaryValue,
                LevelType.DictionaryInDictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithDictionaryValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithDictionaryValueGraph>(5);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey,
                LevelType.DictionaryInDictionaryValue, LevelType.DictionaryKey, LevelType.DictionaryValue,
                LevelType.DictionaryKey, LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithDictionaryKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithDictionaryKeyAndValueGraph>(6);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryInDictionaryKey,
                LevelType.DictionaryKey, LevelType.DictionaryValue, LevelType.DictionaryKey,
                LevelType.DictionaryInDictionaryValue, LevelType.DictionaryKey, LevelType.DictionaryValue,
                LevelType.DictionaryKey, LevelType.DictionaryInDictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithCollectionKeyTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithCollectionKeyGraph>(3);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.CollectionInDictionaryKey,
                LevelType.CollectionItem, LevelType.CollectionItem, LevelType.DictionaryValue,
                LevelType.CollectionInDictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithCollectionValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithCollectionValueGraph>(4);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.DictionaryKey,
                LevelType.CollectionInDictionaryValue, LevelType.CollectionItem, LevelType.CollectionItem,
                LevelType.DictionaryKey);
        }

        [TestMethod]
        public void ReadDictionaryWithCollectionKeyAndValueTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<DictionaryWithCollectionKeyAndValueGraph>(4);
            stats.AssertVisitOrderExact(LevelType.Dictionary, LevelType.CollectionInDictionaryKey,
                LevelType.CollectionItem, LevelType.CollectionItem, LevelType.CollectionInDictionaryValue,
                LevelType.CollectionItem, LevelType.CollectionItem, LevelType.CollectionInDictionaryKey);
        }

        [TestMethod]
        public void ReadCollectionTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<CollectionGraph>(2);
            stats.AssertVisitOrderExact(LevelType.Collection, LevelType.CollectionItem, LevelType.CollectionItem);
        }

        [TestMethod]
        public void ReadCollectionOfComplexTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<CollectionOfComplexGraph>(4);
            stats.AssertVisitOrderExact(LevelType.Collection, LevelType.CollectionItem, LevelType.Value, LevelType.Value,
                LevelType.Value, LevelType.Value, LevelType.CollectionItem);
        }

        [TestMethod]
        public void ReadCollectionOfDictionaryTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<CollectionOfDictionaryGraph>(3);
            stats.AssertVisitOrderExact(LevelType.Collection, LevelType.DictionaryInCollection, LevelType.DictionaryKey,
                LevelType.DictionaryValue, LevelType.DictionaryKey, LevelType.DictionaryInCollection);
        }

        [TestMethod]
        public void ReadCollectionOfCollectionTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<CollectionOfCollectionGraph>(2);
            stats.AssertVisitOrderExact(LevelType.Collection, LevelType.CollectionInCollection, LevelType.CollectionItem,
                LevelType.CollectionItem, LevelType.CollectionInCollection);
        }

        [TestMethod]
        public void ReadJaggedArrayTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<JaggedArrayGraph>(2);
            stats.AssertVisitOrderExact(LevelType.Collection, LevelType.CollectionInCollection, LevelType.CollectionItem,
                LevelType.CollectionItem, LevelType.CollectionInCollection);
        }

        [TestMethod]
        public void ReadMultidimensionalArrayTest()
        {
            var context = new SerializationTestContext();
            var stats = context.AssertRead<MultidimensionalArrayGraph>(2);
            stats.AssertVisitOrderExact(LevelType.Collection, LevelType.CollectionInCollection, LevelType.CollectionItem,
                LevelType.CollectionItem, LevelType.CollectionInCollection);
        }

    }
}