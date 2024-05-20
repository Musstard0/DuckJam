using UnityEngine;

namespace DuckJam.Configuration
{
    [CreateAssetMenu(fileName = nameof(TimeScaleConfig), menuName = "DuckJam/" + nameof(TimeScaleConfig))]
    internal sealed class TimeScaleConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float minTimeScale = 0.5f;
        [SerializeField, Min(0f)] private float maxTimeScale = 1.5f;
        [SerializeField, Min(0f)] private float timeScaleChangeSpeed = .2f;

        public float MinTimeScale => minTimeScale;
        public float MaxTimeScale => maxTimeScale;
        public float TimeScaleChangeSpeed => timeScaleChangeSpeed;
        
        private void OnValidate()
        {
            if (minTimeScale > 1f) minTimeScale = 1f;
            if (maxTimeScale < 1f) maxTimeScale = 1f;
        }
    }
}