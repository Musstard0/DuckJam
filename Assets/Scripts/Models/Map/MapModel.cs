using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class MapModel
    {
        private Quaternion _initialRotation;
        
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
        
        public MapModel(Vector3 position, Vector2 size, Vector3 normal)
        {
            CenterPosition = position;
            Size = size;
            GroundNormal = normal;
            
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
        /// Get the time scale at a given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>1 if position on fast side of line, -1 if position is on slow side of line</returns>
        public float GetTimeScaleAtPosition(Vector3 position)
        {
            var directionToPosition = position - CenterPosition;
            var angleToPosition = Mathf.Atan2(directionToPosition.y, directionToPosition.x) * Mathf.Rad2Deg;
            var adjustedAngle = angleToPosition - TimeScaleLineAngle;
            adjustedAngle = Mathf.Repeat(adjustedAngle + 180f, 360f) - 180f;
            
            return Mathf.Sign(adjustedAngle);
        }
    }
}