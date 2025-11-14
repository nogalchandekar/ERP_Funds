using ERP_Funds.DAL;
using ERP_Funds.Models.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
    public class ExcelReportController : Controller
    {
        ExcelReport_DAL reportDAL = new ExcelReport_DAL();

        // GET: ExcelReport
        public ActionResult Index()
        {
            // Get active customer list for dropdown
            var customerList = reportDAL.GetCustomerList();

            if (customerList != null && customerList.Count > 0)
                ViewBag.CustomerList = new SelectList(customerList, "C_Id", "CustomerName");
            else
                ViewBag.CustomerList = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "-- No Active Customers Found --" }
                }, "Value", "Text");

            return View();
        }
    }
}
