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
        public void Deserialize_Patch_Dimensional_Float()
        {
            Patch patch = Deserialize("01020000000100000002000000031000000078DA636058E0C0C050E0080006290192031000000078DA63605070646058E00800038E0143",
                                        "Models/Patches/Model_Dimensional_Deflate_32bits_FloatingPoint.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((float)5, p1.DimensionAsFloat("X"));
            Assert.Equal((float)10, p1.DimensionAsFloat("Y"));
            Assert.Equal((float)15, p2.DimensionAsFloat("X"));
            Assert.Equal((float)20, p2.DimensionAsFloat("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Double()
        {
            Patch patch = Deserialize("01030000000100000002000000031200000078DA63600001110730C5A0E7000003B400C3031200000078DA63600001150730C560E20000046000D9",
                                        "Models/Patches/Model_Dimensional_Deflate_64bits_FloatingPoint.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((double)5, p1.DimensionAsDouble("X"));
            Assert.Equal((double)10, p1.DimensionAsDouble("Y"));
            Assert.Equal((double)15, p2.DimensionAsDouble("X"));
            Assert.Equal((double)20, p2.DimensionAsDouble("Y"));
        }
    }

}
