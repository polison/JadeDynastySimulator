using MySql.Data.MySqlClient;
using NetWorkLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    class DataBaseManager : Singleton<DataBaseManager>, IDisposable
    {
        private MySqlConnection connection;
        private ILog logger;
        private string sqlHost = "localhost";
        private int sqlPort = 3306;
        private string sqlUser = "root";
        private string sqlPass = "123456";
        private string sqlDbname = "element_world";
        private bool disposedValue = false;

        public string DBHost
        {
            set => sqlHost = value;
        }

        public int DBPort
        {
            set => sqlPort = value;
        }

        public string DBUser
        {
            set => sqlUser = value;
        }
        public string DBPass
        {
            set => sqlPass = value;
        }
        public string DBName
        {
            set => sqlDbname = value;
        }

        public bool Initialize(ILog log)
        {
            logger = log;
            try
            {
                if (sqlHost.Length < 1)
                {
                    logger.Error("数据库--服务器不能为空！");
                    return false;
                }

                if (sqlUser.Length < 1)
                {
                    logger.Error("数据库--用户名不能为空！");
                    return false;
                }

                if (sqlPass.Length < 1)
                {
                    logger.Error("数据库--密码不能为空！");
                    return false;
                }

                if (sqlDbname.Length < 1)
                {
                    logger.Error("数据库--数据库名不能为空！");
                    return false;
                }

                connection = new MySqlConnection();
                connection.ConnectionString = $"server={sqlHost};port={sqlPort};user={sqlUser};password={sqlPass};database={sqlDbname};sslmode=Preferred;";
                connection.Open();

                logger.Message("数据库{0}@{1}已成功连接！", sqlUser, sqlHost);
                PExecute("SET NAMES 'utf8';");
                return true;
            }
            catch (Exception e)
            {
                logger.Error("数据库{0}@{1}连接失败！{2}", sqlUser, sqlHost, e.Message);
                return false;
            }
        }

        public bool PExecute(string sqlQueryFormat, params object[] args)
        {
            if (connection.State != ConnectionState.Open)
                return false;

            lock (connection)
            {
                var queryStr = string.Format(sqlQueryFormat, args);
                try
                {
                    var cmd = new MySqlCommand(queryStr, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    logger.Error("数据库{0}@{1}执行{2}失败！{3}", sqlUser, sqlHost, queryStr, e.Message);
                    return false;
                }
            }
            return true;
        }

        public bool PQuery(DataTable result, string sqlQueryFormat, params object[] args)
        {
            if (connection.State != ConnectionState.Open)
                return false;

            lock (connection)
            {
                var queryStr = string.Format(sqlQueryFormat, args);
                try
                {
                    var cmd = new MySqlCommand(queryStr, connection);
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        result.Columns.Clear();
                        result.Clear();
                        adapter.Fill(result);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("数据库{0}@{1}执行{2}失败！{3}", sqlUser, sqlHost, queryStr, e.Message);
                    return false;
                }
            }
            return true;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Close();
                    connection.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
