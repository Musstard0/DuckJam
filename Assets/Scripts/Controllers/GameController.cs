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
        
        private void OnDrawGizmos()
        {
            if(_mapModel is null) return;

        }
    }
}
