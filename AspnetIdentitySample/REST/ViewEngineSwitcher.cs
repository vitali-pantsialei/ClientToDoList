using System;
using System.Linq;
using System.Web.Mvc;

namespace ToDoManager.REST
{
	public class ViewEngineSwitcher : IViewEngine
	{
		private string[] _jsonTypes = { "application/json", "application/x-javascript", "text/javascript", "text/x-javascript","text/x-json" };

		private IViewEngine _oldEngine;

		public ViewEngineSwitcher(IViewEngine oldEngine)
		{
			_oldEngine = oldEngine;
		}

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			if (controllerContext == null)
				throw new ArgumentNullException("controllerContext");
			if (string.IsNullOrEmpty(partialViewName))
				throw new ArgumentException("Cannot be null or empty", "partialViewName");
			
			var types = controllerContext.HttpContext.Request.AcceptTypes;
			if (types != null && _jsonTypes.Any(types.Contains))
			{
				controllerContext.HttpContext.Response.ContentType = "application/json";
				return new ViewEngineResult(new JsonView(), this);
			}

			return _oldEngine.FindPartialView(controllerContext, partialViewName, useCache);
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			if (controllerContext == null)
				throw new ArgumentNullException("controllerContext");
			if (string.IsNullOrEmpty(viewName))
				throw new ArgumentException("Cannot be null or empty", "viewName");

			var types = controllerContext.HttpContext.Request.AcceptTypes;
			if (types != null && _jsonTypes.Any(types.Contains))
			{
				controllerContext.HttpContext.Response.ContentType = "application/json";
				return new ViewEngineResult(new JsonView(), this);
			}

			return _oldEngine.FindView(controllerContext, viewName, masterName, useCache);
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			IDisposable disposable = view as IDisposable;
			if (disposable == null)
				return;
			disposable.Dispose();
		}
	}
}