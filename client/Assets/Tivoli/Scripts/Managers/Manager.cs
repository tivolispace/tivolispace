using System.Threading.Tasks;

namespace Tivoli.Scripts.Managers
{
    public class Manager
    {
        public virtual Task Init()
        {
            return Task.CompletedTask;
        }

        public virtual void Update()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
    }
}