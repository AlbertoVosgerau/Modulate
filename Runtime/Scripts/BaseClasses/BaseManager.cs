using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace DandyDino.Modulate
{
    [Serializable]
    public abstract class BaseManager<TView> : IManager where TView : IView
    {
        private TView View => _views.Count > 0 ? _views[0] : default;
        public Type ViewType => typeof(TView);
        protected readonly List<TView> _views = new();
        
        public bool IsSingleton { get; }
        
        protected BaseManager()
        {
            IsSingleton = GetType().GetCustomAttribute<ManagerAttribute>()?.IsSingleton ?? true;

            EventBus<RegisterViewEvt>.OnEvent += OnRegister;
            EventBus<UnRegisterViewEvt>.OnEvent += OnUnregister;
        }

        private void OnRegister(RegisterViewEvt evt)
        {
            if (evt.view is not TView view) return;
            if (IsSingleton && _views.Count > 0)
            {
                (view as MonoBehaviour).DestroySelf();
                return;
            }
            _views.Add(view);
        }

        private void OnUnregister(UnRegisterViewEvt evt)
        {
            if (evt.view is TView view) _views.Remove(view);
        }

        public virtual void Dispose()
        {
            EventBus<RegisterViewEvt>.OnEvent -= OnRegister;
            EventBus<UnRegisterViewEvt>.OnEvent -= OnUnregister;
        }
        
        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void LateUpdate()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }
    }
}