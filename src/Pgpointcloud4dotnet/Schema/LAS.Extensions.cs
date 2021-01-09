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
            Span<byte> pcidAsBytes = new Span<byte>(wkb, index, 4);
            uint pcid = MemoryMarshal.Read<uint>(pcidAsBytes);
            index += 4;

            // Read compression
            Span<byte> compressionAsBytes = new Span<byte>(wkb, index, 4);
            uint compression = MemoryMarshal.Read<uint>(compressionAsBytes);
            index += 4;

            // Read number of points
            Span<byte> numberOfPointsAsBytes = new Span<byte>(wkb, index, 4);
            uint numberOfPoints = MemoryMarshal.Read<uint>(numberOfPointsAsBytes);
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

                        for (int i = 0; i < dimensions.Count; i++)
                        {
                            // Read compression type
                            Span<byte> compressionTypeAsBytes = new Span<byte>(wkb, index, 1);
                            byte compressionType = MemoryMarshal.Read<byte>(compressionTypeAsBytes);
                            index += 1;

                            // Read size of compressed data
                            Span<byte> sizeOfCompressedDataAsBytes = new Span<byte>(wkb, index, 4);
                            uint sizeOfCompressedData = MemoryMarshal.Read<uint>(sizeOfCompressedDataAsBytes);
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

                                        int dimensionSize = Utils.GetDimensionSize(dimensions[i]);

                                        int dataIndex = 0;
                                        for (int pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
                                        {
                                            float dimensionValue = Utils.Read<float>(uncompressedData, dataIndex, dimensionSize);
                                            patch.Points[pointIndex][dimensions[i].name] = dimensionValue;
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
