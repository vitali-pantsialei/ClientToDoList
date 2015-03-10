using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using ToDoManager.REST;

namespace ToDoManager
{
	// Note: For instructions on enabling IIS7 classic mode, 
	// visit http://go.microsoft.com/fwlink/?LinkId=301868
	/// <summary>
	/// The mvc application.
	/// </summary>
	public class MvcApplication : HttpApplication
	{
		#region Methods

		/// <summary>
		/// The application_ start.
		/// </summary>
		protected void Application_Start()
		{
			var razorEngine = ViewEngines.Engines.FirstOrDefault(x => x is RazorViewEngine);
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new ViewEngineSwitcher(razorEngine));

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		#endregion
	}
}