using UnityEngine;

namespace DuckJam.Utilities
{
    internal static class LineUtils
    {
        public static bool TryGetIntersection
        (
            in Vector2 lineAStart, 
            in Vector2 lineAEnd, 
            in Vector2 lineBStart, 
            in Vector2 lineBEnd, 
            out Vector2 intersection
        )
        {
            var s1 = lineAEnd - lineAStart;
            var s2 = lineBEnd - lineBStart;
            
            var s = (-s1.y * (lineAStart.x - lineBStart.x) + s1.x * (lineAStart.y - lineBStart.y)) / (-s2.x * s1.y + s1.x * s2.y);
            var t = ( s2.x * (lineAStart.y - lineBStart.y) - s2.y * (lineAStart.x - lineBStart.x)) / (-s2.x * s1.y + s1.x * s2.y);

            // No collision
            if (!(s >= 0) || !(s <= 1) || !(t >= 0) || !(t <= 1))
            {
                intersection = default;
                return false; 
            }
            
            // Collision detected
            intersection = lineAStart + t * s1;
            return true;
        }
    }
}