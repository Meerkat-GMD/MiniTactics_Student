using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MiniTactics.Lesson03.Tests
{
    public sealed class CommandHistoryTests
    {
        [Test]
        public void ExecuteAndUndoLast_ProcessCommandsInReverseOrder()
        {
            List<string> events = new List<string>();
            CommandHistory history = new CommandHistory();

            history.Execute(new SpyCommand(events, "first"));
            history.Execute(new SpyCommand(events, "second"));

            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history.UndoLast(), Is.True);
            Assert.That(history.UndoLast(), Is.True);
            Assert.That(
                events,
                Is.EqualTo(new[]
                {
                    "first.Execute",
                    "second.Execute",
                    "second.Undo",
                    "first.Undo"
                }));
        }

        [Test]
        public void EmptyAndNullRequests_DoNotCreateHistoryEntries()
        {
            CommandHistory history = new CommandHistory();

            Assert.That(history.UndoLast(), Is.False);
            Assert.Throws<ArgumentNullException>(() => history.Execute(null));
            Assert.That(history.Count, Is.Zero);
        }

        private sealed class SpyCommand : IUndoableCommand
        {
            private readonly List<string> _events;
            private readonly string _name;

            public SpyCommand(List<string> events, string name)
            {
                _events = events;
                _name = name;
            }

            public void Execute()
            {
                _events.Add($"{_name}.Execute");
            }

            public void Undo()
            {
                _events.Add($"{_name}.Undo");
            }
        }
    }
}
