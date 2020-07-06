using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Data.Models;
using PKLib_Method.Methods;

namespace External.myProducts
{
    public partial class ProdList : System.Web.UI.Page
    {
        public string ErrMsg;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //[取得/檢查參數] - Keyword
                    if (!string.IsNullOrEmpty(Req_Keyword))
                    {
                        this.tb_Keyword.Text = Req_Keyword;
                    }


                    //Get Data
                    LookupDataList(Req_PageIdx);

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LookupDataList(int pageIndex)
        {
            //----- 宣告:分頁參數 -----
            int RecordsPerPage = 10;    //每頁筆數
            int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
            int TotalRow = 0;   //總筆數
            ArrayList PageParam = new ArrayList();  //條件參數

            //----- 宣告:資料參數 -----
            ProductsRepository _product = new ProductsRepository();
            Dictionary<int, string> search = new Dictionary<int, string>();


            //----- 原始資料:條件篩選 -----
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                search.Add((int)Common.ProdSearch.Keyword, Req_Keyword);

                PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
            }


            //----- 原始資料:取得所有資料 -----
            var query = _product.GetProducts(search);


            //----- 資料整理:取得總筆數 -----
            TotalRow = query.Count();

            //----- 資料整理:頁數判斷 -----
            if (pageIndex > TotalRow && TotalRow > 0)
            {
                StartRow = 0;
                pageIndex = 1;
            }

            //----- 資料整理:選取每頁顯示筆數 -----
            var prod = query.Skip(StartRow).Take(RecordsPerPage);

            //----- 資料整理:繫結 ----- 
            this.lvDataList.DataSource = prod;
            this.lvDataList.DataBind();

            //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
            if (query.Count() > 0)
            {
                Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                lt_Pager.Text = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false);
            }

        }

        #region -- 按鈕事件 --

        /// <summary>
        /// 查詢
        /// </summary>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder SBUrl = new StringBuilder();
                SBUrl.Append("{0}?page=1".FormatThis(PageUrl));

                //[查詢條件] - 關鍵字
                if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
                {
                    SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text));
                }

                //執行轉頁
                Response.Redirect(SBUrl.ToString(), false);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region -- 參數設定 --
        /// <summary>
        /// 取得傳遞參數 - PageIdx(目前索引頁)
        /// </summary>
        private int _Req_PageIdx;
        public int Req_PageIdx
        {
            get
            {
                //int PageID = Convert.ToInt32(Page.RouteData.Values["PageID"]);
                int PageID = Request.QueryString["page"] == null ? 1 : Convert.ToInt32(Request.QueryString["page"]);
                return PageID;
            }
            set
            {
                this._Req_PageIdx = value;
            }
        }

        /// <summary>
        /// 本頁Url
        /// </summary>
        private string _PageUrl;
        public string PageUrl
        {
            get
            {
                return "{0}myProducts/ProdList.aspx".FormatThis(Application["Web_Url"]);
            }
            set
            {
                this._PageUrl = value;
            }
        }

        /// <summary>
        /// 取得傳遞參數 - Keyword
        /// </summary>
        private string _Req_Keyword;
        public string Req_Keyword
        {
            get
            {
                String Keyword = Request.QueryString["Keyword"];
                return (CustomExtension.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? Keyword.Trim() : "";
            }
            set
            {
                this._Req_Keyword = value;
            }
        }

        #endregion


    }
}