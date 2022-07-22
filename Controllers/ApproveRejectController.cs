using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DataVerse_MVC.Controllers
{
    public class ApproveRejectController : Controller
    {

        private IOptions<Configuration.EntityConfiguration> _entityConfiguration;
        private static string _recordId = "";

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
                _recordId = recordId;
                var accessToken = DataVerse.FetchToken(_entityConfiguration.Value.Resource, _entityConfiguration.Value.ClientId,
                                                  _entityConfiguration.Value.ClientSecret, _entityConfiguration.Value.Authority);
                if (accessToken != null)
                {
                    DataVerse.TableResponse response = DataVerse.CallOut(_entityConfiguration.Value.Resource, accessToken, recordId,
                                                  _entityConfiguration.Value.EntityInternalName, _entityConfiguration.Value.Columns);
                    if (response != null && response.isAccess.ToLower() == "true")
                    {
                        ViewBag.Message = "Your response has been already submitted";
                        return View("Approved");
                    }
                    else
                    {
                        ViewBag.isRecordId = true;
                        ViewBag.title = response.title;
                        ViewBag.caseNumber = response.ticketnumber;
                        ViewBag.casetypecode = response.casetypecode;
                        ViewBag.incidentId = response.incidentid;
                        ViewBag.owner = response.ownerid;
                        return View();
                    }
                }
            }
            ViewBag.isRecordId = false;
            return View();
        }

        [Route("/Approve")]
        public ActionResult approve()
        {
            var accessToken = DataVerse.FetchToken(_entityConfiguration.Value.Resource, _entityConfiguration.Value.ClientId,
                                                       _entityConfiguration.Value.ClientSecret, _entityConfiguration.Value.Authority);
            if (accessToken != null)
            {
                var isUpdated = DataVerse.UpdateField(true, _entityConfiguration.Value.Resource, accessToken, _entityConfiguration.Value.EntityInternalName, _recordId);
                if (isUpdated == true)
                {
                    ViewBag.Message = "Your response has been submitted to the system";
                    return View("Approved");
                }
            }
            ViewBag.Message = "Your request has been failed";
            ViewBag.isRecordId = "";
            return View("Approved");
        }

        [Route("/Reject")]
        public ActionResult reject()
        {
            var accessToken = DataVerse.FetchToken(_entityConfiguration.Value.Resource, _entityConfiguration.Value.ClientId,
                                                       _entityConfiguration.Value.ClientSecret, _entityConfiguration.Value.Authority);
            if (accessToken != null)
            {
                var isUpdated = DataVerse.UpdateField(false, _entityConfiguration.Value.Resource, accessToken, _entityConfiguration.Value.EntityInternalName, _recordId);
                if (isUpdated == true)
                {
                    ViewBag.Message = "Your response has been submitted to the system";
                    return View("Approved");
                }
            }
            ViewBag.Message = "Your request has been failed";
            ViewBag.isRecordId = "";
            return View("Approved");
        }
    }
}
