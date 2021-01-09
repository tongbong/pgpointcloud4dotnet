using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Pgpointcloud4dotnet.Schema
{
    internal class PatchHeaderReader
    {

        internal int index = 0;

        internal byte[] Wkb { get; }
        internal PointCloudSchema Schema { get; }

        internal uint Compression { get; private set; }

        public Patch Patch { get; private set; }

        public PatchHeaderReader(byte[] wkb)
        {
            this.Wkb = wkb;
        }

        public PatchHeaderReader(PointCloudSchema pointCloudSchema, byte[] wkb)
        {
            this.Schema = pointCloudSchema;
            this.Wkb = wkb;
            Initialize();
        }

        private void Initialize()
        {
            ReadEndianess();
            ReadPcid();
            ReadCompression();
            ReadNumberOfPoints();
        }

        internal void ReadEndianess()
        {
            index += 1;
        }

        internal void ReadPcid()
        {
            uint pcid = Utils.Read<uint>(Wkb, index, 4);
            index += 4;
        }

        private void ReadCompression()
        {
            uint compression = Utils.Read<uint>(Wkb, index, 4);
            index += 4;
            Compression = compression;
        }

        internal uint ReadNumberOfPoints()
        {
            uint numberOfPoints = Utils.Read<uint>(Wkb, index, 4);
            index += 4;
            Patch = new Patch(numberOfPoints);
            return numberOfPoints;
        }

    }
}
