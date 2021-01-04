using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace testThreadAlongMainWebTread
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static System.Timers.Timer timer = new System.Timers.Timer();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.Start();
            
        }
        public void WorkThreadFunction()
        {
            try
            {
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 10000;
                timer.Start();
            }
            catch (Exception ex)
            {
                // log errors
            }
        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                testThreadAlongMainWebTread.Models.ChatHub ch = new Models.ChatHub();
                ch.SendAll(DateTime.Now);
            }
            catch (Exception err)
            {

                // log errors
            }

        }
    }
}
