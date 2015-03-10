using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace ToDoManagerClient.Tests.Core
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializer", Justification = "Identifier is spelled correctly")]
	public class RestSharpJsonNetDeserializer : IDeserializer
	{
		public T Deserialize<T>(IRestResponse response)
		{
			return JsonConvert.DeserializeObject<T>(response.Content);
		}

		/// <summary>
		/// Unused for JSON Deserialization
		/// </summary>
		public string RootElement { get; set; }

		/// <summary>
		/// Unused for JSON Deserialization
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Unused for JSON Deserialization
		/// </summary>
		public string DateFormat { get; set; }
	}
}
