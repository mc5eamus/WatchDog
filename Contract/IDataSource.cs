namespace WatchDog.Contract
{
    /// <summary>
    /// IDataSource describes a contract for a unit of work producing data of type TModel
    /// (possibly by executing a query based on configuration of type TConfig)
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TConfig"></typeparam>
    public interface IDataSource<TModel, TConfig> 
        where TModel : class 
        where TConfig : class
    {
        Task<TModel> Execute(TConfig config, CancellationToken ctoken);
    }
}
