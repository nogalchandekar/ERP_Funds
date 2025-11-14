using ERP_Funds.DAL;
using ERP_Funds.Models.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
    public class DailyEntryController : Controller
    {
        DailyEntry_DAL dailyEntryDAL = new DailyEntry_DAL();

        // GET: DailyEntry
        public ActionResult Index()
        {
            // Fetch customer list from DAL
            var customerList = dailyEntryDAL.GetCustomerList();

            // Pass list to dropdown
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
