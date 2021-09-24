using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    /// <summary>
    /// you can use this class to convert datarow to common type.<br/>
    /// tips: used error convert function may thow exception.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// value is a null string.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// value does not implement the System.IConvertible interface. -or- The conversion of value to a System.Char is not supported.
    /// </exception>
    class TypedDataRow
    {
        private DataRow Row;

        public TypedDataRow(DataRow row)
        {
            Row = row;
        }

        public bool ToBoolean(string column)
        {
            return Convert.ToBoolean(Row[column]);
        }

        public bool ToBoolean(int index)
        {
            return Convert.ToBoolean(Row[index]);
        }

        public byte ToByte(string column)
        {
            return Convert.ToByte(Row[column]);
        }

        public byte ToByte(int index)
        {
            return Convert.ToByte(Row[index]);
        }

        public ushort ToUInt16(string column)
        {
            return Convert.ToUInt16(Row[column]);
        }

        public ushort ToUInt16(int index)
        {
            return Convert.ToUInt16(Row[index]);
        }

        public uint ToUInt32(string column)
        {
            return Convert.ToUInt32(Row[column]);
        }

        public uint ToUInt32(int index)
        {
            return Convert.ToUInt32(Row[index]);
        }

        public ulong ToUInt64(string column)
        {
            return Convert.ToUInt64(Row[column]);
        }

        public ulong ToUInt64(int index)
        {
            return Convert.ToUInt64(Row[index]);
        }

        public char ToChar(string column)
        {
            return Convert.ToChar(Row[column]);
        }

        public char ToChar(int index)
        {
            return Convert.ToChar(Row[index]);
        }

        public short ToInt16(string column)
        {
            return Convert.ToInt16(Row[column]);
        }

        public short ToInt16(int index)
        {
            return Convert.ToInt16(Row[index]);
        }

        public int ToInt32(string column)
        {
            return Convert.ToInt32(Row[column]);
        }

        public int ToInt32(int index)
        {
            return Convert.ToInt32(Row[index]);
        }

        public long ToInt64(string column)
        {
            return Convert.ToInt64(Row[column]);
        }

        public long ToInt64(int index)
        {
            return Convert.ToInt64(Row[index]);
        }

        public float ToSingle(string column)
        {
            return Convert.ToSingle(Row[column]);
        }

        public float ToSingle(int index)
        {
            return Convert.ToSingle(Row[index]);
        }

        public double ToDouble(string column)
        {
            return Convert.ToDouble(Row[column]);
        }

        public double ToDouble(int index)
        {
            return Convert.ToDouble(Row[index]);
        }

        public byte[] ToByteArray(string column)
        {
            var obj = Row[column];
            if (obj is byte[])
                return obj as byte[];
            else
                return null;
        }

        public byte[] ToByteArray(int index)
        {
            var obj = Row[index];
            if (obj is byte[])
                return obj as byte[];
            else
                return null;
        }

        public string ToString(string column)
        {
            return Row[column].ToString();
        }

        public string ToString(int index)
        {
            return Row[index].ToString();
        }
    }
}
