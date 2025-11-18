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
						   where loan.IsActive == true && loan.CustomerId == CustomerId && loan.LoanId == LoanNoId
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
		public string AddDailyEntry(VMDailyCollection vMDaily)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					tblDailyCollection dailyCollection = db.tblDailyCollections.
										FirstOrDefault(x => x.DailyCollectionId == vMDaily.DailyCollectionId);

					if (dailyCollection == null)
					{
						dailyCollection = new tblDailyCollection
						{
							DailyCollectionId = vMDaily.DailyCollectionId,
							CustomerId = vMDaily.CustomerId,
							LoanNoId = vMDaily.LoanNoId,
							LoanAmount = vMDaily.LoanAmount,
							LoanDuration = vMDaily.LoanDuration,
							PerDayInstallment = vMDaily.PerDayInstallment,
							TotalPaid = vMDaily.TotalPaid,
							PendingAmount = vMDaily.PendingAmount,
							DaysPaid = vMDaily.DaysPaid,
							RemainingDays = vMDaily.RemainingDays,
							TodaysDate = vMDaily.TodaysDate,
							AmountPaidToday = vMDaily.AmountPaidToday,
							IsActive = true,
							CreatedBy = HttpContext.Current.Session["UserName"] as string,
							CreatedDate = DateTime.Now
						};
						db.tblDailyCollections.Add(dailyCollection);
						db.SaveChanges();
						transaction.Commit();
						return "Daily Collections Added Successfully";
					}
					else
					{
						dailyCollection.CustomerId = vMDaily.CustomerId;
						dailyCollection.LoanNoId = vMDaily.LoanNoId;
						dailyCollection.LoanAmount = vMDaily.LoanAmount;
						dailyCollection.LoanDuration = vMDaily.LoanDuration;
						dailyCollection.PerDayInstallment = vMDaily.PerDayInstallment;
						dailyCollection.TotalPaid = vMDaily.TotalPaid;
						dailyCollection.PendingAmount = vMDaily.PendingAmount;
						dailyCollection.DaysPaid = vMDaily.DaysPaid;
						dailyCollection.RemainingDays = vMDaily.RemainingDays;
						dailyCollection.TodaysDate = vMDaily.TodaysDate;
						dailyCollection.AmountPaidToday = vMDaily.AmountPaidToday;
						dailyCollection.IsActive = true;
						dailyCollection.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
						dailyCollection.ModifiedDate = DateTime.Now;
						db.Entry(dailyCollection).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
						transaction.Commit();
						return "Daily Collections Updated Successfully";
					}
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					Console.WriteLine("Error" + ex.Message);
					var errorMessage = ex.InnerException?.InnerException?.Message ?? ex.Message;
					return "Error: " + errorMessage;
				}
			}

		}
		public List<VMDailyCollection> GetDailyCollectionsList(int customerId, int loanNoId)
		{
			List<VMDailyCollection> vMDailyCollections = new List<VMDailyCollection>();
			try
			{
				vMDailyCollections = (from daily in db.tblDailyCollections
									  join c in db.tblCustomers on daily.CustomerId equals c.C_Id
									  join loan in db.tblLoans on daily.LoanNoId equals loan.LoanId
									  where daily.IsActive == true
										 && daily.CustomerId == customerId
										 && daily.LoanNoId == loanNoId
									  orderby daily.DailyCollectionId descending
									  select new VMDailyCollection
									  {
										  DailyCollectionId = daily.DailyCollectionId,
										  CustomerId = daily.CustomerId,
										  LoanNoId = daily.LoanNoId,
										  LoanAmount = daily.LoanAmount,
										  // <-- original loan duration (e.g. 80)
										  LoanDuration = loan.LoanDurationDays,
										  PerDayInstallment = daily.PerDayInstallment,
										  TotalPaid = daily.TotalPaid,
										  PendingAmount = daily.PendingAmount,
										  DaysPaid = daily.DaysPaid,               // will be overwritten by JS
										  RemainingDays = daily.RemainingDays,     // will be overwritten by JS
										  TodaysDate = daily.TodaysDate,
										  AmountPaidToday = daily.AmountPaidToday,
										  LoanNo = loan.LoanNo,
										  CustomerName = c.CustomerName
									  }).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
			return vMDailyCollections;
		}
		public VMDailyCollection GetDailyCollectionById(int DailyCollectionId)
		{
			VMDailyCollection vMDailyCollection = new VMDailyCollection();
			try
			{
				vMDailyCollection = (from daily in db.tblDailyCollections
									 where daily.IsActive == true && daily.DailyCollectionId == DailyCollectionId
									 select new VMDailyCollection
									 {
										 DailyCollectionId = daily.DailyCollectionId,
										 CustomerId = daily.CustomerId,
										 LoanNoId = daily.LoanNoId,
										 LoanAmount = daily.LoanAmount,
										 LoanDuration = daily.LoanDuration,
										 PerDayInstallment = daily.PerDayInstallment,
										 TotalPaid = daily.TotalPaid,
										 PendingAmount = daily.PendingAmount,
										 DaysPaid = daily.DaysPaid,
										 RemainingDays = daily.RemainingDays,
										 TodaysDate = daily.TodaysDate,
										 AmountPaidToday = daily.AmountPaidToday
									 }).FirstOrDefault();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error" + ex.Message);
			}
			return vMDailyCollection;


		}
		public string DeleteDailyCollection(int DailyCollectionId)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					tblDailyCollection dailyCollection = db.tblDailyCollections
						.FirstOrDefault(x => x.DailyCollectionId == DailyCollectionId);
					if (dailyCollection != null)
					{
						dailyCollection.IsActive = false;
						dailyCollection.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
						dailyCollection.ModifiedDate = DateTime.Now;
						db.Entry(dailyCollection).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
						transaction.Commit();
						return "Daily Collection Deleted Successfully";
					}
					else
					{
						return "Daily Collection Not Found";
					}
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					Console.WriteLine("Error" + ex.Message);
					var errorMessage = ex.InnerException?.InnerException?.Message ?? ex.Message;
					return "Error: " + errorMessage;
				}
			}
		}
	}
}