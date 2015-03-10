namespace ToDoManager.Models
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
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether is done.
		/// </summary>
		public bool IsDone { get; set; }

		#endregion
	}
}