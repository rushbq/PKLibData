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
  PKEF - 供應商基本資料
  PKEF - 供應商關聯設定
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// 通訊錄參數
    /// </summary>
    /// <remarks>
    /// </remarks>
    public enum myMember : int
    {
        DataID = 1,
        Keyword = 2,
        SupID = 3,
        Corp = 4
    }

    
    public class SupplierRepository : dbConn
    {
        #region -----// Read //-----

        /// <summary>
        /// [供應商關聯設定] 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<Supplier> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// [供應商關聯設定] 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Supplier> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Supplier> dataList = new List<Supplier>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new Supplier
                    {
                        Sup_UID = item.Field<int>("Sup_UID"),
                        Sup_Name = item.Field<string>("Sup_Name"),
                        DataCnt = item.Field<int>("DataCnt"),
                        twCnt = item.Field<int>("twCnt"),
                        szCnt = item.Field<int>("szCnt"),
                        shCnt = item.Field<int>("shCnt"),
                        Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Create_Who = item.Field<string>("Create_Who"),
                        Update_Who = item.Field<string>("Update_Who"),
                        Create_Name = item.Field<string>("Create_Name"),
                        Update_Name = item.Field<string>("Update_Name")

                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [供應商關聯設定] 取得關聯列表
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<Rel_Data> GetRelList(string dataID)
        {
            //----- 宣告 -----
            List<Rel_Data> dataList = new List<Rel_Data>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Data.Data_UID");
                sql.AppendLine("  , Corp.Corp_UID, Corp.Corp_Name");
                sql.AppendLine("  , RTRIM(ERP.MA001) ERP_SupID, RTRIM(ERP.MA002) ERP_SupName");
                sql.AppendLine(" FROM Supplier_Data Data WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Param_Corp Corp WITH(NOLOCK) ON Data.Corp_UID = Corp.Corp_UID");
                sql.AppendLine("  INNER JOIN Supplier_ERPData ERP WITH(NOLOCK) ON Data.ERP_ID = RTRIM(ERP.MA001) AND Corp.Corp_ID = RTRIM(ERP.COMPANY)");
                sql.AppendLine(" WHERE (Data.Sup_UID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Rel_Data
                        {
                            Data_UID = item.Field<int>("Data_UID"),
                            Corp_UID = item.Field<int>("Corp_UID"),
                            Corp_Name = item.Field<string>("Corp_Name"),
                            ERP_SupID = item.Field<string>("ERP_SupID"),
                            ERP_SupName = item.Field<string>("ERP_SupName")
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
        /// [供應商關聯設定] 取得未關聯列表
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<UnRel_Data> GetUnRelList(string dataID, Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<UnRel_Data> dataList = new List<UnRel_Data>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  Corp.Corp_UID, Corp.Corp_Name");
                sql.AppendLine("  , RTRIM(ERP.MA001) ERP_SupID, RTRIM(ERP.MA002) ERP_SupName");
                sql.AppendLine(" FROM Param_Corp Corp WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Supplier_ERPData ERP WITH(NOLOCK) ON Corp.Corp_ID = RTRIM(ERP.COMPANY)");
                sql.AppendLine(" WHERE (Corp.Display = 'Y')");
                sql.AppendLine("  AND NOT EXISTS(");
                sql.AppendLine("    SELECT Data.Data_UID");
                sql.AppendLine("    FROM Supplier_Data Data WITH(NOLOCK)");
                sql.AppendLine("    WHERE (Data.Corp_UID = Corp.Corp_UID) AND (Data.ERP_ID = RTRIM(ERP.MA001))");
                sql.AppendLine("     AND (Data.Sup_UID = @DataID)");
                sql.AppendLine("  )");


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
                                    sql.Append(" AND (Corp.Corp_UID = @Corp_UID)");

                                    cmd.Parameters.AddWithValue("Corp_UID", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.Keyword:
                                //關鍵字
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(ERP.MA001)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(ERP.MA002)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                        }
                    }
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new UnRel_Data
                        {
                            Corp_UID = item.Field<int>("Corp_UID"),
                            Corp_Name = item.Field<string>("Corp_Name"),
                            ERP_SupID = item.Field<string>("ERP_SupID"),
                            ERP_SupName = item.Field<string>("ERP_SupName")
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
        /// 取得ERP供應商資料
        /// </summary>
        /// <param name="search">dataID/Keyword/Corp</param>
        /// <returns></returns>
        public IQueryable<ERP_Data> GetERPDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ERP_Data> dataList = new List<ERP_Data>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  Corp.Corp_UID, Corp.Corp_Name");
                sql.AppendLine("  , RTRIM(ERP.MA001) ERP_SupID, RTRIM(ERP.MA002) ERP_SupName");
                sql.AppendLine("  , ISNULL(Info.Data_ID, 0) AS InfoID, ISNULL(Prof.Account_Name, '') AS User_Account, ISNULL(Prof.Display_Name, '') AS User_Name");
                sql.AppendLine(" FROM Param_Corp Corp WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Supplier_ERPData ERP WITH(NOLOCK) ON Corp.Corp_ID = RTRIM(ERP.COMPANY)");
                sql.AppendLine("  LEFT JOIN Supplier_ExtendedInfo Info WITH(NOLOCK) ON RTRIM(ERP.MA001) = Info.ERP_ID AND Corp.Corp_UID = Info.Corp_UID");
                sql.AppendLine("  LEFT JOIN User_Profile Prof WITH(NOLOCK) ON Info.Purchaser = Prof.Account_Name");
                sql.AppendLine(" WHERE (Corp.Display = 'Y')");


                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)Common.mySearch.DataID:
                                //編號
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (ERP.MA001 = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.Keyword:
                                //關鍵字
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(ERP.MA001)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(ERP.MA002)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                            case (int)Common.mySearch.Corp:
                                //公司別
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Corp.Corp_UID = @Corp_UID)");

                                    cmd.Parameters.AddWithValue("Corp_UID", item.Value);
                                }

                                break;

                        }
                    }
                }

                sql.Append(" ORDER BY RTRIM(ERP.MA001)");

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
                        var data = new ERP_Data
                        {
                            Corp_UID = item.Field<int>("Corp_UID"),
                            Corp_Name = item.Field<string>("Corp_Name"),
                            ERP_SupID = item.Field<string>("ERP_SupID"),
                            ERP_SupName = item.Field<string>("ERP_SupName"),
                            InfoID = item.Field<int>("InfoID"),
                            User_Account = item.Field<string>("User_Account"),
                            User_Name = item.Field<string>("User_Name")
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
        /// 取得供應商通訊錄
        /// </summary>
        /// <param name="search">dataID</param>
        /// <returns></returns>
        public IQueryable<Member_Data> GetMemberList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Member_Data> dataList = new List<Member_Data>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  Corp.Corp_UID, Corp.Corp_Name");
                sql.AppendLine("  , Base.Data_ID, Base.ERP_ID");
                sql.AppendLine("  , Base.Email, Base.FullName, Base.NickName, Base.Gender, Base.Birthday, Base.Phone, Base.IsSendOrder");
                sql.AppendLine("  , Base.Create_Who, Base.Update_Who, Base.Create_Time, Base.Update_Time");
                sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                sql.AppendLine(" FROM Param_Corp Corp WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Supplier_Members Base WITH(NOLOCK) ON Corp.Corp_UID = RTRIM(Base.Corp_UID)");
                sql.AppendLine(" WHERE (Corp.Display = 'Y')");


                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)myMember.DataID:
                                //編號
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)myMember.SupID:
                                //ERP廠商編號
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.ERP_ID = @ERP_ID)");

                                    cmd.Parameters.AddWithValue("ERP_ID", item.Value);
                                }

                                break;

                            case (int)myMember.Corp:
                                //公司別
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Corp.Corp_UID = @Corp_UID)");

                                    cmd.Parameters.AddWithValue("Corp_UID", item.Value);
                                }

                                break;

                        }
                    }
                }


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
                        var data = new Member_Data
                        {
                            Data_ID = item.Field<int>("Data_ID"),
                            Corp_UID = item.Field<int>("Corp_UID"),
                            ERP_ID = item.Field<string>("ERP_ID"),
                            Email = item.Field<string>("Email"),
                            FullName = item.Field<string>("FullName"),
                            NickName = item.Field<string>("NickName"),
                            Gender = item.Field<string>("Gender"),
                            Birthday = item.Field<DateTime?>("Birthday").ToString().ToDateString("yyyy/MM/dd"),
                            Phone = item.Field<string>("Phone"),
                            IsSendOrder = item.Field<string>("IsSendOrder"),

                            Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Create_Who = item.Field<string>("Create_Who"),
                            Update_Who = item.Field<string>("Update_Who"),
                            Create_Name = item.Field<string>("Create_Name"),
                            Update_Name = item.Field<string>("Update_Name")
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
        /// [供應商關聯設定] 建立基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public int Create(Supplier instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(Sup_UID), 0) + 1 FROM Supplier) ");
                sql.AppendLine(" INSERT INTO Supplier( ");
                sql.AppendLine("  Sup_UID, Sup_Name");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @NewID, @Sup_Name");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");
                sql.AppendLine(" SELECT @NewID AS DataID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Sup_Name", instance.Sup_Name);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    return Convert.ToInt32(DT.Rows[0]["DataID"]);
                }
            }

        }

        /// <summary>
        /// [供應商關聯設定] 建立關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_RelData(Rel_Data instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" IF (SELECT COUNT(*) FROM Supplier_Data WHERE (Sup_UID = @Sup_UID) AND (Corp_UID = @Corp_UID) AND (ERP_ID = @ERP_ID)) = 0");
                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(Data_UID), 0) + 1 FROM Supplier_Data) ");
                sql.AppendLine(" INSERT INTO Supplier_Data( ");
                sql.AppendLine("  Sup_UID, Data_UID, Corp_UID, ERP_ID, ERP_Label");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Sup_UID, @NewID, @Corp_UID, @ERP_ID, @ERP_Label");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Sup_UID", instance.Sup_UID);
                cmd.Parameters.AddWithValue("Corp_UID", instance.Corp_UID);
                cmd.Parameters.AddWithValue("ERP_ID", instance.ERP_SupID);
                cmd.Parameters.AddWithValue("ERP_Label", instance.ERP_SupName);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }


        /// <summary>
        /// [PKEF供應商基本資料] 建立通訊人
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public int Create_Member(Member_Data instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(Data_ID), 0) + 1 FROM Supplier_Members) ");
                sql.AppendLine(" INSERT INTO Supplier_Members( ");
                sql.AppendLine("  Data_ID, Corp_UID, ERP_ID");
                sql.AppendLine("  , Email, FullName, NickName, Gender, Birthday, Phone, IsSendOrder");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @NewID, @Corp_UID, @ERP_ID");
                sql.AppendLine("  , @Email, @FullName, @NickName, @Gender, @Birthday, @Phone, @IsSendOrder");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");
                sql.AppendLine(" SELECT @NewID AS DataID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Corp_UID", instance.Corp_UID);
                cmd.Parameters.AddWithValue("ERP_ID", instance.ERP_ID);
                cmd.Parameters.AddWithValue("Email", instance.Email);
                cmd.Parameters.AddWithValue("FullName", instance.FullName);
                cmd.Parameters.AddWithValue("NickName", instance.NickName);
                cmd.Parameters.AddWithValue("Gender", instance.Gender);
                cmd.Parameters.AddWithValue("Birthday", string.IsNullOrEmpty(instance.Birthday) ? DBNull.Value : (object)instance.Birthday.ToDateString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("Phone", instance.Phone);
                cmd.Parameters.AddWithValue("IsSendOrder", instance.IsSendOrder);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg))
                {
                    return Convert.ToInt32(DT.Rows[0]["DataID"]);
                }
            }

        }


        /// <summary>
        /// [PKEF供應商基本資料] 建立擴充欄位
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Info(ERP_Data instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(Data_ID), 0) + 1 FROM Supplier_ExtendedInfo) ");
                sql.AppendLine(" INSERT INTO Supplier_ExtendedInfo( ");
                sql.AppendLine("  Data_ID, Corp_UID, ERP_ID, Purchaser");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @NewID, @Corp_UID, @ERP_ID, @Purchaser");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");
                sql.AppendLine(" SELECT @NewID AS DataID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Corp_UID", instance.Corp_UID);
                cmd.Parameters.AddWithValue("ERP_ID", instance.ERP_SupID);
                cmd.Parameters.AddWithValue("Purchaser", instance.User_Account);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                //----- 資料取得 -----
                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }

        }

        #endregion


        #region -----// Update //-----

        /// <summary>
        /// [供應商關聯設定] 更新基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update(Supplier instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Supplier SET ");
                sql.AppendLine("  Sup_Name = @Sup_Name");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Sup_UID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Sup_UID);
                cmd.Parameters.AddWithValue("Sup_Name", instance.Sup_Name);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }

        }


        /// <summary>
        /// [PKEF供應商基本資料] 更新通訊人
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Member(Member_Data instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Supplier_Members SET ");
                sql.AppendLine("  Email = @Email, FullName = @FullName, NickName = @NickName, Gender = @Gender, Birthday = @Birthday, Phone = @Phone");
                sql.AppendLine("  , IsSendOrder = @IsSendOrder");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Corp_UID", instance.Corp_UID);
                cmd.Parameters.AddWithValue("ERP_ID", instance.ERP_ID);
                cmd.Parameters.AddWithValue("Email", instance.Email);
                cmd.Parameters.AddWithValue("FullName", instance.FullName);
                cmd.Parameters.AddWithValue("NickName", instance.NickName);
                cmd.Parameters.AddWithValue("Gender", instance.Gender);
                cmd.Parameters.AddWithValue("Birthday", string.IsNullOrEmpty(instance.Birthday) ? DBNull.Value : (object)instance.Birthday.ToDateString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("Phone", instance.Phone);
                cmd.Parameters.AddWithValue("IsSendOrder", instance.IsSendOrder);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }

        }


        /// <summary>
        /// [PKEF供應商基本資料] 更新擴充欄位
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Info(ERP_Data instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Supplier_ExtendedInfo SET ");
                sql.AppendLine("  Purchaser = @Purchaser");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.InfoID);
                cmd.Parameters.AddWithValue("Purchaser", instance.User_Account);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }

        }


        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// [供應商關聯設定] 刪除所有資料
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete(int dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Supplier_Data WHERE (Sup_UID = @DataID);");
                sql.AppendLine(" DELETE FROM Supplier WHERE (Sup_UID = @DataID);");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }

        /// <summary>
        /// [供應商關聯設定] 刪除關聯
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_RelData(int dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Supplier_Data");
                sql.AppendLine(" WHERE (Data_UID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }



        /// <summary>
        /// [PKEF供應商基本資料] 刪除通訊人
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_Member(int dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Supplier_Members");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                return db.ExecuteSql(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }

        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// [供應商關聯設定] 取得原始資料
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
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.Sup_UID, Base.Sup_Name");
                sql.AppendLine(" , Base.Create_Who, Base.Update_Who, Base.Create_Time, Base.Update_Time");
                sql.AppendLine(" , (SELECT COUNT(*) FROM Supplier_Data WHERE (Sup_UID = Base.Sup_UID)) AS DataCnt");
                sql.AppendLine(" , (SELECT COUNT(*) FROM Supplier_Data Rel WHERE (Rel.Sup_UID = Base.Sup_UID) AND (Rel.Corp_UID = 1)) AS twCnt");
                sql.AppendLine(" , (SELECT COUNT(*) FROM Supplier_Data Rel WHERE (Rel.Sup_UID = Base.Sup_UID) AND (Rel.Corp_UID = 2)) AS szCnt");
                sql.AppendLine(" , (SELECT COUNT(*) FROM Supplier_Data Rel WHERE (Rel.Sup_UID = Base.Sup_UID) AND (Rel.Corp_UID = 3)) AS shCnt");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name ");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name ");
                sql.AppendLine(" FROM Supplier Base WITH(NOLOCK)");
                sql.AppendLine(" WHERE (1 = 1) ");

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
                                    sql.Append(" AND (Base.Sup_UID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)Common.mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("  (UPPER(RTRIM(Base.Sup_Name)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR Base.Sup_UID IN (");
                                    sql.Append("   SELECT Sup_UID FROM Supplier_Data WITH(NOLOCK)");
                                    sql.Append("   WHERE");
                                    sql.Append("    (UPPER(RTRIM(ERP_ID)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(ERP_Label)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  ))");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                        }
                    }
                }


                sql.AppendLine(") AS Tbl ");
                sql.AppendLine(" ORDER BY Tbl.Create_Time DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, DBTarget.PKSYS, out ErrMsg);
            }
        }
        #endregion

    }
}
