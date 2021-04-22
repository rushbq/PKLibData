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
 * 取得 AD - 使用者資料
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// AD帳號
    /// </summary>
    public class UsersRepository : dbConn
    {
        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<PKUsers> GetUsers()
        {
            return GetUsers(null, null, Common.DeptArea.ALL);
        }

        public IQueryable<PKUsers> GetUsers(Dictionary<int, string> search, Dictionary<int, string> deptID)
        {
            return GetUsers(search, deptID, Common.DeptArea.ALL);
        }

        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <param name="deptID">部門參數</param>
        /// <param name="area">區域參數</param>
        /// <returns></returns>
        /// <example>
        /// search.Add((int)Common.UserSearch.Keyword, searchVal);
        /// depts.Add(1, Req_DataID);
        /// </example>
        public IQueryable<PKUsers> GetUsers(Dictionary<int, string> search, Dictionary<int, string> deptID, Common.DeptArea area)
        {
            //----- 宣告 -----
            List<PKUsers> _Users = new List<PKUsers>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, deptID, area))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new PKUsers
                    {
                        ProfGuid = item.Field<string>("ProfGuid"),
                        ProfID = item.Field<string>("ProfID"),
                        ProfName = item.Field<string>("ProfName"),
                        DeptID = item.Field<string>("DeptID"),
                        DeptName = item.Field<string>("DeptName"),
                        Email = item.Field<string>("Email"),
                        NickName = item.Field<string>("NickName"),
                        Tel_Ext = item.Field<string>("Tel_Ext"),
                        GP_Rank = item.Field<Int64>("GP_Rank")
                    };

                    //將項目加入至集合
                    _Users.Add(data);

                }

            }

            //回傳集合
            return _Users.AsQueryable();

        }
        

        /// <summary>
        /// 取得部門主管Info
        /// </summary>
        /// <param name="deptID">部門ID</param>
        /// <returns></returns>
        public IQueryable<PKUsers> GetDeptSupervisor(string deptID)
        {
            //----- 宣告 -----
            dbConn db = new dbConn();
            List<PKUsers> _Users = new List<PKUsers>();
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"SELECT
                    Prof.Account_Name ProfID, Prof.Display_Name ProfName, Prof.Guid ProfGuid
                    , Prof.Email, Prof.NickName, Prof.Tel_Ext
                    FROM User_Dept_Supervisor Info WITH (NOLOCK)
                    INNER JOIN User_Profile Prof WITH (NOLOCK) ON Info.Account_Name = Prof.Account_Name
                    WHERE (Info.DeptID = @DeptID) AND (Prof.Display = 'Y')
                ";

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DeptID", deptID);

                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new PKUsers
                        {
                            ProfGuid = item.Field<string>("ProfGuid"),
                            ProfID = item.Field<string>("ProfID"),
                            ProfName = item.Field<string>("ProfName"),
                            Email = item.Field<string>("Email"),
                            NickName = item.Field<string>("NickName"),
                            Tel_Ext = item.Field<string>("Tel_Ext")
                        };

                        //將項目加入至集合
                        _Users.Add(data);
                    }
                }


                //回傳集合
                return _Users.AsQueryable();
            }
        }

        /// <summary>
        /// 取得人員組織 - 樹狀選單(for jQuery zTree)
        /// </summary>
        /// <param name="area">TW/SH/SZ/ALL</param>
        /// <returns></returns>
        public IQueryable<UserTree> GetUserTree(Common.DeptArea area)
        {
            //----- 宣告 -----
            List<UserTree> _Users = new List<UserTree>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.Append("SELECT Tbl.* FROM ");
                sql.Append("( ");
                sql.Append("    SELECT [SID] AS MenuID, '0' AS ParentID, '【' + SName + '】' AS MenuName, CAST(Sort AS NVARCHAR) AS Sort, 'Y' AS IsOpen, 'Y' AS [chkDisabled]");
                sql.Append("    FROM Shipping WITH (NOLOCK) ");
                sql.Append("    WHERE (Display = 'Y') ");

                //>> 區域別 <<
                switch ((int)area)
                {
                    case 1:
                        sql.Append(" AND ([SID] = 'TW')");
                        break;

                    case 2:
                        sql.Append(" AND ([SID] = 'SH')");
                        break;

                    case 3:
                        sql.Append(" AND ([SID] = 'SZ')");
                        break;
                }

                sql.Append("    UNION ALL ");
                sql.Append("    SELECT CAST(Dept.DeptID AS NVARCHAR) AS MenuID, Dept.Area AS ParentID, Dept.DeptName AS MenuName, CAST(100 + Dept.Sort AS NVARCHAR) AS Sort, 'N' AS IsOpen, 'N' AS [chkDisabled]");
                sql.Append("    FROM User_Dept Dept WITH (NOLOCK) ");
                sql.Append("    WHERE (Dept.Display = 'Y') ");

                //>> 區域別 <<
                switch ((int)area)
                {
                    case 1:
                        sql.Append(" AND (Dept.Area = 'TW')");
                        break;

                    case 2:
                        sql.Append(" AND (Dept.Area = 'SH')");
                        break;

                    case 3:
                        sql.Append(" AND (Dept.Area = 'SZ')");
                        break;
                }

                sql.Append("    UNION ALL ");

                //使用v_+ 工號, 用來判斷此為要取用的值, 並在寫入時replace 'v_'為空白
                sql.Append("    SELECT 'v_' + Prof.Guid AS MenuID, Prof.DeptID AS ParentID, Prof.Display_Name AS MenuName, Prof.Account_Name AS Sort, 'N' AS IsOpen, 'N' AS [chkDisabled]");
                sql.Append("    FROM User_Profile Prof WITH (NOLOCK) ");
                sql.Append("        INNER JOIN User_Dept Dept WITH (NOLOCK) ON Prof.DeptID = Dept.DeptID ");
                sql.Append("    WHERE (Dept.Display = 'Y') AND (Prof.Display = 'Y')");

                //>> 區域別 <<
                switch ((int)area)
                {
                    case 1:
                        sql.Append(" AND (Dept.Area = 'TW')");
                        break;

                    case 2:
                        sql.Append(" AND (Dept.Area = 'SH')");
                        break;

                    case 3:
                        sql.Append(" AND (Dept.Area = 'SZ')");
                        break;
                }

                sql.Append(") AS Tbl ");
                sql.Append("ORDER BY Tbl.Sort ");


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
                        var data = new UserTree
                        {
                            MenuID = item.Field<string>("MenuID"),
                            ParentID = item.Field<string>("ParentID").ToString(),
                            MenuName = item.Field<string>("MenuName"),
                            IsOpen = item.Field<string>("IsOpen").Equals("Y"),
                            chkDisabled = item.Field<string>("chkDisabled").Equals("Y")

                        };

                        //將項目加入至集合
                        _Users.Add(data);

                    }

                }

            }

            //回傳集合
            return _Users.AsQueryable();
        }


        /// <summary>
        /// 取得指定資料
        /// </summary>
        /// <param name="queryID">資料編號(工號)</param>
        /// <remarks>
        /// 預設值為(queryID)
        /// </remarks>
        public IQueryable<PKUsers> GetOne(string queryID)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)Common.UserSearch.DataID, queryID);

            return GetUsers(search, null, Common.DeptArea.ALL);
        }


        #region -- 資料取得 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search, Dictionary<int, string> deptID, Common.DeptArea area)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  Prof.Account_Name ProfID, Prof.Display_Name ProfName, Prof.Guid ProfGuid");
                sql.AppendLine("  , Prof.Email, Prof.NickName, Prof.Tel_Ext");
                sql.AppendLine("  , Dept.DeptID DeptID, Dept.DeptName DeptName");
                sql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Dept.DeptID ORDER BY Prof.Display DESC, Dept.Sort, Prof.Account_Name ASC) AS GP_Rank");
                sql.AppendLine(" FROM User_Dept Dept WITH(NOLOCK) INNER JOIN User_Profile Prof WITH(NOLOCK) ON Dept.DeptID = Prof.DeptID");
                sql.AppendLine(" WHERE (Dept.Display = 'Y') ");


                //----- SQL Filter -----

                //>> 區域別 <<
                switch ((int)area)
                {
                    case 1:
                        sql.Append(" AND (Dept.Area = 'TW')");
                        break;

                    case 2:
                        sql.Append(" AND (Dept.Area = 'SH')");
                        break;

                    case 3:
                        sql.Append(" AND (Dept.Area = 'SZ')");
                        break;
                }


                //>> 部門別 <<
                if (deptID != null)
                {
                    if (deptID.Count > 0)
                    {
                        sql.Append(" AND (Dept.DeptID IN ({0}))".FormatThis(Get_ParamNames(deptID.Count, "DeptID")));

                        int deptRow = 0;
                        foreach (var item in deptID)
                        {
                            cmd.Parameters.AddWithValue("DeptID{0}".FormatThis(deptRow), item.Value);

                            deptRow++;
                        }
                    }
                }


                //>> Search Key <<
                //預設只顯示Display=Y
                if (search == null)
                {
                    sql.Append(" AND (Prof.Display = 'Y')");
                }
                else
                {
                    if (search.Count == 0)
                    {
                        sql.Append(" AND (Prof.Display = 'Y')");
                    }
                    else
                    {
                        foreach (var item in search)
                        {
                            switch (item.Key)
                            {
                                case (int)Common.UserSearch.DataID:
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        sql.Append(" AND (UPPER(Prof.Account_Name) = UPPER(@DataID))");

                                        cmd.Parameters.AddWithValue("DataID", item.Value);
                                    }

                                    break;

                                case (int)Common.UserSearch.Guid:
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        sql.Append(" AND (UPPER(Prof.Guid) = UPPER(@myGuid))");

                                        cmd.Parameters.AddWithValue("myGuid", item.Value);
                                    }

                                    break;

                                case (int)Common.UserSearch.Keyword:
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        sql.Append(" AND (Prof.Display = 'Y')");
                                        sql.Append(" AND (");
                                        sql.Append("    (UPPER(Prof.Display_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                        sql.Append("    OR (UPPER(Prof.Account_Name) LIKE '%' + UPPER(@Keyword) + '%')");
                                        sql.Append("    OR (UPPER(Prof.NickName) LIKE '%' + UPPER(@Keyword) + '%')");
                                        sql.Append(" )");

                                        cmd.Parameters.AddWithValue("Keyword", item.Value);
                                    }

                                    break;
                            }
                        }
                    }

                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg);
            }

        }


        string Get_ParamNames(int rowCnt, string paramName)
        {
            ArrayList ary = new ArrayList();

            for (int row = 0; row < rowCnt; row++)
            {
                ary.Add("@{0}{1}".FormatThis(paramName, row));
            }

            return string.Join(",", ary.ToArray());
        }
        #endregion

    }
}
