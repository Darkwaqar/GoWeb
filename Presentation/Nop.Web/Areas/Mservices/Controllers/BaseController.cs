using System.Web.Mvc;
using Nop.Web.Controllers;

namespace Nop.Web.Areas.MServices.Controllers
{
    public class BaseController : BasePublicController
    {

        protected JsonResult View(object model, bool status = true, int messagecode = 200, string message = "")
        {

            var result = new
            {
                status = status,
                messageCode = messagecode,
                message = message,
                model = model
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult InvokeHttp400(string message = "", bool status = false, int messagecode = 400)
        {
            var result = new
            {
                status = status,
                messageCode = messagecode,
                message = message,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}