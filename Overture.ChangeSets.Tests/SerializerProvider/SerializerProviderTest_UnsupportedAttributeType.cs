using System;
using NUnit.Framework;
using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.Tests.SerializerProvider
{
	[TestFixture]
	public class SerializerProviderTest_UnsupportedAttributeType
	{
		private AttributeValueSerializerProvider provider;

		[SetUp]
		public void SetUp()
		{
			provider = new AttributeValueSerializerProvider();
		}

		[Test]
		public void GetProvider_Succcess()
		{
			Assert.Throws<ArgumentException>(() => provider.Get(typeof(DateTime)), "There is no registered AttributeValueBinarySerializer for type DateTime");
		}
	}
}