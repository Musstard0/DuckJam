using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class MapController : MonoBehaviour
    {
        [Header("Map Area")]
        [SerializeField] private Vector2 size = new(64, 64);
        [SerializeField] private Vector2 centerPosition = Vector2.zero;
        
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

        private void Update()
        {
            //transform.rotation = _mapModel.TimeScaleLineRotation;
        }
        
        private void OnDrawGizmos()
        {
            var mapModel = _mapModel ?? CreateMapModel();
            var timeScaleLineStart = mapModel.TimeScaleLineStartPosition;
            var timeScaleLineEnd = mapModel.TimeScaleLineEndPosition;
            
            Gizmos.color = Color.green;
            
            // draw outline of map area
            Gizmos.DrawLine(mapModel.SouthWestPosition, mapModel.SouthEastPosition);
            Gizmos.DrawLine(mapModel.SouthEastPosition, mapModel.NorthEastPosition);
            Gizmos.DrawLine(mapModel.NorthEastPosition, mapModel.NorthWestPosition);
            Gizmos.DrawLine(mapModel.NorthWestPosition, mapModel.SouthWestPosition);
            
            // draw time scale line
            Gizmos.DrawLine(timeScaleLineStart, timeScaleLineEnd);
            
            // draw points where time scale line intersects with the map edge
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(timeScaleLineStart, 1f);
            Gizmos.DrawWireSphere(timeScaleLineEnd, 1f);
        }
        
        private MapModel CreateMapModel()
        {
            return new MapModel(centerPosition, size)
            {
                TimeScaleLineRotationSpeed = timeScaleLineRotationSpeed
            };
        }
    }
}
