using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKLib_Data.Assets
{
    public class dbConn
    {
        public string ErrMsg;

        public enum DBTarget : int
        {
            myLocal = 1,
            PKSYS = 2,
            Product = 3,
            ClickLog = 4
        }

        /// <summary>
        /// 連線字串
        /// 有使用此功能的網站, 也要在webConfig加上對應的連線字串, 才能正常執行
        /// </summary>
        /// <param name="target">資料庫別</param>
        /// <returns></returns>
        private string ConnString(DBTarget target)
        {
            switch ((int)target)
            {
                case 2:
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon_PKSYS"];

                case 3:
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon_Product"];

                case 4:
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon_ClickLog"];
                    
                default:
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["dbCon"];
            }
        }

    
        /// <summary>
        /// (Override) 取得資料列表 (DB=Local)
        /// </summary>
        /// <returns>DataTable</returns>
        /// <remarks>
        /// 若未填寫資料連線來源，預設為 Local 資料庫
        /// </remarks>
        public DataTable LookupDT(SqlCommand cmd, out string errMsg)
        {
            return LookupDT(cmd, DBTarget.myLocal, out errMsg);
        }

        /// <summary>
        /// 取得資料列表
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="target">資料連線來源</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <returns>DataTable</returns>
        public DataTable LookupDT(SqlCommand cmd, DBTarget target, out string errMsg)
        {
            using (SqlConnection connSql = new SqlConnection(ConnString(target)))
            {
                try
                {
                    connSql.Open();
                    cmd.Connection = connSql;

                    //建立DataAdapter
                    SqlDataAdapter dataAdapterSql = new SqlDataAdapter();
                    dataAdapterSql.SelectCommand = cmd;

                    //取得DataTable
                    DataTable DTSql = new DataTable();
                    dataAdapterSql.Fill(DTSql);

                    errMsg = "";

                    return DTSql;

                }
                catch (Exception ex)
                {
                    errMsg = ex.Message.ToString();
                    return null;

                }
            }
        }


        /// <summary>
        /// (Override) 執行SQL (DB=Local)
        /// </summary>
        /// <returns>bool</returns>
        /// <remarks>
        /// 若未填寫資料連線來源，預設為 Local 資料庫
        /// </remarks>
        public bool ExecuteSql(SqlCommand cmd, out string errMsg)
        {
            return ExecuteSql(cmd, DBTarget.myLocal, out errMsg);
        }

        /// <summary>
        /// 執行SQL
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="target">資料連線來源</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public bool ExecuteSql(SqlCommand cmd, DBTarget target, out string errMsg)
        {
            using (SqlConnection connSql = new SqlConnection(ConnString(target)))
            {
                connSql.Open();

                SqlTransaction transActSql;
                transActSql = connSql.BeginTransaction();

                cmd.Connection = connSql;
                cmd.Transaction = transActSql;

                try
                {
                    cmd.ExecuteNonQuery();
                    transActSql.Commit();

                    errMsg = "";

                    return true;
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message.ToString();

                    try
                    {
                        transActSql.Rollback();

                        return false;
                    }
                    catch (Exception sysex)
                    {
                        errMsg = sysex.Message.ToString();
                        return false;
                    }
                }

            }
        }



    }
}
