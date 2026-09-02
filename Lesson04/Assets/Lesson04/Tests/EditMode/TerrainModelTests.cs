using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MiniTactics.Lesson04.Tests
{
    public sealed class TerrainModelTests
    {
        [Test]
        public void Parse_ReusesCanonicalTerrainObjectsAcrossCells()
        {
            Board board = MapLoader.Parse("PFP\nMRF");

            Assert.That(board.GetTerrain(new Vector2Int(0, 1)), Is.SameAs(TerrainTypes.Plain));
            Assert.That(board.GetTerrain(new Vector2Int(2, 1)), Is.SameAs(TerrainTypes.Plain));
            Assert.That(board.GetTerrain(new Vector2Int(1, 1)), Is.SameAs(TerrainTypes.Forest));
            Assert.That(board.GetTerrain(new Vector2Int(2, 0)), Is.SameAs(TerrainTypes.Forest));
            Assert.That(board.GetTerrain(new Vector2Int(0, 0)), Is.SameAs(TerrainTypes.Mountain));
            Assert.That(board.GetTerrain(new Vector2Int(1, 0)), Is.SameAs(TerrainTypes.River));
        }

        [Test]
        public void TerrainRules_ExposeWeightedWalkability()
        {
            Assert.That(TerrainTypes.Plain.MovementCost, Is.EqualTo(1));
            Assert.That(TerrainTypes.Forest.MovementCost, Is.EqualTo(2));
            Assert.That(TerrainTypes.Mountain.MovementCost, Is.EqualTo(3));
            Assert.That(TerrainTypes.River.IsWalkable, Is.False);
        }

        [Test]
        public void Constructor_RejectsInvalidSharedData()
        {
            Assert.That(() => new TerrainType("", 1, true), Throws.ArgumentException);
            Assert.That(
                () => new TerrainType("Broken", 0, true),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase("")]
        [TestCase("PP\n\nPP")]
        [TestCase("PP\nP")]
        [TestCase("PX")]
        public void Parse_RejectsMalformedMaps(string mapText)
        {
            Assert.That(() => MapLoader.Parse(mapText), Throws.ArgumentException);
        }

        [Test]
        public void Board_ReportsMissingCellsInsteadOfInventingTerrain()
        {
            Board board = MapLoader.Parse("PF");

            Assert.That(board.Contains(new Vector2Int(1, 0)), Is.True);
            Assert.That(board.Contains(new Vector2Int(2, 0)), Is.False);
            Assert.That(
                () => board.GetTerrain(new Vector2Int(2, 0)),
                Throws.TypeOf<KeyNotFoundException>());
        }
    }
}
