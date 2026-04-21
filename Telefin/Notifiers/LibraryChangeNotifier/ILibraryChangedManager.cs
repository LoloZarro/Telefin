using System.Threading.Tasks;

namespace Telefin.Notifiers.ItemAddedNotifier;

public interface ILibraryChangedManager
{
    public Task ProcessItemsAsync();
}
