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
        public void Deserialize_Patch_Dimensional_Zlib_Float()
        {
            Patch patch = Deserialize("01020000000100000002000000031000000078DA636058E0C0C050E0080006290192031000000078DA63605070646058E00800038E0143",
                                        "Models/Patches/Model_FloatingPoint_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsFloat("X"));
            Assert.Equal(10, p1.DimensionAsFloat("Y"));
            Assert.Equal(15, p2.DimensionAsFloat("X"));
            Assert.Equal(20, p2.DimensionAsFloat("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Double()
        {
            Patch patch = Deserialize("01030000000100000002000000031200000078DA63600001110730C5A0E7000003B400C3031200000078DA63600001150730C560E20000046000D9",
                                        "Models/Patches/Model_FloatingPoint_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsDouble("X"));
            Assert.Equal(10, p1.DimensionAsDouble("Y"));
            Assert.Equal(15, p2.DimensionAsDouble("X"));
            Assert.Equal(20, p2.DimensionAsDouble("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_SByte()
        {
            Patch patch = Deserialize("010D0000000100000002000000030800000078DA63E50700001B030800000078DAE3120100002A",
                                        "Models/Patches/Model_Integer_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((sbyte)5, p1.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)10, p1.DimensionAsSbyte("Y"));
            Assert.Equal((sbyte)15, p2.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)20, p2.DimensionAsSbyte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Short()
        {
            Patch patch = Deserialize("01080000000100000002000000030C00000078DA6365E067000000360015030C00000078DAE362106100000054001F",
                                        "Models/Patches/Model_Integer_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((short)5, p1.DimensionAsShort("X"));
            Assert.Equal((short)10, p1.DimensionAsShort("Y"));
            Assert.Equal((short)15, p2.DimensionAsShort("X"));
            Assert.Equal((short)20, p2.DimensionAsShort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Int()
        {
            Patch patch = Deserialize("01040000000100000002000000030E00000078DA63656060E0076200006C0015030E00000078DAE36260601001620000A8001F",
                                        "Models/Patches/Model_Integer_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsInt("X"));
            Assert.Equal(10, p1.DimensionAsInt("Y"));
            Assert.Equal(15, p2.DimensionAsInt("X"));
            Assert.Equal(20, p2.DimensionAsInt("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Long()
        {
            Patch patch = Deserialize("01070000000100000002000000030E00000078DA636580007E280D0000D80015030E00000078DAE362800011280D000150001F",
                                        "Models/Patches/Model_Integer_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsLong("X"));
            Assert.Equal(10, p1.DimensionAsLong("Y"));
            Assert.Equal(15, p2.DimensionAsLong("X"));
            Assert.Equal(20, p2.DimensionAsLong("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Byte()
        {
            Patch patch = Deserialize("010C0000000100000002000000030800000078DA63E50700001B030800000078DAE3120100002A",
                                        "Models/Patches/Model_UnsignedInteger_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((byte)5, p1.DimensionAsByte("X"));
            Assert.Equal((byte)10, p1.DimensionAsByte("Y"));
            Assert.Equal((byte)15, p2.DimensionAsByte("X"));
            Assert.Equal((byte)20, p2.DimensionAsByte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Ushort()
        {
            Patch patch = Deserialize("010B0000000100000002000000030C00000078DA6365E067000000360015030C00000078DAE362106100000054001F",
                                        "Models/Patches/Model_UnsignedInteger_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((ushort)5, p1.DimensionAsUshort("X"));
            Assert.Equal((ushort)10, p1.DimensionAsUshort("Y"));
            Assert.Equal((ushort)15, p2.DimensionAsUshort("X"));
            Assert.Equal((ushort)20, p2.DimensionAsUshort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Uint()
        {
            Patch patch = Deserialize("01090000000100000002000000030E00000078DA63656060E0076200006C0015030E00000078DAE36260601001620000A8001F",
                                        "Models/Patches/Model_UnsignedInteger_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((uint)5, p1.DimensionAsUint("X"));
            Assert.Equal((uint)10, p1.DimensionAsUint("Y"));
            Assert.Equal((uint)15, p2.DimensionAsUint("X"));
            Assert.Equal((uint)20, p2.DimensionAsUint("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Zlib_Ulong()
        {
            Patch patch = Deserialize("010A0000000100000002000000030E00000078DA636580007E280D0000D80015030E00000078DAE362800011280D000150001F",
                                        "Models/Patches/Model_UnsignedInteger_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((ulong)5, p1.DimensionAsUlong("X"));
            Assert.Equal((ulong)10, p1.DimensionAsUlong("Y"));
            Assert.Equal((ulong)15, p2.DimensionAsUlong("X"));
            Assert.Equal((ulong)20, p2.DimensionAsUlong("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Runlength_SByte()
        {
            Patch patch = Deserialize("0112000000010000000A00000001020000000A0501020000000A05",
                                        "Models/Patches/Model_Integer_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((sbyte)5, p1.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)5, p1.DimensionAsSbyte("Y"));
            Assert.Equal((sbyte)5, p2.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)5, p2.DimensionAsSbyte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Runlength_Short()
        {
            Patch patch = Deserialize("0110000000010000000A00000001030000000A050001030000000A0500",
                                        "Models/Patches/Model_Integer_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((short)5, p1.DimensionAsShort("X"));
            Assert.Equal((short)5, p1.DimensionAsShort("Y"));
            Assert.Equal((short)5, p2.DimensionAsShort("X"));
            Assert.Equal((short)5, p2.DimensionAsShort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_RunLength_Int()
        {
            Patch patch = Deserialize("010F000000010000000A00000001050000000A0500000001050000000A05000000",
                                        "Models/Patches/Model_Integer_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsInt("X"));
            Assert.Equal(5, p1.DimensionAsInt("Y"));
            Assert.Equal(5, p2.DimensionAsInt("X"));
            Assert.Equal(5, p2.DimensionAsInt("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Runlength_Long()
        {
            Patch patch = Deserialize("0111000000010000000A00000001090000000A050000000000000001090000000A0500000000000000",
                                        "Models/Patches/Model_Integer_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsLong("X"));
            Assert.Equal(5, p1.DimensionAsLong("Y"));
            Assert.Equal(5, p2.DimensionAsLong("X"));
            Assert.Equal(5, p2.DimensionAsLong("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Runlength_Byte()
        {
            Patch patch = Deserialize("0112000000010000000A00000001020000000A0501020000000A05",
                                        "Models/Patches/Model_UnsignedInteger_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((byte)5, p1.DimensionAsByte("X"));
            Assert.Equal((byte)5, p1.DimensionAsByte("Y"));
            Assert.Equal((byte)5, p2.DimensionAsByte("X"));
            Assert.Equal((byte)5, p2.DimensionAsByte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Runlength_Ushort()
        {
            Patch patch = Deserialize("0110000000010000000A00000001030000000A050001030000000A0500",
                                        "Models/Patches/Model_UnsignedInteger_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((ushort)5, p1.DimensionAsUshort("X"));
            Assert.Equal((ushort)5, p1.DimensionAsUshort("Y"));
            Assert.Equal((ushort)5, p2.DimensionAsUshort("X"));
            Assert.Equal((ushort)5, p2.DimensionAsUshort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_RunLength_Uint()
        {
            Patch patch = Deserialize("010F000000010000000A00000001050000000A0500000001050000000A05000000",
                                        "Models/Patches/Model_UnsignedInteger_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((uint)5, p1.DimensionAsUint("X"));
            Assert.Equal((uint)5, p1.DimensionAsUint("Y"));
            Assert.Equal((uint)5, p2.DimensionAsUint("X"));
            Assert.Equal((uint)5, p2.DimensionAsUint("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Runlength_Ulong()
        {
            Patch patch = Deserialize("0111000000010000000A00000001090000000A050000000000000001090000000A0500000000000000",
                                        "Models/Patches/Model_UnsignedInteger_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((ulong)5, p1.DimensionAsUlong("X"));
            Assert.Equal((ulong)5, p1.DimensionAsUlong("Y"));
            Assert.Equal((ulong)5, p2.DimensionAsUlong("X"));
            Assert.Equal((ulong)5, p2.DimensionAsUlong("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_RunLength_Float()
        {
            Patch patch = Deserialize("0113000000010000000A00000001050000000A0000A04001050000000A0000A040",
                                        "Models/Patches/Model_FloatingPoint_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsFloat("X"));
            Assert.Equal(5, p1.DimensionAsFloat("Y"));
            Assert.Equal(5, p2.DimensionAsFloat("X"));
            Assert.Equal(5, p2.DimensionAsFloat("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_RunLength_Double()
        {
            Patch patch = Deserialize("0114000000010000000A000000031000000078DA636000011107062AD1007A800349031000000078DA636000011107062AD1007A800349",
                                        "Models/Patches/Model_FloatingPoint_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsDouble("X"));
            Assert.Equal(5, p1.DimensionAsDouble("Y"));
            Assert.Equal(5, p2.DimensionAsDouble("X"));
            Assert.Equal(5, p2.DimensionAsDouble("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_SByte()
        {
            Patch patch = Deserialize("010200000001000000020000000203000000010A40030800000078DAE3E70700002F",
                                        "Models/Patches/Model_Integer_8bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((sbyte)5, p1.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)5, p1.DimensionAsSbyte("Y"));
            Assert.Equal((sbyte)5, p2.DimensionAsSbyte("X"));
            Assert.Equal((sbyte)5, p2.DimensionAsSbyte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Short()
        {
            Patch patch = Deserialize("0110000000010000000A00000001030000000A050001030000000A050",
                                        "Models/Patches/Model_Integer_16bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal((short)5, p1.DimensionAsShort("X"));
            Assert.Equal((short)5, p1.DimensionAsShort("Y"));
            Assert.Equal((short)5, p2.DimensionAsShort("X"));
            Assert.Equal((short)5, p2.DimensionAsShort("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Int()
        {
            Patch patch = Deserialize("010F000000010000000A00000001050000000A0500000001050000000A0500000",
                                        "Models/Patches/Model_Integer_32bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsInt("X"));
            Assert.Equal(5, p1.DimensionAsInt("Y"));
            Assert.Equal(5, p2.DimensionAsInt("X"));
            Assert.Equal(5, p2.DimensionAsInt("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Long()
        {
            Patch patch = Deserialize("0111000000010000000A00000001090000000A050000000000000001090000000A050000000000000",
                                        "Models/Patches/Model_Integer_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsLong("X"));
            Assert.Equal(5, p1.DimensionAsLong("Y"));
            Assert.Equal(5, p2.DimensionAsLong("X"));
            Assert.Equal(5, p2.DimensionAsLong("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Byte()
        {
            Patch patch = Deserialize("010300000001000000010000000203000000000A00030400000078DAE307",
                                        "Models/Patches/Model_UnsignedInteger_8bits.xml");
            Point p = patch.Points[0];
            Assert.Equal((byte)10, p.DimensionAsByte("X"));
            Assert.Equal((byte)15, p.DimensionAsByte("Y"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Ushort()
        {
            Patch patch = Deserialize("010400000001000000050000000206000000030008005C4E020600000000000F000000",
                                        "Models/Patches/Model_UnsignedInteger_16bits.xml");
            Assert.Equal((ushort)10, patch.Points[0].DimensionAsUshort("X"));
            Assert.Equal((ushort)11, patch.Points[1].DimensionAsUshort("X"));
            Assert.Equal((ushort)12, patch.Points[2].DimensionAsUshort("X"));
            Assert.Equal((ushort)13, patch.Points[3].DimensionAsUshort("X"));
            Assert.Equal((ushort)14, patch.Points[4].DimensionAsUshort("X"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Uint()
        {
            Patch patch = Deserialize("01050000000100000005000000020C000000030000000800000000005C4E020C000000000000000F00000000000000",
                                        "Models/Patches/Model_UnsignedInteger_32bits.xml");
            Assert.Equal((uint)10, patch.Points[0].DimensionAsUint("X"));
            Assert.Equal((uint)11, patch.Points[1].DimensionAsUint("X"));
            Assert.Equal((uint)12, patch.Points[2].DimensionAsUint("X"));
            Assert.Equal((uint)13, patch.Points[3].DimensionAsUint("X"));
            Assert.Equal((uint)14, patch.Points[4].DimensionAsUint("X"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Ulong()
        {
            Patch patch = Deserialize("010600000001000000050000000218000000030000000000000008000000000000000000000000005C4E0109000000050F00000000000000",
                                        "Models/Patches/Model_UnsignedInteger_64bits.xml");
            Assert.Equal((ulong)10, patch.Points[0].DimensionAsUlong("X"));
            Assert.Equal((ulong)11, patch.Points[1].DimensionAsUlong("X"));
            Assert.Equal((ulong)12, patch.Points[2].DimensionAsUlong("X"));
            Assert.Equal((ulong)13, patch.Points[3].DimensionAsUlong("X"));
            Assert.Equal((ulong)14, patch.Points[4].DimensionAsUlong("X"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Float()
        {
            Patch patch = Deserialize("0107000000010000000500000002180000001700000000000041CC3433436626CECC6D66666D00000000020C000000000000000000704100000000",
                                        "Models/Patches/Model_FloatingPoint_32bits.xml");
            Assert.Equal((float)10.1, patch.Points[0].DimensionAsFloat("X"));
            Assert.Equal((float)11.2, patch.Points[1].DimensionAsFloat("X"));
            Assert.Equal((float)12.3, patch.Points[2].DimensionAsFloat("X"));
            Assert.Equal((float)13.4, patch.Points[3].DimensionAsFloat("X"));
            Assert.Equal((float)14.5, patch.Points[4].DimensionAsFloat("X"));
        }

        [Fact]
        public void Deserialize_Patch_Dimensional_Sigbits_Double()
        {
            Patch patch = Deserialize("0114000000010000000A000000031000000078DA636000011107062AD1007A800349031000000078D36000011107062AD1007A800349",
                                        "Models/Patches/Model_FloatingPoint_64bits.xml");
            Point p1 = patch.Points[0];
            Point p2 = patch.Points[1];
            Assert.Equal(5, p1.DimensionAsDouble("X"));
            Assert.Equal(5, p1.DimensionAsDouble("Y"));
            Assert.Equal(5, p2.DimensionAsDouble("X"));
            Assert.Equal(5, p2.DimensionAsDouble("Y"));
        }
    }

}
