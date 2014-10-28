using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using WebUI.App_Start;
using WebUI.DataAccessLayer;

namespace WebUI.Helpers
{

    public interface ICustomController
    {

        ApplicationUserManager getUserManager();
        ApplicationDbContext getDbContext();
    }

    public class CustomControllerImplementation : Controller, ICustomController
    {

        private ApplicationDbContext _dbContext;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public ApplicationDbContext BetDatabase
        {
            get { return _dbContext ?? (_dbContext = new ApplicationDbContext()); }
            set
            {
                _dbContext = value;
            }
        }



        public ApplicationUserManager getUserManager()
        {
            return UserManager;
        }

        public ApplicationDbContext getDbContext()
        {
            return BetDatabase;
        }
    }

    public class CustomController : Controller
    {
        private ApplicationDbContext _dbContext;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager!=null?_userManager: HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public ApplicationDbContext BetDatabase
        {
            get { return _dbContext ?? (_dbContext = new ApplicationDbContext()); }
            set
            {
                _dbContext = value;
            }
        }
    }
}