using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    enum SocketOpcode : int
    {
        SMSG_GAMEMSG_LIST = 0x00,

        SMSG_ACCEPT_CONNECT = 0x01,

        SCMSG_AUTH_SESSION = 0x02,

        CMSG_LOGIN = 0x03,

        SMSG_ACOUNT = 0x04,

        SMSG_ERROR = 0x05,

        SCMSG_GAMEMSG = 0x22,

        SMSG_LINE_INFO = 0x25,

        CMSG_RECONNECT = 0x32,

        SMSG_LOGOUT = 0x45,

        CMSG_SELECTROLE = 0x46,

        SMSG_SELECTROLE = 0x47,

        CMSG_ENTERWORLD = 0x48,

        CMSG_SAY = 0x4F,

        SMSG_SAY = 0x50,

        CMSG_ROLELIST = 0x52,

        SMSG_ROLELIST = 0x53,

        CMSG_CREATEROLE = 0x54,

        SMSG_CREATEROLE = 0x55,

        CMSG_DELETEROLE = 0x56,

        SMSG_DELETEROLE = 0x57,

        CMSG_REGAINROLE = 0x58,

        SMSG_REGAINROLE = 0x59,

        SCMSG_PING = 0x5A,

        CMSG_MESSAGE = 0x60,

        CMSG_QUICKBARSET = 0x66,

        SMSG_QUICKBARSET = 0x67,

        CMSG_QUICKBARGET = 0x68,

        SMSG_QUICKBARGET = 0x69,

        CMSG_80 = 0x80,

        SMSG_81 = 0x81,

        CMSG_HOME_FACATION_SAY = 0x92C3
    };

    enum ServerError
    {
        SE_TOBECONTINUE = 1,

        SE_USERNOTEXIST = 2,

        SE_WRONGUSERORPWD = 3,

        SE_TIMEOUT = 4,

        SE_ARGUEMENT = 5,

        SE_FRIENDSAVE = 6,

        SE_UNSUPPORTED = 7,

        SE_SERVERERROR = 8,

        SE_LOGINANDLOCKED = 9,

        SE_LOGINED = 10,

        SE_ERRORNONCE = 11,

        SE_GETROLE = 13,

        SE_LOGINGAMESERVER = 14,

        SE_SERVERFULLED = 15,

        SE_ROLEINGAME = 16,

        SE_LINEFULLED = 17,

        SE_NOLINENUM = 18,

        SE_NILINE = 19,

        SE_TRANSERROR = 21,

        SE_TRANSTIMEOUT = 22,

        SE_MONEYLESS = 23,

        SE_FORBIDDEN_ACCOUNT = 24,

        SE_BADNAME = 25,

        SE_FORBIDDEN_IP = 26,

        SE_PWDPROCTED = 27,

        SE_PROCTED = 28,

        SE_OUTTIME = 29,

        SE_LOGINFAIL = 31,

        SE_KICKOUT = 32,

        SE_CREATEFAIL = 33,

        SE_DELETEFAIL = 34,

        SE_GETLISTFAIL = 35,

        SE_REBUILDFAIL = 36,

        SE_SERVER = 37,

        SE_ROLEFORBIDDEN = 38,

        SE_SERVERFULL = 39,

        SE_SERVERLEAVE = 40,

        SE_NAMEREPEATED = 45,

        SE_ROLEFULL = 46,
    }
}
