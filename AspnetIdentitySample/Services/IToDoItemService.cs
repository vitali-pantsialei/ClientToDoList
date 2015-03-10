using System.Collections.Generic;
using System.Threading.Tasks;

using ToDoManager.Models;

namespace ToDoManager.Services
{
	/// <summary>
	///     Some service that have to be implemented.
	/// </summary>
	public interface IToDoItemService
	{
		Task<IEnumerable<ToDoModel>> GetAll();

		Task<ToDoModel> Create(ToDoModel todo);

		Task<ToDoModel> GetById(int id);

		Task<bool> RemoveById(int id);

		Task<ToDoModel> Update(ToDoModel todo);
	}
}