using System;
using System.Collections.Generic;
using Ninject.Modules;

namespace Overture.ChangeSets.Tests.MockingNinja
{
	public class TestNinjectModule: NinjectModule
	{
		private readonly Dictionary<Type, object> mocks = new Dictionary<Type, object>();

		public void RegisterMock<T>(T mock)
		{
			RegisterMock(typeof(T), mock);
		}

		public void RegisterMock(Type type, object mock)
		{
			mocks.Add(type, mock);
		}

		public IEnumerable<Type> GetRegisteredTypes()
		{
			return mocks.Keys;
		}

		public override void Load()
		{
			foreach(var mock in mocks)
			{
				Bind(mock.Key).ToConstant(mock.Value);
			}
		}
	}
}