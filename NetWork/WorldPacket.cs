using NetWorkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    class WorldPacket : BaseWorldPacket
    {
        private readonly byte id;

        public WorldPacket(ByteBuffer byteBuffer)
            : base(byteBuffer)
        {
        }

        public WorldPacket(byte _id, ByteBuffer byteBuffer)
            : base(byteBuffer)
        {
            id = _id;
        }

        public override byte[] Pack()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteByte(id);
            WriteLength(buffer);
            buffer.Write(ByteBuffer.GetBytes());
            return buffer.GetBytes();
        }

        private void WriteLength(ByteBuffer buffer)
        {
            var length = ByteBuffer.GetLength();
            if (length > 0x7f)
                buffer.WriteByte((byte)((length >> 8 & 0x7F) | 0x80));
            buffer.WriteByte((byte)(length & 0xFF));
        }

        public override int ReadPacketID()
        {
            return ByteBuffer.ReadByte();
        }

        public override int ReadPacketLength()
        {
            var result = 0;
            var b = ByteBuffer.ReadByte();
            if (b < 0x80)
                result = b;
            else
            {
                var b1 = ByteBuffer.ReadByte();
                result = ((b - 0x80) << 8) + b1;
            }
            return result;
        }
    }
}
