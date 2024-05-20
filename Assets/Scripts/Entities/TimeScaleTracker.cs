using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class TimeScaleTracker : MonoBehaviour
    {
        private MapModel _mapModel;
        public float Scale => _mapModel.GetTimeScaleAtPosition(transform.position);
        
        private void Start() => _mapModel = GameModel.Get<MapModel>();

        private void OnDrawGizmos()
        {
            if(_mapModel is null) return;
            
            Gizmos.color = _mapModel.GetTimeScaleAtPosition(transform.position) > 0 ? Color.red : Color.blue;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }
}
