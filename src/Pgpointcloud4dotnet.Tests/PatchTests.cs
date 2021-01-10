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
                                        "Models/Patches/Dimensional/Deflate/FloatingPoints/Model_32bits.xml");
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
                                        "Models/Patches/Dimensional/Deflate/FloatingPoints/Model_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((double)5, p1.DimensionAsDouble("X"));
            Assert.Equal((double)10, p1.DimensionAsDouble("Y"));
            Assert.Equal((double)15, p2.DimensionAsDouble("X"));
            Assert.Equal((double)20, p2.DimensionAsDouble("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_SByte()
        {
            Patch patch = Deserialize("010D0000000100000002000000030800000078DA63E50700001B030800000078DAE3120100002A",
                                        "Models/Patches/Dimensional/Deflate/Integers/Model_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((sbyte)5, p1.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)10, p1.DimensionAsSbyte("Y"));
            Assert.Equal((sbyte)15, p2.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)20, p2.DimensionAsSbyte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Short()
        {
            Patch patch = Deserialize("01080000000100000002000000030C00000078DA6365E067000000360015030C00000078DAE362106100000054001F",
                                        "Models/Patches/Dimensional/Deflate/Integers/Model_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((short)5, p1.DimensionAsShort("X"));
            Assert.Equal((short)10, p1.DimensionAsShort("Y"));
            Assert.Equal((short)15, p2.DimensionAsShort("X"));
            Assert.Equal((short)20, p2.DimensionAsShort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Int()
        {
            Patch patch = Deserialize("01040000000100000002000000030E00000078DA63656060E0076200006C0015030E00000078DAE36260601001620000A8001F",
                                        "Models/Patches/Dimensional/Deflate/Integers/Model_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((int)5, p1.DimensionAsInt("X"));
            Assert.Equal((int)10, p1.DimensionAsInt("Y"));
            Assert.Equal((int)15, p2.DimensionAsInt("X"));
            Assert.Equal((int)20, p2.DimensionAsInt("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Long()
        {
            Patch patch = Deserialize("01070000000100000002000000030E00000078DA636580007E280D0000D80015030E00000078DAE362800011280D000150001F",
                                        "Models/Patches/Dimensional/Deflate/Integers/Model_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((long)5, p1.DimensionAsLong("X"));
            Assert.Equal((long)10, p1.DimensionAsLong("Y"));
            Assert.Equal((long)15, p2.DimensionAsLong("X"));
            Assert.Equal((long)20, p2.DimensionAsLong("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Byte()
        {
            Patch patch = Deserialize("010C0000000100000002000000030800000078DA63E50700001B030800000078DAE3120100002A",
                                        "Models/Patches/Dimensional/Deflate/UnsignedIntegers/Model_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((byte)5, p1.DimensionAsByte("X"));
            Assert.Equal((byte)10, p1.DimensionAsByte("Y"));
            Assert.Equal((byte)15, p2.DimensionAsByte("X"));
            Assert.Equal((byte)20, p2.DimensionAsByte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Ushort()
        {
            Patch patch = Deserialize("010B0000000100000002000000030C00000078DA6365E067000000360015030C00000078DAE362106100000054001F",
                                        "Models/Patches/Dimensional/Deflate/UnsignedIntegers/Model_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((ushort)5, p1.DimensionAsUshort("X"));
            Assert.Equal((ushort)10, p1.DimensionAsUshort("Y"));
            Assert.Equal((ushort)15, p2.DimensionAsUshort("X"));
            Assert.Equal((ushort)20, p2.DimensionAsUshort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Uint()
        {
            Patch patch = Deserialize("01090000000100000002000000030E00000078DA63656060E0076200006C0015030E00000078DAE36260601001620000A8001F",
                                        "Models/Patches/Dimensional/Deflate/UnsignedIntegers/Model_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((uint)5, p1.DimensionAsUint("X"));
            Assert.Equal((uint)10, p1.DimensionAsUint("Y"));
            Assert.Equal((uint)15, p2.DimensionAsUint("X"));
            Assert.Equal((uint)20, p2.DimensionAsUint("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Ulong()
        {
            Patch patch = Deserialize("010A0000000100000002000000030E00000078DA636580007E280D0000D80015030E00000078DAE362800011280D000150001F",
                                        "Models/Patches/Dimensional/Deflate/UnsignedIntegers/Model_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((ulong)5, p1.DimensionAsUlong("X"));
            Assert.Equal((ulong)10, p1.DimensionAsUlong("Y"));
            Assert.Equal((ulong)15, p2.DimensionAsUlong("X"));
            Assert.Equal((ulong)20, p2.DimensionAsUlong("Y"));
        }
    }

}
