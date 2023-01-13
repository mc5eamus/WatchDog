namespace WatchDog.Contract
{
    public interface IPublisher
    {
        Task Publish<TModel>(TModel model, CancellationToken token) where TModel : class;
    }
}
