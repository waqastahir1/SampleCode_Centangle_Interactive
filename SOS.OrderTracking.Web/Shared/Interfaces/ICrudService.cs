using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces
{
    public interface ICrudService<TFormViewModel, TListViewModel, TKey, TBaseIndexModel>
        where TBaseIndexModel : BaseIndexModel
    {
        public Task<IndexViewModel<TListViewModel>> GetPageAsync(TBaseIndexModel vm);

        public Task<TFormViewModel> GetAsync(TKey id);

        public Task<TKey> PostAsync(TFormViewModel selectedItem);
    }
}
