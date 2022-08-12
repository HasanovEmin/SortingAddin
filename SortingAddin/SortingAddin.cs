using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;

namespace SortingAddin
{
    public class SortingAddin : IAddin
    {
        public string Name => "Sorting Addin";

        public string Description => "Sorts Scoms and Gparts";

        public void Start(ServiceManager serviceManager)
        {
            ICommandManager commandManager = DependencyResolver.GetImplementationOf<ICommandManager>();
            SortCmd sortCmd = new SortCmd();
            commandManager.Commands.Add(sortCmd);
        }

        public void Stop()
        {
            
        }
    }
}
