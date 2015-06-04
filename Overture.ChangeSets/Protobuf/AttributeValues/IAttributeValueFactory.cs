using System;
using System.Linq.Expressions;

namespace Overture.ChangeSets.Protobuf.AttributeValues
{
	public interface IAttributeValueFactory
	{
		AttributeValue Create<T, TP>(Expression<Func<T, TP>> attribute, TP value);
		AttributeValue Create<T>(Expression<Func<T>> attribute, T value);
		AttributeValue Create(string name, Type type, object value);
		AttributeValue[] MapByName<T>(object dataObject);
		AttributeValue[] MapByName(Type businessObjectType, object dataObject);
	}
}