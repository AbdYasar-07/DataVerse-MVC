using DataVerseLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace DataVerse_MVC.Controllers
{
    public class ApproveRejectController : Controller
    {

        private IOptions<Configuration.EntityConfiguration> _entityConfiguration;

        public ApproveRejectController(IOptions<Configuration.EntityConfiguration> entityConfiguration)
        {
            this._entityConfiguration = entityConfiguration;
        }

        // GET: ApproveRejectController
        public ActionResult Index(string recordId)
        {
            //c7f8bf48-c1fd-ec11-82e6-000d3af256af  
            if (!string.IsNullOrEmpty(recordId))
            {
                var accessToken = Crm.ConnectWithOAuth(_entityConfiguration.Value.Resource, _entityConfiguration.Value.ClientId,
                                                       _entityConfiguration.Value.ClientSecret, _entityConfiguration.Value.Authority);

                Crm.CallOut(_entityConfiguration.Value.Resource, recordId, accessToken,_entityConfiguration.Value.EntityInternalName,
                            _entityConfiguration.Value.Columns);

                return View();
            }
            ViewBag.name = "test name";
            ViewBag.age = 211;
            ViewBag.status = "test status";
            return View();
        }









    }





}
