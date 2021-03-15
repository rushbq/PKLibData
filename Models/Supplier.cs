using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PKLib_Data.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// 供應商編號
        /// </summary>
        public int Sup_UID { get; set; }

        /// <summary>
        /// 供應商名稱
        /// </summary>
        public string Sup_Name { get; set; }

        /// <summary>
        /// 資料計數
        /// </summary>
        public int DataCnt { get; set; }
        public int twCnt { get; set; }
        public int szCnt { get; set; }
        public int shCnt { get; set; }

        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }
    }

    /// <summary>
    /// 已關聯資料欄位
    /// </summary>
    public class Rel_Data
    {
        public int Sup_UID { get; set; }
        public int Data_UID { get; set; }
        public int Corp_UID { get; set; }
        public string Corp_Name { get; set; }
        public string ERP_SupID { get; set; }
        public string ERP_SupName { get; set; }
    }

    /// <summary>
    /// 未關聯資料欄位
    /// </summary>
    public class UnRel_Data
    {
        public int Sup_UID { get; set; }
        public int Corp_UID { get; set; }
        public string Corp_Name { get; set; }
        public string ERP_SupID { get; set; }
        public string ERP_SupName { get; set; }
    }

    public class ERP_Data
    {
        public int Corp_UID { get; set; }
        public string Corp_Name { get; set; }
        public string ERP_SupID { get; set; }
        public string ERP_SupName { get; set; }
        public int InfoID { get; set; }

        /// <summary>
        /// 採購人員
        /// </summary>
        public string User_Account { get; set; }
        public string User_Name { get; set; }

        /// <summary>
        /// 台灣:收款人帳號
        /// </summary>
        public string tw_Account { get; set; }
        /// <summary>
        /// 台灣:收款人名稱
        /// </summary>
        public string tw_AccName { get; set; }
        /// <summary>
        /// 台灣:銀行名稱
        /// </summary>
        public string tw_BankName { get; set; }
        /// <summary>
        /// 台灣:銀行代號
        /// </summary>
        public string tw_BankID { get; set; }

        /// <summary>
        /// 中國:收款人帳號
        /// </summary>
        public string cn_Account { get; set; }
        /// <summary>
        /// 中國:收款人名稱
        /// </summary>
        public string cn_AccName { get; set; }
        /// <summary>
        /// 中國:收款人Email
        /// </summary>
        public string cn_Email { get; set; }
        /// <summary>
        /// 中國:開戶行名稱
        /// </summary>
        public string cn_BankName { get; set; }
        /// <summary>
        /// 中國:CNAPS行號
        /// </summary>
        public string cn_BankID { get; set; }
        /// <summary>
        /// 中國:客戶業務編號
        /// </summary>
        public string cn_SaleID { get; set; }
        /// <summary>
        /// 中國:省份
        /// </summary>
        public string cn_State { get; set; }
        /// <summary>
        /// 中國:縣市
        /// </summary>
        public string cn_City { get; set; }
        /// <summary>
        /// 中國:收款人開戶行
        /// </summary>
        public string cn_BankType { get; set; }

        /// <summary>
        /// 外匯:收款人帳號
        /// </summary>
        public string ww_Account { get; set; }
        /// <summary>
        /// 外匯:收款人名稱
        /// </summary>
        public string ww_AccName { get; set; }
        /// <summary>
        /// 外匯:收款人電話
        /// </summary>
        public string ww_Tel { get; set; }
        /// <summary>
        /// 外匯:收款人地址
        /// </summary>
        public string ww_Addr { get; set; }
        /// <summary>
        /// 外匯:銀行名稱
        /// </summary>
        public string ww_BankName { get; set; }
        /// <summary>
        /// 外匯:分行
        /// </summary>
        public string ww_BankBranch { get; set; }
        /// <summary>
        /// 外匯:銀行地址
        /// </summary>
        public string ww_BankAddr { get; set; }
        /// <summary>
        /// 外匯:國家
        /// </summary>
        public string ww_Country { get; set; }
        /// <summary>
        /// 外匯:代碼
        /// </summary>
        public string ww_Code { get; set; }


        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
    }

    /// <summary>
    /// 通訊錄
    /// </summary>
    public class Member_Data
    {
        public int Data_ID { get; set; }
        public int Corp_UID { get; set; }
        public string ERP_ID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string Phone { get; set; }
        public string IsSendOrder { get; set; }

        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }
    }
}