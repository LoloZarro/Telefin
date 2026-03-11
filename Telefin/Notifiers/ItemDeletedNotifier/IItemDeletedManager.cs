using System.Threading.Tasks;

namespace Telefin.Notifiers.ItemDeletedNotifier;

public interface IItemDeletedManager
{
    public Task ProcessItemsAsync();
}
