using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToDoManagerClient.Tests.Core
{
	public abstract class BaseTestObject
	{
		public TestContext TestContext
		{
			get { return MSTestContext.Instance; }
			set { MSTestContext.Instance = value; }
		}
	}
}