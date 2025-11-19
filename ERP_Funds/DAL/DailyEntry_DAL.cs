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


		//public List<VMLoan> getLoanSummaryById(int CustomerId, int LoanNoId)
		//{
		//	List<VMLoan> vMLoans = new List<VMLoan>();
		//	try
		//	{
		//		vMLoans = (from loan in db.tblLoans
		//				   where loan.IsActive == true && loan.CustomerId == CustomerId && loan.LoanId == LoanNoId
		//				   orderby loan.LoanId descending
		//				   select new VMLoan
		//				   {
		//					   LoanId = loan.LoanId,
		//					   CustomerId = loan.CustomerId,
		//					   LoanAmount = loan.LoanAmount,
		//					   LoanDurationDays = loan.LoanDurationDays,
		//					   LoanInterest = loan.LoanInterest,
		//					   DeductAmount = loan.DeductAmount,
		//					   AmountGivenToCustomer = loan.AmountGivenToCustomer,
		//					   LoanDurationWithMonths = loan.LoanDurationWithMonths,
		//					   DailyReturn = loan.DailyReturn,
		//					   TotalPayable = loan.TotalPayable,
		//					   LoanNo = loan.LoanNo
		//				   }).ToList();
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine("Error while fetching loans: " + ex.Message);
		//	}
		//	return vMLoans;
		//}

		// inside DailyEntry_DAL class (replace existing AddDailyEntry, getLoanSummaryById, GetDailyCollectionsList)

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
							   // Ensure VMLoan has TotalPaid and PendingAmount properties (added below)
						   }).ToList();

				// Compute totals from tblDailyCollections for this loan
				foreach (var v in vMLoans)
				{
					var sums = db.tblDailyCollections
								 .Where(d => d.IsActive == true && d.LoanNoId == v.LoanId)
								 .ToList();

					decimal totalPaid = sums.Sum(x => x.AmountPaidToday ?? 0);
					v.TotalPaid = totalPaid;
					v.PendingAmount = (v.LoanAmount ?? 0) - totalPaid;

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error while fetching loans: " + ex.Message);
			}
			return vMLoans;
		}




		//public string AddDailyEntry(VMDailyCollection vMDaily)
		//{
		//	using (var transaction = db.Database.BeginTransaction())
		//	{
		//		try
		//		{
		//			tblDailyCollection dailyCollection = db.tblDailyCollections.
		//								FirstOrDefault(x => x.DailyCollectionId == vMDaily.DailyCollectionId);

		//			if (dailyCollection == null)
		//			{
		//				dailyCollection = new tblDailyCollection
		//				{
		//					DailyCollectionId = vMDaily.DailyCollectionId,
		//					CustomerId = vMDaily.CustomerId,
		//					LoanNoId = vMDaily.LoanNoId,
		//					LoanAmount = vMDaily.LoanAmount,
		//					LoanDuration = vMDaily.LoanDuration,
		//					PerDayInstallment = vMDaily.PerDayInstallment,
		//					TotalPaid = vMDaily.TotalPaid,
		//					PendingAmount = vMDaily.PendingAmount,
		//					DaysPaid = vMDaily.DaysPaid,
		//					RemainingDays = vMDaily.RemainingDays,
		//					TodaysDate = vMDaily.TodaysDate,
		//					AmountPaidToday = vMDaily.AmountPaidToday,
		//					IsActive = true,
		//					CreatedBy = HttpContext.Current.Session["UserName"] as string,
		//					CreatedDate = DateTime.Now
		//				};
		//				db.tblDailyCollections.Add(dailyCollection);
		//				db.SaveChanges();
		//				transaction.Commit();
		//				return "Daily Collections Added Successfully";
		//			}
		//			else
		//			{
		//				dailyCollection.CustomerId = vMDaily.CustomerId;
		//				dailyCollection.LoanNoId = vMDaily.LoanNoId;
		//				dailyCollection.LoanAmount = vMDaily.LoanAmount;
		//				dailyCollection.LoanDuration = vMDaily.LoanDuration;
		//				dailyCollection.PerDayInstallment = vMDaily.PerDayInstallment;
		//				dailyCollection.TotalPaid = vMDaily.TotalPaid;
		//				dailyCollection.PendingAmount = vMDaily.PendingAmount;
		//				dailyCollection.DaysPaid = vMDaily.DaysPaid;
		//				dailyCollection.RemainingDays = vMDaily.RemainingDays;
		//				dailyCollection.TodaysDate = vMDaily.TodaysDate;
		//				dailyCollection.AmountPaidToday = vMDaily.AmountPaidToday;
		//				dailyCollection.IsActive = true;
		//				dailyCollection.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
		//				dailyCollection.ModifiedDate = DateTime.Now;
		//				db.Entry(dailyCollection).State = System.Data.Entity.EntityState.Modified;
		//				db.SaveChanges();
		//				transaction.Commit();
		//				return "Daily Collections Updated Successfully";
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			transaction.Rollback();
		//			Console.WriteLine("Error" + ex.Message);
		//			var errorMessage = ex.InnerException?.InnerException?.Message ?? ex.Message;
		//			return "Error: " + errorMessage;
		//		}
		//	}

		//}
		//public string AddDailyEntry(VMDailyCollection vMDaily)
		//{
		//	using (var transaction = db.Database.BeginTransaction())
		//	{
		//		try
		//		{
		//			// Normalize incoming values
		//			int loanId = vMDaily.LoanNoId ?? 0;
		//			int customerId = vMDaily.CustomerId ?? 0;
		//			decimal newAmount = vMDaily.AmountPaidToday ?? 0;

		//			// Find existing record (edit) or null (insert)
		//			tblDailyCollection existing = db.tblDailyCollections
		//				.FirstOrDefault(x => x.DailyCollectionId == vMDaily.DailyCollectionId);

		//			// Get all active rows of this loan
		//			var existingRows = db.tblDailyCollections
		//								 .Where(d => d.IsActive == true && d.LoanNoId == loanId)
		//								 .ToList();

		//			// SUM excluding current record (if update)
		//			decimal sumExcludingThis;
		//			if (existing != null)
		//			{
		//				sumExcludingThis = existingRows
		//					.Where(x => x.DailyCollectionId != existing.DailyCollectionId)
		//					.Sum(x => x.AmountPaidToday ?? 0);
		//			}
		//			else
		//			{
		//				sumExcludingThis = existingRows.Sum(x => x.AmountPaidToday ?? 0);
		//			}

		//			// TOTAL PAID INCLUDING NEW ENTRY
		//			decimal cumulativeTotalPaid = sumExcludingThis + newAmount;

		//			// Fetch loan
		//			var loan = db.tblLoans.FirstOrDefault(l => l.LoanId == loanId);

		//			decimal loanAmount = loan?.LoanAmount ?? 0;
		//			decimal pending = loanAmount - cumulativeTotalPaid;
		//			if (pending < 0) pending = 0;

		//			// Compute Distinct Days Paid
		//			var distinctDates = existingRows
		//				.Where(x => existing == null || x.DailyCollectionId != existing.DailyCollectionId)
		//				.Select(x => x.TodaysDate.HasValue ? x.TodaysDate.Value.Date : (DateTime?)null)
		//				.Where(d => d != null)
		//				.Select(d => d.Value)
		//				.Distinct()
		//				.ToList();

		//			// include new date
		//			DateTime incomingDate = vMDaily.TodaysDate?.Date ?? DateTime.Now.Date;
		//			if (!distinctDates.Contains(incomingDate))
		//				distinctDates.Add(incomingDate);

		//			int daysPaid = distinctDates.Count;

		//			// Remaining Days calculation
		//			int loanDuration = loan?.LoanDurationDays ?? 0;
		//			int remainingDays = Math.Max(loanDuration - daysPaid, 0);

		//			// ---------------------- INSERT -----------------------
		//			if (existing == null)
		//			{
		//				tblDailyCollection dailyCollection = new tblDailyCollection
		//				{
		//					CustomerId = customerId,
		//					LoanNoId = loanId,
		//					LoanAmount = loanAmount,
		//					LoanDuration = vMDaily.LoanDuration ?? 0,
		//					PerDayInstallment = vMDaily.PerDayInstallment ?? 0,
		//					TotalPaid = cumulativeTotalPaid,
		//					PendingAmount = pending,
		//					DaysPaid = daysPaid,
		//					RemainingDays = remainingDays,
		//					TodaysDate = incomingDate,
		//					AmountPaidToday = newAmount,
		//					IsActive = true,
		//					CreatedBy = HttpContext.Current.Session["UserName"] as string,
		//					CreatedDate = DateTime.Now
		//				};

		//				db.tblDailyCollections.Add(dailyCollection);
		//				db.SaveChanges();
		//				transaction.Commit();
		//				return "Daily Collections Added Successfully";
		//			}

		//			// ---------------------- UPDATE -----------------------
		//			existing.CustomerId = customerId;
		//			existing.LoanNoId = loanId;
		//			existing.LoanAmount = loanAmount;
		//			existing.LoanDuration = vMDaily.LoanDuration ?? 0;
		//			existing.PerDayInstallment = vMDaily.PerDayInstallment ?? 0;
		//			existing.AmountPaidToday = newAmount;
		//			existing.TotalPaid = cumulativeTotalPaid;
		//			existing.PendingAmount = pending;
		//			existing.DaysPaid = daysPaid;
		//			existing.RemainingDays = remainingDays;
		//			existing.TodaysDate = incomingDate;
		//			existing.IsActive = true;
		//			existing.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
		//			existing.ModifiedDate = DateTime.Now;

		//			db.Entry(existing).State = System.Data.Entity.EntityState.Modified;
		//			db.SaveChanges();

		//			transaction.Commit();
		//			return "Daily Collections Updated Successfully";
		//		}
		//		catch (Exception ex)
		//		{
		//			transaction.Rollback();
		//			var errorMessage = ex.InnerException?.InnerException?.Message ?? ex.Message;
		//			return "Error: " + errorMessage;
		//		}
		//	}
		//}


		public string AddDailyEntry(VMDailyCollection vMDaily)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Normalize incoming values
					int loanId = vMDaily.LoanNoId ?? 0;
					int customerId = vMDaily.CustomerId ?? 0;
					decimal newAmount = vMDaily.AmountPaidToday ?? 0;

					// Find existing record
					var existing = db.tblDailyCollections
									 .FirstOrDefault(x => x.DailyCollectionId == vMDaily.DailyCollectionId);

					// Fetch all old entries of this loan
					var existingRows = db.tblDailyCollections
										 .Where(x => x.IsActive == true && x.LoanNoId == loanId)
										 .ToList();

					// Sum excluding the entry in case of update
					decimal sumExcludingThis =
						(existing != null)
						? existingRows.Where(x => x.DailyCollectionId != existing.DailyCollectionId)
									  .Sum(x => x.AmountPaidToday ?? 0)
						: existingRows.Sum(x => x.AmountPaidToday ?? 0);

					// Total paid including new entry
					decimal cumulativeTotalPaid = sumExcludingThis + newAmount;

					// Fetch loan info
					var loan = db.tblLoans.FirstOrDefault(l => l.LoanId == loanId);
					decimal loanAmount = loan?.LoanAmount ?? 0;

					// Pending amount calculation
					decimal pending = loanAmount - cumulativeTotalPaid;
					if (pending < 0) pending = 0;

					// Distinct Days Paid calculation
					var distinctDates = existingRows
						.Where(x => existing == null || x.DailyCollectionId != existing.DailyCollectionId)
						.Select(x => x.TodaysDate.HasValue ? x.TodaysDate.Value.Date : (DateTime?)null)
						.Where(d => d != null)
						.Select(d => d.Value)
						.ToList();

					// Include new date
					DateTime incomingDate = vMDaily.TodaysDate?.Date ?? DateTime.Now.Date;
					if (!distinctDates.Contains(incomingDate))
						distinctDates.Add(incomingDate);

					int daysPaid = distinctDates.Count;

					// Correct Remaining Days
					int duration = loan?.LoanDurationDays ?? 0;
					int remainingDays = Math.Max(duration - daysPaid, 0);


					// ---------------- NEW ENTRY -----------------
					if (existing == null)
					{
						var daily = new tblDailyCollection
						{
							CustomerId = customerId,
							LoanNoId = loanId,
							LoanAmount = loanAmount,
							LoanDuration = vMDaily.LoanDuration ?? 0,
							PerDayInstallment = vMDaily.PerDayInstallment ?? 0,
							TotalPaid = cumulativeTotalPaid,
							PendingAmount = pending,
							DaysPaid = daysPaid,
							RemainingDays = remainingDays,
							TodaysDate = incomingDate,
							AmountPaidToday = newAmount,
							IsActive = true,
							CreatedBy = HttpContext.Current.Session["UserName"] as string,
							CreatedDate = DateTime.Now
						};

						db.tblDailyCollections.Add(daily);
						db.SaveChanges();
						transaction.Commit();
						return "Daily Collections Added Successfully";
					}

					// ---------------- UPDATE ENTRY -----------------
					existing.CustomerId = customerId;
					existing.LoanNoId = loanId;
					existing.LoanAmount = loanAmount;
					existing.LoanDuration = vMDaily.LoanDuration ?? 0;
					existing.PerDayInstallment = vMDaily.PerDayInstallment ?? 0;
					existing.AmountPaidToday = newAmount;
					existing.TotalPaid = cumulativeTotalPaid;
					existing.PendingAmount = pending;
					existing.DaysPaid = daysPaid;
					existing.RemainingDays = remainingDays;
					existing.TodaysDate = incomingDate;
					existing.IsActive = true;
					existing.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
					existing.ModifiedDate = DateTime.Now;

					db.Entry(existing).State = System.Data.Entity.EntityState.Modified;
					db.SaveChanges();
					transaction.Commit();

					return "Daily Collections Updated Successfully";
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					return "Error: " + (ex.InnerException?.InnerException?.Message ?? ex.Message);
				}
			}
		}






		//public List<VMDailyCollection> GetDailyCollectionsList(int customerId, int loanNoId)
		//{
		//	List<VMDailyCollection> vMDailyCollections = new List<VMDailyCollection>();
		//	try
		//	{
		//		vMDailyCollections = (from daily in db.tblDailyCollections
		//							  join c in db.tblCustomers on daily.CustomerId equals c.C_Id
		//							  join loan in db.tblLoans on daily.LoanNoId equals loan.LoanId
		//							  where daily.IsActive == true
		//								 && daily.CustomerId == customerId
		//								 && daily.LoanNoId == loanNoId
		//							  orderby daily.DailyCollectionId descending
		//							  select new VMDailyCollection
		//							  {
		//								  DailyCollectionId = daily.DailyCollectionId,
		//								  CustomerId = daily.CustomerId,
		//								  LoanNoId = daily.LoanNoId,
		//								  LoanAmount = daily.LoanAmount,
		//								  // <-- original loan duration (e.g. 80)
		//								  LoanDuration = loan.LoanDurationDays,
		//								  PerDayInstallment = daily.PerDayInstallment,
		//								  TotalPaid = daily.TotalPaid,
		//								  PendingAmount = daily.PendingAmount,
		//								  DaysPaid = daily.DaysPaid,               // will be overwritten by JS
		//								  RemainingDays = daily.RemainingDays,     // will be overwritten by JS
		//								  TodaysDate = daily.TodaysDate,
		//								  AmountPaidToday = daily.AmountPaidToday,
		//								  LoanNo = loan.LoanNo,
		//								  CustomerName = c.CustomerName
		//							  }).ToList();
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine("Error: " + ex.Message);
		//	}
		//	return vMDailyCollections;
		//}

		public List<VMDailyCollection> GetDailyCollectionsList(int customerId, int loanNoId)
{
    List<VMDailyCollection> vMDailyCollections = new List<VMDailyCollection>();
    try
    {
        // fetch ordered entries
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
                                  // use original loan duration
                                  LoanDuration = loan.LoanDurationDays,
                                  PerDayInstallment = daily.PerDayInstallment,
                                  TotalPaid = daily.TotalPaid,
                                  PendingAmount = daily.PendingAmount,
                                  DaysPaid = daily.DaysPaid,
                                  RemainingDays = daily.RemainingDays,
                                  TodaysDate = daily.TodaysDate,
                                  AmountPaidToday = daily.AmountPaidToday,
                                  LoanNo = loan.LoanNo,
                                  CustomerName = c.CustomerName
                              }).ToList();

        // If you want to ensure TotalPaid/PendingAmount are consistent across rows, 
        // you could additionally recalc them here (but AddDailyEntry already does).
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