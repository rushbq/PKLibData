using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using PKLib_Data.Assets;
using PKLib_Method.Methods;
using ProdSMPData.Models;

/*
  產品中心 - 新品取樣
 */
namespace ProdSMPData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        Company = 3,
        Source = 4,
        Check = 5,
        Status = 6,
        DateType = 7,  //要放在sDate, eDate之前
        StartDate = 8,
        EndDate = 9,
        RelID = 10
    }


    /// <summary>
    /// 類別參數
    /// </summary>
    /// <remarks>
    /// 1:來源 / 2:檢驗類別 / 3:狀態
    /// </remarks>
    public enum myClass : int
    {
        source = 1,
        check = 2,
        status = 3
    }

    public class ProdSampleRepository
    {
        public string ErrMsg;


        #region -----// Read //-----

        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<ProdSMP> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ProdSMP> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ProdSMP> dataList = new List<ProdSMP>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new ProdSMP
                    {
                        SeqNo = item.Field<int>("SeqNo"),
                        SP_ID = item.Field<Guid>("SP_ID"),

                        Qty = item.Field<int?>("Qty"),
                        Cls_Source = item.Field<int?>("Cls_Source"),
                        Cls_Check = item.Field<int?>("Cls_Check"),
                        Cls_Status = item.Field<int?>("Cls_Status"),

                        Date_Come = item.Field<DateTime?>("Date_Come").ToString().ToDateString("yyyy/MM/dd"),
                        Date_Est = item.Field<DateTime?>("Date_Est").ToString().ToDateString("yyyy/MM/dd"),
                        Date_Actual = item.Field<DateTime?>("Date_Actual").ToString().ToDateString("yyyy/MM/dd"),
                        Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),

                        //樣品編號:TWS-1602-001
                        SerialNo = (item.Field<int>("SerialID")) == null ? "資料建立中" :
                         "{0}-{1}-{2}".FormatThis(
                            item.Field<string>("Company")
                            , item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyMM")
                            , ("00" + item.Field<int>("SerialID")).Right(3)
                        ),
                        Company = item.Field<string>("Company"),
                        Assign_Who = item.Field<string>("Assign_Who"),
                        Model_No = item.Field<string>("Model_No"),
                        Cust_ID = item.Field<int?>("Cust_ID"),
                        Cust_ModelNo = item.Field<string>("Cust_ModelNo"),
                        Cust_Newguy = item.Field<string>("Cust_Newguy"),
                        Description1 = string.IsNullOrEmpty(item.Field<string>("Description1")) ? "" : item.Field<string>("Description1"),
                        Description2 = string.IsNullOrEmpty(item.Field<string>("Description2")) ? "" : item.Field<string>("Description2"),
                        Remark = string.IsNullOrEmpty(item.Field<string>("Remark")) ? "" : item.Field<string>("Remark"),
                        Create_Who = item.Field<string>("Create_Who"),
                        Update_Who = item.Field<string>("Update_Who"),
                        Company_Name = item.Field<string>("myCompany"),
                        Assign_Name = item.Field<string>("AssignName"),
                        Source_Name = item.Field<string>("mySource"),
                        Check_Name = item.Field<string>("myCheck"),
                        Status_Name = item.Field<string>("myStatus"),
                        Cust_Name = item.Field<string>("CustName"),
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
        /// 取得類別
        /// </summary>
        /// <param name="cls">type</param>
        /// <returns></returns>
        public IQueryable<SampleClass> GetClassList(myClass cls)
        {
            //----- 宣告 -----
            List<SampleClass> dataList = new List<SampleClass>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData_Status(cls))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new SampleClass
                    {
                        ID = item.Field<int>("ID"),
                        Label = item.Field<string>("Label")
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得品號關聯
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<RelModelNo> GetRelModelList(string dataID)
        {
            //----- 宣告 -----
            List<RelModelNo> dataList = new List<RelModelNo>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT SP_ID AS ID, Model_No AS Label");
                sql.AppendLine(" FROM Sample_Rel_ModelNo WITH(NOLOCK)");
                sql.AppendLine(" WHERE (SP_ID = @DataID)");
                sql.AppendLine(" ORDER BY Model_No");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, dbConn.DBTarget.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RelModelNo
                        {
                            SP_ID = item.Field<Guid>("ID"),
                            Model_No = item.Field<string>("Label")
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
        /// 取得檔案附件
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<SampleFiles> GetFileList(string dataID)
        {
            //----- 宣告 -----
            List<SampleFiles> dataList = new List<SampleFiles>();
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT AttachID, SP_ID, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                sql.AppendLine(" FROM Sample_Attachment WITH(NOLOCK)");
                sql.AppendLine(" WHERE (SP_ID = @DataID)");
                sql.AppendLine(" ORDER BY Create_Time");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = db.LookupDT(cmd, dbConn.DBTarget.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new SampleFiles
                        {
                            AttachID = item.Field<int>("AttachID"),
                            SP_ID = item.Field<Guid>("SP_ID"),
                            AttachFile = item.Field<string>("AttachFile"),
                            AttachFile_Name = item.Field<string>("AttachFile_Name"),
                            Create_Who = item.Field<string>("Create_Who"),
                            Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")
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
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.* ");
                sql.AppendLine("   , (CASE Base.Company WHEN 'TWS' THEN '台灣' WHEN 'SHS' THEN '上海' ELSE '深圳' END) AS myCompany");
                sql.AppendLine("   , ClsSrc.Class_Name AS mySource, ClsChk.Class_Name AS myCheck, ClsSt.Class_Name AS myStatus");
                sql.AppendLine("   , (Prof.Account_Name + ' (' + Prof.Display_Name + ')') AS AssignName");
                sql.AppendLine("   , ISNULL(RTRIM(Sup.Sup_Name), Base.Cust_Newguy) AS CustName");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name ");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name ");
                sql.AppendLine(" FROM Sample_List Base");
                sql.AppendLine("  LEFT JOIN Sample_Class ClsSrc ON Base.Cls_Source = ClsSrc.Class_ID");
                sql.AppendLine("  LEFT JOIN Sample_Class ClsChk ON Base.Cls_Check = ClsChk.Class_ID");
                sql.AppendLine("  LEFT JOIN Sample_Class ClsSt ON Base.Cls_Status = ClsSt.Class_ID");
                sql.AppendLine("  LEFT JOIN PKSYS.dbo.User_Profile Prof WITH(NOLOCK) ON Base.Assign_Who = Prof.Account_Name");
                sql.AppendLine("  LEFT JOIN PKSYS.dbo.Supplier Sup WITH(NOLOCK) ON Base.Cust_ID = Sup.Sup_UID");
                sql.AppendLine(" WHERE (1 = 1) ");


                /* Search */
                #region >> filter <<

                if (search != null)
                {
                    string filterDateType = "";

                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.SP_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Base.Model_No)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Base.Cust_ModelNo)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Sup.Sup_Name)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.Cust_Newguy) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.Description1) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR ((Company + RIGHT(CONVERT(VARCHAR(6), Base.Create_Time, 112), 4) + RIGHT('00' + CAST(SerialID AS VARCHAR), 3)) LIKE '%' + REPLACE(UPPER(@Keyword), '-', '') + '%')");
                                    sql.Append("    OR (Sup.Sup_UID IN (");
                                    sql.Append("     SELECT Sup_UID FROM PKSYS.dbo.Supplier_Data WITH(NOLOCK)");
                                    sql.Append("     WHERE");
                                    sql.Append("      (UPPER(RTRIM(ERP_ID)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("      OR (UPPER(RTRIM(ERP_Label)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    ))");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)mySearch.Company:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Company = @Company)");

                                    cmd.Parameters.AddWithValue("Company", item.Value);
                                }

                                break;

                            case (int)mySearch.Source:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Cls_Source = @Source)");

                                    cmd.Parameters.AddWithValue("Source", item.Value);
                                }

                                break;

                            case (int)mySearch.Check:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Cls_Check = @Check)");

                                    cmd.Parameters.AddWithValue("Check", item.Value);
                                }

                                break;

                            case (int)mySearch.Status:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Cls_Status = @Status)");

                                    cmd.Parameters.AddWithValue("Status", item.Value);
                                }

                                break;

                            case (int)mySearch.DateType:
                                switch (item.Value)
                                {
                                    case "1":
                                        filterDateType = "Base.Date_Come";
                                        break;

                                    case "2":
                                        filterDateType = "Base.Date_Est";
                                        break;

                                    case "3":
                                        filterDateType = "Base.Date_Actual";
                                        break;

                                    default:
                                        filterDateType = "Base.Create_Time";
                                        break;
                                }

                                break;


                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));

                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));

                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;

                            case (int)mySearch.RelID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append("AND (Base.SP_ID IN (");
                                    sql.Append(" SELECT Rel_ID FROM Sample_Rel_ID");
                                    sql.Append(" WHERE (SP_ID = @CurrID)");
                                    sql.Append("))");

                                    cmd.Parameters.AddWithValue("CurrID", item.Value);
                                }

                                break;

                        }
                    }
                }
                #endregion

                sql.AppendLine(") AS Tbl ");
                sql.AppendLine(" ORDER BY Tbl.Create_Time DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return db.LookupDT(cmd, dbConn.DBTarget.Product, out ErrMsg);
            }
        }


        /// <summary>
        /// 取得類別資料
        /// </summary>
        /// <param name="cls">類別參數</param>
        /// <returns></returns>
        private DataTable LookupRawData_Status(myClass cls)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Class_ID AS ID, Class_Name AS Label");
                sql.AppendLine(" FROM Sample_Class WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Class_Type = @Class) AND (Display = 'Y')");
                sql.AppendLine(" ORDER BY Sort");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Class", cls);


                //----- 回傳資料 -----
                return db.LookupDT(cmd, dbConn.DBTarget.Product, out ErrMsg);
            }
        }

        #endregion

    }
}