using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace Pgpointcloud4dotnet.Tests
{
    public class PointTests
    {

        private Point Deserialize(string wkb, string schemaFile)
        {
            PointCloudSchema schema = PointCloudSchema.LoadSchemaFromFile(schemaFile);
            Point point = schema.DeserializePointFromWkb(wkb);
            return point;
        }

        //[Fact]
        //public void LoadModel_Ok()
        //{
        //    PointCloudSchema schema = LoadSchemaFromFile("Models/Model1.xml");
        //    Assert.NotNull(schema);
        //}

        [Fact]
        public void Deserialize_8bits_Integer()
        {
            Point point = Deserialize("010300000000000A", "Models/Points/Integers/Model_For_8bits_Integer.xml");
            Assert.Equal((sbyte)10, point.DimensionAsSbyte("A"));
        }

        [Fact]
        public void Deserialize_16bits_Integer()
        {
            Point point = Deserialize("010400000000000A00", "Models/Points/Integers/Model_For_16bits_Integer.xml");
            Assert.Equal((short)10, point.DimensionAsShort("A"));
        }

        [Fact]
        public void Deserialize_32bits_Integer()
        {
            Point point = Deserialize("010500000000000A000000", "Models/Points/Integers/Model_For_32bits_Integer.xml");
            Assert.Equal((int)10, point.DimensionAsInt("A"));
        }

        [Fact]
        public void Deserialize_64bits_Integer()
        {
            Point point = Deserialize("010600000000000A00000000000000", "Models/Points/Integers/Model_For_64bits_Integer.xml");
            Assert.Equal((long)10, point.DimensionAsLong("A"));
        }

        [Fact]
        public void Deserialize_8bits_UnsignedInteger()
        {
            Point point = Deserialize("010700000000000A", "Models/Points/UnsignedIntegers/Model_For_8bits_UnsignedInteger.xml");
            Assert.Equal((byte)10, point.DimensionAsByte("A"));
        }

        [Fact]
        public void Deserialize_16bits_UnsignedInteger()
        {
            Point point = Deserialize("010800000000000A00", "Models/Points/UnsignedIntegers/Model_For_16bits_UnsignedInteger.xml");
            Assert.Equal((ushort)10, point.DimensionAsUshort("A"));
        }

        [Fact]
        public void Deserialize_32bits_UnsignedInteger()
        {
            Point point = Deserialize("010900000000000A000000", "Models/Points/UnsignedIntegers/Model_For_32bits_UnsignedInteger.xml");
            Assert.Equal((uint)10, point.DimensionAsUint("A"));
        }

        [Fact]
        public void Deserialize_64bits_UnsignedInteger()
        {
            Point point = Deserialize("010A00000000000A00000000000000", "Models/Points/UnsignedIntegers/Model_For_64bits_UnsignedInteger.xml");
            Assert.Equal((ulong)10, point.DimensionAsUlong("A"));
        }

        [Fact]
        public void Deserialize_32bits_FloatingPoint()
        {
            Point point = Deserialize("010B000000000000002841", "Models/Points/FloatingPoints/Model_For_32bits_FloatingPoint.xml");
            Assert.Equal((float)10.5, point.DimensionAsFloat("A"), 1);
        }

        [Fact]
        public void Deserialize_64bits_FloatingPoint()
        {
            Point point = Deserialize("010C00000000000000000000002540", "Models/Points/FloatingPoints/Model_For_64bits_FloatingPoint.xml");
            Assert.Equal((double)10.5, point.DimensionAsDouble("A"), 1);
        }

        [Fact]
        public void Deserialize_Active()
        {
            Point point = Deserialize("010D000000000020410000A040", "Models/Points/Model_For_Active.xml");
            Assert.Equal((float)0, point.DimensionAsFloat("X"), 1);
            Assert.Equal((float)5, point.DimensionAsFloat("Y"), 1);
        }

        //[Fact]
        //public void Foo()
        //{
        //    // https://adamstorr.azurewebsites.net/blog/span-t-byte-int-conversions-update

        //    PointCloudSchema schema = LoadSchemaFromFile("Models/Model1.xml");

        //    Point point = schema.DeserializePointFromWkb("0101000000FC3C26459E87A342980B05C3");
        //    //Assert.Equal((uint)1, point.SchemaId);

        //    Assert.Equal((float)2659.81, point.DimensionAsFloat("X"), 2);


        //    //string wkbAsString = "0101000000FC3C26459E87A342980B05C3";
        //    //// { "pcid":1,"pt":[26.5981,0.817649,-1.33045]}
        //    //byte[] wkb = StringToByteArray(wkbAsString);

        //    //byte endianness = wkb[0];

        //    //Span<byte> pcidAsBytes = new Span<byte>(wkb, 1, 4);
        //    //uint pcid = MemoryMarshal.Read<uint>(pcidAsBytes);
        //    //Assert.Equal((uint)1, pcid);

        //    //Span<byte> xAsBytes = new Span<byte>(wkb, 5, 4);
        //    //var b = BitConverter.IsLittleEndian;
        //    //float x = MemoryMarshal.Read<float>(xAsBytes);
        //    //Assert.Equal((float)26.59, (float)x, 2);

        //    //Span<byte> yAsBytes = new Span<byte>(wkb, 9, 4);
        //    //float y = MemoryMarshal.Read<float>(yAsBytes);
        //    //Assert.Equal((float)0.81, (float)y, 2);

        //    //Span<byte> zAsBytes = new Span<byte>(wkb, 13, 4);
        //    //float z = MemoryMarshal.Read<float>(zAsBytes);
        //    //Assert.Equal((float)-1.33, (float)z, 2);
        //}

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
