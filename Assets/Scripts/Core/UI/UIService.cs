using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

namespace Core.UI
{
    public class UIService : IUIService, IInitializable, IDisposable
    {
        [Inject]
        private DiContainer _container;

        private GameObject _rootObject;
        private readonly Dictionary<UILayer, Canvas> _layerCanvases = new Dictionary<UILayer, Canvas>();
        private readonly Dictionary<UILayer, UIWindow> _activeWindows = new Dictionary<UILayer, UIWindow>();
        private readonly Dictionary<UILayer, Queue<PendingWindow>> _windowQueues = new Dictionary<UILayer, Queue<PendingWindow>>();
        private readonly Dictionary<UIWindow, AsyncOperationHandle<GameObject>> _handles = new Dictionary<UIWindow, AsyncOperationHandle<GameObject>>();
        private readonly List<UIWindow> _hiddenWindows = new List<UIWindow>();

        public void Initialize()
        {
            CreateCanvasHierarchy();
        }

        public void Dispose()
        {
            CloseAll();

            if (_rootObject != null)
            {
                UnityEngine.Object.Destroy(_rootObject);
            }
        }

        public async Task<TWindow> Open<TWindow>(string address, UILayer layer = UILayer.Default)
            where TWindow : UIWindow
        {
            if (_activeWindows.TryGetValue(layer, out var existing) && existing.Address == address)
            {
                return (TWindow)existing;
            }

            if (_activeWindows.ContainsKey(layer))
            {
                var tcs = new TaskCompletionSource<TWindow>();

                _windowQueues[layer].Enqueue(new PendingWindow
                {
                    Address = address,
                    Layer = layer,
                    OnInstantiated = window => tcs.SetResult((TWindow)window)
                });

                return await tcs.Task;
            }

            return await InstantiateWindow<TWindow>(address, layer);
        }

        public async Task<TWindow> Open<TWindow, TParams>(string address, TParams parameters,
            UILayer layer = UILayer.Default)
            where TWindow : UIWindow<TParams>
            where TParams : struct
        {
            if (_activeWindows.TryGetValue(layer, out var existing) && existing.Address == address)
            {
                var existingTyped = (TWindow)existing;
                existingTyped.SetParams(parameters);
                return existingTyped;
            }

            if (_activeWindows.ContainsKey(layer))
            {
                var tcs = new TaskCompletionSource<TWindow>();

                _windowQueues[layer].Enqueue(new PendingWindow
                {
                    Address = address,
                    Layer = layer,
                    OnInstantiated = window =>
                    {
                        var typed = (TWindow)window;
                        typed.SetParams(parameters);
                        tcs.SetResult(typed);
                    }
                });

                return await tcs.Task;
            }

            var instance = await InstantiateWindow<TWindow>(address, layer);
            instance.SetParams(parameters);
            return instance;
        }

        public void Close(UIWindow window)
        {
            if (window == null)
            {
                return;
            }

            var layer = window.Layer;

            window.OnClosed();

            if (_handles.TryGetValue(window, out var handle))
            {
                _handles.Remove(window);
                Addressables.ReleaseInstance(handle);
            }

            if (_activeWindows.TryGetValue(layer, out var active) && active == window)
            {
                _activeWindows.Remove(layer);
            }

            RevealHiddenWindows();
            ProcessQueue(layer);
        }

        public void CloseLayer(UILayer layer)
        {
            if (_activeWindows.TryGetValue(layer, out var window))
            {
                Close(window);
            }

            if (_windowQueues.TryGetValue(layer, out var queue))
            {
                queue.Clear();
            }
        }

        public void CloseAll()
        {
            var layers = _activeWindows.Keys.ToList();

            foreach (var layer in layers)
            {
                if (_activeWindows.TryGetValue(layer, out var window))
                {
                    window.OnClosed();

                    if (_handles.TryGetValue(window, out var handle))
                    {
                        _handles.Remove(window);
                        Addressables.ReleaseInstance(handle);
                    }

                    _activeWindows.Remove(layer);
                }

                if (_windowQueues.TryGetValue(layer, out var queue))
                {
                    queue.Clear();
                }
            }

            _hiddenWindows.Clear();
        }

        private async Task<TWindow> InstantiateWindow<TWindow>(string address, UILayer layer)
            where TWindow : UIWindow
        {
            var layerCanvas = _layerCanvases[layer];
            var handle = Addressables.InstantiateAsync(address, layerCanvas.transform);
            var go = await handle.Task;

            var window = go.GetComponent<TWindow>();

            if (window == null)
            {
                Debug.LogError($"Loaded addressable '{address}' does not have component {typeof(TWindow).Name}");
                Addressables.ReleaseInstance(handle);
                return null;
            }

            _container.InjectGameObject(go);

            window.Address = address;
            window.Layer = layer;
            window.CloseRequested = () => Close(window);

            _activeWindows[layer] = window;
            _handles[window] = handle;

            HideLowerLayers(layer);

            window.OnOpened();

            return window;
        }

        private void HideLowerLayers(UILayer openedLayer)
        {
            foreach (var kvp in _activeWindows)
            {
                if (kvp.Key < openedLayer && kvp.Value != null && !_hiddenWindows.Contains(kvp.Value))
                {
                    var canvasGroup = _layerCanvases[kvp.Key].GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;

                    kvp.Value.OnHidden();
                    _hiddenWindows.Add(kvp.Value);
                }
            }
        }

        private void RevealHiddenWindows()
        {
            for (int i = _hiddenWindows.Count - 1; i >= 0; i--)
            {
                var hidden = _hiddenWindows[i];
                var shouldStayHidden = false;

                foreach (var kvp in _activeWindows)
                {
                    if (kvp.Key > hidden.Layer)
                    {
                        shouldStayHidden = true;
                        break;
                    }
                }

                if (shouldStayHidden)
                {
                    continue;
                }

                var canvasGroup = _layerCanvases[hidden.Layer].GetComponent<CanvasGroup>();
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;

                hidden.OnRevealed();
                _hiddenWindows.RemoveAt(i);
            }
        }

        private async void ProcessQueue(UILayer layer)
        {
            if (!_windowQueues.TryGetValue(layer, out var queue) || queue.Count == 0)
            {
                return;
            }

            var pending = queue.Dequeue();
            var window = await InstantiateWindow<UIWindow>(pending.Address, pending.Layer);
            pending.OnInstantiated?.Invoke(window);
        }

        private void CreateCanvasHierarchy()
        {
            _rootObject = new GameObject("UIServiceRoot");
            UnityEngine.Object.DontDestroyOnLoad(_rootObject);

            var rootCanvas = _rootObject.AddComponent<Canvas>();
            rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rootCanvas.sortingOrder = 0;

            _rootObject.AddComponent<CanvasScaler>();
            _rootObject.AddComponent<GraphicRaycaster>();

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var layerObject = new GameObject("Layer_" + layer);
                layerObject.transform.SetParent(_rootObject.transform, false);

                var rect = layerObject.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                var canvas = layerObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int)layer;

                layerObject.AddComponent<GraphicRaycaster>();

                var canvasGroup = layerObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;

                _layerCanvases[layer] = canvas;
                _windowQueues[layer] = new Queue<PendingWindow>();
            }
        }

        private class PendingWindow
        {
            public string Address;
            public UILayer Layer;
            public Action<UIWindow> OnInstantiated;
        }
    }
}
