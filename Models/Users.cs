using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PKLib_Data.Models
{
    public class PKUsers
    {
        /// <summary>
        /// User Guid
        /// </summary>
        public string ProfGuid { get; set; }

        /// <summary>
        /// 工號
        /// </summary>
        public string ProfID { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string ProfName { get; set; }

        /// <summary>
        /// 部門代號
        /// </summary>
        public string DeptID { get; set; }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { get; set; }
        public string NickName { get; set; }

        public string Email { get; set; }
        public string Tel_Ext { get; set; }

        public Int64 GP_Rank { get; set; }
    }


    /// <summary>
    /// 人員樹狀選單 - for jquery zTree
    /// </summary>
    public class UserTree
    {
        public string MenuID { get; set; }
        public string ParentID { get; set; }
        public string MenuName { get; set; }
        public string Sort { get; set; }
        public bool IsOpen { get; set; }
        public bool chkDisabled { get; set; }
    }
}