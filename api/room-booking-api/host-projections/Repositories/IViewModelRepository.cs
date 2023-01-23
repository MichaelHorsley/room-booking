using view_models;

namespace host_projections.Repositories;

public interface IViewModelRepository
{
    public T Get<T>(string id) where T : ViewModel;
    public void Save<T>(T objectToBeSaved) where T : ViewModel;
}