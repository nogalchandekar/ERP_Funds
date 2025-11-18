using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.Models.ViewModel
{
	public class VMExcelReport
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
		public Nullable<bool> IsActive { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<System.DateTime> CreatedDate { get; set; }
		public string ModifiedBy { get; set; }
		public Nullable<System.DateTime> ModifiedDate { get; set; }
		public string LoanNo { get; set; }
		public int DailyCollectionId { get; set; }
		public Nullable<int> LoanNoId { get; set; }
		public Nullable<int> LoanDuration { get; set; }
		public Nullable<decimal> PerDayInstallment { get; set; }
		public Nullable<decimal> TotalPaid { get; set; }
		public Nullable<decimal> PendingAmount { get; set; }
		public Nullable<int> DaysPaid { get; set; }
		public Nullable<int> RemainingDays { get; set; }
		public Nullable<System.DateTime> TodaysDate { get; set; }
		public Nullable<decimal> AmountPaidToday { get; set; }

		public string CustomerName { get; set; }
		public Nullable<decimal> MobileNo { get; set; }
		public string EmailId { get; set; }
		public string Address { get; set; }
		public string AdhaarNo { get; set; }
		public string PanNo { get; set; }
	}
}