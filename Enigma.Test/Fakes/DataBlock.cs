﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Enigma.IO;
using Enigma.Serialization;
using Enigma.Test.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Fakes
{
    public class DataBlock
    {
        public Guid Id { get; set; }
        public string String { get; set; }
        public Int16 Int16 { get; set; }
        public Int32 Int32 { get; set; }
        public Int64 Int64 { get; set; }
        public UInt16 UInt16 { get; set; }
        public UInt32 UInt32 { get; set; }
        public UInt64 UInt64 { get; set; }
        public Single Single { get; set; }
        public Double Double { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public Decimal Decimal { get; set; }
        public DateTime DateTime { get; set; }
        public Byte Byte { get; set; }
        public Boolean Boolean { get; set; }
        public byte[] Blob { get; set; }

        public ICollection<string> Messages { get; set; }
        public IList<DateTime> Stamps { get; set; }

        public Relation Relation { get; set; }

        public static DataBlock Filled()
        {
            return new DataBlock {
                Id = Guid.Parse("F5159142-B9A3-45FA-85AE-E0C9E60990A9"),
                Blob = new byte[] { 1, 2, 3},
                Boolean = true,
                Byte = 42,
                DateTime = new DateTime(2014, 04, 01, 10, 00, 00),
                Decimal = 44754.324M,
                Double = 4357.32,
                Int16 = 20234,
                Int32 = 43554654,
                Int64 = 4349893895849554545,
                Messages = new Collection<string> { "Test1", "Test2", "Test3", "Test4", "Test5"},
                Single = 32.1f,
                String = "Hello World",
                TimeSpan = new TimeSpan(10, 30, 00),
                UInt16 = 64322,
                UInt32 = 3454654454,
                UInt64 = 9859459485984955454,
                Stamps = new []{new DateTime(2010, 03, 01, 22, 00, 00)},
                Relation = new Relation {
                    Id = Guid.Parse("F68EF7D4-6F62-476B-BC5E-71AD86549A63"),
                    Name = "Connection",
                    Description = "Generic connection between relations",
                    Value = 77
                }
            };
        }

        public static byte[] SerializedFilled()
        {
            var stream = new MemoryStream();
            var writer = new BinaryDataWriter(stream);
            var visitor = new PackedDataWriteVisitor(writer);
            var traveller = new DataBlockHardCodedTraveller();
            traveller.Travel(visitor, Filled());

            var bytes = stream.ToArray();
            return bytes;
        }

    }
}
