using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MiniTactics.Lesson04.Tests
{
    public sealed class UnitSpawnerTests
    {
        private GameObject _spawnerObject;
        private GameObject _prefabObject;

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_spawnerObject);
            Object.DestroyImmediate(_prefabObject);
        }

        [Test]
        public void SpawnAll_CreatesOneUnitPerConfiguredCellAndIsIdempotent()
        {
            _prefabObject = new GameObject("Unit Template");
            Unit prefab = _prefabObject.AddComponent<Unit>();
            _spawnerObject = new GameObject("Unit Spawner");
            UnitSpawner spawner = _spawnerObject.AddComponent<UnitSpawner>();
            SerializedObject serialized = new SerializedObject(spawner);
            serialized.FindProperty("_unitPrefab").objectReferenceValue = prefab;
            SerializedProperty cells = serialized.FindProperty("_spawnCells");
            cells.arraySize = 3;
            cells.GetArrayElementAtIndex(0).vector2IntValue = new Vector2Int(2, 2);
            cells.GetArrayElementAtIndex(1).vector2IntValue = new Vector2Int(5, 4);
            cells.GetArrayElementAtIndex(2).vector2IntValue = new Vector2Int(8, 6);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            Unit[] first = spawner.SpawnAll();
            Unit[] second = spawner.SpawnAll();

            Assert.That(first, Has.Length.EqualTo(3));
            Assert.That(second, Has.Length.EqualTo(3));
            Assert.That(_spawnerObject.transform.childCount, Is.EqualTo(3));
            Assert.That(first[0].transform.position, Is.EqualTo(new Vector3(2f, 2f, 0f)));
            Assert.That(first[2].transform.position, Is.EqualTo(new Vector3(8f, 6f, 0f)));
        }
    }
}
