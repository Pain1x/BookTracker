using System.Threading.Tasks;

namespace BookTracker.BLL.Abstractions
{
	public interface IDatabaseInitializer
	{
		/// <summary>
		/// Initializes the database by ensuring the necessary migrations are applied.
		/// </summary>
		Task InitializeAsync();
	}
}