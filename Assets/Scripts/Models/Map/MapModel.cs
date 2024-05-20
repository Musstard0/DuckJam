using DuckJam.Utilities;
using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class MapModel
    {
        private readonly MapPlane _mapPlane;
        private readonly Quaternion _initialRotation;
        
        public Vector3 CenterPosition { get; }
        public Vector2 Size { get; }
        public Vector3 GroundNormal { get; }

        public Vector3 HorizontalDirection => Vector3.right;
        public Vector3 VerticalDirection { get; }
        
        // corners of the map
        public Vector3 SouthWestPosition { get; }
        public Vector3 SouthEastPosition { get; }
        public Vector3 NorthWestPosition { get; }
        public Vector3 NorthEastPosition { get; }
        
        // speed of rotation - is anti-clockwise when positive, clockwise when negative
        public float TimeScaleLineRotationSpeed { get; set; } = 1f; 
        // current angle in degrees
        public float TimeScaleLineAngle { get; private set; }

        public Vector3 TimeScaleLineDirection => Quaternion.AngleAxis(TimeScaleLineAngle, -GroundNormal) * HorizontalDirection;
        public Quaternion TimeScaleLineRotation => _initialRotation * Quaternion.AngleAxis(TimeScaleLineAngle, Vector3.forward);

        public Vector3 TimeScaleLineStart => _mapPlane == MapPlane.XY 
            ? GetTimeScaleLineIntersectionXYPlane(false) 
            : GetTimeScaleLineIntersectionXZPlane(false);
        
        public Vector3 TimeScaleLineEnd => _mapPlane == MapPlane.XY 
            ? GetTimeScaleLineIntersectionXYPlane(true) 
            : GetTimeScaleLineIntersectionXZPlane(true);
        
        public MapModel(MapPlane plane, Vector3 position, Vector2 size)
        {
            _mapPlane = plane;
            CenterPosition = position;
            Size = size;
            GroundNormal = plane == MapPlane.XY ? Vector3.back : Vector3.up;
            
            _initialRotation = Quaternion.LookRotation(-GroundNormal);
            
            VerticalDirection = -Vector3.Cross(GroundNormal, HorizontalDirection);
            
            var horizontalOffset = HorizontalDirection * Size.x / 2;
            var verticalOffset = VerticalDirection * Size.y / 2;
            
            SouthWestPosition = CenterPosition - horizontalOffset - verticalOffset;
            SouthEastPosition = CenterPosition + horizontalOffset - verticalOffset;
            NorthWestPosition = CenterPosition - horizontalOffset + verticalOffset;
            NorthEastPosition = CenterPosition + horizontalOffset + verticalOffset;
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
            var directionToPosition = position - CenterPosition;
            var angleToPosition = Mathf.Atan2(directionToPosition.y, directionToPosition.x) * Mathf.Rad2Deg;
            var adjustedAngle = angleToPosition - TimeScaleLineAngle;
            adjustedAngle = Mathf.Repeat(adjustedAngle + 180f, 360f) - 180f;
            
            return Mathf.Sign(adjustedAngle);
        }
        
        // hacky way to get intersection of time scale line with map edge (XY plane version)
        private Vector3 GetTimeScaleLineIntersectionXYPlane(bool forward)
        {
            var lineDirection = forward ? TimeScaleLineDirection : -TimeScaleLineDirection;
            var lineEndTemp = CenterPosition + lineDirection * (Size.x + Size.y);
            
            var lineStart = new Vector2(CenterPosition.x, CenterPosition.y);
            var lineEnd = new Vector2(lineEndTemp.x, lineEndTemp.y);

            if (LineUtils.TryGetIntersection
                (
                    lineStart, 
                    lineEnd, 
                    new Vector2(SouthWestPosition.x, SouthWestPosition.y), 
                    new Vector2(SouthEastPosition.x, SouthEastPosition.y), 
                    out var intersection
                ))
            {
                return new Vector3(intersection.x, intersection.y, CenterPosition.z);
            }
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, 
                    lineEnd, 
                    new Vector2(SouthEastPosition.x, SouthEastPosition.y), 
                    new Vector2(NorthEastPosition.x, NorthEastPosition.y), 
                    out intersection
                ))
            {
                return new Vector3(intersection.x, intersection.y, CenterPosition.z);
            }
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, 
                    lineEnd, 
                    new Vector2(NorthEastPosition.x, NorthEastPosition.y), 
                    new Vector2(NorthWestPosition.x, NorthWestPosition.y), 
                    out intersection
                ))
            {
                return new Vector3(intersection.x, intersection.y, CenterPosition.z);
            }

            if (LineUtils.TryGetIntersection
                (
                    lineStart,
                    lineEnd,
                    new Vector2(NorthWestPosition.x, NorthWestPosition.y),
                    new Vector2(SouthWestPosition.x, SouthWestPosition.y),
                    out intersection
                ))
            {
                return new Vector3(intersection.x, intersection.y, CenterPosition.z);

            }

            Debug.LogError($"Could not find intersection of time scale line with map area.");
            return default;
        }
        
        // hacky way to get intersection of time scale line with map edge (XZ plane version)
        private Vector3 GetTimeScaleLineIntersectionXZPlane(bool forward)
        {
            var lineDirection = forward ? TimeScaleLineDirection : -TimeScaleLineDirection;
            var lineEndTemp = CenterPosition + lineDirection * (Size.x + Size.y);
            
            var lineStart = new Vector2(CenterPosition.x, CenterPosition.z);
            var lineEnd = new Vector2(lineEndTemp.x, lineEndTemp.z);

            if (LineUtils.TryGetIntersection
                (
                    lineStart, 
                    lineEnd, 
                    new Vector2(SouthWestPosition.x, SouthWestPosition.z), 
                    new Vector2(SouthEastPosition.x, SouthEastPosition.z), 
                    out var intersection
                ))
            {
                return new Vector3(intersection.x, CenterPosition.y, intersection.y);
            }
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, 
                    lineEnd, 
                    new Vector2(SouthEastPosition.x, SouthEastPosition.z), 
                    new Vector2(NorthEastPosition.x, NorthEastPosition.z), 
                    out intersection
                ))
            {
                return new Vector3(intersection.x, CenterPosition.y, intersection.y);
            }
            
            if (LineUtils.TryGetIntersection
                (
                    lineStart, 
                    lineEnd, 
                    new Vector2(NorthEastPosition.x, NorthEastPosition.z), 
                    new Vector2(NorthWestPosition.x, NorthWestPosition.z), 
                    out intersection
                ))
            {
                return new Vector3(intersection.x, CenterPosition.y, intersection.y);
            }

            if (LineUtils.TryGetIntersection
                (
                    lineStart,
                    lineEnd,
                    new Vector2(NorthWestPosition.x, NorthWestPosition.z),
                    new Vector2(SouthWestPosition.x, SouthWestPosition.z),
                    out intersection
                ))
            {
                return new Vector3(intersection.x, CenterPosition.y, intersection.y);
            }

            Debug.LogError($"Could not find intersection of time scale line with map area.");
            return default;
        }
    }
}