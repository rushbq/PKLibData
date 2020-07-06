using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace External
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_showOK_Click(object sender, EventArgs e)
        {
            this.ph_showOK.Visible = true;
            this.ph_showFail.Visible = false;
        }

        protected void btn_showFail_Click(object sender, EventArgs e)
        {
            this.ph_showOK.Visible = false;
            this.ph_showFail.Visible = true;
        }


        protected void btn_doSave_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(3000);

            Response.Redirect("default.aspx");
        }


        #region -- 參數設定 --
        /// <summary>
        /// 取得系統參數 - FileDiskUrl
        /// </summary>
        private string _FileDiskUrl;
        public string FileDiskUrl
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["File_Url"];
            }
            set
            {
                this._FileDiskUrl = value;
            }
        }




        #endregion

    }
}