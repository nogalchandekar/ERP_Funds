using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.Models.ViewModel
{
	public class VMLoan
	{
		public int LoanId { get; set; }
		public Nullable<int> CustomerId { get; set; }
		public Nullable<decimal> LoanAmount { get; set; }
		public Nullable<int> LoanDurationDays { get; set; }
		public Nullable<decimal> LoanInterest { get; set; }
		public string DeductAmount { get; set; }
		public string AmountGivenToCustomer { get; set; }
		public string LoanDurationWithMonths { get; set; }
		public string DailyReturn { get; set; }
		public string TotalPayable { get; set; }
		public string LoanNo { get; set; }

		public string CustomerName { get; set; }
		public Nullable<bool> IsActive { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<System.DateTime> CreatedDate { get; set; }
		public string ModifiedBy { get; set; }
		public Nullable<System.DateTime> ModifiedDate { get; set; }
		// in VMLoan (add)
		public decimal TotalPaid { get; set; }
		public decimal PendingAmount { get; set; }

	}
}