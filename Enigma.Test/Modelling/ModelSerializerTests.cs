using System.IO;
using System.Linq;
using System.Text;
using Enigma.Test.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enigma.Test.Modelling
{
    [TestClass]
    public class ModelSerializerTests
    {

        [TestMethod]
        public void SaveTest()
        {

            var model = Enigma.Modelling.Model.ByConvention<MyEntity>();
            model.Register(typeof(MyEntity));
            model.Entity<MyEntity>().Index(e => e.Value);

            using (var stream = new MemoryStream())
            {
                model.Save(stream);
                var modelAsXml = Encoding.UTF8.GetString(stream.ToArray());
                StringAssert.Contains(modelAsXml, "<Name>MyEntity</Name>");
                StringAssert.Contains(modelAsXml, "<Name>Message</Name>");
                StringAssert.Contains(modelAsXml, "<KeyName>Id</KeyName>");
                StringAssert.Contains(modelAsXml, "<Index><UniqueName>Value</UniqueName></Index>");
            }

        }

        [TestMethod]
        public void LoadTest()
        {
            using (var stream = Resource.Get("Enigma.Test.Res.Model.xml"))
            {
                var model = Enigma.Modelling.Model.Load(stream);
                Assert.AreEqual(1, model.EntityMaps.Count());

                var entity = model.EntityMaps.First();
                Assert.AreEqual("MyEntity", entity.Name);
                Assert.AreEqual("Id", entity.KeyName);
                Assert.AreEqual(typeof(MyEntity), entity.EntityType);

                var properties = entity.Properties.ToArray();
                Assert.AreEqual(3, properties.Length);
                Assert.AreEqual(properties[0].PropertyName, "Id");
                Assert.AreEqual(properties[0].Name, "Id");
                Assert.AreEqual(properties[0].Index, 1);
                Assert.AreEqual(properties[1].PropertyName, "Message");
                Assert.AreEqual(properties[1].Name, "Message");
                Assert.AreEqual(properties[1].Index, 2);
                Assert.AreEqual(properties[2].PropertyName, "Value");
                Assert.AreEqual(properties[2].Name, "Value");
                Assert.AreEqual(properties[2].Index, 3);

                var indexes = entity.Indexes.ToArray();
                Assert.AreEqual(1, indexes.Length);
                Assert.AreEqual("Value", indexes[0].UniqueName);
            }
        }

    }
}
