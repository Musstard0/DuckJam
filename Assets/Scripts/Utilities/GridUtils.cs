using Unity.Burst;
using Unity.Mathematics;

namespace DuckJam.Utilities
{
    [BurstCompile]
    internal static class GridUtils
    {
        [BurstCompile]
        public static void PositionToCoordinates
        (
            in float2 position, 
            in float2 unitSize, 
            out int2 coordinates
        )
        {
            coordinates = (int2)math.floor(position / unitSize);
        }
        
        [BurstCompile]
        public static void CoordinatesToPosition
        (
            in int2 coordinates, 
            in float2 unitSize, 
            out float2 position
        )
        {
            position = coordinates * unitSize;
        }
    }
}