using System;
using System.Diagnostics;
using System.Threading;
using System.Web.Mvc;
using Hangfire;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FireAndForget()
        {
            BackgroundJob.Enqueue(() => DoSomething(3));
            return View("Index");
        }

        public ActionResult FireAfter10()
        {
            BackgroundJob.Schedule(() => DoSomething(4), TimeSpan.FromSeconds(10));
            return View("Index");
        }

        public ActionResult FireDependent()
        {
            var firstId = BackgroundJob.Enqueue(() => DoSomething(5));
            BackgroundJob.ContinueWith(firstId,() => DoSomething(6));

            return View("Index");
        }

        public ActionResult AddRemoveRecurring()
        {
            RecurringJob.AddOrUpdate(() => DoSomething(7), Cron.Minutely);
            return View("Index");
        }

        public void DoSomething(int amountWorkToDo)
        {
            if (DateTime.Now.Second%6 == 0)
            {
                throw new Exception("Random error here and there.");
            }

            Thread.Sleep(TimeSpan.FromSeconds(amountWorkToDo));
            Debug.WriteLine("Amount of work={0}, date={1}, threadId={2}", amountWorkToDo, DateTime.Now, Thread.CurrentThread.ManagedThreadId);
        }
    }
}