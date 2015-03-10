using Newtonsoft.Json;

namespace ToDoManagerClient.Tests.Models
{
	/// <summary>
	/// The to do model.
	/// </summary>
	public class ToDoModel
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		[JsonProperty("Description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		[JsonProperty("Id")]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether is done.
		/// </summary>
		[JsonProperty("IsDone")]
		public bool IsDone { get; set; }

		#endregion
	}
}