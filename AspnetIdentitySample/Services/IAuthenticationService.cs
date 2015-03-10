using ToDoManager.Models;

namespace ToDoManager.Services
{
	/// <summary>
	/// The UserService interface.
	/// </summary>
	public interface IAuthenticationService
	{
		UserModel Login(UserModel user);
	}
}