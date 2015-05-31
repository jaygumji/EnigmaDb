namespace Enigma.Testing.Fakes.Entities
{
    public class Identifier
    {
        public int Id { get; set; }
        public ApplicationType Type { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Identifier;
            if (other == null) return false;
            return Type == other.Type && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Type.GetHashCode();
        }
    }
}