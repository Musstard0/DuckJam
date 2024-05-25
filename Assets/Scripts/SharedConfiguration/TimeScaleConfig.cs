using UnityEngine;

namespace DuckJam.SharedConfiguration
{
    [CreateAssetMenu(fileName = nameof(TimeScaleConfig), menuName = "DuckJam/" + nameof(TimeScaleConfig))]
    internal sealed class TimeScaleConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float minTimeScale = 0.5f;
        [SerializeField, Min(1f)] private float maxTimeScale = 1.5f;
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

        public Color GetTimeScaleColor(float timeScale)
        {
            timeScale = Mathf.Clamp(timeScale, minTimeScale, maxTimeScale);
            
            if (timeScale > 1f)
            {
                return Color.Lerp(normalColor, fastColor, (timeScale - 1f) / (maxTimeScale - 1f));
            }
            
            if (timeScale < 1f)
            {
                return Color.Lerp(normalColor, slowColor, (1f - timeScale) / (1f - minTimeScale));
            }
            
            return normalColor;
        }
        
        private void OnValidate()
        {
            if (minTimeScale > 1f) minTimeScale = 1f;
            if (maxTimeScale < 1f) maxTimeScale = 1f;
        }
    }
}