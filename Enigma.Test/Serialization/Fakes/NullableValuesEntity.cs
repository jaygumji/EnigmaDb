using System;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization.Fakes
{
    public class NullableValuesEntity
    {
        public int Id { get; set; }
        public bool? MayBool { get; set; }
        public int? MayInt { get; set; }
        public DateTime? MayDateTime { get; set; }
        public TimeSpan? MayTimeSpan { get; set; }
        public ApplicationType? Type { get; set; }
    }
}
