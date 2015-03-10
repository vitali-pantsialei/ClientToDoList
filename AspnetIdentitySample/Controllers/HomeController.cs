using System.Web.Mvc;

namespace ToDoManager.Controllers
{
	/// <summary>
	/// The home controller.
	/// </summary>
	public class HomeController : Controller
	{
		#region Public Methods and Operators

		/// <summary>
		/// The index.
		/// </summary>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		public ActionResult Index()
		{
			return this.RedirectToAction("Index", "ToDo");
		}

		#endregion
	}
}