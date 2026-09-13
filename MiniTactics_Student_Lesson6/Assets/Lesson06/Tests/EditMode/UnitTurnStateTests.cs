using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class UnitTurnStateTests
    {
        private readonly List<GameObject> _unitObjects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject unitObject in _unitObjects)
            {
                Object.DestroyImmediate(unitObject);
            }
        }

        [Test]
        public void SetMoved_ChangesTurnEligibilityWithoutChangingStaticMoveCapability()
        {
            Unit unit = CreateUnit(Team.Player, canMove: true);

            unit.SetMoved(true);

            Assert.That(unit.HasMoved, Is.True);
            Assert.That(unit.CanMove, Is.True);
        }

        [Test]
        public void FixedBlocker_IsNotATurnUnit()
        {
            Unit unit = CreateUnit(Team.Player, canMove: false);

            Assert.That(unit.IsTurnUnit, Is.False);
        }

        [TestCase(Team.Player)]
        [TestCase(Team.Enemy)]
        public void Team_ReportsConfiguredIdentity(Team team)
        {
            Unit unit = CreateUnit(team, canMove: true);

            Assert.That(unit.Team, Is.EqualTo(team));
        }

        [Test]
        public void Teams_ApplyExpectedSpriteTints()
        {
            Unit player = CreateUnit(Team.Player, canMove: true);
            Unit enemy = CreateUnit(Team.Enemy, canMove: true);

            Color playerTint = player.GetComponent<SpriteRenderer>().color;
            Color enemyTint = enemy.GetComponent<SpriteRenderer>().color;

            Assert.That(playerTint, Is.EqualTo(new Color(0.35f, 0.75f, 1f)));
            Assert.That(enemyTint, Is.EqualTo(new Color(1f, 0.4f, 0.4f)));
        }

        [Test]
        public void FixedBlocker_RetainsConfiguredSpriteTintAfterEnable()
        {
            GameObject unitObject = new GameObject("Fixed Unit", typeof(SpriteRenderer));
            unitObject.SetActive(false);
            Unit unit = unitObject.AddComponent<Unit>();
            SpriteRenderer spriteRenderer = unit.GetComponent<SpriteRenderer>();
            Color fixedBlockerTint = new Color(1f, 0.5f, 0f);
            spriteRenderer.color = fixedBlockerTint;

            SerializedObject serializedUnit = new SerializedObject(unit);
            serializedUnit.FindProperty("_canMove").boolValue = false;
            serializedUnit.ApplyModifiedPropertiesWithoutUndo();
            unitObject.SetActive(true);
            _unitObjects.Add(unitObject);

            Assert.That(spriteRenderer.color, Is.EqualTo(fixedBlockerTint));
        }

        private Unit CreateUnit(Team team, bool canMove)
        {
            GameObject unitObject = new GameObject("Unit", typeof(SpriteRenderer));
            unitObject.SetActive(false);
            Unit unit = unitObject.AddComponent<Unit>();
            SerializedObject serializedUnit = new SerializedObject(unit);
            serializedUnit.FindProperty("_team").enumValueIndex = (int)team;
            serializedUnit.FindProperty("_canMove").boolValue = canMove;
            serializedUnit.ApplyModifiedPropertiesWithoutUndo();
            unitObject.SetActive(true);
            _unitObjects.Add(unitObject);
            return unit;
        }
    }
}
