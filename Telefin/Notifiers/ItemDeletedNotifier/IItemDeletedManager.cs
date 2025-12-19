using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;

namespace Telefin.Notifiers.ItemDeletedNotifier;

public interface IItemDeletedManager
{
    public Task ProcessItemsAsync();

    public void AddItem(BaseItem item);
}
