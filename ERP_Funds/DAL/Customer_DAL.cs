using ERP_Funds.Models.DataModel;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ERP_Funds.DAL
{
	public class Customer_DAL
	{
		db_FundsEntities db = new db_FundsEntities();

		public string AddData(VMCustomer vMCustomer)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					tblCustomer customer = db.tblCustomers
						.FirstOrDefault(x=>x.C_Id ==  vMCustomer.C_Id);

					if (customer == null)
					{
						customer = new tblCustomer
						{
						    C_Id = vMCustomer.C_Id,
						    CustomerName = vMCustomer.CustomerName,
							MobileNo = vMCustomer.MobileNo,
							EmailId = vMCustomer.EmailId,
							Address = vMCustomer.Address,
							PanNo = vMCustomer.PanNo,
							AdhaarNo = vMCustomer.AdhaarNo,
							CreatedBy = HttpContext.Current.Session["UserName"] as string,
							CreatedDate = DateTime.Now ,
							IsActive = true
						};
						db.tblCustomers.Add(customer);
						db.SaveChanges();
						transaction.Commit();
						return "Customer Added Successfully";
					}
					else
					{
						customer.C_Id = vMCustomer.C_Id;
						customer.CustomerName = vMCustomer.CustomerName;
						customer.MobileNo = vMCustomer.MobileNo;
						customer.EmailId = vMCustomer.EmailId;
						customer.Address = vMCustomer.Address;
						customer.PanNo = vMCustomer.PanNo;
						customer.AdhaarNo = vMCustomer.AdhaarNo;
						customer.IsActive = true;
						customer.ModifiedBy = HttpContext.Current.Session["UserName"] as string;
						customer.ModifiedDate = DateTime.Now;
						db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
						transaction.Commit();
						return "Customer Updated Successfully";
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

		public List<VMCustomer> GetList()
		{ 
		   List<VMCustomer> vMCustomers = new List<VMCustomer>();

			try
			{
			   vMCustomers = (from cust in db.tblCustomers
							  where cust.IsActive == true
							  orderby cust.C_Id descending
							  select new VMCustomer
							 {
								 C_Id = cust.C_Id,
								 CustomerName = cust.CustomerName,
								 MobileNo = cust.MobileNo,
								 EmailId = cust.EmailId,
								 Address = cust.Address,
								 PanNo = cust.PanNo,
								 AdhaarNo = cust.AdhaarNo,
								 CreatedBy = cust.CreatedBy,
								 CreatedDate = cust.CreatedDate,
								 ModifiedBy = cust.ModifiedBy,
								 ModifiedDate = cust.ModifiedDate,
								 IsActive = cust.IsActive
							 }).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error" + ex.Message);
			}
			return vMCustomers;
		}

		public VMCustomer GetById(int C_Id)
		{ 
		 VMCustomer vMCustomer = new VMCustomer();
			try
			{
				vMCustomer = (from cust in db.tblCustomers
							  where cust.C_Id == C_Id && cust.IsActive == true
							  select new VMCustomer
							  {
								  C_Id = cust.C_Id,
								  CustomerName = cust.CustomerName,
								  MobileNo = cust.MobileNo,
								  EmailId = cust.EmailId,
								  Address = cust.Address,
								  PanNo = cust.PanNo,
								  AdhaarNo = cust.AdhaarNo,
								  CreatedBy = cust.CreatedBy,
								  CreatedDate = cust.CreatedDate,
								  ModifiedBy = cust.ModifiedBy,
								  ModifiedDate = cust.ModifiedDate,
								  IsActive = cust.IsActive
							  }).FirstOrDefault();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error" + ex.Message);
			}
			return vMCustomer;
		}

		public string DeleteById(int C_Id)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					var customer = db.tblCustomers
						.FirstOrDefault(x => x.C_Id == C_Id);
					if (customer != null)
					{
						customer.IsActive = false;
						db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
						transaction.Commit();
						return "Customer Deleted Successfully";
					}
					else
					{
						return "Customer not found";
					}
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					var errorMessage = ex.InnerException?.InnerException?.Message ?? ex.Message;
					return "Error: " + errorMessage;
				}
			}
		}




	}
}