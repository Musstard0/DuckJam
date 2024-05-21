using System.Runtime.CompilerServices;
using UnityEngine;

namespace DuckJam.Utilities
{
    public static class Vector3Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XX(this Vector3 v) => new(v.x, v.x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XY(this Vector3 v) => new(v.x, v.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XZ(this Vector3 v) => new(v.x, v.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YX(this Vector3 v) => new(v.y, v.x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YY(this Vector3 v) => new(v.y, v.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YZ(this Vector3 v) => new(v.y, v.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ZX(this Vector3 v) => new(v.z, v.x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ZY(this Vector3 v) => new(v.z, v.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ZZ(this Vector3 v) => new(v.z, v.z);
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithX(this Vector3 v, float x) => new(x, v.y, v.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithY(this Vector3 v, float y) => new(v.x, y, v.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithZ(this Vector3 v, float z) => new(v.x, v.y, z);
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddX(this Vector3 v, float x) => new(v.x + x, v.y, v.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddY(this Vector3 v, float y) => new(v.x, v.y + y, v.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AddZ(this Vector3 v, float z) => new(v.x, v.y, v.z + z);
    }
}