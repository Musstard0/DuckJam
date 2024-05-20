using System;
using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class Enemy : MonoBehaviour
    {
        public float Speed { get; set; }
        public float CurrentTimeScale { get; set; } = 1f;
        public Vector3 TargetPosition { get; set; }

        public Color Color 
        {
            set
            {
                var meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer == null) return;
                meshRenderer.material.color = value;
            }
        }

        private void Awake()
        {
            TargetPosition = transform.position;
        }
    }
}