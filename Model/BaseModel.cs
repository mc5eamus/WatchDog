namespace WatchDog.Model
{
    /// <summary>
    /// Some example model returned by a query
    /// </summary>
    public class BaseModel
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"Id={Id}";
        }
    }
}
