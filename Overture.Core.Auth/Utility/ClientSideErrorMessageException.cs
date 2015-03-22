using System;

namespace Overture.Core.Auth.Utility
{
	public class ClientSideErrorMessageException: Exception
	{
		public ClientSideErrorMessageException(string message, string logMessage) : base(message)
		{
			LogMessage = logMessage;
		}

		public string LogMessage { get; set; }
	}
}