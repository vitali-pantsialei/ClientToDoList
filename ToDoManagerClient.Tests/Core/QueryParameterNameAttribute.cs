using System;

namespace ToDoManagerClient.Tests.Core
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class QueryParameterNameAttribute : Attribute
	{
		private readonly string _value;

		public string Value
		{
			get { return _value; }
		}

		public QueryParameterNameAttribute(string value)
		{
			_value = value;
		}
	}
}