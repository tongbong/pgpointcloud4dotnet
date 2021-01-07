using System;
using System.Collections.Generic;
using System.Text;

namespace Pgpointcloud4dotnet
{
    public class Point
    {

        Dictionary<string, object> _dimensionValues = new Dictionary<string, object>();
    
        public object this[string dimensionName]
        {
            get { return _dimensionValues[dimensionName]; }
            set { _dimensionValues[dimensionName] = value; }
        }

        public float DimensionAsFloat(string dimensionName)
        {
            return DimensionAs<float>(dimensionName);
        }

        public sbyte DimensionAsSbyte(string dimensionName)
        {
            return DimensionAs<sbyte>(dimensionName);
        }

        public short DimensionAsShort(string dimensionName)
        {
            return DimensionAs<short>(dimensionName);
        }

        public int DimensionAsInt(string dimensionName)
        {
            return DimensionAs<int>(dimensionName);
        }

        public long DimensionAsLong(string dimensionName)
        {
            return DimensionAs<long>(dimensionName);
        }

        public byte DimensionAsByte(string dimensionName)
        {
            return DimensionAs<byte>(dimensionName);
        }

        public ushort DimensionAsUshort(string dimensionName)
        {
            return DimensionAs<ushort>(dimensionName);
        }

        public uint DimensionAsUint(string dimensionName)
        {
            return DimensionAs<uint>(dimensionName);
        }

        public ulong DimensionAsUlong(string dimensionName)
        {
            return DimensionAs<ulong>(dimensionName);
        }

        private T DimensionAs<T>(string dimensionName)
        {
            return (T)this[dimensionName];
        }
    }
}
