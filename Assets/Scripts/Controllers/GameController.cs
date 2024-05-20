using System;
using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Controllers
{
    internal sealed class GameController : MonoBehaviour
    {
        private MapModel _mapModel;
        
        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _mapModel.RotateTimeScaleLine(deltaTime);
        }

        private void OnGUI()
        {
            if(_mapModel is null) return;
            
            var angle = _mapModel.TimeScaleLineAngle;
            
            GUI.Label(new Rect(10, 30, 200, 20), $"Time scale line angle: {angle}");
        }

        private void OnDrawGizmos()
        {
            if(_mapModel is null) return;

            
            
            Gizmos.color = Color.green;
            var directionOffset = _mapModel.TimeScaleLineDirection * 50f;
            Gizmos.DrawLine(_mapModel.CenterPosition - directionOffset, _mapModel.CenterPosition + directionOffset);
            
        }
    }
}
