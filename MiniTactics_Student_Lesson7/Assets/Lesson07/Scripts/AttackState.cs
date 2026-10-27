using UnityEngine;
namespace MiniTactics.Lesson07
{
    public sealed class AttackState : InputState
    {
        private readonly BattleController _controller;
        private readonly Unit _attacker;
        public AttackState(BattleController controller, Unit attacker)
        {
            _controller = controller;
            _attacker = attacker;
        }
        public override void Enter() => _controller.ShowAttackTargets(_attacker);
        public override void Exit() => _controller.ClearSelection();
        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            if (!_controller.CanAcceptInput()) return;
            if (!_controller.CanAttack(_attacker, clickedUnit))
            {
                _controller.RequestAction(_attacker, false);
                return;
            }
            _controller.Attack(_attacker, clickedUnit);
            _controller.Select(null);
            _controller.CompleteAction(_attacker);
        }
        public override void HandleUndo()
        {
            if (!_controller.CanAcceptInput()) return;
            if (_attacker.HasMoved) _controller.UndoLast();
            _controller.Select(null);
        }
    }
}
