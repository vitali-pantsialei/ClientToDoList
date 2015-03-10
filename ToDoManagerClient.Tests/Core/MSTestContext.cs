using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToDoManagerClient.Tests.Core
{
	public static class MSTestContext
	{
		[ThreadStatic]
		private static TestContext _instance;

		public static TestContext Instance
		{
			get { return _instance; }
			set { _instance = value; }
		}
	}
}
