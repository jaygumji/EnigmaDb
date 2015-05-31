using System;

namespace Enigma.Testing.Fakes.Entities
{
    public class Relation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ (Name != null ? Name.GetHashCode() : 0) ^
                   (Description != null ? Description.GetHashCode() : 0) ^ Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Relation;
            if (other == null) return false;

            return Id.Equals(other.Id)
                   && string.Equals(Name, other.Name)
                   && string.Equals(Description, other.Description)
                   && Value.Equals(other.Value);
        }
    }
}