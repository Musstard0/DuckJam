using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class MapModelInitialiser : MonoBehaviour
    {
        [Header("Map Area")]
        [SerializeField] private Plane plane = Plane.XY;
        [SerializeField] private Vector2 size = new(10, 10);
        [SerializeField] private Vector3 centerPosition = Vector3.zero;
        
        [Header("Time Scale Line")]
        [SerializeField] private float timeScaleLineRotationSpeed = 8f;
        
        
        private MapModel _mapModel;
        
        private void OnValidate()
        {
            size = new Vector2(Mathf.Max(size.x, 1f), Mathf.Max(size.y, 1f));
            
            if(_mapModel is null) return;
            _mapModel.TimeScaleLineRotationSpeed = timeScaleLineRotationSpeed;
        }

        private void Awake()
        {
            _mapModel = CreateMapModel();
            GameModel.Register(_mapModel);
        }

        private void OnDrawGizmos()
        {
            var mapModel = _mapModel ?? CreateMapModel();
            
            Gizmos.color = Color.green;
            
            // draw outline of map area
            Gizmos.DrawLine(mapModel.SouthWestPosition, mapModel.SouthEastPosition);
            Gizmos.DrawLine(mapModel.SouthEastPosition, mapModel.NorthEastPosition);
            Gizmos.DrawLine(mapModel.NorthEastPosition, mapModel.NorthWestPosition);
            Gizmos.DrawLine(mapModel.NorthWestPosition, mapModel.SouthWestPosition);
            
            // draw time scale line
            var directionOffset = mapModel.TimeScaleLineDirection * 50f;
            Gizmos.DrawLine(mapModel.CenterPosition - directionOffset, mapModel.CenterPosition + directionOffset);




            var p = mapModel.CenterPosition + directionOffset;

        }

        private MapModel CreateMapModel()
        {
            return new MapModel(centerPosition, size, plane == Plane.XY ? Vector3.back : Vector3.up)
            {
                TimeScaleLineRotationSpeed = timeScaleLineRotationSpeed
            };
        }
        
        private enum Plane : byte
        {
            XY = 0,
            XZ = 1,
        }
    }
}
