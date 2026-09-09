using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MiniTactics.Lesson05.Tests
{
    public sealed class InputStateMachineTests
    {
        [Test]
        public void ChangeState_ExitsPreviousBeforeEnteringNext()
        {
            List<string> events = new List<string>();
            InputStateMachine machine = new InputStateMachine();
            RecordingState idle = new RecordingState("idle", events);
            RecordingState selected = new RecordingState("selected", events);

            machine.ChangeState(idle);
            machine.ChangeState(selected);

            Assert.That(events, Is.EqualTo(new[]
            {
                "idle.enter",
                "idle.exit",
                "selected.enter"
            }));
            Assert.That(machine.Current, Is.SameAs(selected));
        }

        [Test]
        public void ChangeState_ToCurrentInstance_DoesNothing()
        {
            List<string> events = new List<string>();
            InputStateMachine machine = new InputStateMachine();
            RecordingState state = new RecordingState("idle", events);

            machine.ChangeState(state);
            machine.ChangeState(state);

            Assert.That(events, Is.EqualTo(new[] { "idle.enter" }));
        }

        [Test]
        public void Input_IsForwardedOnlyToCurrentState()
        {
            List<string> events = new List<string>();
            InputStateMachine machine = new InputStateMachine();
            RecordingState state = new RecordingState("selected", events);
            machine.ChangeState(state);

            machine.HandleBoardClick(null, new Vector2Int(2, 3));
            machine.HandleUndo();

            Assert.That(events, Is.EqualTo(new[]
            {
                "selected.enter",
                "selected.click(2, 3)",
                "selected.undo"
            }));
        }

        private sealed class RecordingState : InputState
        {
            private readonly string _name;
            private readonly List<string> _events;

            public RecordingState(string name, List<string> events)
            {
                _name = name;
                _events = events;
            }

            public override void Enter() => _events.Add($"{_name}.enter");
            public override void Exit() => _events.Add($"{_name}.exit");

            public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
            {
                _events.Add($"{_name}.click{cell}");
            }

            public override void HandleUndo() => _events.Add($"{_name}.undo");
        }
    }
}
