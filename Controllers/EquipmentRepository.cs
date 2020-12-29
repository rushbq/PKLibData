using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Data.Assets;
using PKLib_Data.Models;
using PKLib_Method.Methods;


/*
  PKEF - ERP 資產設備資料
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// ERP資產設備資料
    /// </summary>
    public class EquipmentRepository : dbConn
    {
        #region -----// Read //-----

        #region >> TW <<
        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<Equipment> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Equipment> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Equipment> dataList = new List<Equipment>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new Equipment
                    {
                        Who = item.Field<string>("Who"),
                        ID = item.Field<string>("ID"),
                        Name = item.Field<string>("Name"),
                        Spec = item.Field<string>("Spec"),
                        GetItemDate = item.Field<string>("GetItemDate").ToDateString_ERP("-"),
                        GetItemMoney = item.Field<Decimal>("GetItemMoney")
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        #endregion


        #region >> SH <<
        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<Equipment> GetDataList_SH()
        {
            return GetDataList_SH(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Equipment> GetDataList_SH(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Equipment> dataList = new List<Equipment>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData_SH(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new Equipment
                    {
                        Who = item.Field<string>("Who"),
                        ID = item.Field<string>("ID"),
                        Name = item.Field<string>("Name"),
                        Spec = item.Field<string>("Spec"),
                        GetItemDate = item.Field<string>("GetItemDate").ToDateString_ERP("-"),
                        GetItemMoney = item.Field<Decimal>("GetItemMoney")
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        #endregion

        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢</param>
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
                sql.AppendLine("SELECT ISNULL(Prof.Display_Name, '-') AS Who");
                sql.AppendLine(", RTRIM(Base.MB001) AS ID");
                sql.AppendLine(", RTRIM(Base.MB002) AS Name");
                sql.AppendLine(", RTRIM(Base.MB003) AS Spec");
                sql.AppendLine(", Base.MB016 AS GetItemDate");
                sql.AppendLine(", Base.MB019 AS GetItemMoney");
                sql.AppendLine(" FROM [prokit2].dbo.ASTMB Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.ASTMC DT WITH(NOLOCK) ON Base.MB001 = DT.MC001");
                sql.AppendLine("  LEFT JOIN User_Profile Prof WITH(NOLOCK) ON DT.MC003 = Prof.ERP_UserID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine(" WHERE (Base.MB039 = 'Y') AND (MB017 = '')");
                sql.AppendLine(" AND (");
                sql.AppendLine("  (UPPER(LEFT(Base.MB001, 4)) IN ('0E-A','0E-M','0E-S','1C10'))");
                sql.AppendLine("  OR (UPPER(LEFT(Base.MB001, 4)) = '0E-F' AND UPPER(Base.MB057) = 'MIS')");
                sql.AppendLine(" )");



                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("  (UPPER(Prof.Display_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Prof.Account_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.MB001) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.MB002) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.MB003) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.AppendLine(" )");


                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.DeptID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Prof.DeptID = @DeptID)");


                                    cmd.Parameters.AddWithValue("DeptID", item.Value);
                                }

                                break;


                            case (int)Common.mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //傳入格式為yyyyMMdd
                                    sql.Append(" AND (Base.MB016 >= @sDate)");


                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //傳入格式為yyyyMMdd
                                    sql.Append(" AND (Base.MB016 <= @eDate)");


                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;
                        }
                    }
                }


                sql.AppendLine(" ORDER BY Who, ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }


        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData_SH(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT ISNULL(Prof.Display_Name, '-') AS Who");
                sql.AppendLine(", RTRIM(Base.MB001) AS ID");
                sql.AppendLine(", RTRIM(Base.MB002) AS Name");
                sql.AppendLine(", RTRIM(Base.MB003) AS Spec");
                sql.AppendLine(", Base.MB016 AS GetItemDate");
                sql.AppendLine(", Base.MB019 AS GetItemMoney");
                sql.AppendLine(" FROM [SHPK2].dbo.ASTMB Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [SHPK2].dbo.ASTMC DT WITH(NOLOCK) ON Base.MB001 = DT.MC001");
                sql.AppendLine("  LEFT JOIN User_Profile Prof WITH(NOLOCK) ON DT.MC003 = Prof.ERP_UserID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine(" WHERE (Base.MB039 = 'Y')");
                sql.AppendLine(" AND (");
                sql.AppendLine("  (UPPER(LEFT(Base.MB001, 5)) IN ('0SE-A','0SE-M','0SE-P','0SE-C','0SE-D','0SE-F'))");
                sql.AppendLine("  OR (UPPER(LEFT(Base.MB001, 3)) IN ('SI1','SI2'))");
                sql.AppendLine(" )");



                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("  (UPPER(Prof.Display_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Prof.Account_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.MB001) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.MB002) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.MB003) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.AppendLine(" )");


                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.DeptID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Prof.DeptID = @DeptID)");


                                    cmd.Parameters.AddWithValue("DeptID", item.Value);
                                }

                                break;


                            case (int)Common.mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //傳入格式為yyyyMMdd
                                    sql.Append(" AND (Base.MB016 >= @sDate)");


                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //傳入格式為yyyyMMdd
                                    sql.Append(" AND (Base.MB016 <= @eDate)");


                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;
                        }
                    }
                }


                sql.AppendLine(" ORDER BY Who, ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }

        #endregion

    }
}
