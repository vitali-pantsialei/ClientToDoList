using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

using ToDoManager.Models;
using ToDoManager.Services;

using System.Collections.Generic;

namespace ToDoManager.Controllers
{
	/// <summary>
	/// The to do controller.
	/// </summary>
	public class ToDoController : Controller
	{
		private static IAuthenticationService _authenticationService;
		private static ToDoServiceReference.ToDoServiceClient _todoService;

		#region Constructors and Destructors

		static ToDoController()
		{
			var userService = new UserService();

			_authenticationService = userService;
            _todoService = new ToDoServiceReference.ToDoServiceClient();

			_authenticationService.Login(null);
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The all.
		/// </summary>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		public async Task<ActionResult> All()
		{
            List<ToDoModel> list = new List<ToDoModel>();
            foreach (var item in _todoService.GetTodoList(123))
            {
                list.Add(new ToDoModel()
                {
                    IsDone = item.IsCompleted,
                    Description = item.Name,
                    Id = item.ToDoId
                });
            }
            
			return View(list);
		}

		/// <summary>
		/// The create.
		/// </summary>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		public ActionResult Create()
		{
			return this.View();
		}

		/// <summary>
		/// The create.
		/// </summary>
		/// <param name="todo">
		/// The todo.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> Create([Bind(Include = "Id,Description,IsDone")] ToDoModel todo)
		{
			//Thread.Sleep(5000);
			if (this.ModelState.IsValid)
			{
                _todoService.CreateToDoItem(GetDTOItem(todo));
				return this.RedirectToAction("Index");
			}

			return this.View(todo);
		}

		/// <summary>
		/// The delete.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

            var item = _todoService.GetById(id.Value);
            ToDoModel todo = new ToDoModel()
                {
                    Description = item.Name,
                    Id = item.ToDoId,
                    IsDone = item.IsCompleted
                };

			if (todo == null)
			{
				return this.HttpNotFound();
			}

			return this.View(todo);
		}

		/// <summary>
		/// The delete confirmed.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		[HttpPost]
		[ActionName("Delete")]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
            var item = _todoService.GetById((int)id);
            ToDoModel todo = new ToDoModel()
            {
                Description = item.Name,
                Id = item.ToDoId,
                IsDone = item.IsCompleted
            };

			if (todo == null)
			{
				return this.HttpNotFound();
			}

            _todoService.DeleteToDoItem(id);
			return this.RedirectToAction("Index");
		}

		/// <summary>
		/// The details.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

            var item = _todoService.GetById((int)id);
            ToDoModel todo = new ToDoModel()
            {
                Description = item.Name,
                Id = item.ToDoId,
                IsDone = item.IsCompleted
            };

			if (todo == null)
			{
				return this.HttpNotFound();
			}

			return this.View(todo);
		}

		/// <summary>
		/// The edit.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		public async Task<ActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

            var item = _todoService.GetById((int)id);
            ToDoModel todo = new ToDoModel()
            {
                Description = item.Name,
                Id = item.ToDoId,
                IsDone = item.IsCompleted
            };

			if (todo == null)
			{
				return this.HttpNotFound();
			}

			return this.View(todo);
		}

		/// <summary>
		/// The edit.
		/// </summary>
		/// <param name="todo">
		/// The todo.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> Edit([Bind(Include = "Id,Description,IsDone")] ToDoModel todo)
		{
			if (this.ModelState.IsValid)
			{
                ToDoServiceReference.ToDoItem item = new ToDoServiceReference.ToDoItem()
                {
                    IsCompleted = todo.IsDone,
                    Name = todo.Description,
                    ToDoId = todo.Id
                };

                _todoService.UpdateToDoItem(item);
				return this.RedirectToAction("Index");
			}

			return this.View(todo);
		}

		/// <summary>
		/// The index.
		/// </summary>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> Index()
		{
            List<ToDoModel> list = new List<ToDoModel>();
            foreach (var item in _todoService.GetTodoList(123))
            {
                list.Add(new ToDoModel()
                {
                    IsDone = item.IsCompleted,
                    Description = item.Name,
                    Id = item.ToDoId
                });
            }

			return View(list);
		}

		#endregion

        private static ToDoServiceReference.ToDoItem GetDTOItem(ToDoModel model)
        {
            return new ToDoServiceReference.ToDoItem()
            {
                IsCompleted = model.IsDone,
                Name = model.Description,
                ToDoId = model.Id,
                UserId = 123
            };
        }
	}
}