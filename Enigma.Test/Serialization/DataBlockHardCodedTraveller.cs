using System;
using System.Collections.Generic;
using Enigma.Serialization;
using Enigma.Serialization.Reflection;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    public class DataBlockHardCodedTraveller : IGraphTraveller<DataBlock>
    {

        private readonly IGraphTraveller<Relation> _travellerRelation0;
        private readonly IGraphTraveller<Identifier> _travellerIdentifier1;
        private readonly IGraphTraveller<Category> _travellerCategory2;

        private readonly VisitArgs _argsId0;
        private readonly VisitArgs _argsString1;
        private readonly VisitArgs _argsInt162;
        private readonly VisitArgs _argsInt323;
        private readonly VisitArgs _argsInt644;
        private readonly VisitArgs _argsUInt165;
        private readonly VisitArgs _argsUInt326;
        private readonly VisitArgs _argsUInt647;
        private readonly VisitArgs _argsSingle8;
        private readonly VisitArgs _argsDouble9;
        private readonly VisitArgs _argsTimeSpan10;
        private readonly VisitArgs _argsDecimal11;
        private readonly VisitArgs _argsDateTime12;
        private readonly VisitArgs _argsByte13;
        private readonly VisitArgs _argsBoolean14;
        private readonly VisitArgs _argsBlob15;
        private readonly VisitArgs _argsMessages16;
        private readonly VisitArgs _argsStamps17;
        private readonly VisitArgs _argsRelation18;
        private readonly VisitArgs _argsDummyRelation19;
        private readonly VisitArgs _argsIndexedValues20;
        private readonly VisitArgs _argsCategories21;

        public DataBlockHardCodedTraveller(IVisitArgsFactory factory)
        {
            _travellerRelation0 = new RelationHardCodedTraveller(factory.ConstructWith(typeof(Relation)));
            _travellerIdentifier1 = new IdentifierHardCodedTraveller(factory.ConstructWith(typeof(Identifier)));
            _travellerCategory2 = new CategoryHardCodedTraveller(factory.ConstructWith(typeof(Category)));

            _argsId0 = factory.Construct("Id");
            _argsString1 = factory.Construct("String");
            _argsInt162 = factory.Construct("Int16");
            _argsInt323 = factory.Construct("Int32");
            _argsInt644 = factory.Construct("Int64");
            _argsUInt165 = factory.Construct("UInt16");
            _argsUInt326 = factory.Construct("UInt32");
            _argsUInt647 = factory.Construct("UInt64");
            _argsSingle8 = factory.Construct("Single");
            _argsDouble9 = factory.Construct("Double");
            _argsTimeSpan10 = factory.Construct("TimeSpan");
            _argsDecimal11 = factory.Construct("Decimal");
            _argsDateTime12 = factory.Construct("DateTime");
            _argsByte13 = factory.Construct("Byte");
            _argsBoolean14 = factory.Construct("Boolean");
            _argsBlob15 = factory.Construct("Blob");
            _argsMessages16 = factory.Construct("Messages");
            _argsStamps17 = factory.Construct("Stamps");
            _argsRelation18 = factory.Construct("Relation");
            _argsDummyRelation19 = factory.Construct("DummyRelation");
            _argsIndexedValues20 = factory.Construct("IndexedValues");
            _argsCategories21 = factory.Construct("Categories");
        }

        public void Travel(IWriteVisitor visitor, DataBlock graph)
        {
            visitor.VisitValue(graph.Id, _argsId0);
            visitor.VisitValue(graph.String, _argsString1);
            visitor.VisitValue(graph.Int16, _argsInt162);
            visitor.VisitValue(graph.Int32, _argsInt323);
            visitor.VisitValue(graph.Int64, _argsInt644);
            visitor.VisitValue(graph.UInt16, _argsUInt165);
            visitor.VisitValue(graph.UInt32, _argsUInt326);
            visitor.VisitValue(graph.UInt64, _argsUInt647);
            visitor.VisitValue(graph.Single, _argsSingle8);
            visitor.VisitValue(graph.Double, _argsDouble9);
            visitor.VisitValue(graph.TimeSpan, _argsTimeSpan10);
            visitor.VisitValue(graph.Decimal, _argsDecimal11);
            visitor.VisitValue(graph.DateTime, _argsDateTime12);
            visitor.VisitValue(graph.Byte, _argsByte13);
            visitor.VisitValue(graph.Boolean, _argsBoolean14);
            visitor.VisitValue(graph.Blob, _argsBlob15);

            {
                var c = graph.Messages;
                visitor.Visit(c, _argsMessages16);

                if (c != null) {
                    foreach (var cv in c)
                        visitor.VisitValue(cv, VisitArgs.CollectionItem);
                }

                visitor.Leave(c, _argsMessages16);
            }

            {
                var c = (ICollection<DateTime>) graph.Stamps;
                visitor.Visit(c, _argsStamps17);
                if (c != null)
                    foreach (var cv in c)
                        visitor.VisitValue(cv, VisitArgs.CollectionItem);

                visitor.Leave(c, _argsStamps17);
            }

            {
                var c = graph.Relation;
                visitor.Visit(c, _argsRelation18);
                if (c != null)
                    _travellerRelation0.Travel(visitor, c);
                visitor.Leave(c, _argsRelation18);
            }

            {
                var c = graph.DummyRelation;
                visitor.Visit(c, _argsDummyRelation19);
                if (c != null)
                    _travellerRelation0.Travel(visitor, c);
                visitor.Leave(c, _argsDummyRelation19);
            }

            {
                var c = (IDictionary<string, int>) graph.IndexedValues;
                visitor.Visit(c, _argsIndexedValues20);
                if (c != null) {
                    foreach (var ckv in c) {
                        visitor.VisitValue(ckv.Key, VisitArgs.DictionaryKey);
                        visitor.VisitValue(ckv.Value, VisitArgs.DictionaryValue);
                    }
                }
                visitor.Leave(c, _argsIndexedValues20);
            }

            {
                var c = (IDictionary<Identifier, Category>)graph.Categories;
                visitor.Visit(c, _argsCategories21);
                if (c != null) {
                    foreach (var ckv in c) {
                        visitor.Visit(ckv.Key, VisitArgs.DictionaryKey);
                        _travellerIdentifier1.Travel(visitor, ckv.Key);
                        visitor.Leave(ckv.Key, VisitArgs.DictionaryKey);

                        visitor.Visit(ckv.Value, VisitArgs.DictionaryValue);
                        _travellerCategory2.Travel(visitor, ckv.Value);
                        visitor.Leave(ckv.Value, VisitArgs.DictionaryValue);
                    }
                }
                visitor.Leave(c, _argsCategories21);
            }
        }

        public void Travel(IWriteVisitor visitor, object graph)
        {
            Travel(visitor, (DataBlock) graph);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            Travel(visitor, (DataBlock) graph);
        }

        public void Travel(IReadVisitor visitor, DataBlock graph)
        {
            Guid? v0;
            if (visitor.TryVisitValue(_argsId0, out v0) && v0.HasValue)
                graph.Id = v0.Value;

            String v1;
            if (visitor.TryVisitValue(_argsString1, out v1))
                graph.String = v1;

            Int16? v2;
            if (visitor.TryVisitValue(_argsInt162, out v2) && v2.HasValue)
                graph.Int16 = v2.Value;

            Int32? v3;
            if (visitor.TryVisitValue(_argsInt323, out v3) && v3.HasValue)
                graph.Int32 = v3.Value;

            Int64? v4;
            if (visitor.TryVisitValue(_argsInt644, out v4) && v4.HasValue)
                graph.Int64 = v4.Value;

            UInt16? v5;
            if (visitor.TryVisitValue(_argsUInt165, out v5) && v5.HasValue)
                graph.UInt16 = v5.Value;

            UInt32? v6;
            if (visitor.TryVisitValue(_argsUInt326, out v6) && v6.HasValue)
                graph.UInt32 = v6.Value;

            UInt64? v7;
            if (visitor.TryVisitValue(_argsUInt647, out v7) && v7.HasValue)
                graph.UInt64 = v7.Value;

            Single? v8;
            if (visitor.TryVisitValue(_argsSingle8, out v8) && v8.HasValue)
                graph.Single = v8.Value;

            Double? v9;
            if (visitor.TryVisitValue(_argsDouble9, out v9) && v9.HasValue)
                graph.Double = v9.Value;

            TimeSpan? v10;
            if (visitor.TryVisitValue(_argsTimeSpan10, out v10) && v10.HasValue)
                graph.TimeSpan = v10.Value;

            Decimal? v11;
            if (visitor.TryVisitValue(_argsDecimal11, out v11) && v11.HasValue)
                graph.Decimal = v11.Value;

            DateTime? v12;
            if (visitor.TryVisitValue(_argsDateTime12, out v12) && v12.HasValue)
                graph.DateTime = v12.Value;

            Byte? v13;
            if (visitor.TryVisitValue(_argsByte13, out v13) && v13.HasValue)
                graph.Byte = v13.Value;

            Boolean? v14;
            if (visitor.TryVisitValue(_argsBoolean14, out v14) && v14.HasValue)
                graph.Boolean = v14.Value;

            Byte[] v15;
            if (visitor.TryVisitValue(_argsBlob15, out v15))
                graph.Blob = v15;

            ValueState state;
            state = visitor.TryVisit(_argsMessages16);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new List<string>();
                    string cv;
                    while (visitor.TryVisitValue(VisitArgs.CollectionItem, out cv) && cv != null)
                        c.Add(cv);
                    graph.Messages = c;

                    visitor.Leave(_argsMessages16);
                }
                else
                    graph.Messages = null;
            }

            state = visitor.TryVisit(_argsStamps17);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new List<DateTime>();
                    DateTime? cv;
                    while (visitor.TryVisitValue(VisitArgs.CollectionItem, out cv) && cv.HasValue)
                        c.Add(cv.Value);
                    graph.Stamps = c;

                    visitor.Leave(_argsStamps17);
                }
                else
                    graph.Stamps = null;
            }

            state = visitor.TryVisit(_argsRelation18);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new Relation();
                    _travellerRelation0.Travel(visitor, c);
                    graph.Relation = c;

                    visitor.Leave(_argsRelation18);
                }
                else
                    graph.Relation = null;
            }

            state = visitor.TryVisit(_argsDummyRelation19);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new Relation();
                    _travellerRelation0.Travel(visitor, c);
                    graph.DummyRelation = c;

                    visitor.Leave(_argsDummyRelation19);
                }
                else
                    graph.DummyRelation = null;
            }

            state = visitor.TryVisit(_argsIndexedValues20);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    IDictionary<string, int> c = new Dictionary<string, int>();
                    string ck;
                    while (visitor.TryVisitValue(VisitArgs.DictionaryKey, out ck) && ck != null) {
                        int? cv;
                        if (visitor.TryVisitValue(VisitArgs.DictionaryValue, out cv) && cv.HasValue)
                            c.Add(ck, cv.Value);
                        else
                            throw InvalidGraphException.NoDictionaryValue("IndexedValues");
                    }
                    graph.IndexedValues = (Dictionary<string, int>) c;

                    visitor.Leave(_argsIndexedValues20);
                }
                else
                    graph.IndexedValues = null;
            }

            state = visitor.TryVisit(_argsCategories21);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    IDictionary<Identifier, Category> c = new Dictionary<Identifier, Category>();
                    while (visitor.TryVisit(VisitArgs.DictionaryKey) == ValueState.Found) {
                        var ck = new Identifier();
                        _travellerIdentifier1.Travel(visitor, ck);
                        visitor.Leave(VisitArgs.DictionaryKey);

                        if (visitor.TryVisit(VisitArgs.DictionaryValue) == ValueState.Found) {
                            var cv = new Category();
                            _travellerCategory2.Travel(visitor, cv);
                            visitor.Leave(VisitArgs.DictionaryValue);

                            c.Add(ck, cv);
                        }
                        else
                            throw InvalidGraphException.NoDictionaryValue("Categories");
                    }
                    graph.Categories = (Dictionary<Identifier, Category>) c;

                    visitor.Leave(_argsCategories21);
                }
                else
                    graph.IndexedValues = null;
            }

        }

        public static DataBlockHardCodedTraveller Create()
        {
            var factory = new VisitArgsFactory(new SerializableTypeProvider(new SerializationReflectionInspector()), typeof(DataBlock));
            return new DataBlockHardCodedTraveller(factory);
        }
    }
}
