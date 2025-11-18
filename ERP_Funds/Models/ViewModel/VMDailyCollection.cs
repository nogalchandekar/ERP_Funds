using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.Models.ViewModel
{
	public class VMDailyCollection
	{
		public int DailyCollectionId { get; set; }
		public Nullable<int> CustomerId { get; set; }
		public Nullable<int> LoanNoId { get; set; }
		public Nullable<decimal> LoanAmount { get; set; }
		public Nullable<int> LoanDuration { get; set; }
		public Nullable<decimal> PerDayInstallment { get; set; }
		public Nullable<decimal> TotalPaid { get; set; }
		public Nullable<decimal> PendingAmount { get; set; }
		public Nullable<int> DaysPaid { get; set; }
		public Nullable<int> RemainingDays { get; set; }
		public Nullable<System.DateTime> TodaysDate { get; set; }
		public Nullable<decimal> AmountPaidToday { get; set; }
		public string LoanNo { get; set; }
		public string CustomerName { get; set; }

		public Nullable<bool> IsActive { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<System.DateTime> CreatedDate { get; set; }
		public string ModifiedBy { get; set; }
		public Nullable<System.DateTime> ModifiedDate { get; set; }

	}
}