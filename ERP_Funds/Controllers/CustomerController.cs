using ERP_Funds.DAL;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
    public class CustomerController : Controller
    {
		Customer_DAL dal = new Customer_DAL();

		// GET: Customer
		public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(VMCustomer vMCustomer)
        {
            return Json(dal.AddData(vMCustomer), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerList()
        {
            return Json(dal.GetList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CustomerById(int C_Id)
        {
            return Json(dal.GetById(C_Id), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult DeleteById(int C_Id)
		{
			return Json(dal.DeleteById(C_Id), JsonRequestBehavior.AllowGet);
		}

	}
}