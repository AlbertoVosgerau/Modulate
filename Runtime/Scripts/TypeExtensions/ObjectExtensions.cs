using UnityEngine;

namespace DandyDino.Modulate
{
    public static class ObjectExtensions
    {
        public static void DestroySelf(this Object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(obj);
                return;
            }
            Object.DestroyImmediate(obj);
        }
    }
}