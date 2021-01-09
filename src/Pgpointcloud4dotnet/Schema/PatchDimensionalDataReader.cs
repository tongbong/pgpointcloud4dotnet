using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pgpointcloud4dotnet.Schema
{
    internal class PatchDimensionalDataReader : PatchDataReader
    {
        private PatchHeaderReader _headerReader;

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
                            Span<byte> compressedDimensionData = new Span<byte>(_headerReader.Wkb, index, (int)sizeOfCompressedData);
                            index += (int)sizeOfCompressedData;

                            byte[] uncompressedData = ZlibStream.UncompressBuffer(compressedDimensionData.ToArray());

                            int dimensionSize = Utils.GetDimensionSize(dimension);

                            int dataIndex = 0;
                            for (int pointIndex = 0; pointIndex < Patch.NumberOfPoints; pointIndex++)
                            {
                                float dimensionValue = Utils.Read<float>(uncompressedData, dataIndex, dimensionSize);
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
