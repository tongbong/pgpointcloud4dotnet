using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace Pgpointcloud4dotnet.Tests
{
    public class PatchTests
    {

        private Patch Deserialize(string wkb, string schemaFile)
        {
            PointCloudSchema schema = PointCloudSchema.LoadSchemaFromFile(schemaFile);
            Patch patch = schema.DeserializePatchFromWkb(wkb);
            return patch;
        }

        [Fact]
        public void Deserialize_Patch_Uncompressed()
        {
            Patch patch = Deserialize("010100000000000000020000000000A04000002041000070410000A041",
                                        "Models/Patches/Model_NoCompression.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((float)5, p1.DimensionAsFloat("X"));
            Assert.Equal((float)10, p1.DimensionAsFloat("Y"));
            Assert.Equal((float)15, p2.DimensionAsFloat("X"));
            Assert.Equal((float)20, p2.DimensionAsFloat("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional()
        {
            Patch patch = Deserialize("01020000000100000002000000031000000078DA636058E0C0C050E0080006290192031000000078DA63605070646058E00800038E0143",
                                        "Models/Patches/Model_Dimensional_Deflate.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((float)5, p1.DimensionAsFloat("X"));
            Assert.Equal((float)10, p1.DimensionAsFloat("Y"));
            Assert.Equal((float)15, p2.DimensionAsFloat("X"));
            Assert.Equal((float)20, p2.DimensionAsFloat("Y"));
        }
    }
}
