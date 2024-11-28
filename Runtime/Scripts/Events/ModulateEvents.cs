using UnityEngine;

namespace DandyDino.Modulate
{
    public struct OnInitialize : IEvent
    {
        public IController Controller;

        public OnInitialize(IController controller)
        {
            Controller = controller;
        }
    }
}
