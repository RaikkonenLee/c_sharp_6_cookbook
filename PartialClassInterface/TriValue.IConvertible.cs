using System;

namespace PartialClassInterfaces
{
    /// Partial class that implements IConvertible
    public partial class TriValue : IConvertible
    {
        public bool ToBoolean(IFormatProvider provider)
        {
            if (Average > 0)
                return true;
            else
                return false;
        }

        public byte ToByte(IFormatProvider provider) => Convert.ToByte(Average);

        public char ToChar(IFormatProvider provider)
        {
            decimal val = Average;
            if (val > char.MaxValue)
                val = char.MaxValue;
            if (val < char.MinValue)
                val = char.MinValue;
            return Convert.ToChar((ulong)val);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        public decimal ToDecimal(IFormatProvider provider) => Average;

        public double ToDouble(IFormatProvider provider) => Convert.ToDouble(Average);

        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Average);

        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Average);

        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Average);

        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Average);

        public float ToSingle(IFormatProvider provider) => Convert.ToSingle(Average);

        public string ToString(IFormatProvider provider) => $"({First.ToString(provider)}," +
                $"{Second.ToString(provider)},{Third.ToString(provider)})";

        public object ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(Average, conversionType, provider);

        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Average, provider);

        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Average, provider);

        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Average, provider);
    }
}
