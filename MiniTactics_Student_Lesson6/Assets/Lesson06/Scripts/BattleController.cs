using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MiniTactics.Lesson06
{
    public sealed class BattleController : MonoBehaviour
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _overlayRoot;
        [SerializeField] private SpriteRenderer _overlayPrefab;
        [SerializeField] private LayerMask _unitLayerMask = ~0;
        private RangeHighlighter _highlighter;
        private readonly CommandHistory _history = new CommandHistory();
        private InputStateMachine _stateMachine;

        public Unit SelectedUnit { get; private set; }

        private void Awake()
        {
            _highlighter = new RangeHighlighter(_overlayRoot, _overlayPrefab, _board);
            _stateMachine = new InputStateMachine();
            _stateMachine.ChangeState(new IdleState(this));
        }

        public void Select(Unit unit)
        {
            _stateMachine.ChangeState(CanSelect(unit)
                ? new UnitSelectedState(this, unit)
                : new IdleState(this));
        }

        internal bool CanSelect(Unit unit) => GameManager.Instance.Turns.CanAct(unit);

        internal void ShowSelection(Unit unit)
        {
            SelectedUnit = unit;
            _highlighter.Show(_board.CalculateMovementRange(unit), unit);
        }

        internal void ClearSelection()
        {
            SelectedUnit = null;
            _highlighter.Hide();
        }

        internal bool ExecuteMove(Unit unit, Vector2Int targetCell)
        {
            if (!CanSelect(unit) || !_board.CanMoveTo(unit, targetCell))
            {
                return false;
            }

            _history.Execute(new MoveUnitCommand(unit, _board, targetCell));
            return true;
        }

        internal bool UndoLast() => _history.UndoLast();

        public void OnUndoClicked()
        {
            if (!CanAcceptInput()) return;
            _stateMachine.HandleUndo();
        }

        private bool CanAcceptInput() => GameManager.Instance.Turns.Phase == BattlePhase.Player;

        public void OnEndTurnClicked()
        {
            if (!CanAcceptInput()) return;
            Select(null);
            _history.Clear();
            GameManager.Instance.Turns.EndPlayerPhase();
        }

        internal void CompletePlayerMove()
        {
            if (GameManager.Instance.Turns.AreAllPlayersMoved()) OnEndTurnClicked();
        }

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = screenPosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            return hits.Count > 0;
        }

        private void Update()
        {
            if (!CanAcceptInput()) return;
            if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                OnEndTurnClicked();
                return;
            }
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            if (IsPointerOverUI(screenPosition))
            {
                return;
            }

            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPosition, _unitLayerMask);
            Unit clickedUnit = hit != null ? hit.GetComponentInParent<Unit>() : null;
            _stateMachine.HandleBoardClick(clickedUnit, _board.WorldToCell(worldPosition));
        }
    }
}
