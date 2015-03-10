using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.Administration;
using RestSharp;
using ToDoManagerClient.Tests.Actions;
using ToDoManagerClient.Tests.Core;
using ToDoManagerClient.Tests.Models;

namespace ToDoManagerClient.Tests
{
	[TestClass]
	public class ToDoTests : BaseTestObject
	{
		private ToDoAction action;
		private Service.ToDoManager toDoService;
		private int userId;

		[TestInitialize]
		public void TestInitialize()
		{
			action = new ToDoAction();
			toDoService = new Service.ToDoManager();
			userId = Convert.ToInt32(ConfigurationManager.AppSettings["userId"]);
		}

		#region Add/Update/Delete MVC site tests

		[TestMethod]
		public void AddMethodTest()
		{
			// Arange
			var testData = new ToDoModel
			{
				Description = Guid.NewGuid().ToString(),
				IsDone = false
			};

			//Act
			action.Execute(testData, Method.POST, "/ToDo/Create");

			//Assert
			var result = action.Execute(null, Method.GET).Data;

			Assert.IsNotNull(result);
			var toDoEntity = result.First(x => x.Description == testData.Description);

			Assert.AreNotEqual(testData.Id, toDoEntity.Id);
			Assert.AreEqual(testData.Description, toDoEntity.Description);
			Assert.AreEqual(testData.IsDone, toDoEntity.IsDone);
		}

		[TestMethod]
		public void UpdateMethodTest()
		{
			// Arrange
			var testData = AddTestData().First();

			var url = "/ToDo/Edit/" + testData.Id;

			var dataForEdit = new ToDoModel
			{
				Id = testData.Id,
				Description = "1234"
			};

			// Act
			action.Execute(dataForEdit, Method.POST, url);

			// Assert
			var result = action.Execute(null, Method.GET).Data;
			Assert.IsNotNull(result);
			var toDoEntity = result.First(x => x.Id == testData.Id);

			Assert.AreEqual(dataForEdit.Id, toDoEntity.Id);
			Assert.AreEqual(dataForEdit.Description, toDoEntity.Description);
			Assert.AreEqual(dataForEdit.IsDone, toDoEntity.IsDone);
		}

		[TestMethod]
		public void DeleteMethodTest()
		{
			// Arrange
			var testData = AddTestData().First();

			var url = "/ToDo/Delete/" + testData.Id;

			// Act
			action.Execute(null, Method.POST, url);

			// Assert
			var result = action.Execute(null, Method.GET).Data;
			Assert.IsNotNull(result);
			var toDoEntity = result.Where(x => x.Id == testData.Id).ToList();

			Assert.AreEqual(0, toDoEntity.Count());
		}

		#endregion

		#region Check MVC delay

		[TestMethod]
		public void AddWithDelayMethodTest()
		{
			// Arange
			var testData = new ToDoModel
			{
				Description = Guid.NewGuid().ToString(),
				IsDone = false
			};

			//Act
			var sw = new Stopwatch();
			sw.Start();
			action.Execute(testData, Method.POST, "/ToDo/Create");
			sw.Stop();

			//Assert
			var result = action.Execute(null, Method.GET).Data;

			Assert.IsNotNull(result);
			var toDoEntity = result.First(x => x.Description == testData.Description);

			Assert.AreNotEqual(testData.Id, toDoEntity.Id);
			Assert.AreEqual(testData.Description, toDoEntity.Description);
			Assert.AreEqual(testData.IsDone, toDoEntity.IsDone);

			Assert.IsTrue(5 > sw.ElapsedMilliseconds / 1000);
		}

		[TestMethod]
		public void UpdateWithDelayMethodTest()
		{
			// Arrange
			var testData = AddTestData().First();

			var url = "/ToDo/Edit/" + testData.Id;

			var dataForEdit = new ToDoModel
			{
				Id = testData.Id,
				Description = "1234"
			};

			// Act
			var sw = new Stopwatch();
			sw.Start();
			action.Execute(dataForEdit, Method.POST, url);
			sw.Stop();

			// Assert
			var result = action.Execute(null, Method.GET).Data;
			Assert.IsNotNull(result);
			var toDoEntity = result.First(x => x.Id == testData.Id);

			Assert.AreEqual(dataForEdit.Id, toDoEntity.Id);
			Assert.AreEqual(dataForEdit.Description, toDoEntity.Description);
			Assert.AreEqual(dataForEdit.IsDone, toDoEntity.IsDone);

			Assert.IsTrue(5 > sw.ElapsedMilliseconds / 1000);
		}

		[TestMethod]
		public void DeleteWithDelayMethodTest()
		{
			// Arrange
			var testData = AddTestData().First();

			var url = "/ToDo/Delete/" + testData.Id;

			// Act
			var sw = new Stopwatch();
			sw.Start();
			action.Execute(null, Method.POST, url);
			sw.Stop();

			// Assert
			var result = action.Execute(null, Method.GET).Data;
			Assert.IsNotNull(result);
			var toDoEntity = result.Where(x => x.Id == testData.Id).ToList();

			Assert.AreEqual(0, toDoEntity.Count());

			Assert.IsTrue(5 > sw.ElapsedMilliseconds / 1000);
		}
		#endregion

		#region StopStartWCF

		[TestMethod]
		public void AddWithCheckWcfTest()
		{
			// Arange
			var testData = new ToDoModel
			{
				Description = Guid.NewGuid().ToString(),
				IsDone = false
			};

			//Act
			StopWCF();
			action.Execute(testData, Method.POST, "/ToDo/Create");

			Thread.Sleep(30000);

			StartWCF();
			Thread.Sleep(15000);

			var toDoItemsFromWcf = toDoService.GetTodoList(this.userId, true);

			//Assert
			Assert.IsNotNull(toDoItemsFromWcf);
			Assert.AreNotEqual(0, toDoItemsFromWcf.Count());
			Assert.IsTrue(toDoItemsFromWcf.All(x => x.Name == testData.Description));

			var toDoEntityWcf = toDoItemsFromWcf.First(x => x.Name == testData.Description);
			Assert.AreEqual(testData.IsDone, toDoEntityWcf.IsCompleted);
		}

		[TestMethod]
		public void UpdateWithCheckWcfTest()
		{
			// Arrange
			var testData = AddTestData().First();

			var url = "/ToDo/Edit/" + testData.Id;

			var dataForEdit = new ToDoModel
			{
				Id = testData.Id,
				Description = Guid.NewGuid().ToString()
			};

			// Act
			StopWCF();
			action.Execute(dataForEdit, Method.POST, url);

			Thread.Sleep(30000);
			StartWCF();
			Thread.Sleep(15000);

			var toDoItemsFromWcf = toDoService.GetTodoList(this.userId, true);

			//Assert
			Assert.IsNotNull(toDoItemsFromWcf);
			Assert.AreNotEqual(0, toDoItemsFromWcf.Count());
			Assert.IsTrue(toDoItemsFromWcf.All(x => x.Name == dataForEdit.Description));

			var toDoEntityWcf = toDoItemsFromWcf.First(x => x.Name == dataForEdit.Description);
			Assert.AreEqual(dataForEdit.IsDone, toDoEntityWcf.IsCompleted);
		}

		[TestMethod]
		public void DeleteWithCheckWcfTest()
		{
			// Arrange
			var testData = AddTestData().First();

			var url = "/ToDo/Delete/" + testData.Id;

			Thread.Sleep(15000);

			var toDoItemsFromWcfTestData = toDoService.GetTodoList(this.userId, true);

			// Act
			StopWCF();
			action.Execute(null, Method.POST, url);

			Thread.Sleep(30000);
			StartWCF();

			Thread.Sleep(15000);
			var toDoItemsFromWcf = toDoService.GetTodoList(this.userId, true);

			//Assert
			Assert.IsNotNull(toDoItemsFromWcf);
			Assert.IsNotNull(toDoItemsFromWcfTestData);
			Assert.AreNotEqual(0, toDoItemsFromWcf.Count());
			Assert.IsTrue(toDoItemsFromWcfTestData.All(x => x.Name == testData.Description));

			Assert.IsTrue(toDoItemsFromWcf.All(x => x.Name != testData.Description));
		}

		#endregion

		#region StopStartMVC

		[TestMethod]
		public void AddWithStartStopMvcTest()
		{
			// Arange
			var testData = new ToDoModel
			{
				Description = Guid.NewGuid().ToString(),
				IsDone = false
			};

			//Act
			action.Execute(testData, Method.POST, "/ToDo/Create");
			StopMVC();
			Thread.Sleep(30000);
			StartMVC();

			Thread.Sleep(15000);

			var toDoItemsFromWcf = toDoService.GetTodoList(this.userId, true);

			//Assert
			Assert.IsNotNull(toDoItemsFromWcf);
			Assert.AreNotEqual(0, toDoItemsFromWcf.Count());
			Assert.IsTrue(toDoItemsFromWcf.All(x => x.Name == testData.Description));

			var toDoEntityWcf = toDoItemsFromWcf.First(x => x.Name == testData.Description);
			Assert.AreEqual(testData.IsDone, toDoEntityWcf.IsCompleted);
		}
		#endregion

		private List<ToDoModel> AddTestData()
		{
			var testData = new ToDoModel
			{
				Description = Guid.NewGuid().ToString(),
				IsDone = false
			};

			return action.Execute(testData, Method.POST, "/ToDo/Create").Data;
		}

		private void StartWCF()
		{
			var wcfSiteName = ConfigurationManager.AppSettings["wcfSiteName"];
			var server = new ServerManager();
			var site = server.Sites.FirstOrDefault(s => s.Name == wcfSiteName);

			site.Start();

			if (site.State == ObjectState.Started)
			{
				//do deployment tasks...
			}
			else
			{
				throw new InvalidOperationException("Could not START website!");
			}
		}

		private void StopWCF()
		{
			var wcfSiteName = ConfigurationManager.AppSettings["wcfSiteName"];
			var server = new ServerManager();
			var site = server.Sites.FirstOrDefault(s => s.Name == wcfSiteName);

			site.Stop();

			if (site.State == ObjectState.Stopped)
			{
				//do deployment tasks...
			}
			else
			{
				throw new InvalidOperationException("Could not stop website!");
			}
		}

		private void StartMVC()
		{
			var wcfSiteName = ConfigurationManager.AppSettings["todoSiteName"];
			var server = new ServerManager();
			var site = server.Sites.FirstOrDefault(s => s.Name == wcfSiteName);

			site.Start();

			if (site.State == ObjectState.Started)
			{
				//do deployment tasks...
			}
			else
			{
				throw new InvalidOperationException("Could not START website!");
			}
		}

		private void StopMVC()
		{
			var wcfSiteName = ConfigurationManager.AppSettings["todoSiteName"];
			var server = new ServerManager();
			var site = server.Sites.FirstOrDefault(s => s.Name == wcfSiteName);

			site.Stop();

			if (site.State == ObjectState.Stopped)
			{
				//do deployment tasks...
			}
			else
			{
				throw new InvalidOperationException("Could not stop website!");
			}
		}
	}
}
