using System;
using Overture.ChangeSets.Protobuf.Composite;

namespace Overture.ChangeSets.BusinessObjects
{
	public class CompositeObjectChangeSetExceptionRecord
	{
		public CompositeObjectChangeSetExceptionRecord(CompositeObjectChangeSet changeSet, Exception exception)
		{
			Exception = exception;
			ChangeSet = changeSet;
		}

		public Exception Exception { get; private set; }
		public CompositeObjectChangeSet ChangeSet { get; private set; }
	}
}