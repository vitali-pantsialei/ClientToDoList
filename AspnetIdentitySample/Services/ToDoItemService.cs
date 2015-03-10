using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ToDoManager.Models;

namespace ToDoManager.Services
{
	public class ToDoItemService : IToDoItemService
	{
		// this is fake solution! !NEVER TRY TO ACCESS DB FROM SERVICES WITHOUT A REASON!
		private static IList<ToDoModel> _db = new List<ToDoModel>();
		private IUserRetrieverService _userService;

		public ToDoItemService(IUserRetrieverService userService)
		{
			_userService = userService;
		}

		public Task<IEnumerable<ToDoModel>> GetAll()
		{
			// Filter by user here
			return Task.FromResult((IEnumerable<ToDoModel>)_db);
		}

		public Task<ToDoModel> Create(ToDoModel todo)
		{
			todo.Id = Guid.NewGuid().GetHashCode(); // almost random
			_db.Add(todo);

			return Task.FromResult(todo);
		}

		public Task<ToDoModel> GetById(int id)
		{
			return Task.FromResult(_db.FirstOrDefault(x => x.Id == id));
		}

		public Task<bool> RemoveById(int id)
		{
			_db.Remove(_db.FirstOrDefault(x => x.Id == id));

			return Task.FromResult(true);
		}

		public Task<ToDoModel> Update(ToDoModel todo)
		{
			var existingTodo = _db.FirstOrDefault(x => x.Id == todo.Id);
			_db.Remove(existingTodo);
			_db.Add(todo);

			return Task.FromResult(todo);
		}
	}
}