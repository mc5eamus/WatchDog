namespace WatchDog.Contract
{
    /// <summary>
    /// Describes the contract used to publish query / data source results
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Publishes an instance of TModel to assigned targets 
        /// </summary>
        /// <typeparam name="TModel">Type of the model to publish</typeparam>
        /// <param name="model">TModel instance</param>
        /// <param name="token">cancellation token to gracefully terminate long-running operations</param>
        /// <returns>async void</returns>
        Task Publish<TModel>(TModel model, CancellationToken token) where TModel : class;
    }
}
