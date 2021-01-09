using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pgpointcloud4dotnet.Schema
{
    internal class PatchUncompressedDataReader : PatchDataReader
    {
        private PatchHeaderReader _headerReader;

        private int index = 0;

        public PatchUncompressedDataReader(PatchHeaderReader headerReader)
        {
            this._headerReader = headerReader;
            this.index = _headerReader.index;
        }

        public Patch Patch => _headerReader.Patch;

        internal override void FillPatch()
        {
            ReadFromUncompressedData();
        }

        private void ReadFromUncompressedData()
        {
            for (int i = 0; i < Patch.NumberOfPoints; i++)
            {
                int newIndex;
                Point point = DeserializePointFromBinaryData(_headerReader.Wkb, index, out newIndex);
                Patch.AddPoint(point);
                index = newIndex;
            }
        }

        private Point DeserializePointFromBinaryData(byte[] wkb, int startIndex, out int newIndex)
        {
            int index = startIndex;
            Point point = new Point();

            IEnumerable<dimensionType> dimensions = _headerReader.Schema.dimension.OrderBy(x => Convert.ToInt32(x.position));
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
