using NetWorkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    class WorldPacketClass : WorldPacket
    {
        public byte ID;

        public WorldPacketClass(ByteBuffer byteBuffer)
            :base(byteBuffer)
        {

        }

        public override byte[] Pack()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.Write(ID);
            WriteLength(buffer);
            buffer.Write(ByteBuffer.GetBytes());
            return buffer.GetBytes();
        }

        private void WriteLength(ByteBuffer buffer)
        {
            var length = ByteBuffer.GetLength();
            buffer.Write((byte)length);
            if (length > 0x7f)
                buffer.Write((byte)((length >> 8 & 0x7F) | 0x80));
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
