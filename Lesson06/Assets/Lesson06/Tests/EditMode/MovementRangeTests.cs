using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace MiniTactics.Lesson06.Tests
{
    public sealed class MovementRangeTests
    {
        [Test]
        public void Calculate_UsesTerrainCostAndBudget()
        {
            Board board = MapLoader.Parse("PFP");

            MovementRangeResult result =
                MovementRange.Calculate(board, new Vector2Int(0, 0), 3);

            Assert.That(result.TryGetCost(new Vector2Int(0, 0), out int startCost), Is.True);
            Assert.That(startCost, Is.Zero);
            Assert.That(result.TryGetCost(new Vector2Int(1, 0), out int forestCost), Is.True);
            Assert.That(forestCost, Is.EqualTo(2));
            Assert.That(result.TryGetCost(new Vector2Int(2, 0), out int farCost), Is.True);
            Assert.That(farCost, Is.EqualTo(3));
        }

        [Test]
        public void Calculate_SkipsRiverAndCellsBeyondBudget()
        {
            Board board = MapLoader.Parse("PRP\nPMP");

            MovementRangeResult result =
                MovementRange.Calculate(board, new Vector2Int(0, 0), 2);

            Assert.That(result.ReachableCells, Has.Member(new Vector2Int(0, 1)));
            Assert.That(result.ReachableCells, Has.No.Member(new Vector2Int(1, 1)));
            Assert.That(result.ReachableCells, Has.No.Member(new Vector2Int(1, 0)));
            Assert.That(result.ReachableCells, Has.No.Member(new Vector2Int(2, 0)));
        }

        [Test]
        public void Calculate_ReplacesAnEarlierExpensiveRoute()
        {
            Board board = MapLoader.Parse("FP\nPP");

            MovementRangeResult result =
                MovementRange.Calculate(board, new Vector2Int(0, 0), 3);

            Assert.That(result.TryGetCost(new Vector2Int(1, 1), out int cost), Is.True);
            Assert.That(cost, Is.EqualTo(2));
        }

        [Test]
        public void GetPathTo_ReturnsLowestCostAdjacentPath()
        {
            Board board = MapLoader.Parse("FP\nPP");
            MovementRangeResult result =
                MovementRange.Calculate(board, new Vector2Int(0, 0), 3);

            Vector2Int[] path = result.GetPathTo(new Vector2Int(1, 1)).ToArray();

            Assert.That(path, Is.EqualTo(new[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1)
            }));
        }

        [Test]
        public void GetPathTo_ReturnsEmptyForUnreachableDestination()
        {
            Board board = MapLoader.Parse("PRP");
            MovementRangeResult result =
                MovementRange.Calculate(board, new Vector2Int(0, 0), 10);

            Assert.That(result.GetPathTo(new Vector2Int(2, 0)), Is.Empty);
        }

        [Test]
        public void Calculate_ReturnsEmptyWhenStartIsMissingOrBlocked()
        {
            Board board = MapLoader.Parse("RP");

            Assert.That(
                MovementRange.Calculate(board, new Vector2Int(3, 0), 3).ReachableCells,
                Is.Empty);
            Assert.That(
                MovementRange.Calculate(board, new Vector2Int(0, 0), 3).ReachableCells,
                Is.Empty);
        }

        [Test]
        public void Calculate_RejectsInvalidInputs()
        {
            Board board = MapLoader.Parse("P");

            Assert.That(
                () => MovementRange.Calculate(null, Vector2Int.zero, 1),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => MovementRange.Calculate(board, Vector2Int.zero, -1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Calculate_DoesNotTraverseThroughBlockedCells()
        {
            Board board = MapLoader.Parse("PPPPP");
            IReadOnlyCollection<Vector2Int> blockedCells = new HashSet<Vector2Int>
            {
                new Vector2Int(2, 0)
            };

            MovementRangeResult result = MovementRange.Calculate(
                board,
                new Vector2Int(0, 0),
                4,
                blockedCells);

            Assert.That(result.ReachableCells, Has.Member(new Vector2Int(1, 0)));
            Assert.That(result.ReachableCells, Has.No.Member(new Vector2Int(2, 0)));
            Assert.That(result.ReachableCells, Has.No.Member(new Vector2Int(3, 0)));
            Assert.That(result.ReachableCells, Has.No.Member(new Vector2Int(4, 0)));
        }
    }
}
