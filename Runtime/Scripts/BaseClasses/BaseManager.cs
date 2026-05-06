using System;
using System.Collections.Generic;
using System.Reflection;
using R3;
using UnityEngine;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class BaseManager<TView> : IManager where TView : IView
    {
        private TView View => _views.Count > 0 ? _views[0] : default;
        public Type ViewType => typeof(TView);
        protected readonly List<TView> _views = new();

        public bool IsSingleton { get; }

        private DisposableBag _disposable;

        protected BaseManager()
        {
            IsSingleton = GetType().GetCustomAttribute<ManagerAttribute>()?.IsSingleton ?? true;

            EventBus<RegisterViewEvt>.AsObservable()
                .Subscribe(OnRegisterView)
                .AddTo(ref _disposable);

            EventBus<UnRegisterViewEvt>.AsObservable()
                .Subscribe(OnUnregisterView)
                .AddTo(ref _disposable);
        }

        private void OnRegisterView(RegisterViewEvt evt)
        {
            if (evt.view is not TView view) return;
            if (IsSingleton && _views.Count > 0)
            {
                (view as MonoBehaviour).DestroySelf();
                return;
            }
            _views.Add(view);
        }

        private void OnUnregisterView(UnRegisterViewEvt evt)
        {
            if (evt.view is TView view)
            {
                _views.Remove(view);
            }
        }

        public virtual void Dispose() => _disposable.Dispose();

        public virtual void Start() { }
        public void PreUpdate() { }

        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }
    }
}