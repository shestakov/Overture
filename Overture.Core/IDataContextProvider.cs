using System.Data.Linq;

namespace Overture.Core
{
	public interface IDataContextProvider
	{
		DataContext Get();
	}
}