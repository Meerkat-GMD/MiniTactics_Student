using System;
using UnityEngine;

namespace MiniTactics.Lesson08
{
    [Serializable]
    public sealed class UnitPlacement
    {
        [SerializeField] private UnitClass _unitClass;
        [SerializeField] private Vector2Int _cell;

        public UnitClass UnitClass => _unitClass;
        public Vector2Int Cell => _cell;
    }
}
