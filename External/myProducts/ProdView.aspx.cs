using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Data.Models;
using PKLib_Method.Methods;

namespace External.myProducts
{
    public partial class ProdView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //[取得/檢查參數] - DataID
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Message.Visible = true;
                    this.ph_Data.Visible = false;

                    return;
                }

                //Get Data
                LookupData();

            }
        }


        private void LookupData()
        {
            //----- 宣告:資料參數 -----
            ProductsRepository _product = new ProductsRepository();


            //----- 原始資料:取得資料 -----
            var prod = _product.GetOne(Req_DataID).FirstOrDefault();


            //----- 資料整理:判斷資料 ----- 
            if (prod == null)
            {
                this.ph_Message.Visible = true;
                this.ph_Data.Visible = false;

                return;
            }

            //----- 資料整理:顯示明細 ----- 
            this.ph_Message.Visible = false;
            this.ph_Data.Visible = true;

            //填入資料
            this.lt_ModelNo.Text = prod.ModelNo;
            this.lt_ModelName.Text = prod.Name_TW;
            this.lt_NameEN.Text = prod.Name_EN;
            this.lt_NameCN.Text = prod.Name_CN;
            this.lt_OpenDate.Text = prod.OpenDate;
            this.lt_CloseDate.Text = prod.CloseDate;
            this.lt_Vol.Text = prod.Vol;
            this.lt_Page.Text = prod.Page;

        }

        #region -- 參數設定 --
        /// <summary>
        /// 取得傳遞參數 - DataID
        /// </summary>
        private string _Req_DataID;
        public string Req_DataID
        {
            get
            {
                string DataID = Request.QueryString["DataID"] == null ? "" : Request.QueryString["DataID"].ToString();
                return DataID;
            }
            set
            {
                this._Req_DataID = value;
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
                return "{0}myProducts/ProdView.aspx?DataID={1}".FormatThis(Application["Web_Url"], Server.UrlEncode(Req_DataID));
            }
            set
            {
                this._PageUrl = value;
            }
        }


        #endregion
    }
}