using UnityEngine;

namespace DefaultNamespace
{
    public class Util
    {
        public static Vector2 Vector2FromAngle(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static bool asteroidTutorialPlayed = false;
        public static bool firstUpgradeTutorialPlayed = false;
        public static bool firstHitTutorialPlayed = false;
        public static bool manyDinosTutorialPlayed = false;
        
        public static int godLineCounter = 0;
        public static bool godLinesShuffled = false;
    }
}