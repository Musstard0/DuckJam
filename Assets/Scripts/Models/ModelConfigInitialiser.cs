using DuckJam.Configuration;
using UnityEngine;

namespace DuckJam.Models
{
    internal sealed class ModelConfigInitialiser : MonoBehaviour
    {
        [SerializeField] private TimeScaleConfig timeScaleConfig;
#if UNITY_EDITOR
        [SerializeField, TextArea] private string notes;
#endif
        
        private void Awake()
        {
            GameModel.Register(timeScaleConfig);
        }
    }
}