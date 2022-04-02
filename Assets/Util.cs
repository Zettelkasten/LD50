using UnityEngine;

namespace DefaultNamespace
{
    public class Util
    {
        public static Vector2 Vector2FromAngle(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        
    }
}