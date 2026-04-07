using System.Threading.Tasks;

namespace Core.UI
{
    public interface IUIService
    {
        Task<TWindow> Open<TWindow>(string address, UILayer layer = UILayer.Default)
            where TWindow : UIWindow;

        Task<TWindow> Open<TWindow, TParams>(string address, TParams parameters, UILayer layer = UILayer.Default)
            where TWindow : UIWindow<TParams>
            where TParams : struct;

        void Close(UIWindow window);
        void CloseLayer(UILayer layer);
        void CloseAll();
    }
}
