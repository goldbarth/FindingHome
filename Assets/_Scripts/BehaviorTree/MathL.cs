using UnityEngine;

namespace BehaviorTree
{
    // Minimal math(linear algebra) library
    public struct MathL
    {
        public static Vector2 MoveAway(Vector2 a, Vector2 backup, float step)
        {
            return Vector2.MoveTowards(a, a + backup, step);
        }
    }
}