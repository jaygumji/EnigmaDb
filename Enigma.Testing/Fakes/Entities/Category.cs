namespace Enigma.Testing.Fakes.Entities
{
    public class Category
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Category;
            if (other == null) return false;

            return Name == other.Name && Description == other.Description && BlobComparer.AreEqual(Image, other.Image);
        }

        public override int GetHashCode()
        {
            return (Name == null ? 0 : Name.GetHashCode())
                   ^ (Description == null ? 0 : Description.GetHashCode())
                   ^ BlobComparer.GetBlobHashCode(Image);
        }
    }
}