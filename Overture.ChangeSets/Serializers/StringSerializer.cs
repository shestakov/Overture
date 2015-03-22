using System;
using System.Text;

namespace Overture.ChangeSets.Serializers
{
	public class StringSerializer : AttributeValueBinarySerializer<string>
	{
		protected override bool CanBeNull
		{
			get { return true; }
		}

		protected override byte[] SerializeInternal(string name, Type type, string value)
		{
			return Encoding.UTF8.GetBytes(value);
		}

		protected override string DeserializeInternal(string name, Type type, byte[] serializedValue)
		{
			return Encoding.UTF8.GetString(serializedValue);
		}
	}
}