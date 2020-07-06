using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;

namespace PKLib_Data.External.myUpload
{
    public partial class upload : System.Web.UI.Page
    {
        //設定FTP連線參數
        private FtpMethod _ftp = new FtpMethod(
            System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Username"]
            , System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Password"]
            , System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"]);


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            #region --檔案處理--

            //宣告
            List<IOTempParam> ITempList = new List<IOTempParam>();
            Random rnd = new Random();
            int getFileCnt = 0;

            //取得上傳檔案集合
            HttpFileCollection hfc = Request.Files;


            //--- 限制上傳數量 ---
            for (int idx = 0; idx <= hfc.Count - 1; idx++)
            {
                //取得個別檔案
                HttpPostedFile hpf = hfc[idx];

                if (hpf.ContentLength > 0)
                {
                    getFileCnt++;
                }
            }
            if (getFileCnt > FileCountLimit)
            {
                Response.Write("檔案上傳上限為 3 個, 請重新選擇檔案.");
                return;
            }

            //--- 檔案檢查 ---
            for (int idx = 0; idx <= hfc.Count - 1; idx++)
            {
                //取得個別檔案
                HttpPostedFile hpf = hfc[idx];

                if (hpf.ContentLength > FileSizeLimit)
                {
                    Response.Write("檔案上傳限制為 5 MB, 請重新選擇檔案.");
                    return;
                }

                if (hpf.ContentLength > 0)
                {
                    //取得原始檔名
                    string OrgFileName = Path.GetFileName(hpf.FileName);
                    //取得副檔名
                    string FileExt = Path.GetExtension(OrgFileName).ToLower();
                    if (false == CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                    {
                        Response.Write("檔案上傳只允許副檔名為 jpg, png");
                        return;
                    }
                }
            }


            //檔案暫存List
            for (int idx = 0; idx <= hfc.Count - 1; idx++)
            {
                //取得個別檔案
                HttpPostedFile hpf = hfc[idx];

                if (hpf.ContentLength > 0)
                {
                    //取得原始檔名
                    string OrgFileName = Path.GetFileName(hpf.FileName);
                    //取得副檔名
                    string FileExt = Path.GetExtension(OrgFileName).ToLower();

                    //設定檔名, 重新命名
                    string myFullFile = String.Format(@"{0:yyMMddHHmmssfff}{1}{2}"
                        , DateTime.Now
                        , rnd.Next(0, 99)
                        , FileExt);


                    //判斷副檔名, 未符合規格的檔案不上傳
                    if (CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                    {
                        //設定暫存-檔案
                        ITempList.Add(new IOTempParam(myFullFile, OrgFileName, hpf));
                    }
                }
            }

            #endregion

            #region -- 儲存檔案 --

            //判斷資料夾, 不存在則建立
            _ftp.FTP_CheckFolder(UploadFolder);

            //暫存檔案List
            for (int row = 0; row < ITempList.Count; row++)
            {
                //取得個別檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;

                ////執行上傳
                //if (false == _ftp.FTP_doUpload(hpf, UploadFolder, ITempList[row].Param_FileName))
                //{
                //    Response.Write("Fail~~<br>");
                //}

                //執行上傳(指定Size)
                if (false == _ftp.FTP_doUpload(hpf, UploadFolder, ITempList[row].Param_FileName
                    , true, 1024, 768))
                {
                    Response.Write("Fail~~<br>");
                }
            }

            #endregion

            Response.Write("done..");


        }



        protected void Button2_Click(object sender, EventArgs e)
        {
            //_ftp.FTP_doDownload(UploadFolder, "16110711332899368.jpg", "hello.jpg");
            _ftp.RotateImage(UploadFolder, "1611071132058497.jpg", 90);
        }


        #region -- 上傳參數 --
        /// <summary>
        /// 限制上傳的副檔名
        /// </summary>
        private string _FileExtLimit;
        public string FileExtLimit
        {
            get
            {
                return "jpg|png";
            }
            set
            {
                this._FileExtLimit = value;
            }
        }

        /// <summary>
        /// 限制上傳的檔案大小(1MB = 1024000), 5MB
        /// </summary>
        private int _FileSizeLimit;
        public int FileSizeLimit
        {
            get
            {
                return 5120000;
            }
            set
            {
                this._FileSizeLimit = value;
            }
        }

        /// <summary>
        /// 限制上傳檔案數
        /// </summary>
        private int _FileCountLimit;
        public int FileCountLimit
        {
            get
            {
                return 3;
            }
            set
            {
                this._FileCountLimit = value;
            }
        }

        /// <summary>
        /// 上傳目錄
        /// </summary>
        private string _UploadFolder;
        public string UploadFolder
        {
            get
            {
                return @"PKEvent/Test/";
            }
            set
            {
                this._UploadFolder = value;
            }
        }

        /// <summary>
        /// 暫存參數
        /// </summary>
        public class IOTempParam
        {
            /// <summary>
            /// [參數] - 檔名
            /// </summary>
            private string _Param_FileName;
            public string Param_FileName
            {
                get { return this._Param_FileName; }
                set { this._Param_FileName = value; }
            }

            /// <summary>
            /// [參數] -原始檔名
            /// </summary>
            private string _Param_OrgFileName;
            public string Param_OrgFileName
            {
                get { return this._Param_OrgFileName; }
                set { this._Param_OrgFileName = value; }
            }


            private HttpPostedFile _Param_hpf;
            public HttpPostedFile Param_hpf
            {
                get { return this._Param_hpf; }
                set { this._Param_hpf = value; }
            }

            /// <summary>
            /// 設定參數值
            /// </summary>
            /// <param name="Param_FileName">系統檔名</param>
            /// <param name="Param_OrgFileName">原始檔名</param>
            /// <param name="Param_hpf">上傳檔案</param>
            /// <param name="Param_FileKind">檔案類別</param>
            public IOTempParam(string Param_FileName, string Param_OrgFileName, HttpPostedFile Param_hpf)
            {
                this._Param_FileName = Param_FileName;
                this._Param_OrgFileName = Param_OrgFileName;
                this._Param_hpf = Param_hpf;
            }

        }
        #endregion


    }
}