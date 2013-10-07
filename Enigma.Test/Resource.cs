using System.IO;
namespace Enigma.Test
{
    public static class Resource
    {
        public static Stream Get(string name)
        {
            return typeof(Resource).Assembly.GetManifestResourceStream(name);
        }
    }
}
