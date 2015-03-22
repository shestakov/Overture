using System;
using NUnit.Framework;
using Overture.ChangeSets.SerializerProvider;

namespace Overture.ChangeSets.Tests.SerializerProvider
{
	[TestFixture(typeof(Int32))]
	[TestFixture(typeof(Int32?))]
	[TestFixture(typeof(Int64))]
	[TestFixture(typeof(Int64?))]
	[TestFixture(typeof(Double))]
	[TestFixture(typeof(Double?))]
	[TestFixture(typeof(Guid))]
	[TestFixture(typeof(Guid?))]
	[TestFixture(typeof(DateTimeOffset))]
	[TestFixture(typeof(DateTimeOffset?))]
	[TestFixture(typeof(String))]
	public class SerializerProviderTest_SimpleTypes<T>
	{
		[SetUp]
		public void SetUp()
		{
			provider = new AttributeValueSerializerProvider();
		}

		private AttributeValueSerializerProvider provider;

		[Test]
		public void GetProvider_Succcess()
		{
			var serializer = provider.Get(typeof(T));
			Assert.AreEqual(typeof(T), serializer.TargetType);
		}
	}
}