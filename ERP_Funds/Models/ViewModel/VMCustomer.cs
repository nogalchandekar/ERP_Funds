using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.Models.ViewModel
{
	public class VMCustomer
	{
		public int C_Id { get; set; }
		public string CustomerName { get; set; }
		public Nullable<decimal> MobileNo { get; set; }
		public string EmailId { get; set; }
		public string Address { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<System.DateTime> CreatedDate { get; set; }
		public string ModifiedBy { get; set; }
		public Nullable<System.DateTime> ModifiedDate { get; set; }
		public Nullable<bool> IsActive { get; set; }
		public string AdhaarNo { get; set; }
		public string PanNo { get; set; }
	}
}