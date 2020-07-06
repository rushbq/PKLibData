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
 * 取得 ERP 資料 - 客戶
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// 取得ERP客戶資料
    /// </summary>
    public class CustomersRepository : dbConn
    {
        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<Customers> GetCustomers()
        {
            return GetCustomers(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Customers> GetCustomers(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Customers> _DataList = new List<Customers>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new Customers
                    {
                        CustID = item.Field<string>("CustID"),
                        CustName = item.Field<string>("CustName"),
                        SalesID = item.Field<string>("SalesID"),
                        Corp_ID = item.Field<string>("Corp_ID"),
                        Corp_Name = item.Field<string>("Corp_Name"),
                        ContactWho = item.Field<string>("ContactWho"),
                        ContactAddr = item.Field<string>("ContactAddr"),
                        Tel = item.Field<string>("Tel")
                    };

                    //將項目加入至集合
                    _DataList.Add(data);

                }

            }

            //回傳集合
            return _DataList.AsQueryable();

        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <remarks>
        /// 預設值為(queryID)
        /// </remarks>
        public IQueryable<Customers> GetOne(string queryID)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)Common.CustSearch.DataID, queryID);

            return GetCustomers(search);
        }


        #region -- 資料取得 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  RTRIM(Base.MA001) CustID, RTRIM(Base.MA002) CustName, RTRIM(Base.MA016) SalesID");
                sql.AppendLine("  , RTRIM(Base.MA005) AS ContactWho, RTRIM(Base.MA027) AS ContactAddr, RTRIM(Base.MA006) AS Tel");
                sql.AppendLine("  , Corp.Corp_ID, Corp.Corp_Name");
                sql.AppendLine(" FROM Customer Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Param_Corp Corp WITH (NOLOCK) ON Base.DBC = Corp.Corp_ID");
                sql.AppendLine(" WHERE (Base.DBS = Base.DBC) ");


                //----- SQL Filter -----
                //>> Search Key <<
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.CustSearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (UPPER(RTRIM(Base.MA001)) = UPPER(@DataID))");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)Common.CustSearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Base.MA001)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.MA002) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.MA003) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                            case (int)Common.CustSearch.Block:
                                /*
                                  [開票系統用] - 限制條件
                                    MA110=戶名
                                    MA071=稅號
                                */
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (MA071 <> '' AND MA071 IS NOT NULL) AND (MA110 <> '' AND MA110 IS NOT NULL)");

                                }

                                break;


                            case (int)Common.CustSearch.Corp:
                                /*
                                    [指定公司別]
                                    1=prokit2
                                    2=ProUnion
                                    3=SHPK2
                                */
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Corp.Corp_UID = @Corp_UID)");

                                    cmd.Parameters.AddWithValue("Corp_UID", item.Value);
                                }

                                break;

                        }
                    }
                }


                //----- SQL OrderBy -----
                sql.AppendLine("ORDER BY Base.MA001");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, dbConn.DBTarget.PKSYS, out ErrMsg);
            }

        }


        #endregion

    }
}
