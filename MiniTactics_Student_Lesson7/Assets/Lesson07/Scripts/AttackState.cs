using UnityEngine;

namespace MiniTactics.Lesson07
{
    public sealed class AttackState : InputState
    {
        private readonly InputStateMachine _machine;
        private readonly BattleController _controller;
        private readonly Unit _attacker;

        public AttackState(InputStateMachine machine, BattleController controller, Unit attacker)
        {
            _machine = machine;
            _controller = controller;
            _attacker = attacker;
        }

        public override void Enter()
        {
            _controller.ShowAttackTargets(_attacker);
        }

        public override void Exit()
        {
            _controller.ClearSelection();
        }

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (clickedUnit != null && _controller.CanAttack(_attacker, clickedUnit))
            {
                _controller.Attack(_attacker, clickedUnit);
            }

            _machine.ChangeState(new IdleState(_machine, _controller));
            _controller.CompleteAction(_attacker);
        }

        public override void HandleUndo()
        {
            if (_attacker.HasMoved)
            {
                _controller.UndoLast();
            }

            _machine.ChangeState(new IdleState(_machine, _controller));
        }
    }
}
