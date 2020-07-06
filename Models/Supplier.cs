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
        public string User_Account { get; set; }
        public string User_Name { get; set; }

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