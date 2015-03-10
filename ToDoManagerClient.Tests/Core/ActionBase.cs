using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RestSharp;

namespace ToDoManagerClient.Tests.Core
{
	public abstract class ActionBase<TParameters, TResponse> : BaseTestObject
		where TResponse : new()
		where TParameters : new()
	{
		/// <summary>
		/// Gets the base host.
		/// </summary>
		/// <value>
		/// The base host.
		/// </value>
		protected virtual string BaseHost
		{
			get { return ConfigurationManager.AppSettings["testServiceUrl"]; }
		}

		public abstract Method HttpMethod { get; set; }

		public string HostPath = "";

		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="httpMethod">The httpMethod.</param>
		/// <param name="hostPath">The hostPath.</param>
		/// <returns>Response.</returns>
		public virtual IRestResponse<TResponse> Execute(TParameters parameters, Method httpMethod, string hostPath = "")
		{
			this.HttpMethod = httpMethod;
			this.HostPath = hostPath;

			var restClient = this.GetRestClient();
			var restRequest = this.GetRestRequest(parameters);
			this.PrintRequest(restClient, restRequest);
			this.PrintTestDataId(parameters);
			var response = restClient.Execute<TResponse>(restRequest);
			this.PrintResponse(response);
			this.CheckResponse(parameters, restRequest, response);
			
			return response;
		}

		protected virtual RestClient GetRestClient()
		{
			var restClient = new RestClient(this.BaseHost + this.HostPath);
			restClient.AddHandler("application/json", new RestSharpJsonNetDeserializer());
			return restClient;
		}

		protected RestRequest GetRestRequest(TParameters parameters)
		{
			var restRequest = new RestRequest(this.GetPath(parameters), this.HttpMethod)
			{
				JsonSerializer = new RestSharpJsonNetSerializer(),
				RequestFormat = DataFormat.Json
			};

			this.AddHeader(restRequest, parameters);
			this.AddParameters(restRequest, parameters);
			this.AddBody(restRequest, parameters);
			return restRequest;
		}

		protected virtual void AddHeader(RestRequest restRequest, TParameters parameters)
		{
			restRequest.AddHeader("Content-Type", "application/json");
		}

		protected virtual void AddBody(RestRequest restRequest, TParameters parameters)
		{
			restRequest.AddBody(parameters);
		}

		/// <summary>
		/// Add the query parameters depends on request http method:
		/// using <see cref="QueryParameterNameAttribute"/> as parameter name
		/// and property value as parameter value.
		/// </summary>
		/// <param name="restRequest"></param>
		/// <param name="parameters"></param>
		protected void AddParameters(RestRequest restRequest, TParameters parameters)
		{
			//don't read parameters if null
			if ((object)parameters == null)
			{
				return;
			}

			var fieldsWithParamNameAttribute =
				parameters.GetType()
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(p => p.IsDefined(typeof(QueryParameterNameAttribute)))
					.ToList();

			foreach (var propertyInfo in fieldsWithParamNameAttribute)
			{
				var defaultType = propertyInfo.PropertyType;
				if (typeof(ICollection).IsAssignableFrom(defaultType) ||
					(defaultType.IsGenericType && defaultType.GetGenericArguments().Length == 1))
				{
					var fieldAsList = propertyInfo.GetValue(parameters, null) as IList;
					if (fieldAsList == null) continue;
					foreach (var listItem in fieldAsList)
					{
						restRequest.AddParameter(propertyInfo.GetCustomAttribute<QueryParameterNameAttribute>().Value, listItem, ParameterType.QueryString);
					}
				}
				else
				{
					var singleItemValue = propertyInfo.GetValue(parameters, null);
					if (singleItemValue == null) continue;
					restRequest.AddParameter(propertyInfo.GetCustomAttribute<QueryParameterNameAttribute>().Value, singleItemValue, ParameterType.QueryString);
				}
			}
		}

		protected virtual void CheckResponse(TParameters parameters, IRestRequest request, IRestResponse<TResponse> response)
		{
			Assert.IsFalse(HttpStatusCode.InternalServerError.Equals(response.StatusCode), "InternalServerError is really bad!");
		}

		private void PrintTestDataId(TParameters parameters)
		{
			if ((object)parameters == null)
			{
				return;
			}

			var testIdValue = new List<PropertyInfo>(parameters.GetType().GetProperties()).Where(p => p.Name == "TestDataId").Select(p => p.GetValue(parameters, null)).SingleOrDefault();
			if (testIdValue != null)
			{
				TestContext.WriteLine("Test Data Id: {0}", testIdValue);
			}
		}

		private void PrintRequest(IRestClient client, IRestRequest request)
		{
			string resultUrl = client.BuildUri(request).AbsoluteUri;
			var headers = request.Parameters.Where(x => x.Type == ParameterType.HttpHeader);
			var bodyParameters = request.Parameters.Where(x => x.Type == ParameterType.RequestBody).ToList();
			TestContext.WriteLine("\nRequest: ");
			TestContext.WriteLine("\tMethod: " + request.Method);
			TestContext.WriteLine("\tURL: " + resultUrl);
			TestContext.WriteLine("\tHeaders: ");
			foreach (var header in headers)
			{
				TestContext.WriteLine("\t\t" + header.Name + ":" + header.Value);
			}
			if (bodyParameters.Count == 1)
			{
				TestContext.WriteLine("\tRequest Body:");
				TestContext.WriteLine("\t\t" + bodyParameters.First().Value.ToString().Replace('{', '[').Replace('}', ']'));
			}
		}

		private void PrintResponse(IRestResponse response)
		{
			TestContext.WriteLine("\nResponse:\n\tStatus code: {0}-{1}\r\n\tResponse content: {2}",
				(int)response.StatusCode, response.StatusCode,
				string.IsNullOrWhiteSpace(response.Content) ? "<empty>" : response.Content);
			TestContext.WriteLine(
				"-------------------------------------------------------------------------------------------------------\r\n");
		}

		protected abstract string GetPath(TParameters parameters);
	}
}