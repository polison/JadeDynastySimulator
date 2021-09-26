using NetWorkLibrary;
using NetWorkLibrary.Algorithm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    class WorldSocket : BaseWorldSocket
    {
        private bool needEncrypt = false;
        private bool needCompress = false;

        private DataTable dataTable = null;

        private int accountId;
        private string userName;
        private byte[] loginHash;
        private byte[] onlineHash;
        private byte[] version = { 0, 4, 8, 0 };
        private string clientSign = "100000a410000094b515a0d41a6";
        private byte authHashLength = 0x10;

        private RC4 s2cEncrypt, c2sEncrypt;
        private MPPC compressor;
        private Dictionary<int, Player> accountPlayers;

        public WorldSocket(Type packetType, Socket linkSocket, WorldSocketManager socketManager)
            : base(packetType, linkSocket, socketManager)
        {
            accountId = -1;
            compressor = new MPPC();
            dataTable = new DataTable();
            accountPlayers = new Dictionary<int, Player>();
        }

        protected override void Initialize()
        {
            RegisterHandler((int)SocketOpcode.SCMSG_AUTH_SESSION, HandleHash);
            RegisterHandler((int)SocketOpcode.CMSG_LOGIN, HandleLogin);
            RegisterHandler((int)SocketOpcode.CMSG_ROLELIST, HandleCharacterEnum);
            RegisterHandler((int)SocketOpcode.CMSG_CREATEROLE, HandleCharacterCreate);
            RegisterHandler((int)SocketOpcode.CMSG_DELETEROLE, HandleCharacterDelete);
            RegisterHandler((int)SocketOpcode.CMSG_REGAINROLE, HandleCharacterRestore);
            RegisterHandler((int)SocketOpcode.SCMSG_PING, HandlePing);

            SendLink();
        }

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

        private bool ArrayCompare(byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;

            return BitConverter.ToString(a) == BitConverter.ToString(b);
        }

        private void SendLink()
        {
            var packer = new ByteBuffer();
            var link = new WorldPacket((int)SocketOpcode.SMSG_ACCEPT_CONNECT, packer);
            onlineHash = new byte[authHashLength];
            MakeRandomKey(onlineHash);
            packer.WriteByte(authHashLength);
            packer.Write(onlineHash);
            packer.Write(version);
            var bytes = Encoding.ASCII.GetBytes(clientSign);
            packer.WriteByte((byte)bytes.Length);
            packer.Write(bytes);
            for (int i = 0; i < 9; i++)
                packer.WriteByte(0);
            SendPacket(link);
        }

        private void HandleLogin(byte[] packetData)
        {
            var reader = new ByteBuffer(packetData);
            int length = reader.ReadByte();
            userName = Encoding.ASCII.GetString(reader.ReadBytes(length));
            worldSocketManager.Log(LogType.Message, "客户端[{0}]{1}请求登录用户{2}！", ID, connSocket.RemoteEndPoint, userName);
            var hashLength = reader.ReadByte();
            loginHash = reader.ReadBytes(hashLength);

            var packer = new ByteBuffer();
            if (DataBaseManager.Instance.PQuery(dataTable, "select id,upwd from USERS where uname='{0}'", userName))
            {
                if (dataTable.Rows.Count > 0)
                {
                    var row = new TypedDataRow(dataTable.Rows[0]);
                    accountId = row.ToInt32("id");
                    var loginMd5 = row.ToByteArray("upwd");
                    var hMACMD = new HMACMD5(loginMd5);
                    var dbHash = hMACMD.ComputeHash(onlineHash);
                    if (ArrayCompare(loginHash, dbHash))
                    {
                        var auth = new WorldPacket((int)SocketOpcode.SCMSG_AUTH_SESSION, packer);
                        var authHash = new byte[authHashLength];
                        MakeRandomKey(authHash);
                        packer.WriteByte(authHashLength);
                        packer.Write(authHash);
                        packer.WriteInt32(0x00);
                        SendPacket(auth);
                        var c2sKey = ComputeKey(authHash);
                        c2sEncrypt = new RC4(c2sKey);
                        needEncrypt = true;
                        return;
                    }
                }
            }

            var error = new WorldPacket((int)SocketOpcode.SMSG_ERROR, packer);
            packer.WriteByte((byte)ServerError.SE_WRONGUSERORPWD);
            var bytes = Encoding.ASCII.GetBytes("Server Error!");
            packer.WriteByte((byte)bytes.Length);
            packer.Write(bytes);
            SendPacket(error);
        }

        private void HandleHash(byte[] packetData)
        {
            var reader = new ByteBuffer(packetData);
            int length = reader.ReadByte();
            var authHash = reader.ReadBytes(length);
            var forceLogin = reader.ReadBoolean();

            var s2cKey = ComputeKey(authHash);
            s2cEncrypt = new RC4(s2cKey);
            needCompress = true;

            var packer = new ByteBuffer();
            var line = new WorldPacket((int)SocketOpcode.SMSG_LINE_INFO, packer);
            packer.WriteUInt32(1);
            packer.WriteByte(1);
            var lineName = Encoding.Unicode.GetBytes("单机一线");
            packer.WriteByte((byte)lineName.Length);
            packer.Write(lineName);
            packer.WriteByte(8);
            for (int j = 0; j < 13; j++)
                packer.WriteByte(0);
            SendPacket(line);

            packer = new ByteBuffer();
            if (DataBaseManager.Instance.PQuery(dataTable, "select online from USERS where id={0};", accountId))
            {
                var online = Convert.ToBoolean(dataTable.Rows[0]["online"]);
                if (online && !forceLogin)
                {
                    var error = new WorldPacket((int)SocketOpcode.SMSG_ERROR, packer);
                    packer.WriteByte((byte)ServerError.SE_LOGINED);
                    var bytes = Encoding.ASCII.GetBytes("Server Error!");
                    packer.WriteByte((byte)bytes.Length);
                    packer.Write(bytes);
                    SendPacket(error);
                }
                else
                {
                    DataBaseManager.Instance.PExecute("UPDATE USERS set online=1 where id={0};", accountId);
                    var account = new WorldPacket((int)SocketOpcode.SMSG_ACOUNT, packer);
                    packer.WriteInt32(accountId, false);
                    packer.WriteInt32(0);
                    packer.WriteInt32(0);
                    packer.WriteInt32(500, false);
                    packer.WriteInt32(0);
                    packer.WriteInt32(-1);
                    packer.WriteInt32(0);
                    SendPacket(account);
                }
            }
        }

        private void HandleCharacterEnum(byte[] packetData)
        {
            var reader = new ByteBuffer(packetData);
            Player player = Player.LoadPlayer(this, reader);
            if (player != null)
                accountPlayers.Add(player.GetID(), player);
        }

        private void HandleCharacterCreate(byte[] packetData)
        {
            var reader = new ByteBuffer(packetData);
            Player player = Player.CreatePlayer(this, reader);
            if (player != null)
                accountPlayers.Add(player.GetID(), player);
        }

        private void HandleCharacterDelete(byte[] packetData)
        {
            var reader = new ByteBuffer(packetData);
            var roleId = reader.ReadInt32(false);
            var sid = reader.ReadInt32(false);
            var packer = new ByteBuffer();
            if (accountPlayers.ContainsKey(roleId))
            {
                packer.WriteInt32(0);
                accountPlayers[roleId].SetStatus(0x03);
            }
            else
            {
                packer.WriteInt32((int)ServerError.SE_DELETEFAIL);
            }
            packer.WriteInt32(roleId, false);
            packer.WriteInt32(sid, false);
            var delete = new WorldPacket((int)SocketOpcode.SMSG_DELETEROLE, packer);
            SendPacket(delete);
        }

        private void HandleCharacterRestore(byte[] packetData)
        {
            var reader = new ByteBuffer(packetData);
            var roleId = reader.ReadInt32(false);
            var sid = reader.ReadInt32(false);
            var packer = new ByteBuffer();
            if (accountPlayers.ContainsKey(roleId))
            {
                packer.WriteInt32(0);
                accountPlayers[roleId].SetStatus(0x01);
            }
            else
            {
                packer.WriteInt32((int)ServerError.SE_DELETEFAIL);
            }
            packer.WriteInt32(roleId, false);
            packer.WriteInt32(sid, false);
            var restore = new WorldPacket((int)SocketOpcode.SMSG_REGAINROLE, packer);
            SendPacket(restore);
        }

        private void HandlePing(byte[] packetData)
        {
            var packer = new ByteBuffer();
            var ping = new WorldPacket((int)SocketOpcode.SCMSG_PING, packer);
            packer.WriteByte(0x5A);
            SendPacket(ping);
        }

        protected override void BeforeRead()
        {
            if (needEncrypt)
                ReadBuffer.Write(ReadArgs, c2sEncrypt);
            else
                ReadBuffer.Write(ReadArgs);
        }

        protected override void HandleUnRegister(int cmdId, byte[] packetData)
        {
            worldSocketManager.Log(LogType.Warning, "cmd:0x{0:X2},data:{1}", cmdId, BitConverter.ToString(packetData));
        }

        protected override byte[] BeforeSend(BaseWorldPacket packet)
        {
            var bytes = packet.Pack();
            if (needCompress)
            {
                bytes = compressor.Compress(bytes);
                bytes = s2cEncrypt.Encrypt(bytes);
            }

            return bytes;
        }

        protected override void BeforeClose()
        {
            if (accountId > 0)
            {
                DataBaseManager.Instance.PExecute("UPDATE USERS set online=0 where id={0};", accountId);
                accountId = -1;
            }
        }
    }
}
