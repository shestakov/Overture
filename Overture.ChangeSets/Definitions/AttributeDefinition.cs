using System;
using Overture.ChangeSets.Serializers;

namespace Overture.ChangeSets.Definitions
{
	public class AttributeDefinition
	{
		private bool Equals(AttributeDefinition other)
		{
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((AttributeDefinition) obj);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public AttributeDefinition(string name, Type attributeType, IAttributeValueBinarySerializer serializer)
		{
			Name = name;
			AttributeType = attributeType;
			Serializer = serializer;
		}

		public string Name { get; private set; }
		public Type AttributeType { get; private set; }
		public IAttributeValueBinarySerializer Serializer { get; private set; }
	}
}