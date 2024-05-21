using System.Runtime.CompilerServices;
using UnityEngine;

namespace DuckJam.Utilities
{
    public static class Vector2Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 X0Y(this Vector2 current, float y = 0f) => new(current.x, y, current.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 XY0(this Vector2 current, float z = 0f) => new(current.x, current.y, z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithX(this Vector2 v, float x) => new(x, v.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithY(this Vector2 v, float y) => new(v.x, y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AddX(this Vector2 v, float x) => new(v.x + x, v.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AddY(this Vector2 v, float y) => new(v.x, v.y + y);
    }
}