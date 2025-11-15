using ERP_Funds.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
    public class DailyEntryController : Controller
    {
		// GET: DailyEntry
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

		[HttpGet]
		public ActionResult getLoanById(int CustomerId)
		{
			return Json(dailyEntryDAL.getLoanById(CustomerId), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult getLoanSummaryById(int CustomerId, int LoanNoId)
		{
			return Json(dailyEntryDAL.getLoanSummaryById(CustomerId, LoanNoId), JsonRequestBehavior.AllowGet);
		}

	}
}