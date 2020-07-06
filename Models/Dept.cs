using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PKLib_Data.Models
{
    public class PKDept
    {
        public string AreaCode { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public string GroupName { get; set; }
        public string ERP_DeptID { get; set; }
        public string Display { get; set; }
        public Int16 Sort { get; set; }
        public Int16 AreaSort { get; set; }
        public string Email { get; set; }
        public Int64 GP_Rank { get; set; }
    }

    public class PKDept_Supervisor
    {
        public int UID { get; set; }
        public string DeptID { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
    }
}