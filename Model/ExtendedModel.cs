namespace WatchDog.Model
{
    /// <summary>
    /// Some other example model returned by a query or a data source
    /// </summary>
    public class ExtendedModel : BaseModel
    {
        public string? SomeProperty { get; set; }

        public override string ToString()
        {
            return $"Id={Id},SomeProperty={SomeProperty}";
        }
    }
}
