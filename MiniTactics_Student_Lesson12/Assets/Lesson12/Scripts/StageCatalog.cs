using System;
using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class StageCatalog : ScriptableObject
    {
        [SerializeField] private string _titleScene;
        [SerializeField] private string _endingScene;
        [SerializeField] private string[] _battleScenes;
        [SerializeField] private StageDefinition[] _stages;

        public string TitleScene => _titleScene;
        public string EndingScene => _endingScene;
        public string BattleScene(int index) => _battleScenes[index];
        public StageDefinition Stage(int index) => _stages[index];
        public int IndexOf(string scene) => Array.IndexOf(_battleScenes, scene);
    }
}
