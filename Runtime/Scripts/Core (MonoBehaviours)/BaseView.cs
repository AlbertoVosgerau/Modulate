using UnityEngine;

namespace DandyDino.Modulate
{
    [AddComponentMenu("")]
    public abstract class BaseView : MonoBehaviour, IView
    {
#if UNITY_EDITOR
        [HideInInspector] public bool showEditor = true;
        [HideInInspector] public Vector2 scrollPosition;
#endif
        
        protected virtual void OnEnable()
        {
            EventBus<RegisterViewEvt>.Raise(new RegisterViewEvt(){view =  this});
        }

        protected virtual  void OnDisable()
        {
            EventBus<UnRegisterViewEvt>.Raise(new UnRegisterViewEvt(){view =  this});
        }
    }
}