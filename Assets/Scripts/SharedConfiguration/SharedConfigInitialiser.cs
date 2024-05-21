using UnityEngine;

namespace DuckJam.SharedConfiguration
{
    internal sealed class SharedConfigInitialiser : MonoBehaviour
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