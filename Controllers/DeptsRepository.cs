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
 * 取得 AD - 部門資料
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// AD帳號
    /// </summary>
    public class DeptsRepository : dbConn
    {

        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<PKDept> GetDepts()
        {
            return GetDepts(null, Common.DeptArea.ALL);
        }

        public IQueryable<PKDept> GetDepts(Dictionary<int, string> search)
        {
            return GetDepts(search, Common.DeptArea.ALL);
        }

        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <param name="area">區域參數</param>
        /// <returns></returns>
        public IQueryable<PKDept> GetDepts(Dictionary<int, string> search, Common.DeptArea area)
        {
            //----- 宣告 -----
            List<PKDept> _dataList = new List<PKDept>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, area))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new PKDept
                    {
                        AreaCode = item.Field<string>("AreaCode"),
                        GroupName = item.Field<string>("GroupName"),
                        DeptID = item.Field<string>("DeptID"),
                        DeptName = item.Field<string>("DeptName"),
                        ERP_DeptID = item.Field<string>("ERP_DeptID"),
                        Display = item.Field<string>("Display"),
                        Sort = item.Field<Int16>("Sort"),
                        AreaSort = item.Field<Int16>("AreaSort"),
                        Email = item.Field<string>("Email"),
                        GP_Rank = item.Field<Int64>("GP_Rank")

                    };

                    //將項目加入至集合
                    _dataList.Add(data);

                }

            }

            //回傳集合
            return _dataList.AsQueryable();

        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">部門代號</param>
        /// <remarks>
        /// 預設值為(queryID)
        /// </remarks>
        public IQueryable<PKDept> GetOne(string queryID)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)Common.DeptSearch.DataID, queryID);

            return GetDepts(search, Common.DeptArea.ALL);
        }


        /// <summary>
        /// 取得主管清單
        /// </summary>
        /// <param name="deptID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<PKDept_Supervisor> Get_Supervisor(string deptID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();
            List<PKDept_Supervisor> _dataList = new List<PKDept_Supervisor>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.UID, Base.DeptID, Base.Account_Name, Prof.Display_Name");
                sql.AppendLine(" FROM User_Dept_Supervisor Base");
                sql.AppendLine("  INNER JOIN User_Profile Prof ON Base.Account_Name = Prof.Account_Name");
                sql.AppendLine(" WHERE (1=1) ");


                //----- SQL Filter -----
                if (!string.IsNullOrWhiteSpace(deptID))
                {
                    sql.Append(" AND (Base.DeptID = @DeptID)");

                    cmd.Parameters.AddWithValue("DeptID", deptID);
                }

                //OrderBy
                sql.Append(" ORDER BY Base.Account_Name");


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
                        var data = new PKDept_Supervisor
                        {
                            UID = item.Field<int>("UID"),
                            DeptID = item.Field<string>("DeptID"),
                            AccountName = item.Field<string>("Account_Name"),
                            DisplayName = item.Field<string>("Display_Name")

                        };

                        //將項目加入至集合
                        _dataList.Add(data);

                    }
                }


                //回傳集合
                return _dataList.AsQueryable();


            }
        }


        #region -- 資料取得 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search, Common.DeptArea area)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  Shipping.SID AreaCode, Shipping.SName GroupName, User_Dept.DeptID, User_Dept.DeptName");
                sql.AppendLine("  , User_Dept.ERP_DeptID, ISNULL(User_Dept.Email, '') AS Email");
                sql.AppendLine("  , User_Dept.Display, User_Dept.Sort, User_Dept.Area_Sort AS AreaSort");
                sql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Shipping.SID ORDER BY Shipping.Sort, User_Dept.Sort ASC) AS GP_Rank");
                sql.AppendLine(" FROM User_Dept WITH(NOLOCK) INNER JOIN Shipping WITH(NOLOCK) ON User_Dept.Area = Shipping.SID");
                sql.AppendLine(" WHERE (Shipping.Display = 'Y') ");


                //----- SQL Filter -----

                //>> 區域別 <<
                switch ((int)area)
                {
                    case 1:
                        sql.Append(" AND (User_Dept.Area = 'TW')");
                        break;

                    case 2:
                        sql.Append(" AND (User_Dept.Area = 'SH')");
                        break;

                    case 3:
                        sql.Append(" AND (User_Dept.Area = 'SZ')");
                        break;
                }


                //>> Search Key <<
                //預設只顯示Display=Y
                if (search == null)
                {
                    sql.Append(" AND (User_Dept.Display = 'Y')");
                }
                else
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.DeptSearch.DataID:
                                sql.Append(" AND (UPPER(User_Dept.DeptID) = UPPER(@DataID))");

                                cmd.Parameters.AddWithValue("DataID", item.Value);

                                break;

                            case (int)Common.DeptSearch.Area:
                                sql.Append(" AND (UPPER(User_Dept.Area) = UPPER(@Area))");

                                cmd.Parameters.AddWithValue("Area", item.Value);

                                break;


                            case (int)Common.DeptSearch.Keyword:
                                sql.Append(" AND (User_Dept.Display = 'Y')");
                                sql.Append(" AND (");
                                sql.Append("    (UPPER(User_Dept.DeptName) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("    OR (UPPER(User_Dept.DeptID) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("    OR (UPPER(User_Dept.DeptID + ' - ' + User_Dept.DeptName) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);

                                break;
                        }
                    }
                }


                //OrderBy
                sql.Append(" ORDER BY Shipping.Sort, User_Dept.Sort");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg);
            }

        }


        #endregion


        #region -- Edit --

        /// <summary>
        /// Insert & Update
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update(PKDept inst, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @AreaSort AS INT");
                sql.AppendLine(" SET @AreaSort = (SELECT Sort FROM Shipping WHERE [SID] = @AreaCode)");
                sql.AppendLine(" IF (SELECT COUNT(*) FROM User_Dept WHERE (Area = @AreaCode) AND (DeptID = @DeptID)) = 0");
                sql.AppendLine(" BEGIN");
                sql.AppendLine("  INSERT INTO User_Dept (");
                sql.AppendLine("   Area, Area_Sort");
                sql.AppendLine("   , DeptID, DeptName, ERP_DeptID, EF_DeptID");
                sql.AppendLine("   , Display, Email");
                sql.AppendLine("  ) VALUES (");
                sql.AppendLine("   @AreaCode, @AreaSort");
                sql.AppendLine("   , @DeptID, @DeptName, @ERP_DeptID, @DeptID");
                sql.AppendLine("   , 'Y', @Email");
                sql.AppendLine("  )");
                sql.AppendLine(" END");
                sql.AppendLine(" ELSE");
                sql.AppendLine(" BEGIN");
                sql.AppendLine("  UPDATE User_Dept");
                sql.AppendLine("  SET Area = @AreaCode, Area_Sort = @AreaSort");
                sql.AppendLine("   , DeptID = @DeptID, DeptName = @DeptName");
                sql.AppendLine("   , ERP_DeptID = @ERP_DeptID, EF_DeptID = @DeptID");
                sql.AppendLine("   , Display = @Display, Email = @Email");
                sql.AppendLine("  WHERE (Area = @AreaCode) AND (DeptID = @DeptID)");
                sql.AppendLine(" END");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", inst.AreaCode);
                cmd.Parameters.AddWithValue("DeptID", inst.DeptID);
                cmd.Parameters.AddWithValue("DeptName", inst.DeptName);
                cmd.Parameters.AddWithValue("ERP_DeptID", inst.ERP_DeptID);
                cmd.Parameters.AddWithValue("Display", inst.Display);
                cmd.Parameters.AddWithValue("Email", inst.Email);

                //execute
                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }


        /// <summary>
        /// 建立部門主管(同一部門可多個)
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_Supervisor(PKDept_Supervisor inst, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT");
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX([UID]), 0) + 1 FROM User_Dept_Supervisor)");

                sql.AppendLine(" IF (SELECT COUNT(*) FROM User_Dept_Supervisor WHERE (DeptID = @DeptID) AND (Account_Name = @Account_Name)) = 0");
                sql.AppendLine(" BEGIN");
                sql.AppendLine("  INSERT INTO User_Dept_Supervisor ([UID], DeptID, Account_Name)");
                sql.AppendLine("  VALUES (@NewID, @DeptID, @Account_Name)");
                sql.AppendLine(" END");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DeptID", inst.DeptID);
                cmd.Parameters.AddWithValue("Account_Name", inst.AccountName);

                //execute
                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除主管名單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Delete_Supervisor(int id, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM User_Dept_Supervisor WHERE [UID] = @id");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("id", id);

                //execute
                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除部門
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Delete(int id, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM User_Dept WHERE DeptID = @id");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("id", id);

                //execute
                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }

        #endregion
    }
}
