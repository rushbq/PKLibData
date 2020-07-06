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
 * 產品中心 - 產品資料
 */
namespace PKLib_Data.Controllers
{
    /// <summary>
    /// 產品資料
    /// </summary>
    public class ProductsRepository : dbConn
    {
        /// <summary>
        /// 取得所有產品資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(zh-TW, Prokits, null)
        /// </remarks>
        public IQueryable<Product> GetProducts()
        {
            return GetProducts(Common.ColLang.TW, Common.DBSrc.Prokits, null);
        }

        /// <summary>
        /// 取得所有產品資料(查詢參數)
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Product> GetProducts(Dictionary<int, string> search)
        {
            return GetProducts(Common.ColLang.TW, Common.DBSrc.Prokits, search);
        }

        /// <summary>
        /// 取得所有產品資料
        /// </summary>
        /// <param name="langCode">語系</param>
        /// <param name="dbs">資料來源</param>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Product> GetProducts(Common.ColLang langCode, Common.DBSrc dbs, Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Product> products = new List<Product>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(langCode, dbs, search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable()
                    .OrderBy(o => o.Field<string>("Model_No"));

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var prod = new Product
                    {
                        SerialNo = item.Field<Int64>("SerialNo"),
                        ModelNo = item.Field<string>("Model_No"),
                        Name_EN = item.Field<string>("Name_EN"),
                        Name_TW = item.Field<string>("Name_TW"),
                        Name_CN = item.Field<string>("Name_CN"),
                        ClassID = item.Field<string>("Class_ID"),
                        ClassName_EN = item.Field<string>("ClassName_EN"),
                        ClassName_TW = item.Field<string>("ClassName_TW"),
                        ClassName_CN = item.Field<string>("ClassName_CN"),
                        Vol = item.Field<string>("Vol"),
                        Page = item.Field<string>("Page"),
                        OpenDate = item.Field<string>("OpenDate").ToDateString_ERP("-"),
                        CloseDate = item.Field<System.DateTime?>("CloseDate").ToString().ToDateString("yyyy-MM-dd"),
                        SafeQty_SZEC = item.Field<int>("SafeQty_SZEC"),
                        SafeQty_A01 = item.Field<int>("SafeQty_A01"),
                        SafeQty_B01 = item.Field<int>("SafeQty_B01")
                    };

                    //將項目加入至集合
                    products.Add(prod);

                }

            }

            //回傳集合
            return products.AsQueryable();

        }


        /// <summary>
        /// 取得指定產品資料
        /// </summary>
        /// <param name="queryID">資料編號</param>
        /// <remarks>
        /// 預設值為(zh-TW, Prokits, queryID)
        /// </remarks>
        public IQueryable<Product> GetOne(string queryID)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)Common.ProdSearch.DataID, queryID);

            return GetProducts(Common.ColLang.TW, Common.DBSrc.Prokits, search);
        }

        /// <summary>
        /// 取得指定產品資料
        /// </summary>
        /// <param name="langCode">語系</param>
        /// <param name="dbs">資料來源</param>
        /// <param name="queryID">資料編號</param>
        /// <remarks>
        /// 預設值為(zh-TW, Prokits, queryID)
        /// </remarks>
        public IQueryable<Product> GetOne(Common.ColLang langCode, Common.DBSrc dbs, string queryID)
        {
            Dictionary<int, string> search = new Dictionary<int, string>();
            search.Add((int)Common.ProdSearch.DataID, queryID);

            return GetProducts(langCode, dbs, search);
        }



        #region -- 資料取得 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="langCode">語系</param>
        /// <param name="dbs">資料來源</param>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        private DataTable LookupRawData(Common.ColLang langCode, Common.DBSrc dbs, Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            dbConn db = new dbConn();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                switch ((int)dbs)
                {
                    case 2:
                        //2020年, 深圳已移除
                        sql = Query_SZ(search);
                        break;

                    default:
                        sql = Query_Default(search);
                        break;
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();

                //----- SQL Filter -----
                foreach (var item in search)
                {
                    switch (item.Key)
                    {
                        case (int)Common.ProdSearch.DataID:
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                cmd.Parameters.AddWithValue("DataID", item.Value);
                            }

                            break;

                        case (int)Common.ProdSearch.Keyword:
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                cmd.Parameters.AddWithValue("Keyword", item.Value);
                            }

                            break;
                    }
                }

                //----- 回傳資料 -----
                return db.LookupDT(cmd, dbConn.DBTarget.Product, out ErrMsg);
            }
        }

        /// <summary>
        /// 回傳SQL - ProdItem
        /// </summary>
        /// <param name="search">查詢參數集合</param>
        /// <returns></returns>
        private StringBuilder Query_Default(Dictionary<int, string> search)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY Base.Model_No) AS SerialNo");
            sql.AppendLine("  , RTRIM(Base.Model_No) AS Model_No, RTRIM(Base.Model_Name_zh_TW) AS Name_TW, RTRIM(Base.Model_Name_zh_CN) AS Name_CN, RTRIM(Base.Model_Name_en_US) AS Name_EN");
            sql.AppendLine("  , ISNULL(Base.Catelog_Vol, '') AS Vol, ISNULL(Base.Page, '') AS Page");
            sql.AppendLine("  , Base.Date_Of_Listing AS OpenDate, Base.Stop_Offer_Date AS CloseDate");
            sql.AppendLine("  , RTRIM(Cls.Class_ID) Class_ID");
            sql.AppendLine("  , RTRIM(Cls.Class_Name_zh_TW) AS ClassName_TW, RTRIM(Cls.Class_Name_en_US) AS ClassName_EN, RTRIM(Cls.Class_Name_zh_CN) AS ClassName_CN");
            sql.AppendLine("  , ISNULL(Info.SafeQty_SZEC, 0) AS SafeQty_SZEC , ISNULL(Info.SafeQty_A01, 0) AS SafeQty_A01 , ISNULL(Info.SafeQty_B01, 0) AS SafeQty_B01");
            sql.AppendLine(" FROM Prod_Item Base WITH(NOLOCK)");
            sql.AppendLine("  INNER JOIN Prod_Class Cls WITH(NOLOCK) ON Base.Class_ID = Cls.Class_ID");
            sql.AppendLine("  LEFT JOIN PKSYS.dbo.Prod_ExtendedInfo info WITH(NOLOCK) ON info.Model_No = Base.Model_No");
            sql.AppendLine(" WHERE (LEFT(Base.Model_No, 1) <> '0')");

            foreach (var item in search)
            {
                switch (item.Key)
                {
                    case (int)Common.ProdSearch.DataID:
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            sql.Append(" AND (UPPER(Base.Model_No) = UPPER(@DataID))");
                        }

                        break;

                    case (int)Common.ProdSearch.Keyword:
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            sql.Append(" AND (");
                            sql.Append("    (UPPER(Base.Model_No) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR (UPPER(Base.Model_Name_zh_TW) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR (UPPER(Base.Model_Name_zh_CN) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR (UPPER(Base.Model_Name_en_US) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append(" )");
                        }

                        break;
                }
            }

            sql.AppendLine(" ORDER BY Base.Model_No");

            return sql;
        }

        ///// <summary>
        ///// 回傳SQL - ProdItem_SZ(2020/3, 深圳移除)
        ///// </summary>
        ///// <param name="search">查詢參數集合</param>
        ///// <returns></returns>
        //private StringBuilder Query_SZ(Dictionary<int, string> search)
        //{
        //    StringBuilder sql = new StringBuilder();

        //    sql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY Base.Model_No) AS SerialNo");
        //    sql.AppendLine("  , RTRIM(Base.Model_No) AS Model_No, RTRIM(Base.Model_Name_zh_TW) AS Name_TW, RTRIM(Base.Model_Name_zh_CN) AS Name_CN, RTRIM(Base.Model_Name_en_US) AS Name_EN");
        //    sql.AppendLine("  , ISNULL(Base.Catelog_Vol, '') AS Vol, ISNULL(Base.Page, '') AS Page");
        //    sql.AppendLine("  , Base.Date_Of_Listing AS OpenDate, Base.Stop_Offer_Date AS CloseDate");
        //    sql.AppendLine("  , RTRIM(Cls.Class_ID) Class_ID");
        //    sql.AppendLine("  , RTRIM(Cls.Class_Name_zh_TW) AS ClassName_TW, RTRIM(Cls.Class_Name_en_US) AS ClassName_EN, RTRIM(Cls.Class_Name_zh_CN) AS ClassName_CN");
        //    sql.AppendLine("  , ISNULL(Info.SafeQty_SZEC, 0) AS SafeQty_SZEC, ISNULL(Info.SafeQty_A01, 0) AS SafeQty_A01, ISNULL(Info.SafeQty_B01, 0) AS SafeQty_B01");
        //    sql.AppendLine(" FROM Prod_ItemSZ Base");
        //    sql.AppendLine("  INNER JOIN Prod_Class Cls WITH(NOLOCK) ON Base.Class_ID = Cls.Class_ID");
        //    sql.AppendLine("  LEFT JOIN PKSYS.dbo.Prod_ExtendedInfo info WITH(NOLOCK) ON info.Model_No = Base.Model_No");
        //    sql.AppendLine(" WHERE (LEFT(Base.Model_No, 1) <> '0')");

        //    foreach (var item in search)
        //    {
        //        switch (item.Key)
        //        {
        //            case (int)Common.ProdSearch.DataID:
        //                if (!string.IsNullOrEmpty(item.Value))
        //                {
        //                    sql.Append(" AND (UPPER(Base.Model_No) = UPPER(@DataID))");
        //                }

        //                break;

        //            case (int)Common.ProdSearch.Keyword:
        //                if (!string.IsNullOrEmpty(item.Value))
        //                {
        //                    sql.Append(" AND (");
        //                    sql.Append("    (UPPER(Base.Model_No) LIKE '%' + UPPER(@Keyword) + '%')");
        //                    sql.Append("    OR (UPPER(Base.Model_Name_zh_TW) LIKE '%' + UPPER(@Keyword) + '%')");
        //                    sql.Append("    OR (UPPER(Base.Model_Name_zh_CN) LIKE '%' + UPPER(@Keyword) + '%')");
        //                    sql.Append("    OR (UPPER(Base.Model_Name_en_US) LIKE '%' + UPPER(@Keyword) + '%')");
        //                    sql.Append(" )");
        //                }
        //                break;
        //        }
        //    }

        //    sql.AppendLine(" ORDER BY Base.Model_No");

        //    return sql;
        //}

        /// <summary>
        /// 回傳語系字串
        /// </summary>
        /// <param name="langcode">語系選擇</param>
        /// <returns></returns>
        private string Get_Lang(int langcode)
        {
            switch (langcode)
            {
                case 1:
                    return "en-US";

                case 2:
                    return "zh-TW";

                case 3:
                    return "zh-CN";

                default:
                    return "en-US";
            }
        }

        #endregion

    }
}
