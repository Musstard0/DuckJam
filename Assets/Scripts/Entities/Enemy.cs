using System;
using DuckJam.Models;
using UnityEngine;

namespace DuckJam.Entities
{
    internal sealed class Enemy : MonoBehaviour
    {
        public Vector3 TargetPosition { get; set; }

        private void Awake()
        {
            TargetPosition = transform.position;
        }
    }
}