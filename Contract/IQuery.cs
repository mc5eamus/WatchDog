namespace WatchDog.Contract
{
    public interface IQuery<T> where T : class
    {
        Task<T> Execute(CancellationToken ctoken);
    }
}
