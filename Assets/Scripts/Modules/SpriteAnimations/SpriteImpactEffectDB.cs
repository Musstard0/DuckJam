using System;
using System.Collections.Generic;
using UnityEngine;

namespace DuckJam.Modules
{
    [CreateAssetMenu(fileName = nameof(SpriteImpactEffectDB), menuName = "DuckJam/" + nameof(SpriteImpactEffectDB))]
    internal sealed class SpriteImpactEffectDB : ScriptableObject
    {
        // hacks lol
        private static readonly int[] RowCounts =
        {
            4, 
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            6, 6, 6, 6, 6, 6, 6, 6, 6
        };
        
        [SerializeField] private Sprite[] orange;
        [SerializeField] private Sprite[] purple;
        [SerializeField] private Sprite[] blue;
        [SerializeField] private Sprite[] white;

        public int EffectCount => RowCounts.Length;

        public ImpactFXSprites[] CreateEffects()
        {
            var fxArr = new ImpactFXSprites[EffectCount];

            var currentSpriteIndex = 0;

            for (var i = 0; i < fxArr.Length; i++)
            {
                var rowCount = RowCounts[i];
                
                var orangeArr = new Sprite[rowCount];
                var purpleArr = new Sprite[rowCount];
                var blueArr = new Sprite[rowCount];
                var greenArr = new Sprite[rowCount];
                var pinkArr = new Sprite[rowCount];
                var whiteArr = new Sprite[rowCount];
                
                for(var j = 0; j < rowCount; j++)
                {
                    var spriteIndex = currentSpriteIndex + j;
                    
                    orangeArr[j] = orange[spriteIndex];
                    purpleArr[j] = purple[spriteIndex];
                    blueArr[j] = blue[spriteIndex];
                    whiteArr[j] = white[spriteIndex];
                }
                
                fxArr[i] = new ImpactFXSprites(orangeArr, purpleArr, blueArr, greenArr, pinkArr, whiteArr);
                
                currentSpriteIndex += rowCount;
            }
            
            return fxArr;
        }
    }
    
    internal sealed class ImpactFXSprites
    {
        private readonly Sprite[] _orange;
        private readonly Sprite[] _purple;
        private readonly Sprite[] _blue;
        private readonly Sprite[] _white;

        public ImpactFXSprites(Sprite[] orangeArr, Sprite[] purpleArr, Sprite[] blueArr, Sprite[] greenArr, Sprite[] pinkArr, Sprite[] whiteArr)
        {
            _orange = orangeArr;
            _purple = purpleArr;
            _blue = blueArr;
            _white = whiteArr;
        }

        public IReadOnlyList<Sprite> Orange => _orange;
        public IReadOnlyList<Sprite> Purple => _purple;
        public IReadOnlyList<Sprite> Blue => _blue;
        public IReadOnlyList<Sprite> White => _white;

        public IReadOnlyList<Sprite> GetFramesForColor(ImpactFXColor color)
        {
            return color switch 
            {
                ImpactFXColor.Orange => Orange,
                ImpactFXColor.Purple => Purple,
                ImpactFXColor.Blue => Blue,
                ImpactFXColor.White => White,
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }
    }
}