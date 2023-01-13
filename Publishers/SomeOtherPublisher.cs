using Microsoft.Extensions.Logging;

namespace WatchDog.Publishers
{
    /// <summary>
    /// Inherited publisher class to demonstrate how DI can be used with multiple classes and their dependencies
    /// </summary>
    public class SomeOtherPublisher : BasePublisher
    {
        public SomeOtherPublisher(ILogger<SomeOtherPublisher> logger) : base(logger) { 
        }

    }
}
