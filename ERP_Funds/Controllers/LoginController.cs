using ERP_Funds.DAL;
using ERP_Funds.Models.ViewModel;
using System;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
    public class LoginController : Controller
    {
        LoginMaster_DAL dal = new LoginMaster_DAL();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(VMLoginMaster model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                {
                    TempData["Error"] = "Please enter username and password.";
                    return View(model);
                }

                var loginData = dal.CheckAdminLogin(model);
                if (loginData != null && loginData.IsActive == true)

                {
                    // Set session variables
                    Session["Login_Id"] = loginData.Login_Id;
                    Session["UserName"] = loginData.UserName;
                    Session["IsActive"] = loginData.IsActive;

                    TempData["Success"] = "Login successful!";
                    return RedirectToAction("Index", "Customer"); // redirect to your dashboard page
                }
                else
                {
                    TempData["Error"] = "Invalid username or password.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong: " + ex.Message;
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            TempData["Success"] = "You have logged out successfully.";
            return RedirectToAction("Index", "Login");
        }
    }
}
