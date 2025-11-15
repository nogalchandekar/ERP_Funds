using ERP_Funds.Models.DataModel;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.DAL
{
	public class DailyEntry_DAL
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
		public List<VMLoan> getLoanById(int CustomerId)
		{ 
		    List<VMLoan> vMLoans = new List<VMLoan>();
			try
			{
				vMLoans = (from loan in db.tblLoans
						   where loan.IsActive == true && loan.CustomerId == CustomerId
						   orderby loan.LoanId descending
						   select new VMLoan
						   {
							   LoanId = loan.LoanId,
							   CustomerId = loan.CustomerId,
							   LoanAmount = loan.LoanAmount,
							   LoanDurationDays = loan.LoanDurationDays,
							   LoanInterest = loan.LoanInterest,
							   DeductAmount = loan.DeductAmount,
							   AmountGivenToCustomer = loan.AmountGivenToCustomer,
							   LoanDurationWithMonths = loan.LoanDurationWithMonths,
							   DailyReturn = loan.DailyReturn,
							   TotalPayable = loan.TotalPayable,
							   LoanNo = loan.LoanNo
						   }).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error while fetching loans: " + ex.Message);
			}
			return vMLoans;
		}
		public List<VMLoan> getLoanSummaryById(int CustomerId, int LoanNoId)
		{
			List<VMLoan> vMLoans = new List<VMLoan>();
			try
			{
				vMLoans = (from loan in db.tblLoans
						   where loan.IsActive == true && loan.CustomerId == CustomerId	&& loan.LoanId == LoanNoId
						   orderby loan.LoanId descending
						   select new VMLoan
						   {
							   LoanId = loan.LoanId,
							   CustomerId = loan.CustomerId,
							   LoanAmount = loan.LoanAmount,
							   LoanDurationDays = loan.LoanDurationDays,
							   LoanInterest = loan.LoanInterest,
							   DeductAmount = loan.DeductAmount,
							   AmountGivenToCustomer = loan.AmountGivenToCustomer,
							   LoanDurationWithMonths = loan.LoanDurationWithMonths,
							   DailyReturn = loan.DailyReturn,
							   TotalPayable = loan.TotalPayable,
							   LoanNo = loan.LoanNo
						   }).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error while fetching loans: " + ex.Message);
			}
			return vMLoans;
		}







	}
}