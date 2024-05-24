using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Modules
{
    internal sealed class MapModel
    {
        private readonly Rect _mapRect;

        public Vector2 CenterPosition2D => _mapRect.center;
        public Vector3 CenterPosition => CenterPosition2D.XY0();
        public Vector2 SouthWestPosition2D => _mapRect.min;
        public Vector2 SouthEastPosition2D => new(_mapRect.xMax, _mapRect.yMin);
        public Vector2 NorthWestPosition2D => new(_mapRect.xMin, _mapRect.yMax);
        public Vector2 NorthEastPosition2D => _mapRect.max;
        public Vector3 SouthWestPosition => SouthWestPosition2D.XY0();
        public Vector3 SouthEastPosition => SouthEastPosition2D.XY0();
        public Vector3 NorthWestPosition => NorthWestPosition2D.XY0();
        public Vector3 NorthEastPosition => NorthEastPosition2D.XY0();
        public Vector2 Size => _mapRect.size;
        
        
        // speed of rotation - is anti-clockwise when positive, clockwise when negative
        public float TimeScaleLineRotationSpeed { get; set; } = 1f; 
        // current angle in degrees
        public float TimeScaleLineAngle { get; private set; }
        public Vector2 TimeScaleLineDirection2D
        {
            get
            {
                var angleInRadians = TimeScaleLineAngle * Mathf.Deg2Rad;
                return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            }
        }
        public Vector3 TimeScaleLineDirection => TimeScaleLineDirection2D.XY0();
        public Quaternion TimeScaleLineRotation => Quaternion.AngleAxis(TimeScaleLineAngle, Vector3.forward);

        public Vector2 TimeScaleLineStartPosition2D
        {
            get
            {
                var lineEnd = CenterPosition2D - TimeScaleLineDirection2D * (_mapRect.width + _mapRect.height);
                if(!TryGetMapEdgeLineIntersection(CenterPosition2D, lineEnd, out var intersection))
                {
#if UNITY_EDITOR
                    Debug.LogError($"{nameof(MapModel)}.{nameof(TimeScaleLineStartPosition2D)}: Failed to find intersection with map edge line.");
#endif
                    return default;
                }
                return intersection;
            }
        }
        
        public Vector2 TimeScaleLineEndPosition2D
        {
            get
            {
                var lineEnd = CenterPosition2D + TimeScaleLineDirection2D * (_mapRect.width + _mapRect.height);
                if(!TryGetMapEdgeLineIntersection(CenterPosition2D, lineEnd, out var intersection))
                {
#if UNITY_EDITOR
                    Debug.LogError($"{nameof(MapModel)}.{nameof(TimeScaleLineEndPosition2D)}: Failed to find intersection with map edge line.");
#endif
                    return default;
                }
                return intersection;
            }
        }
        
        public Vector3 TimeScaleLineStartPosition => TimeScaleLineStartPosition2D.XY0();
        public Vector3 TimeScaleLineEndPosition => TimeScaleLineEndPosition2D.XY0();
        
        public MapModel(Vector2 centerPosition, Vector2 size)
        {
            var minPosition = centerPosition - size / 2;
            _mapRect = new Rect(minPosition, size);
        }

        public void RotateTimeScaleLine(float deltaTime)
        {
            TimeScaleLineAngle = Mathf.Repeat(TimeScaleLineAngle + TimeScaleLineRotationSpeed * deltaTime, 360f);
        }
        
        /// <summary>
        /// Get the time scale sign at a position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>1 if position on fast side of line, -1 if position is on slow side of line</returns>
        public float GetTimeScaleSignAtPosition(Vector3 position)
        {
            return GetTimeScaleSignAtPosition(position.XY());
        }
        
        /// <summary>
        /// Get the time scale sign at a position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>1 if position on fast side of line, -1 if position is on slow side of line</returns>
        public float GetTimeScaleSignAtPosition(Vector2 position)
        {
            var directionToPosition = position - CenterPosition2D;
            var angleToPosition = Mathf.Atan2(directionToPosition.y, directionToPosition.x) * Mathf.Rad2Deg;
            var adjustedAngle = angleToPosition - TimeScaleLineAngle;
            // adjustedAngle = Mathf.Repeat(adjustedAngle + 180f, 360f) - 180f;
            //
            // return Mathf.Sign(adjustedAngle);
            
            adjustedAngle = Mathf.Repeat(adjustedAngle, 360f);
            
            var value = (int)(adjustedAngle / 90f);
            
            if(value is 0 or 2) return 1;
            return -1;
        }

        public bool IsPositionInMapBounds(Vector2 position)
        {
            return _mapRect.Contains(position);
        }
        
        public bool IsPositionInMapBounds(Vector3 position)
        {
            return _mapRect.Contains(position);
        }

        public void ClampMovementToMapBounds(Vector2 startPosition, ref Vector2 endPosition, float edgeBuffer = 0.1f)
        {
            if (IsPositionInMapBounds(endPosition)) return;
            
            if(TryGetMapEdgeLineIntersection(startPosition, endPosition, out var intersection))
            {
                var direction = (endPosition - startPosition).normalized;
                
                endPosition = intersection - direction * edgeBuffer;
            }
        }
        
        private bool TryGetMapEdgeLineIntersection(Vector2 lineStart, Vector2 lineEnd, out Vector2 intersection)
        {
            if (LineUtils.TryGetIntersection
                (
                    lineStart, lineEnd, 
                    SouthWestPosition2D, SouthEastPosition2D, 
                    out intersection
                )) return true;
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, lineEnd, 
                    SouthEastPosition2D, NorthEastPosition2D, 
                    out intersection
                )) return true;
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, lineEnd, 
                    NorthEastPosition2D, NorthWestPosition2D, 
                    out intersection
                )) return true;
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, lineEnd, 
                    NorthWestPosition2D, SouthWestPosition2D, 
                    out intersection
                )) return true;

            intersection = default;
            return false;
        }
    }
}