using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Controllers
{
    internal sealed class MapController : MonoBehaviour
    {
        private MapModel _mapModel;

        private void Start()
        {
            _mapModel = GameModel.Get<MapModel>();
            GetComponent<MeshRenderer>().enabled = true;
        }

        private void Update()
        {
            transform.rotation = _mapModel.TimeScaleLineRotation;
        }
    }
}
