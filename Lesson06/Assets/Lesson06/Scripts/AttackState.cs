using UnityEngine;

namespace MiniTactics.Lesson06
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

        public override void Enter() => _controller.ShowAttackTargets(_attacker);
        public override void Exit() => _controller.ClearSelection();

        public override void HandleBoardClick(Unit clickedUnit, Vector2Int cell)
        {
            // 학습 과제: 유효 공격과 빈 공간 건너뛰기를 구분해 행동을 완료한다.
        }

        public override void HandleUndo()
        {
            // 공격 선택 중의 입력 정책을 이 상태에서 완성한다.
        }
    }
}
