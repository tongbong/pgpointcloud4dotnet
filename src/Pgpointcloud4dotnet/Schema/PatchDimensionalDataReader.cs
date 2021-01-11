using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pgpointcloud4dotnet.Schema
{
    internal class PatchDimensionalDataReader : PatchDataReader
    {
        private readonly PatchHeaderReader _headerReader;

        private int index = 0;

        public PatchDimensionalDataReader(PatchHeaderReader headerReader)
        {
            _headerReader = headerReader;
            this.index = _headerReader.index;
        }

        public Patch Patch => _headerReader.Patch;

        internal override void FillPatch()
        {
            for (int i = 0; i < Patch.NumberOfPoints; i++)
            {
                Patch.AddPoint(new Point());
            }
            List<dimensionType> dimensions = _headerReader.Schema.dimension
                                                .OrderBy(x => Convert.ToInt32(x.position))
                                                .ToList();

            foreach (var dimension in dimensions)
            {
                // Read compression type
                byte compressionType = Utils.Read<byte>(_headerReader.Wkb, index, 1);
                index += 1;

                // Read size of compressed data
                uint sizeOfCompressedData = Utils.Read<uint>(_headerReader.Wkb, index, 4);
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
                            byte wordRepeats = Utils.Read<byte>(_headerReader.Wkb, index, 1);
                            index += 1;

                            object dimensionValue = null;

                            switch (dimension.interpretation)
                            {
                                case interpretationType.@float:
                                    {
                                        dimensionValue = Utils.Read<float>(_headerReader.Wkb, index, 4);
                                        index += 4;
                                        break;
                                    }

                                case interpretationType.@double:
                                    {
                                        dimensionValue = Utils.Read<double>(_headerReader.Wkb, index, 8);
                                        index += 8;
                                        break;
                                    }

                                case interpretationType.int8_t:
                                    {
                                        dimensionValue = Utils.Read<sbyte>(_headerReader.Wkb, index, 1);
                                        index += 1;
                                        break;
                                    }

                                case interpretationType.int16_t:
                                    {
                                        dimensionValue = Utils.Read<short>(_headerReader.Wkb, index, 2);
                                        index += 2;
                                        break;
                                    }

                                case interpretationType.int32_t:
                                    {
                                        dimensionValue = Utils.Read<int>(_headerReader.Wkb, index, 4);
                                        index += 4;
                                        break;
                                    }

                                case interpretationType.int64_t:
                                    {
                                        dimensionValue = Utils.Read<long>(_headerReader.Wkb, index, 8);
                                        index += 8;
                                        break;
                                    }

                                case interpretationType.uint8_t:
                                    {
                                        dimensionValue = Utils.Read<byte>(_headerReader.Wkb, index, 1);
                                        index += 1;
                                        break;
                                    }

                                case interpretationType.uint16_t:
                                    {
                                        dimensionValue = Utils.Read<ushort>(_headerReader.Wkb, index, 2);
                                        index += 2;
                                        break;
                                    }

                                case interpretationType.uint32_t:
                                    {
                                        dimensionValue = Utils.Read<uint>(_headerReader.Wkb, index, 4);
                                        index += 4;
                                        break;
                                    }

                                case interpretationType.uint64_t:
                                    {
                                        dimensionValue = Utils.Read<ulong>(_headerReader.Wkb, index, 8);
                                        index += 8;
                                        break;
                                    }

                                default:
                                    throw new InvalidOperationException("Type " + dimension.interpretation + " is not supported");
                            }

                            if (dimensionValue != null)
                            {
                                foreach (var p in Patch.Points)
                                {
                                    p[dimension.name] = dimensionValue;
                                }
                            }

                            //throw new InvalidOperationException();
                            break;
                        }
                    case 2:
                        {
                            throw new InvalidOperationException();
                            break;
                        }
                    case 3:
                        {
                            Span<byte> compressedDimensionData = new Span<byte>(_headerReader.Wkb, index, (int)sizeOfCompressedData);
                            index += (int)sizeOfCompressedData;

                            byte[] uncompressedData = ZlibStream.UncompressBuffer(compressedDimensionData.ToArray());
                            int dimensionSize = Utils.GetDimensionSize(dimension);

                            int dataIndex = 0;
                            for (int pointIndex = 0; pointIndex < Patch.NumberOfPoints; pointIndex++)
                            {
                                object dimensionValue = null;

                                switch (dimension.interpretation)
                                {
                                    case interpretationType.@float:
                                        {
                                            dimensionValue = Utils.Read<float>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.@double:
                                        {
                                            dimensionValue = Utils.Read<double>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.int8_t:
                                        {
                                            dimensionValue = Utils.Read<sbyte>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.int16_t:
                                        {
                                            dimensionValue = Utils.Read<short>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.int32_t:
                                        {
                                            dimensionValue = Utils.Read<int>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.int64_t:
                                        {
                                            dimensionValue = Utils.Read<long>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.uint8_t:
                                        {
                                            dimensionValue = Utils.Read<byte>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.uint16_t:
                                        {
                                            dimensionValue = Utils.Read<ushort>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.uint32_t:
                                        {
                                            dimensionValue = Utils.Read<uint>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    case interpretationType.uint64_t:
                                        {
                                            dimensionValue = Utils.Read<ulong>(uncompressedData, dataIndex, dimensionSize);
                                            break;
                                        }

                                    default:
                                        throw new InvalidOperationException("Type " + dimension.interpretation + " is not supported");
                                }


                                Patch.Points[pointIndex][dimension.name] = dimensionValue;
                                dataIndex += dimensionSize;
                            }

                            break;
                        }
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
