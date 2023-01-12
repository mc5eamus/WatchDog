namespace WatchDog.Model
{
    public class BaseModel
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"Id={Id}";
        }
    }
}
