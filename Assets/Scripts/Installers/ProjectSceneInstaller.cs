using System.Threading.Tasks;
using Core.SceneLoading;
using Zenject;

namespace Installers
{
    public class ProjectSceneInstaller : MonoInstaller
    {
        [Inject]
        private ISceneLoader _sceneLoader;
        
        public override void InstallBindings()
        {
            FinishLoad();
        }

        private async void FinishLoad()
        {
            while (Container.IsInstalling)
            {
                await Task.Yield();
            }
            
            _sceneLoader.LoadScene("Login");
        }
    }
}