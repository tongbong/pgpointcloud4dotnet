using Ionic.Zlib;
using Pgpointcloud4dotnet.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Patch DeserializePatchFromWkb(string wkbAsString)
        {
            byte[] wkb = StringToByteArray(wkbAsString);
            return DeserializePatchFromWkb(wkb);
        }

        public Patch DeserializePatchFromWkb(byte[] wkb)
        {
            PatchHeaderReader headerReader = new PatchHeaderReader(this, wkb);

            PatchDataReader dataReader = null;

            switch (headerReader.Compression)
            {
                case 0:
                    {
                        dataReader = new PatchUncompressedDataReader(headerReader);
                        break;
                    }

                case 1:
                    {
                        dataReader = new PatchDimensionalDataReader(headerReader);
                        break;
                    }

                default:
                    throw new InvalidOperationException();
            }
            dataReader.FillPatch();

            return headerReader.Patch;
        }

        public Point DeserializePointFromWkb(string wkbAsString)
        {
            byte[] wkb = StringToByteArray(wkbAsString);
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
                int step = Utils.GetDimensionSize(d);
                switch (d.interpretation)
                {

                    case interpretationType.@float:
                        {
                            newValue = ExtractValue<float>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.@double:
                        {
                            newValue = ExtractValue<double>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int8_t:
                        {
                            newValue = ExtractValue<sbyte>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int16_t:
                        {
                            newValue = ExtractValue<short>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int32_t:
                        {
                            newValue = ExtractValue<int>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.int64_t:
                        {
                            newValue = ExtractValue<long>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint8_t:
                        {
                            newValue = ExtractValue<byte>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint16_t:
                        {
                            newValue = ExtractValue<ushort>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint32_t:
                        {
                            newValue = ExtractValue<uint>(d, wkb, index, step);
                            break;
                        }

                    case interpretationType.uint64_t:
                        {
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

        private static T ExtractValue<T>(dimensionType d, byte[] binaryData, int dataIndex, int dataSize)
            where T : struct
        {
            if (!d.active)
            {
                return default(T);
            }

            return Utils.Read<T>(binaryData, dataIndex, dataSize);
        }
    }
}
