using ToDoManager.Models;

namespace ToDoManager.Services
{
	public interface IUserRetrieverService
	{
		UserModel GetCurrentUser();
	}
}