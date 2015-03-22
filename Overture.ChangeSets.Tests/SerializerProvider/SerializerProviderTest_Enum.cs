using System;
using NUnit.Framework;
using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.Tests.SerializerProvider
{
	public class SerializerProviderTest_Enum
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
			var serializer = provider.Get(typeof(TestEnum));
			Assert.AreEqual(typeof(Enum), serializer.TargetType);
		}

		private enum TestEnum
		{
		}
	}
}