using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKLib_Data.Assets
{
    public static class Common
    {
        /// <summary>
        /// 資料庫別
        /// </summary>
        public enum DBSrc : int
        {
            Prokits = 1,
            Prounion = 2,
            SHPK2 = 3
        }

        /// <summary>
        /// 欄位語系
        /// </summary>
        public enum ColLang : int
        {
            EN = 1,
            TW = 2,
            CN = 3
        }

        /// <summary>
        /// 一般查詢
        /// </summary>
        public enum mySearch : int
        {
            DataID = 1,
            Keyword = 2,
            Corp = 3,
            DeptID = 4,
            StartDate = 5,
            EndDate = 6,
            IsMIS = 7
        }

        /// <summary>
        /// 客戶查詢
        /// </summary>
        public enum CustSearch : int
        {
            DataID = 1,
            Keyword = 2,
            Block = 3,
            Corp = 4
        }

        /// <summary>
        /// 產品查詢
        /// </summary>
        public enum ProdSearch : int
        {
            DataID = 1,
            Keyword = 2,
            ItemKey = 3
        }

        /// <summary>
        /// 部門區域
        /// </summary>
        public enum DeptArea : int
        {
            ALL = 0,
            TW = 1,
            SH = 2,
            SZ = 3
        }

        /// <summary>
        /// 員工查詢
        /// </summary>
        public enum UserSearch : int
        {
            DataID = 1,
            Guid = 2,
            DeptID = 3,
            Keyword = 4
        }

        /// <summary>
        /// 部門查詢
        /// </summary>
        public enum DeptSearch : int
        {
            DataID = 1,
            Area = 2,
            Keyword = 4
        }
    }
}
