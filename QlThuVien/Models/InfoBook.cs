using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QlThuVien.Models
{
    public class InfoBook
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string BookAuthor { get; set; }

        public string BookImage { get; set; }

        public string CategoryName { get; set; }

        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string UserPhone { get; set; }

        public int TranId { get; set; }
        public string TranStatus { get; set; }
        public string TranDate_Accepted { get; set; }
        public string TranDate_Returned { get; set; }
        public string TranDate_AccReturned { get; set; }

        public string TranDate_GiveBack { get; set; }
        public string TranDate_DeleteRequest { get; set; }
      
        public string TranDate { get; set; }
        public string TranDate_Get { get; set; }
        public string Description  { get; set; }
        public string Email { get; set; }
        

        

    }
}