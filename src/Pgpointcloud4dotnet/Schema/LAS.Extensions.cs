using Ionic.Zlib;
using Pgpointcloud4dotnet.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public Patch DeserializePatchFromWkb(string wkbAsString)
        {
            byte[] wkb = StringToByteArray(wkbAsString);
            return DeserializePatchFromWkb(wkb);
        }

        public Patch DeserializePatchFromWkb(byte[] wkb)
        {
            //byte endianness = wkb[0];

            int index = 0;

            // Read endianess
            index += 1;

            // Read pcid
            uint pcid = Utils.Read<uint>(wkb, index, 4);
            index += 4;

            // Read compression
            uint compression = Utils.Read<uint>(wkb, index, 4);
            index += 4;

            // Read number of points
            uint numberOfPoints = Utils.Read<uint>(wkb, index, 4);
            index += 4;

            Patch patch = new Patch(numberOfPoints);

            switch (compression)
            {
                case 0:
                    {
                        for (int i = 0; i < numberOfPoints; i++)
                        {
                            int newIndex;
                            Point point = DeserializePointFromBinaryData(wkb, index, out newIndex);
                            patch.AddPoint(point);
                            index = newIndex;
                        }
                        break;
                    }

                case 1:
                    {
                        for (int i = 0; i < numberOfPoints; i++)
                        {
                            patch.AddPoint(new Point());
                        }
                        List<dimensionType> dimensions = dimension
                                                            .OrderBy(x => Convert.ToInt32(x.position))
                                                            .ToList();

                        foreach (var dimension in dimensions)
                        {
                            // Read compression type
                            byte compressionType = Utils.Read<byte>(wkb, index, 1);
                            index += 1;

                            // Read size of compressed data
                            uint sizeOfCompressedData = Utils.Read<uint>(wkb, index, 4);
                            index += 4;

                            switch (compressionType)
                            {
                                case 0:
                                    {
                                        throw new InvalidOperationException();
                                        break;
                                    }
                                case 1:
                                    {
                                        throw new InvalidOperationException();
                                        break;
                                    }
                                case 2:
                                    {
                                        throw new InvalidOperationException();
                                        break;
                                    }
                                case 3:
                                    {
                                        Span<byte> compressedDimensionData = new Span<byte>(wkb, index, (int)sizeOfCompressedData);
                                        index += (int)sizeOfCompressedData;

                                        byte[] uncompressedData = ZlibStream.UncompressBuffer(compressedDimensionData.ToArray());

                                        int dimensionSize = Utils.GetDimensionSize(dimension);

                                        int dataIndex = 0;
                                        for (int pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
                                        {
                                            float dimensionValue = Utils.Read<float>(uncompressedData, dataIndex, dimensionSize);
                                            patch.Points[pointIndex][dimension.name] = dimensionValue;
                                            dataIndex += dimensionSize;
                                        }

                                        break;
                                    }
                                default:
                                    throw new InvalidOperationException();
                            }
                        }

                        break;
                    }

                default:
                    throw new InvalidOperationException();
            }

            return patch;
        }

        private Point DeserializePointFromBinaryData(byte[] wkb, int startIndex, out int newIndex)
        {
            int index = startIndex;
            Point point = new Point();

            IEnumerable<dimensionType> dimensions = dimension.OrderBy(x => Convert.ToInt32(x.position));
            foreach (var d in dimensions)
            {
                object newValue = null;
                int dimensionSize = Utils.GetDimensionSize(d);

                switch (d.interpretation)
                {

                    case interpretationType.@float:
                        {
                            newValue = ExtractValue<float>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.@double:
                        {
                            newValue = ExtractValue<double>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.int8_t:
                        {
                            newValue = ExtractValue<sbyte>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.int16_t:
                        {
                            newValue = ExtractValue<short>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.int32_t:
                        {
                            newValue = ExtractValue<int>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.int64_t:
                        {
                            newValue = ExtractValue<long>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.uint8_t:
                        {
                            newValue = ExtractValue<byte>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.uint16_t:
                        {
                            newValue = ExtractValue<ushort>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.uint32_t:
                        {
                            newValue = ExtractValue<uint>(d, wkb, index, dimensionSize);
                            break;
                        }

                    case interpretationType.uint64_t:
                        {
                            newValue = ExtractValue<ulong>(d, wkb, index, dimensionSize);
                            break;
                        }

                    default:
                        throw new InvalidOperationException("Type " + d.interpretation + " is not supported");
                }

                point[d.name] = newValue;
                index += dimensionSize;
            }
            newIndex = index;
            return point;
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
