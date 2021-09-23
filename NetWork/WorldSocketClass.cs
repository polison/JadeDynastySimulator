using NetWorkLibrary;
using NetWorkLibrary.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    class WorldSocketClass : WorldSocket
    {
        private bool needEncrypt = false;
        private bool needCompress = false;

        private RC4 s2cEncrypt, c2sEncrypt;
        private MPPC compressor;

        public WorldSocketClass(Socket linkSocket, WorldSocketManager socketManager)
            : base(linkSocket, socketManager)
        {
            compressor = new MPPC();
        }

        protected override void BeforeRead()
        {
            if (needEncrypt)
                ReadBuffer.Write(ReadArgs, c2sEncrypt);
            else
                ReadBuffer.Write(ReadArgs);
        }

        protected override byte[] BeforeSend(WorldPacket packet)
        {
            var bytes = packet.Pack();
            if (needCompress)
            {
                bytes = compressor.Compress(bytes);
                bytes = s2cEncrypt.Encrypt(bytes);
            }
            return bytes;
        }

        protected override void Initialize()
        {
            WorldPacketType = typeof(WorldPacketClass);

            RegisterHandler(0x02, HandleHash);
            RegisterHandler(0x03, HandleLogin);
            RegisterHandler(0x5A, HandlePing);

            SendLink();
        }

        private string userName;
        private byte[] loginHash;
        private byte[] onlineHash;
        private byte[] version = { 0, 4, 8, 0 };
        private string clientSign = "100000a410000094b515a0d41a6";

        private void MakeRandomKey(byte[] bytes)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            r.NextBytes(bytes);
        }

        private byte[] ComputeKey(byte[] authHash)
        {
            var hmacmd5 = new HMACMD5(Encoding.ASCII.GetBytes(userName));
            var macHash = loginHash.Concat(authHash).ToArray();
            var key = hmacmd5.ComputeHash(macHash);
            return key;
        }

        private void SendLink()
        {
            var byteBuffer = new ByteBuffer();
            var linkPacket = new WorldPacketClass(byteBuffer);
            linkPacket.ID = 0x01;
            onlineHash = new byte[0x10];
            MakeRandomKey(onlineHash);
            byteBuffer.Write((byte)onlineHash.Length);
            byteBuffer.Write(onlineHash);
            byteBuffer.Write(version);
            var bytes = Encoding.ASCII.GetBytes(clientSign);
            byteBuffer.Write((byte)bytes.Length);
            byteBuffer.Write(bytes);
            for (int i = 0; i < 9; i++)
                byteBuffer.Write((byte)0);

            SendPacket(linkPacket);
        }

        private void HandleLogin(byte[] packetData)
        {
            var byteBuffer = new ByteBuffer(packetData);
            int length = byteBuffer.ReadByte();
            userName = Encoding.ASCII.GetString(byteBuffer.ReadBytes(length));
            worldSocketManager.Log(LogType.Message, "客户端[{0}]{1}请求登录用户{2}！", ID, connSocket.RemoteEndPoint, userName);
            var hashLength = byteBuffer.ReadByte();
            loginHash = byteBuffer.ReadBytes(hashLength);

            ByteBuffer packer = new ByteBuffer();
            var authHash = new byte[0x10];
            MakeRandomKey(authHash);
            packer.Write((byte)authHash.Length);
            packer.Write(authHash);
            packer.Write(0x00);
            var authPacket = new WorldPacketClass(packer);
            authPacket.ID = 0x02;
            SendPacket(authPacket);

            var c2sKey = ComputeKey(authHash);
            c2sEncrypt = new RC4(c2sKey);
            needEncrypt = true;
        }

        private void HandleHash(byte[] packetData)
        {
            var byteBuffer = new ByteBuffer(packetData);
            int length = byteBuffer.ReadByte();
            var authHash = byteBuffer.ReadBytes(length);
            var s2cKey = ComputeKey(authHash);
            s2cEncrypt = new RC4(s2cKey);
            needCompress = true;
        }

        private void HandlePing(byte[] packetData)
        {
            var packer = new ByteBuffer();
            packer.Write((byte)0x5A);
            var pingPacket = new WorldPacketClass(packer);
            pingPacket.ID = 0x5A;
            SendPacket(pingPacket);
        }
    }
}
