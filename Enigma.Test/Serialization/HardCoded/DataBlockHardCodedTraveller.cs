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

        private readonly VisitArgs _argsId;
        private readonly VisitArgs _argsString;
        private readonly VisitArgs _argsInt16;
        private readonly VisitArgs _argsInt32;
        private readonly VisitArgs _argsInt64;
        private readonly VisitArgs _argsUInt16;
        private readonly VisitArgs _argsUInt32;
        private readonly VisitArgs _argsUInt64;
        private readonly VisitArgs _argsSingle;
        private readonly VisitArgs _argsDouble;
        private readonly VisitArgs _argsTimeSpan;
        private readonly VisitArgs _argsDecimal;
        private readonly VisitArgs _argsDateTime;
        private readonly VisitArgs _argsByte;
        private readonly VisitArgs _argsBoolean;
        private readonly VisitArgs _argsBlob;
        private readonly VisitArgs _argsMessages;
        private readonly VisitArgs _argsStamps;
        private readonly VisitArgs _argsRelation;
        private readonly VisitArgs _argsDummyRelation;
        private readonly VisitArgs _argsSecondaryRelations;
        private readonly VisitArgs _argsIndexedValues;
        private readonly VisitArgs _argsCategories;

        public DataBlockHardCodedTraveller(IVisitArgsFactory factory)
        {
            _travellerRelation0 = new RelationHardCodedTraveller(factory.ConstructWith(typeof(Relation)));
            _travellerIdentifier1 = new IdentifierHardCodedTraveller(factory.ConstructWith(typeof(Identifier)));
            _travellerCategory2 = new CategoryHardCodedTraveller(factory.ConstructWith(typeof(Category)));

            _argsId = factory.Construct("Id");
            _argsString = factory.Construct("String");
            _argsInt16 = factory.Construct("Int16");
            _argsInt32 = factory.Construct("Int32");
            _argsInt64 = factory.Construct("Int64");
            _argsUInt16 = factory.Construct("UInt16");
            _argsUInt32 = factory.Construct("UInt32");
            _argsUInt64 = factory.Construct("UInt64");
            _argsSingle = factory.Construct("Single");
            _argsDouble = factory.Construct("Double");
            _argsTimeSpan = factory.Construct("TimeSpan");
            _argsDecimal = factory.Construct("Decimal");
            _argsDateTime = factory.Construct("DateTime");
            _argsByte = factory.Construct("Byte");
            _argsBoolean = factory.Construct("Boolean");
            _argsBlob = factory.Construct("Blob");
            _argsMessages = factory.Construct("Messages");
            _argsStamps = factory.Construct("Stamps");
            _argsRelation = factory.Construct("Relation");
            _argsDummyRelation = factory.Construct("DummyRelation");
            _argsSecondaryRelations = factory.Construct("SecondaryRelations");
            _argsIndexedValues = factory.Construct("IndexedValues");
            _argsCategories = factory.Construct("Categories");
        }

        public void Travel(IWriteVisitor visitor, DataBlock graph)
        {
            visitor.VisitValue(graph.Id, _argsId);
            visitor.VisitValue(graph.String, _argsString);
            visitor.VisitValue(graph.Int16, _argsInt16);
            visitor.VisitValue(graph.Int32, _argsInt32);
            visitor.VisitValue(graph.Int64, _argsInt64);
            visitor.VisitValue(graph.UInt16, _argsUInt16);
            visitor.VisitValue(graph.UInt32, _argsUInt32);
            visitor.VisitValue(graph.UInt64, _argsUInt64);
            visitor.VisitValue(graph.Single, _argsSingle);
            visitor.VisitValue(graph.Double, _argsDouble);
            visitor.VisitValue(graph.TimeSpan, _argsTimeSpan);
            visitor.VisitValue(graph.Decimal, _argsDecimal);
            visitor.VisitValue(graph.DateTime, _argsDateTime);
            visitor.VisitValue(graph.Byte, _argsByte);
            visitor.VisitValue(graph.Boolean, _argsBoolean);
            visitor.VisitValue(graph.Blob, _argsBlob);

            {
                var c = graph.Messages;
                visitor.Visit(c, _argsMessages);

                if (c != null) {
                    foreach (var cv in c)
                        visitor.VisitValue(cv, VisitArgs.CollectionItem);
                }

                visitor.Leave(c, _argsMessages);
            }

            {
                var c = (ICollection<DateTime>) graph.Stamps;
                visitor.Visit(c, _argsStamps);
                if (c != null)
                    foreach (var cv in c)
                        visitor.VisitValue(cv, VisitArgs.CollectionItem);

                visitor.Leave(c, _argsStamps);
            }

            {
                var c = graph.Relation;
                visitor.Visit(c, _argsRelation);
                if (c != null)
                    _travellerRelation0.Travel(visitor, c);
                visitor.Leave(c, _argsRelation);
            }

            {
                var c = graph.DummyRelation;
                visitor.Visit(c, _argsDummyRelation);
                if (c != null)
                    _travellerRelation0.Travel(visitor, c);
                visitor.Leave(c, _argsDummyRelation);
            }

            {
                var c = (ICollection<Relation>)graph.SecondaryRelations;
                visitor.Visit(c, _argsSecondaryRelations);
                if (c != null) {
                    foreach (var cv in c) {
                        visitor.Visit(cv, VisitArgs.CollectionItem);
                        _travellerRelation0.Travel(visitor, cv);
                        visitor.Leave(cv, VisitArgs.CollectionItem);
                    }
                }

                visitor.Leave(c, _argsSecondaryRelations);
            }

            {
                var c = (IDictionary<string, int>) graph.IndexedValues;
                visitor.Visit(c, _argsIndexedValues);
                if (c != null) {
                    foreach (var ckv in c) {
                        visitor.VisitValue(ckv.Key, VisitArgs.DictionaryKey);
                        visitor.VisitValue(ckv.Value, VisitArgs.DictionaryValue);
                    }
                }
                visitor.Leave(c, _argsIndexedValues);
            }

            {
                var c = (IDictionary<Identifier, Category>)graph.Categories;
                visitor.Visit(c, _argsCategories);
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
                visitor.Leave(c, _argsCategories);
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
            if (visitor.TryVisitValue(_argsId, out v0) && v0.HasValue)
                graph.Id = v0.Value;

            String v1;
            if (visitor.TryVisitValue(_argsString, out v1))
                graph.String = v1;

            Int16? v2;
            if (visitor.TryVisitValue(_argsInt16, out v2) && v2.HasValue)
                graph.Int16 = v2.Value;

            Int32? v3;
            if (visitor.TryVisitValue(_argsInt32, out v3) && v3.HasValue)
                graph.Int32 = v3.Value;

            Int64? v4;
            if (visitor.TryVisitValue(_argsInt64, out v4) && v4.HasValue)
                graph.Int64 = v4.Value;

            UInt16? v5;
            if (visitor.TryVisitValue(_argsUInt16, out v5) && v5.HasValue)
                graph.UInt16 = v5.Value;

            UInt32? v6;
            if (visitor.TryVisitValue(_argsUInt32, out v6) && v6.HasValue)
                graph.UInt32 = v6.Value;

            UInt64? v7;
            if (visitor.TryVisitValue(_argsUInt64, out v7) && v7.HasValue)
                graph.UInt64 = v7.Value;

            Single? v8;
            if (visitor.TryVisitValue(_argsSingle, out v8) && v8.HasValue)
                graph.Single = v8.Value;

            Double? v9;
            if (visitor.TryVisitValue(_argsDouble, out v9) && v9.HasValue)
                graph.Double = v9.Value;

            TimeSpan? v10;
            if (visitor.TryVisitValue(_argsTimeSpan, out v10) && v10.HasValue)
                graph.TimeSpan = v10.Value;

            Decimal? v11;
            if (visitor.TryVisitValue(_argsDecimal, out v11) && v11.HasValue)
                graph.Decimal = v11.Value;

            DateTime? v12;
            if (visitor.TryVisitValue(_argsDateTime, out v12) && v12.HasValue)
                graph.DateTime = v12.Value;

            Byte? v13;
            if (visitor.TryVisitValue(_argsByte, out v13) && v13.HasValue)
                graph.Byte = v13.Value;

            Boolean? v14;
            if (visitor.TryVisitValue(_argsBoolean, out v14) && v14.HasValue)
                graph.Boolean = v14.Value;

            Byte[] v15;
            if (visitor.TryVisitValue(_argsBlob, out v15))
                graph.Blob = v15;

            ValueState state;
            state = visitor.TryVisit(_argsMessages);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new List<string>();
                    string cv;
                    while (visitor.TryVisitValue(VisitArgs.CollectionItem, out cv) && cv != null)
                        c.Add(cv);
                    graph.Messages = c;

                    visitor.Leave(_argsMessages);
                }
                else
                    graph.Messages = null;
            }

            state = visitor.TryVisit(_argsStamps);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new List<DateTime>();
                    DateTime? cv;
                    while (visitor.TryVisitValue(VisitArgs.CollectionItem, out cv) && cv.HasValue)
                        c.Add(cv.Value);
                    graph.Stamps = c;

                    visitor.Leave(_argsStamps);
                }
                else
                    graph.Stamps = null;
            }

            state = visitor.TryVisit(_argsRelation);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new Relation();
                    _travellerRelation0.Travel(visitor, c);
                    graph.Relation = c;

                    visitor.Leave(_argsRelation);
                }
                else
                    graph.Relation = null;
            }

            state = visitor.TryVisit(_argsDummyRelation);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new Relation();
                    _travellerRelation0.Travel(visitor, c);
                    graph.DummyRelation = c;

                    visitor.Leave(_argsDummyRelation);
                }
                else
                    graph.DummyRelation = null;
            }

            state = visitor.TryVisit(_argsSecondaryRelations);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    var c = new List<Relation>();
                    while (visitor.TryVisit(VisitArgs.CollectionItem) == ValueState.Found) {
                        var cv = new Relation();
                        _travellerRelation0.Travel(visitor, cv);
                        visitor.Leave(VisitArgs.CollectionItem);
                        c.Add(cv);
                    }
                    graph.SecondaryRelations = c;

                    visitor.Leave(_argsSecondaryRelations);
                }
                else
                    graph.SecondaryRelations = null;
            }

            state = visitor.TryVisit(_argsIndexedValues);
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

                    visitor.Leave(_argsIndexedValues);
                }
                else
                    graph.IndexedValues = null;
            }

            state = visitor.TryVisit(_argsCategories);
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

                    visitor.Leave(_argsCategories);
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
