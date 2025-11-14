using ERP_Funds.Models.DataModel;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.DAL
{
    public class Loan_DAL
    {
        db_FundsEntities db = new db_FundsEntities();

        public string AddData(VMLoan vMLoan)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    tblLoan tblLoan = db.tblLoans
                        .FirstOrDefault(x => x.LoanId == vMLoan.LoanId);

					if (tblLoan == null)
					{
                        tblLoan = new tblLoan
                        {
							LoanId = vMLoan.LoanId,
							CustomerId = vMLoan.CustomerId,
							LoanAmount = vMLoan.LoanAmount,
							LoanDurationDays = vMLoan.LoanDurationDays,
							LoanInterest = vMLoan.LoanInterest,
							DeductAmount = vMLoan.DeductAmount,
							AmountGivenToCustomer = vMLoan.AmountGivenToCustomer,
							LoanDurationWithMonths = vMLoan.LoanDurationWithMonths,
							DailyReturn = vMLoan.DailyReturn,
							TotalPayable = vMLoan.TotalPayable,
							IsActive = true,
							CreatedBy = HttpContext.Current.Session["UserName"] as string,
                            CreatedDate = DateTime.Now,
						};
                        db.tblLoans.Add(tblLoan);
                        db.SaveChanges();
                        transaction.Commit();
                        return "Loan Details Added Successfully";
					}
                    else
                    {
                        tblLoan.CustomerId = vMLoan.CustomerId;
                        tblLoan.LoanAmount = vMLoan.LoanAmount; 
                        tblLoan.LoanDurationDays = vMLoan.LoanDurationDays;
                        tblLoan.LoanInterest = vMLoan.LoanInterest;
                        tblLoan.DeductAmount = vMLoan.DeductAmount;
                        tblLoan.AmountGivenToCustomer = vMLoan.AmountGivenToCustomer;
                        tblLoan.LoanDurationWithMonths = vMLoan.LoanDurationWithMonths;
                        tblLoan.DailyReturn = vMLoan.DailyReturn;
                        tblLoan.TotalPayable = vMLoan.TotalPayable;
                        tblLoan.IsActive = true;
                        tblLoan.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
                        tblLoan.ModifiedDate = DateTime.Now;
                        db.Entry(tblLoan).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        transaction.Commit();
                        return "Loan Details Updated Successfully";
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

        public List<VMLoan> getList()
        {
            List<VMLoan> vMLoans = new List<VMLoan>();

			try
            {
              vMLoans = (from loan in db.tblLoans
                         join cust in db.tblCustomers on loan.CustomerId equals cust.C_Id
						 where loan.IsActive == true
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
							   //============= Customer Table Fields =================///
							   CustomerName = cust.CustomerName
						   }).ToList();
			}
            catch (Exception ex)
            {
				Console.WriteLine("Error" + ex.Message);
			}
            return vMLoans;
		}

        public VMLoan GetLoanById(int loanId)
        {
            VMLoan vMLoan = new VMLoan();
            try
            {
                vMLoan = (from loan in db.tblLoans
                           where loan.LoanId == loanId && loan.IsActive == true
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
                           }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching loan by ID: " + ex.Message);
            }
            return vMLoan;
		}

        public string DeleteLoanById(int loanId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    tblLoan tblLoan = db.tblLoans
                        .FirstOrDefault(x => x.LoanId == loanId);
                    if (tblLoan != null)
                    {
                        tblLoan.IsActive = false;
                        tblLoan.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
                        tblLoan.ModifiedDate = DateTime.Now;
                        db.Entry(tblLoan).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        transaction.Commit();
                        return "Loan Deleted Successfully";
                    }
                    else
                    {
                        return "Loan Not Found";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error while deleting loan: " + ex.Message);
                    return "Error: " + ex.Message;
                }
            }
		}

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
    }
}
