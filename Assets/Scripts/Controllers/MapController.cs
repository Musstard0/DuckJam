using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Controllers
{
    internal sealed class MapController : MonoBehaviour
    {
        private MapModel _mapModel;
        private Quaternion _initialRotation;

        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();

            _initialRotation = Quaternion.LookRotation(-_mapModel.GroundNormal);
            transform.rotation = _initialRotation;
        }

        private void Update()
        {
            transform.rotation = _initialRotation * Quaternion.AngleAxis(_mapModel.TimeScaleLineAngle, Vector3.forward);
        }
    }
}
