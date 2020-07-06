using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKLib_Data.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Product
    {
        public Int64 SerialNo { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string ModelNo { get; set; }

        /// <summary>
        /// 英文品名
        /// </summary>
        public string Name_EN { get; set; }

        /// <summary>
        /// 中文品名
        /// </summary>
        public string Name_TW { get; set; }

        /// <summary>
        /// 簡中品名
        /// </summary>
        public string Name_CN { get; set; }

        /// <summary>
        /// 目錄
        /// </summary>
        public string Vol { get; set; }

        /// <summary>
        /// 頁次
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// 生效日
        /// </summary>
        public string OpenDate { get; set; }

        /// <summary>
        /// 失效日
        /// </summary>
        public string CloseDate { get; set; }

        /// <summary>
        /// 類別編號
        /// </summary>
        public string ClassID { get; set; }
        public string ClassName_EN { get; set; }
        public string ClassName_TW { get; set; }
        public string ClassName_CN { get; set; }


        /* 其他欄位 */
        public double Adv_Price { get; set; }

        public int Adv_StockNum { get; set; }
        public int SafeQty_SZEC { get; set; }
        public int SafeQty_A01 { get; set; }
        public int SafeQty_B01 { get; set; }

    }

}
