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


        #region -----// Create //-----

        /// <summary>
        /// 建立網站選單點擊資料
        /// </summary>
        /// <param name="Menu_Zone">選單所屬區域1:ProductCenter, 2:PKHome, 3:PKEF, 4:PKReport</param>
        /// <param name="Menu_ID">選單編號</param>
        /// <param name="User_Guid">使用者guid</param>
        /// <returns></returns>
        public bool Create_ClickInfo(Int16 Menu_Zone, Int32 Menu_ID, string User_Guid, out string ErrMsg)
        {
            //----- 宣告 -----
            string sql = "";
            bool doTableCheck = false;
            dbConn db = new dbConn();

            #region -- 每年1/1檢查Table --
            DateTime _today = DateTime.Today;
            int _thisYear = _today.Year;
            if (_today.ToShortDateString().Equals(_thisYear + "/1/1"))
            {
                doTableCheck = true;
            }
            #endregion

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                #region -- Table Check --

                if (doTableCheck)
                {
                    sql = @"
                        IF (EXISTS (SELECT * 
                                    FROM INFORMATION_SCHEMA.TABLES 
                                    WHERE TABLE_NAME = 'MenuClick_#Year#'))
	                        BEGIN
		                        --Do Nothing
		                        SELECT 1
	                        END
                        ELSE

                        BEGIN

                        /* Create Table Start */
                        CREATE TABLE [dbo].[MenuClick_#Year#](
	                        [SeqNo] [bigint] IDENTITY(1,1) NOT NULL,
	                        [Menu_Zone] [smallint] NOT NULL,
	                        [Menu_ID] [int] NOT NULL,
	                        [User_Guid] [varchar](38) NOT NULL,
	                        [ClickTime] [datetime] NULL,
                         CONSTRAINT [PK_MenuClick_#Year#] PRIMARY KEY CLUSTERED 
                        (
	                        [SeqNo] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        --)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY]


                        ALTER TABLE [dbo].[MenuClick_#Year#] ADD  CONSTRAINT [DF_MenuClick_#Year#_ClickTime]  DEFAULT (getdate()) FOR [ClickTime]


                        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1:ProductCenter, 2:PKHome, 3:PKEF, 4:PKReport' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MenuClick_#Year#', @level2type=N'COLUMN',@level2name=N'Menu_Zone'


                        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'選單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MenuClick_#Year#', @level2type=N'COLUMN',@level2name=N'Menu_ID'


                        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'AD帳戶的GUID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MenuClick_#Year#', @level2type=N'COLUMN',@level2name=N'User_Guid'


                        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'點擊時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'MenuClick_#Year#', @level2type=N'COLUMN',@level2name=N'ClickTime'

                        /* Create Table End */

                        END";
                }

                #endregion

                //Insert statment
                sql += @"
                    INSERT INTO MenuClick_#Year# (
                     Menu_Zone, Menu_ID, User_Guid
                    ) VALUES (
                     @Menu_Zone, @Menu_ID, @User_Guid
                    )";
                
                //SQL replace
                sql = sql.Replace("#Year#", _thisYear.ToString());


                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("Menu_Zone", Menu_Zone);
                cmd.Parameters.AddWithValue("Menu_ID", Menu_ID);
                cmd.Parameters.AddWithValue("User_Guid", User_Guid);

                //----- 資料取得 -----
                return db.ExecuteSql(cmd, DBTarget.ClickLog, out ErrMsg);
            }

        }

        #endregion
    }
}
