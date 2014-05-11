﻿using System;
using System.Collections.Generic;
using Enigma.Serialization;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    public class DataBlockHardCodedTraveller : IGraphTraveller<DataBlock>
    {

        private readonly IGraphTraveller<Relation> _travellerRelation0;

        public DataBlockHardCodedTraveller()
        {
            _travellerRelation0 = new RelationHardCodedTraveller();
        }

        public void Travel(IWriteVisitor visitor, DataBlock graph)
        {
            visitor.VisitValue(graph.Id, WriteVisitArgs.Value("Id", 1));
            visitor.VisitValue(graph.String, WriteVisitArgs.Value("String", 2));
            visitor.VisitValue(graph.Int16, WriteVisitArgs.Value("Int16", 3));
            visitor.VisitValue(graph.Int32, WriteVisitArgs.Value("Int32", 4));
            visitor.VisitValue(graph.Int64, WriteVisitArgs.Value("Int64", 5));
            visitor.VisitValue(graph.UInt16, WriteVisitArgs.Value("UInt16", 6));
            visitor.VisitValue(graph.UInt32, WriteVisitArgs.Value("UInt32", 7));
            visitor.VisitValue(graph.UInt64, WriteVisitArgs.Value("UInt64", 8));
            visitor.VisitValue(graph.Single, WriteVisitArgs.Value("Single", 9));
            visitor.VisitValue(graph.Double, WriteVisitArgs.Value("Double", 10));
            visitor.VisitValue(graph.TimeSpan, WriteVisitArgs.Value("TimeSpan", 11));
            visitor.VisitValue(graph.Decimal, WriteVisitArgs.Value("Decimal", 12));
            visitor.VisitValue(graph.DateTime, WriteVisitArgs.Value("DateTime", 13));
            visitor.VisitValue(graph.Byte, WriteVisitArgs.Value("Byte", 14));
            visitor.VisitValue(graph.Boolean, WriteVisitArgs.Value("Boolean", 15));
            visitor.VisitValue(graph.Blob, WriteVisitArgs.Value("Blob", 16));

            {
                var c = graph.Messages;
                visitor.Visit(WriteVisitArgs.Collection("Messages", 17, c));

                if (c != null)
                    foreach (var cv in c)
                        visitor.VisitValue(cv, WriteVisitArgs.Item);

                visitor.Leave();
            }

            {
                var c = graph.Stamps;
                visitor.Visit(WriteVisitArgs.Collection("Stamps", 18, c));
                if (c != null)
                    foreach (var cv in c)
                        visitor.VisitValue(cv, WriteVisitArgs.Item);

                visitor.Leave();
            }

            {
                var c = graph.Relation;
                visitor.Visit(WriteVisitArgs.Single("Relation", 19, c));
                if (c != null)
                    _travellerRelation0.Travel(visitor, c);
                visitor.Leave();
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
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Id", 1), out v0) && v0.HasValue)
                graph.Id = v0.Value;

            String v1;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("String", 2), out v1))
                graph.String = v1;

            Int16? v2;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Int16", 3), out v2) && v2.HasValue)
                graph.Int16 = v2.Value;

            Int32? v3;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Int32", 4), out v3) && v3.HasValue)
                graph.Int32 = v3.Value;

            Int64? v4;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Int64", 5), out v4) && v4.HasValue)
                graph.Int64 = v4.Value;

            UInt16? v5;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("UInt16", 6), out v5) && v5.HasValue)
                graph.UInt16 = v5.Value;

            UInt32? v6;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("UInt32", 7), out v6) && v6.HasValue)
                graph.UInt32 = v6.Value;

            UInt64? v7;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("UInt64", 8), out v7) && v7.HasValue)
                graph.UInt64 = v7.Value;

            Single? v8;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Single", 9), out v8) && v8.HasValue)
                graph.Single = v8.Value;

            Double? v9;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Double", 10), out v9) && v9.HasValue)
                graph.Double = v9.Value;

            TimeSpan? v10;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("TimeSpan", 11), out v10) && v10.HasValue)
                graph.TimeSpan = v10.Value;

            Decimal? v11;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Decimal", 12), out v11) && v11.HasValue)
                graph.Decimal = v11.Value;

            DateTime? v12;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("DateTime", 13), out v12) && v12.HasValue)
                graph.DateTime = v12.Value;

            Byte? v13;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Byte", 14), out v13) && v13.HasValue)
                graph.Byte = v13.Value;

            Boolean? v14;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Boolean", 15), out v14) && v14.HasValue)
                graph.Boolean = v14.Value;

            Byte[] v15;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Blob", 16), out v15))
                graph.Blob = v15;

            if (visitor.TryVisit(ReadVisitArgs.Collection("Messages", 17))) {
                var c = new List<string>();
                string cv;
                while (visitor.TryVisitValue(ReadVisitArgs.Item, out cv) && cv != null)
                    c.Add(cv);
                graph.Messages = c;
                visitor.Leave();
            }

            if (visitor.TryVisit(ReadVisitArgs.Collection("Stamps", 18))) {
                var c = new List<DateTime>();
                DateTime? cv;
                while (visitor.TryVisitValue(ReadVisitArgs.Item, out cv) && cv.HasValue)
                    c.Add(cv.Value);
                graph.Stamps = c;
                visitor.Leave();
            }

            if (visitor.TryVisit(ReadVisitArgs.Single("Relation", 19))) {
                var c = new Relation();
                _travellerRelation0.Travel(visitor, c);
                graph.Relation = c;
                visitor.Leave();
            }

        }
    }
}