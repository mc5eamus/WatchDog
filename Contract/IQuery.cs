namespace WatchDog.Contract
{
    /// <summary>
    /// Contract for a query returning data of type TModel
    /// </summary>
    /// <typeparam name="TModel">type of the returned model</typeparam>
    public interface IQuery<TModel> where TModel : class
    {
        /// <summary>
        /// The actual workload for the query
        /// </summary>
        /// <param name="cancellationToken">cancellation token for termination, use consistently in all downstream async operations</param>
        /// <returns>async void</returns>

        Task<TModel> Execute(CancellationToken ctoken);
    }
}
