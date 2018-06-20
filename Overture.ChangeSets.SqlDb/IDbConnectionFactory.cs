using System.Data;

namespace Overture.ChangeSets.SqlDb
{
	public interface IDbConnectionFactory
	{
		IDbConnection Get();
	}
}