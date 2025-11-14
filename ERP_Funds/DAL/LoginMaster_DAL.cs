using ERP_Funds.Models.DataModel;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_Funds.DAL
{
    public class LoginMaster_DAL
    {
        db_FundsEntities db = new db_FundsEntities();
        public VMLoginMaster CheckAdminLogin(VMLoginMaster vmtblAdminrequest)
        {
            VMLoginMaster vmloginmaster = new VMLoginMaster();

            try
            {
                var login = db.tblLoginMasters
                              .AsEnumerable() // brings data to memory for case-sensitive check
                              .Where(x => string.Equals(x.UserName, vmtblAdminrequest.UserName, StringComparison.Ordinal) &&
                                          string.Equals(x.Password, vmtblAdminrequest.Password, StringComparison.Ordinal) &&
                                          x.IsActive == true)
                              .FirstOrDefault();

                if (login != null)
                {
                    vmloginmaster = new VMLoginMaster
                    {
                        Login_Id = login.Login_Id,
                        UserName = login.UserName,
                        Password = login.Password,
                        IsActive = true
                    };
                }
                else
                {
                    vmloginmaster.IsActive = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                vmloginmaster.IsActive = false;
            }

            return vmloginmaster;
        }
    }
}