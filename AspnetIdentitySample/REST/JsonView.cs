using System.IO;
using System.Web.Mvc;

using Newtonsoft.Json;

namespace ToDoManager.REST
{
	public class JsonView: IView
	{
		public void Render(ViewContext viewContext, TextWriter writer)
		{
			var model = viewContext.ViewData.Model;
			JsonSerializer.Create().Serialize(writer, model);
		}
	}
}