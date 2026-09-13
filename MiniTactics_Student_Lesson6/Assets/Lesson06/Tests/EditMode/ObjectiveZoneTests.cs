using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class ObjectiveZoneTests
    {
        private GameObject _zoneObject;
        private GameObject _unitObject;

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_unitObject);
            Object.DestroyImmediate(_zoneObject);
        }

        [Test]
        public void Properties_ExposeTheConfiguredOwnerAndCell()
        {
            ObjectiveZone zone = CreateZone(Team.Enemy, new Vector2Int(8, 3));

            Assert.That(zone.Owner, Is.EqualTo(Team.Enemy));
            Assert.That(zone.Cell, Is.EqualTo(new Vector2Int(8, 3)));
        }

        [Test]
        public void GetResultFor_ReturnsPlayingForNull()
        {
            ObjectiveZone zone = CreateZone(Team.Enemy, Vector2Int.zero);

            Assert.That(zone.GetResultFor(null), Is.EqualTo(BattleResult.Playing));
        }

        [Test]
        public void GetResultFor_ReturnsPlayingForAFixedBlocker()
        {
            ObjectiveZone zone = CreateZone(Team.Enemy, Vector2Int.zero);
            Unit fixedBlocker = CreateUnit(Team.Player, canMove: false);

            Assert.That(zone.GetResultFor(fixedBlocker), Is.EqualTo(BattleResult.Playing));
        }

        [Test]
        public void GetResultFor_ReturnsPlayingForAnInactiveUnit()
        {
            ObjectiveZone zone = CreateZone(Team.Enemy, Vector2Int.zero);
            Unit player = CreateUnit(Team.Player, canMove: true);
            player.gameObject.SetActive(false);

            Assert.That(zone.GetResultFor(player), Is.EqualTo(BattleResult.Playing));
        }

        [Test]
        public void GetResultFor_ReturnsPlayingWhenAUnitEntersItsFriendlyZone()
        {
            ObjectiveZone zone = CreateZone(Team.Player, Vector2Int.zero);
            Unit player = CreateUnit(Team.Player, canMove: true);

            Assert.That(zone.GetResultFor(player), Is.EqualTo(BattleResult.Playing));
        }

        [Test]
        public void GetResultFor_PlayerEnteringEnemyZoneWins()
        {
            ObjectiveZone zone = CreateZone(Team.Enemy, Vector2Int.zero);
            Unit player = CreateUnit(Team.Player, canMove: true);

            Assert.That(zone.GetResultFor(player), Is.EqualTo(BattleResult.PlayerWon));
        }

        [Test]
        public void GetResultFor_EnemyEnteringPlayerZoneLoses()
        {
            ObjectiveZone zone = CreateZone(Team.Player, Vector2Int.zero);
            Unit enemy = CreateUnit(Team.Enemy, canMove: true);

            Assert.That(zone.GetResultFor(enemy), Is.EqualTo(BattleResult.PlayerLost));
        }

        private ObjectiveZone CreateZone(Team owner, Vector2Int cell)
        {
            _zoneObject = new GameObject("Objective Zone");
            ObjectiveZone zone = _zoneObject.AddComponent<ObjectiveZone>();
            SerializedObject serialized = new SerializedObject(zone);
            serialized.FindProperty("_owner").enumValueIndex = (int)owner;
            serialized.FindProperty("_cell").vector2IntValue = cell;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return zone;
        }

        private Unit CreateUnit(Team team, bool canMove)
        {
            _unitObject = new GameObject("Unit");
            Unit unit = _unitObject.AddComponent<Unit>();
            SerializedObject serialized = new SerializedObject(unit);
            serialized.FindProperty("_team").enumValueIndex = (int)team;
            serialized.FindProperty("_canMove").boolValue = canMove;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return unit;
        }
    }
}
