using ERP_Funds.Models.DataModel;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace ERP_Funds.DAL
{
	public class ExcelReport_DAL
	{
		db_FundsEntities db = new db_FundsEntities();

		public List<VMCustomer> GetCustomerList()
		{
			List<VMCustomer> vMCustomers = new List<VMCustomer>();
			try
			{
				vMCustomers = (from cust in db.tblCustomers
							   where cust.IsActive == true
							   orderby cust.CustomerName
							   select new VMCustomer
							   {
								   C_Id = cust.C_Id,
								   CustomerName = cust.CustomerName
							   }).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error while fetching customers: " + ex.Message);
			}
			return vMCustomers;
		}

		public List<VMExcelReport> getExcelList(int? customerId = null)
		{
			List<VMExcelReport> list = new List<VMExcelReport>();

			try
			{
				var query =
					from loan in db.tblLoans
					join cust in db.tblCustomers on loan.CustomerId equals cust.C_Id
					where loan.IsActive == true
					   && (customerId == null || loan.CustomerId == customerId)
					orderby loan.LoanId descending
					select new VMExcelReport
					{
						LoanId = loan.LoanId,
						CustomerId = loan.CustomerId,
						CustomerName = cust.CustomerName,
						MobileNo = cust.MobileNo,
						Address = cust.Address,
						AdhaarNo = cust.AdhaarNo,
						PanNo = cust.PanNo,

						LoanNo = loan.LoanNo,
						LoanAmount = loan.LoanAmount,
						LoanDurationDays = loan.LoanDurationDays,
						LoanInterest = loan.LoanInterest,
						DeductAmount = loan.DeductAmount,
						AmountGivenToCustomer = loan.AmountGivenToCustomer,
						LoanDurationWithMonths = loan.LoanDurationWithMonths,
						DailyReturn = loan.DailyReturn,
						TotalPayable = loan.TotalPayable,

						// Status logic
						//Status = loan.TotalPayable == loan.AmountGivenToCustomer ? "Completed" : "Pending"
					};

				list = query.ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			return list;
		}

		public List<VMExcelReport> getExcelReport(int? CustomerId = null)
		{
			List<VMExcelReport> vmexcellist = new List<VMExcelReport>();

			try
			{
				var query = from a in db.tblLoans
							join b in db.tblDailyCollections on a.CustomerId equals b.CustomerId
							join c in db.tblCustomers on a.CustomerId equals c.C_Id
							where a.IsActive == true
							   && (CustomerId == null || a.CustomerId == CustomerId)
							orderby a.LoanId descending
							select new VMExcelReport
							{
								LoanId = a.LoanId,
								CustomerId = a.CustomerId,
								CustomerName = c.CustomerName,
								MobileNo = c.MobileNo,
								Address = c.Address,
								AdhaarNo = c.AdhaarNo,
								PanNo = c.PanNo,
								LoanAmount = a.LoanAmount,
								LoanDurationDays = a.LoanDurationDays,
								LoanInterest = a.LoanInterest,
								DeductAmount = a.DeductAmount,
								AmountGivenToCustomer = a.AmountGivenToCustomer,
								LoanDurationWithMonths = a.LoanDurationWithMonths,
								DailyReturn = a.DailyReturn,
								TotalPayable = a.TotalPayable,
								LoanNo = a.LoanNo,
								LoanNoId = b.LoanNoId,
								DailyCollectionId = b.DailyCollectionId,
								LoanDuration = b.LoanDuration,
								PerDayInstallment = b.PerDayInstallment,
								TotalPaid = b.TotalPaid,
								PendingAmount = b.PendingAmount,
								DaysPaid = b.DaysPaid,
								RemainingDays = b.RemainingDays,
								TodaysDate = b.TodaysDate,
								AmountPaidToday = b.AmountPaidToday
							};

				vmexcellist = query.ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			return vmexcellist;
		}


	}
}
