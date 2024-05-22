using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DuckJam
{
    public static class GameModel
    {
        private static readonly Dictionary<Type, object> Models = new();

        /// <summary>
        /// Registers a model instance.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called from a MonoBehaviour's Awake method.
        /// This is to ensure that the model is registered before another object tries to access it.
        /// </remarks>
        /// <param name="model">The model instance</param>
        /// <typeparam name="T">The model type</typeparam>
        public static void Register<T>(T model) where T : class 
        {
            var type = typeof(T);
            if (!Models.TryAdd(type, model)) 
            {
                Debug.LogError($"{nameof(GameModel)}.{nameof(Register)}: {type.FullName} already registered");
            }
        }
        
        /// <summary>
        /// Gets a model instance.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called from a MonoBehaviour's Start method.
        /// This is to ensure that the model is already registered before trying to access it.
        /// </remarks>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The instance of the model</returns>
        /// <exception cref="ArgumentException"></exception>
        public static T Get<T>() where T : class 
        {
            var type = typeof(T);
            if (Models.TryGetValue(type, out var obj)) return obj as T;
            
            throw new ArgumentException($"{nameof(GameModel)}.{nameof(Register)}: {type.FullName} not registered");
        }
        
        /// <summary>
        /// Tries to get a model instance, returning a boolean indicating success.
        /// </summary>
        /// <remarks>
        /// This method is intended to be called from a MonoBehaviour's Start method.
        /// This is to ensure that the model is already registered before trying to access it.
        /// </remarks>>
        /// <param name="model">The model instance</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>Boolean indicating if the model was found</returns>
        public static bool TryGet<T>(out T model) where T : class 
        {
            if (Models.TryGetValue(typeof(T), out var obj)) 
            {
                model = obj as T;
                return true;
            }

            model = null;
            return false;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset() 
        {
            Models.Clear();
        }
    }
}
