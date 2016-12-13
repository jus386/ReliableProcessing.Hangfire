using System.Web.Mvc;
using Processing;

namespace WebApp.Controllers
{
    public class SignalRSampleController : Controller
    {
        private readonly CountingProcessor _countingProcessor;
        private readonly CheckProcessor _checkProcessor;

        public SignalRSampleController(CountingProcessor countProcessor, CheckProcessor checkProcessor)
        {
            _countingProcessor = countProcessor;
            _checkProcessor = checkProcessor;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CountTo10()
        {
            _countingProcessor.CountTo10Once();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult CountTo20()
        {
            _countingProcessor.CountTo20Once();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult CountToRandom()
        {
            _countingProcessor.CountToRandomOnce();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult CountTo10Then20ThenRandom()
        {
            _countingProcessor.CountTo10Then20ThenRandom();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult Count100Times()
        {
            for (int i = 0; i < 100; i++)
            {
                _countingProcessor.CountTo10Then20ThenRandom();
            }
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult Check20()
        {
            _checkProcessor.CheckIfCountedTo20();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult Check42()
        {
            _checkProcessor.CheckIfCountedTo42();
            return Json(string.Empty);
        }

        [HttpPost]
        public ActionResult CountTo10In5()
        {
            _countingProcessor.CountTo10In5();
            return Json(string.Empty);
        }

        [HttpPost]
        public ActionResult CountTo10In5ThanRandom()
        {
            _countingProcessor.CountTo10In5ThanRandom();
            return Json(string.Empty);
        }
    }
}