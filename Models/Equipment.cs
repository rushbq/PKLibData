using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * ERP 資產設備資料 (for MIS)
 */
namespace PKLib_Data.Models
{
    public class Equipment
    {
        /// <summary>
        /// 資產編號
        /// </summary>
        public string Who { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public string GetItemDate { get; set; }
        public Decimal? GetItemMoney { get; set; }
    }
}