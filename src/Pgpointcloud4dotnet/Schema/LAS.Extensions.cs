using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Pgpointcloud4dotnet
{
    public partial class PointCloudSchema
    {

        public static PointCloudSchema LoadSchemaFromFile(string schemaFile)
        {
            XmlSerializer deserialize = new XmlSerializer(typeof(PointCloudSchema));
            PointCloudSchema schema = null;
            using (var stream = File.OpenRead(schemaFile))
            {
                schema = (PointCloudSchema)deserialize.Deserialize(stream);
            }
            return schema;
        }

        public Point DeserializePointFromWkb(string wkbAsString)
        {
            byte[] wkb = StringToByteArray(wkbAsString);

            //byte endianness = wkb[0];

            //Span<byte> pcidAsBytes = new Span<byte>(wkb, 1, 4);
            //uint pcid = MemoryMarshal.Read<uint>(pcidAsBytes);
            //Assert.Equal((uint)1, pcid);

            return DeserializePointFromWkb(wkb);
        }

        public Point DeserializePointFromWkb(byte[] wkb)
        {
            Point point = new Point();

            IEnumerable<dimensionType> dimensions = dimension.OrderBy(x => Convert.ToInt32(x.position));
            int index = 5;
            foreach (var d in dimensions)
            {
                object newValue = null;
                int step = 0;
                switch (d.interpretation)
                {

                    case interpretationType.@float:
                        {
                            step = 4;
                            newValue = ExtractValue<float>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.@double:
                        {
                            step = 8;
                            newValue = ExtractValue<double>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int8_t:
                        {
                            step = 1;
                            newValue = ExtractValue<sbyte>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int16_t:
                        {
                            step = 2;
                            newValue = ExtractValue<short>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int32_t:
                        {
                            step = 4;
                            newValue = ExtractValue<int>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int64_t:
                        {
                            step = 8;
                            newValue = ExtractValue<long>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint8_t:
                        {
                            step = 1;
                            newValue = ExtractValue<byte>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint16_t:
                        {
                            step = 2;
                            newValue = ExtractValue<ushort>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint32_t:
                        {
                            step = 4;
                            newValue = ExtractValue<uint>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint64_t:
                        {
                            step = 8;
                            newValue = ExtractValue<ulong>(d, wkb, index, step);
                            break;
                        }

                    default:
                        throw new InvalidOperationException("Type " + d.interpretation + " is not supported");
                }

                point[d.name] = newValue;
                index += step;
            }
            return point;
        }

        private static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private static T ExtractValue<T>(dimensionType d, byte[] wkb, int index, int step)
            where T : struct
        {
            if (!d.active)
            {
                return default(T);
            }
            Span<byte> intAsBytes = new Span<byte>(wkb, index, step);
            return MemoryMarshal.Read<T>(intAsBytes);
        }
    }
}
