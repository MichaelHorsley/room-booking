using view_models;

namespace host_api.Repositories;

public interface IViewModelRepository
{
    public T Get<T>(string id) where T : ViewModel;
    public List<T> GetAll<T>() where T : ViewModel;
}