using ERP_Funds.Models.DataModel;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
