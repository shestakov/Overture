using System;
using Overture.ChangeSets.Protobuf.Simple;

namespace Overture.ChangeSets.BusinessObjects
{
	public class SimpleObjectChangeSetExceptionRecord
	{
		public SimpleObjectChangeSetExceptionRecord(SimpleObjectChangeSet changeSet, Exception exception)
		{
			Exception = exception;
			ChangeSet = changeSet;
		}

		public Exception Exception { get; private set; }
		public SimpleObjectChangeSet ChangeSet { get; private set; }
	}
}