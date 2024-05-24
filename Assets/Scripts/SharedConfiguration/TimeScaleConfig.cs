using UnityEngine;

namespace DuckJam.SharedConfiguration
{
    [CreateAssetMenu(fileName = nameof(TimeScaleConfig), menuName = "DuckJam/" + nameof(TimeScaleConfig))]
    internal sealed class TimeScaleConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float minTimeScale = 0.5f;
        [SerializeField, Min(0f)] private float maxTimeScale = 1.5f;
        [SerializeField, Min(0f)] private float timeScaleChangeSpeed = .2f;

        [SerializeField] private Color slowColor = Color.blue;
        [SerializeField] private Color fastColor = Color.red;
        [SerializeField] private Color normalColor = Color.white;

        public float MinTimeScale => minTimeScale;
        public float MaxTimeScale => maxTimeScale;
        public float TimeScaleChangeSpeed => timeScaleChangeSpeed;
        
        public Color SlowColor => slowColor;
        public Color FastColor => fastColor;
        public Color NormalColor => normalColor;
        
        private void OnValidate()
        {
            if (minTimeScale > 1f) minTimeScale = 1f;
            if (maxTimeScale < 1f) maxTimeScale = 1f;
        }
    }
}