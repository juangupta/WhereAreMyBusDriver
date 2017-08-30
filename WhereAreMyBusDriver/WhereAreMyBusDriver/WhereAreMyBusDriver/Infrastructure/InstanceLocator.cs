using WhereAreMyBusDriver.ViewModels;

namespace WhereAreMyBusDriver.Infrastructure
{
    public class InstanceLocator
    {
        public MainViewModel Main { get; set; }

        public InstanceLocator()
        {
            Main = new MainViewModel();
        }
    }
}
