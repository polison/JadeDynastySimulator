using NetWorkLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    class Player
    {
        public static Player CreatePlayer(WorldSocket socket, ByteBuffer reader)
        {
            var _accountid = reader.ReadUInt32(false);
            Player player = new Player(_accountid, socket);
            reader.ReadBytes(4);
            var index = reader.ReadInt32() + 1;
            if (player.Initailize(index, reader))
                return player;
            else
                return null;
        }

        public static Player LoadPlayer(WorldSocket socket, ByteBuffer reader)
        {
            Player player = null;
            var _accountid = reader.ReadUInt32(false);
            reader.ReadBytes(4);
            var index = reader.ReadInt32() + 1;

            var packer = new ByteBuffer();
            var character = new WorldPacket((int)SocketOpcode.SMSG_ROLELIST, packer);
            var dataTable = new DataTable();
            if (DataBaseManager.Instance.PQuery(dataTable, "select * from roles where uid={0} limit {1},1;", _accountid, index))
            {
                if (dataTable.Rows.Count > 0)
                {
                    var row = new TypedDataRow(dataTable.Rows[0]);
                    player = new Player(_accountid, socket);
                    player.Initailize(index, row);
                    player.MakeEnumPacket(packer);
                }
                else
                {
                    packer.WriteInt32(0);
                    packer.WriteInt32(-1);
                    packer.WriteUInt32(_accountid, false);
                    packer.WriteInt32(0);
                    packer.WriteByte(0);
                    packer.WriteInt32(0);
                }
            }
            socket.SendPacket(character);
            return player;
        }

        private void Initailize(int index, TypedDataRow row)
        {
            roleIndex = index;
            roleid = row.ToUInt32("id");
            sex = row.ToByte("sex");
            faceid = row.ToByte("faceid");
            hairid = row.ToByte("hairid");
            earid = row.ToByte("earid");
            tailid = row.ToByte("tailid");
            classid = row.ToByte("class");
            level = row.ToByte("level");
            rolename = row.ToString("rolename");
            status = row.ToByte("status");
            createTime = UnixTime.ToDateTime(row.ToUInt32("createtime"));
            deleteTime = UnixTime.ToDateTime(row.ToUInt32("deletetime"));
            lastLoginTime = UnixTime.ToDateTime(row.ToUInt32("lastlogintime"));
            showclassic = row.ToBoolean("showclassic");
            fashionMode = row.ToBoolean("fashionmode");
            powerid = row.ToByte("powerid");
            mapid = row.ToUInt32("mapid");
            posx = row.ToSingle("posx");
            posh = row.ToSingle("posy");
            posy = row.ToSingle("posz");
            fashionHead = row.ToUInt32("fashionhead");
            fashionCloth = row.ToUInt32("fashioncloth");
            fashionShoes = row.ToUInt32("fashionshoes");
            fashionWeapon = row.ToUInt32("fashionweapon");
        }

        private bool Initailize(int index, ByteBuffer reader)
        {
            roleIndex = index;
            sex = reader.ReadByte();
            faceid = reader.ReadByte();
            hairid = reader.ReadByte();
            earid = reader.ReadByte();
            tailid = reader.ReadByte();
            classid = reader.ReadByte();
            level = (byte)reader.ReadUInt32(false);
            var namelength = reader.ReadByte();
            rolename = Encoding.Unicode.GetString(reader.ReadBytes(namelength));
            status = 1;
            createTime = DateTime.Now;
            deleteTime = UnixTime.ToDateTime(0);
            lastLoginTime = UnixTime.ToDateTime(0);
            reader.ReadBytes(36);
            showclassic = reader.ReadBoolean(); ;
            fashionMode = true;
            powerid = 0;
            if (sex == 0)
            {
                fashionHead = 66906;
                fashionCloth = 66904;
                fashionShoes = 66909;
            }
            else
            {
                fashionHead = 66911;
                fashionCloth = 66910;
                fashionShoes = 66912;
            }
            fashionWeapon = 0;

            var packer = new ByteBuffer();
            var character = new WorldPacket((int)SocketOpcode.SMSG_CREATEROLE, packer);

            var insertformat = "insert into roles(uid, rolename, class, `level`, sex, faceid, hairid, earid, tailid, showclassic,  createtime, lastlogintime,fashionmode,fashionhead,fashioncloth,fashionshoes,fashionweapon) values({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})";
            if (DataBaseManager.Instance.PExecute(insertformat, accountid, rolename, classid, level, sex, faceid, hairid, earid, tailid, showclassic
                , UnixTime.ToTimestamp(createTime), UnixTime.ToTimestamp(lastLoginTime), fashionMode, fashionHead
                , fashionCloth, fashionShoes, fashionWeapon))
            {
                var dataTable = new DataTable();
                if (DataBaseManager.Instance.PQuery(dataTable, "select id,mapid,posx,posy,posz from roles where rolename='{0}';", rolename))
                {
                    var row = new TypedDataRow(dataTable.Rows[0]);
                    roleid = row.ToUInt32("id");
                    mapid = row.ToUInt32("id");
                    posx = row.ToSingle("posx");
                    posh = row.ToSingle("posy");
                    posy = row.ToSingle("posz");
                    MakeCreatePacket(packer, true);
                    worldSocket.SendPacket(character);
                    return true;
                }
            }
            else
            {
                MakeCreatePacket(packer, false);
                worldSocket.SendPacket(character);
                return false;
            }
            return false;
        }

        private void MakeEnumPacket(ByteBuffer packer)
        {
            packer.WriteInt32(0);
            packer.WriteInt32(roleIndex, false);
            packer.WriteUInt32(accountid, false);
            packer.WriteInt32(0);
            packer.WriteByte(1);
            packer.WriteUInt32(roleid, false);
            packer.WriteByte(sex);
            packer.WriteByte(faceid);
            packer.WriteByte(hairid);
            packer.WriteByte(earid);
            packer.WriteByte(tailid);
            packer.WriteByte(classid);
            packer.WriteInt32(level, false);
            var name = Encoding.Unicode.GetBytes(rolename);
            packer.WriteByte((byte)name.Length);
            packer.Write(name);
            packer.WriteByte(0);
            packer.WriteByte(status);
            packer.WriteUInt32(UnixTime.ToTimestamp(deleteTime), false);
            packer.WriteUInt32(UnixTime.ToTimestamp(createTime), false);
            packer.WriteUInt32(UnixTime.ToTimestamp(lastLoginTime), false);
            packer.WriteSingle(posx, false);
            packer.WriteSingle(posh, false);
            packer.WriteSingle(posy, false);
            packer.WriteUInt32(mapid, false);
            packer.WriteBoolean(!fashionMode);
            if (fashionMode)
            {
                packer.WriteByte(0x20);
                packer.WriteUInt32(1);
                packer.WriteUInt32(1);
            }
            else
                packer.WriteByte(0x18);
            packer.WriteUInt32(3);
            packer.WriteUInt32(0);
            packer.WriteUInt32(4);
            packer.WriteUInt32(0);
            packer.WriteUInt32(5);
            packer.WriteUInt32(0);
            packer.WriteUInt32(0);
            packer.WriteBoolean(showclassic);
            packer.WriteByte(0);
            packer.WriteByte(powerid);
            packer.WriteUInt32(fashionHead, false);
            for (int i = 0; i < 12; i++)
                packer.WriteByte(0);
            packer.WriteUInt32(fashionCloth, false);
            for (int i = 0; i < 12; i++)
                packer.WriteByte(0);
            packer.WriteUInt32(fashionShoes, false);
            for (int i = 0; i < 12; i++)
                packer.WriteByte(0);
            packer.WriteUInt32(fashionWeapon, false);
            packer.WriteInt32(0);
        }

        private void MakeCreatePacket(ByteBuffer packer, bool ok)
        {
            if (ok)
                packer.WriteInt32(0);
            else
                packer.WriteInt32((int)ServerError.SE_NAMEREPEATED);
            packer.WriteInt32(roleIndex, false);
            packer.WriteUInt32(accountid, false);
            packer.WriteInt32(0);
            packer.WriteByte(1);
            packer.WriteUInt32(roleid, false);
            packer.WriteByte(sex);
            packer.WriteByte(faceid);
            packer.WriteByte(hairid);
            packer.WriteByte(earid);
            packer.WriteByte(tailid);
            packer.WriteByte(classid);
            packer.WriteInt32(level, false);
            var name = Encoding.Unicode.GetBytes(rolename);
            packer.WriteByte((byte)name.Length);
            packer.Write(name);
            packer.WriteByte(0);
            packer.WriteByte(status);
            packer.WriteSingle(posx, false);
            packer.WriteSingle(posh, false);
            packer.WriteSingle(posy, false);
            packer.WriteUInt32(mapid, false);
            packer.WriteBoolean(!fashionMode);
            if (fashionMode)
            {
                packer.WriteByte(0x20);
                packer.WriteUInt32(1);
                packer.WriteUInt32(1);
            }
            else
                packer.WriteByte(0x18);
            packer.WriteUInt32(3);
            packer.WriteUInt32(0);
            packer.WriteUInt32(4);
            packer.WriteUInt32(0);
            packer.WriteUInt32(5);
            packer.WriteUInt32(0);
            packer.WriteUInt32(0);
            packer.WriteBoolean(showclassic);
            packer.WriteByte(0);
            packer.WriteByte(powerid);
            packer.WriteUInt32(fashionHead, false);
            for (int i = 0; i < 12; i++)
                packer.WriteByte(0);
            packer.WriteUInt32(fashionCloth, false);
            for (int i = 0; i < 12; i++)
                packer.WriteByte(0);
            packer.WriteUInt32(fashionShoes, false);
            for (int i = 0; i < 12; i++)
                packer.WriteByte(0);
            packer.WriteUInt32(fashionWeapon, false);
            packer.WriteInt32(0);
        }

        private int roleIndex;
        private uint roleid;
        private uint accountid;
        private bool showclassic;
        private byte sex;
        private byte faceid, earid, hairid, tailid;
        private byte classid;
        private byte level;
        private byte status;
        private string rolename;
        private float posx, posh, posy;
        private uint mapid;
        private bool fashionMode;
        private byte powerid;
        private DateTime createTime, lastLoginTime, deleteTime;
        private uint fashionHead, fashionCloth, fashionShoes, fashionWeapon;
        private WorldSocket worldSocket;

        public Player(uint _accountid, WorldSocket socket)
        {
            accountid = _accountid;
            worldSocket = socket;
        }

        public void SendPacket(WorldPacket packet)
        {
            worldSocket.SendPacket(packet);
        }

        public uint GetID()
        {
            return roleid;
        }
    }
}
