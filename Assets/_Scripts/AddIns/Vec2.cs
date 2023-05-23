using BehaviorTree.NPCStats;
using UnityEngine;

namespace AddIns
{
    // Mini custom Vector2 library
    public struct Vec2
    {
        private SpitterStats _stats;

        /// <summary>
        /// Calculates the direction from a to b with a ref. magnitude 1.
        /// </summary>
        /// <param name="a">Vector2</param>
        /// <param name="b">Vector2</param>
        /// <returns>Returns the direction from a to b.</returns>
        public static Vector2 Direction(Vector2 a, Vector2 b)
        {
            return (b - a).normalized;
        }
        
        /// <summary>
        /// Compact version of Vector2.MoveTowards Method.
        /// </summary>
        /// <param name="current">Transform</param>
        /// <param name="other">Transform</param>
        /// <param name="step">float</param>
        public static void MoveTo(Transform current, Transform other, float step)
        {
            current.position = Vector2.MoveTowards(current.position, other.position, step);
        }

        /// <summary>
        /// The opposite of Vector2.MoveTowards Method.<br/> For the second parameter you need to calculate the reverse direction
        /// <br/> and multiply it by the distance you want to move away.
        /// </summary>
        /// <param name="a">Vector2</param>
        /// <param name="backup">Vector2</param>
        /// <param name="step">float</param>
        /// <returns>Returns a movement away from an object.</returns>
        public static Vector2 MoveAway(Vector2 a, Vector2 backup, float step)
        {
            return Vector2.MoveTowards(a, a + backup, step);
        }

        /// <summary>
        /// Calculates the distance between two points with an offset. The offset can adjusted in the stats.
        /// </summary>
        /// <param name="stats">SpitterStats</param>
        /// <param name="startPoint">Vector2</param>
        /// <param name="endPoint">Vector2</param>
        /// <param name="range">float</param>
        /// <returns>Returns a distance with an offset.</returns>
        public static bool DistanceBetween(SpitterStats stats, Transform startPoint, Transform endPoint, float range)
        {
            var distance = Vector2.Distance(startPoint.position, endPoint.position);
            var stopDistance = stats.DetectionRadiusPlayer - range;
            var stopDistanceWithOffset = stopDistance - stats.DistanceBetweenOffset;
            var backupDistanceWithOffset = (stopDistance - stats.MaxBackupDistance) + stats.DistanceBetweenOffset;
            
            

            return (distance < stopDistanceWithOffset && distance > backupDistanceWithOffset);
        }
        
        public static bool BackupDistance(SpitterStats stats, Transform startPoint, Transform endPoint)
        {
            var distance = Vector2.Distance(startPoint.position, endPoint.position);
            var stopDistance = stats.DetectionRadiusPlayer - stats.NearRangeStopDistance;

            return (distance < stopDistance - stats.MinBackupDistance && 
                   distance < stopDistance - stats.MaxBackupDistance);
        }
        
        /// <summary>
        /// Rotates a GameObject on the x axis towards an object.
        /// </summary>
        /// <param name="rigid">Rigidbody2D</param>
        /// <param name="direction">Vector2</param>
        public static void LookAt(Rigidbody2D rigid, Vector2 direction)
        {
            rigid.transform.localScale = new Vector2(direction.x > 0 ? 1 : -1, 1);
        }
    }
}