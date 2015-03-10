using System.Collections.Generic;
using RestSharp;
using ToDoManagerClient.Tests.Core;
using ToDoManagerClient.Tests.Models;

namespace ToDoManagerClient.Tests.Actions
{
	class ToDoAction : ActionBase<ToDoModel, List<ToDoModel>>
	{
		public override Method HttpMethod
		{
			get; set;
		}

		protected override string GetPath(ToDoModel parameters)
		{
			return "";
		}
	}
}