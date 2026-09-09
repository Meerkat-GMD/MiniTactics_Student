using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniTactics.Lesson06
{
    [ExecuteAlways]
    public sealed class Unit : MonoBehaviour
    {
        [FormerlySerializedAs("_moveDistance")]
        [SerializeField, Min(0)] private int _moveBudget = 4;
        [SerializeField] private bool _canMove = true;
        [SerializeField] private Team _team = Team.Player;
        [SerializeField] private bool _hasMoved;
        [SerializeField, Min(1)] private int _maxHp = 10;
        [SerializeField, Min(0)] private int _currentHp = 10;
        [SerializeField, Min(0)] private int _attackPower = 6;
        [SerializeField, Min(0)] private int _defense = 1;

        public int MoveBudget => _moveBudget;
        public bool CanMove => _canMove;
        public Team Team => _team;
        public bool HasMoved => _hasMoved;
        public bool IsTurnUnit => _canMove;
        public int MaxHp => _maxHp;
        public int CurrentHp => _currentHp;
        public int AttackPower => _attackPower;
        public int Defense => _defense;
        public bool IsAlive => _currentHp > 0;

        public event Action<Unit> HpChanged;
        public event Action<Unit, Vector2Int, Vector2Int> Moved;
        public event Action<Unit> Died;

        private void Awake()
        {
            ApplyTeamTint();
        }

        private void OnEnable()
        {
            ApplyTeamTint();
        }

        private void ApplyTeamTint()
        {
            if (!_canMove)
            {
                return;
            }

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = _team == Team.Player
                    ? new Color(0.35f, 0.75f, 1f)
                    : new Color(1f, 0.4f, 0.4f);
            }
        }

        public void SetMoved(bool moved) => _hasMoved = moved;

        public void Configure(Team team, bool canMove, int moveBudget = 4)
        {
            _team = team;
            _canMove = canMove;
            _moveBudget = Mathf.Max(0, moveBudget);
            _hasMoved = false;
            ApplyTeamTint();
        }

        public void ConfigureCombat(int maxHp, int currentHp, int attackPower, int defense)
        {
            _maxHp = Mathf.Max(1, maxHp);
            _currentHp = Mathf.Clamp(currentHp, 0, _maxHp);
            _attackPower = Mathf.Max(0, attackPower);
            _defense = Mathf.Max(0, defense);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || !IsAlive)
            {
                return;
            }

            _currentHp = Mathf.Max(0, _currentHp - damage);
            // 학습 과제: HP 변경과 사망 전이의 알림 및 수명 처리를 연결한다.
        }

        public void MoveTo(BoardView board, Vector2Int targetCell)
        {
            if (board == null)
            {
                return;
            }

            transform.position = board.CellToWorld(targetCell);
            // 학습 과제: 실제 셀 변경을 관찰자에게 알린다.
        }
    }
}
