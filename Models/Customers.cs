using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PKLib_Data.Models
{
    public class Customers
    {
        /// <summary>
        /// 客戶編號
        /// </summary>
        public string CustID { get; set; }

        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CustName { get; set; }

        /// <summary>
        /// 業務人員代號(MA016)
        /// </summary>
        public string SalesID { get; set; }

        /// <summary>
        /// 資料庫別ID
        /// </summary>
        public string Corp_ID { get; set; }
        /// <summary>
        /// 資料庫別Name
        /// </summary>
        public string Corp_Name { get; set; }
        public string ContactWho { get; set; }
        public string ContactAddr { get; set; }
        public string Tel { get; set; }
    }
}