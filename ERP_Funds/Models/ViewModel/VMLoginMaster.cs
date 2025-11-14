using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.Models.ViewModel
{
    public class VMLoginMaster
    {
        public int Login_Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}