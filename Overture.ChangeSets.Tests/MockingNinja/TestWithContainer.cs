using System.Linq;
using Ninject;
using NUnit.Framework;
using Rhino.Mocks;

namespace Overture.ChangeSets.Tests.MockingNinja
{
	public abstract class TestWithContainer
	{
		protected MockRepository Mocks;
		private TestNinjectModule ninjectModule;

		[SetUp]
		public void SetUp()
		{
			Mocks = new MockRepository();
			ninjectModule = new TestNinjectModule();
		}

		protected T RegisterStrictMock<T>()
		{
			var mock = Mocks.StrictMock<T>();
			ninjectModule.RegisterMock(mock);
			return mock;
		}

		protected T Resolve<T>(bool useDynamicMocks = false) where T : class
		{
			var type = typeof(T);
			var constructors = type.GetConstructors();
			var constructorParameterTypes = constructors.SelectMany(e => e.GetParameters()).Select(e => e.ParameterType).ToArray();
			var registeredTypes = ninjectModule.GetRegisteredTypes();
			var typesForMocking = constructorParameterTypes.Except(registeredTypes);
			foreach(var e in typesForMocking)
			{
				ninjectModule.RegisterMock(e, useDynamicMocks ? Mocks.DynamicMock(e) : Mocks.StrictMock(e));
			}
			var result = new StandardKernel(ninjectModule).Get<T>();
			Mocks.ReplayAll();
			return result;
		}
	}
}