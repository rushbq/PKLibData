using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Data.Assets;
using PKLib_Data.Models;
using PKLib_Method.Methods;


/*
 * 取得PKSYS資料
 * 公司別 / 資料庫別 / 
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// 參數查詢
    /// </summary>
    public class ParamsRepository : dbConn
    {
        #region -----// Read //-----

        /// <summary>
        /// 取得資料 - 公司別
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Corp> GetCorpList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Corp> dataList = new List<Corp>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Corp_UID, Corp_ID, Corp_Name, DB_Name");
                sql.AppendLine(" FROM Param_Corp WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Display = 'Y')");

                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.mySearch.DataID:
                                //公司別
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Corp_UID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.Keyword:
                                //關鍵字
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(Corp_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Corp_ID) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                        }
                    }
                }

                sql.AppendLine(" ORDER BY Sort");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Corp
                        {
                            Corp_UID = item.Field<int>("Corp_UID"),
                            Corp_ID = item.Field<string>("Corp_ID"),
                            Corp_Name = item.Field<string>("Corp_Name"),
                            DB_Name = item.Field<string>("DB_Name")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        /// <summary>
        /// 取得資料 - 資料庫別
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<DBs> GetDBList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<DBs> dataList = new List<DBs>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT UID, DB_Name, DB_Desc");
                sql.AppendLine(" FROM Home_DB WITH(NOLOCK)");
                sql.AppendLine(" WHERE (1=1)");

                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.mySearch.DataID:

                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (UID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.Keyword:
                                //關鍵字
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(DB_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(DB_Desc) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                        }
                    }
                }

                sql.AppendLine(" ORDER BY Sort");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new DBs
                        {
                            UID = item.Field<int>("UID"),
                            DB_Name = item.Field<string>("DB_Name"),
                            DB_Desc = item.Field<string>("DB_Desc")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        #endregion


    }
}
