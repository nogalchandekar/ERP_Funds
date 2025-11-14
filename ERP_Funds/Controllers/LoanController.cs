using ERP_Funds.DAL;
using ERP_Funds.Models.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
    public class LoanController : Controller
    {
        Loan_DAL loanDAL = new Loan_DAL();


        [HttpPost]
        public ActionResult AddLoan(VMLoan vMLoan)
        { 
          return Json(loanDAL.AddData(vMLoan), JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public ActionResult LoanList()
        {
            return Json(loanDAL.getList(), JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public ActionResult GetLoanById(int LoanId)
        { 
             return Json(loanDAL.GetLoanById(LoanId), JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        public ActionResult DeleteLoanById(int LoanId)
        {
            return Json(loanDAL.DeleteLoanById(LoanId), JsonRequestBehavior.AllowGet);
		}

		// GET: Loan
		public ActionResult Index()
        {
            var customerList = loanDAL.GetCustomerList();

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
