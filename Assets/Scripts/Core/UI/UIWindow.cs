using System;
using UnityEngine;

namespace Core.UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        public UILayer Layer { get; internal set; }
        public string Address { get; internal set; }

        internal Action CloseRequested;

        public virtual void OnOpened() { }
        public virtual void OnClosed() { }
        public virtual void OnHidden() { }
        public virtual void OnRevealed() { }

        public void Close()
        {
            CloseRequested?.Invoke();
        }
    }

    public abstract class UIWindow<TParams> : UIWindow where TParams : struct
    {
        public void SetParams(TParams parameters)
        {
            OnParamsSet(parameters);
        }

        protected abstract void OnParamsSet(TParams parameters);
    }
}
