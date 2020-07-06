using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PKLib_Data.Models
{
    public class Corp
    {
        /// <summary>
        /// 公司別UID
        /// </summary>
        public int Corp_UID { get; set; }

        /// <summary>
        /// ERP的Company
        /// </summary>
        public string Corp_ID { get; set; }

        /// <summary>
        /// 公司別中文名稱
        /// </summary>
        public string Corp_Name { get; set; }

        /// <summary>
        /// 對應的DB名稱
        /// </summary>
        public string DB_Name { get; set; }

    }

    public class DBs
    {
        public int UID { get; set; }
        public string DB_Name { get; set; }
        public string DB_Desc { get; set; }
    }
}