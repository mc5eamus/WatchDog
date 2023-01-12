namespace WatchDog.Model
{
    public class ExtendedModel : BaseModel
    {
        public string SomeProperty { get; set; }

        public override string ToString()
        {
            return $"Id={Id},SomeProperty={SomeProperty}";
        }
    }
}
