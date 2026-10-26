using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MiniTactics.Lesson03
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

        public Unit SelectedUnit { get; private set; }

        private void Awake()
        {
            _highlighter = new RangeHighlighter(_overlayRoot, _overlayPrefab, _board);
        }

        public void Select(Unit unit)
        {
            SelectedUnit = unit != null && unit.CanMove ? unit : null;
            _highlighter.Hide();
            if (SelectedUnit == null)
            {
                return;
            }

            _highlighter.Show(_board.CalculateMovementRange(SelectedUnit), SelectedUnit);
        }

        public bool TryMoveSelectedUnit(Vector2Int targetCell)
        {
            if (SelectedUnit == null || !_board.CanMoveTo(SelectedUnit, targetCell))
            {
                return false;
            }

            MoveUnitCommand command = new MoveUnitCommand(SelectedUnit, _board, targetCell);
            _history.Execute(command);
            SelectedUnit = null;
            _highlighter.Hide();
            return true;
        }

        public void OnUndoClicked()
        {
            if (_history.UndoLast() && SelectedUnit != null)
            {
                Select(SelectedUnit);
            }
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
            if (clickedUnit != null && clickedUnit.CanMove)
            {
                Select(clickedUnit);
                return;
            }

            if (SelectedUnit != null)
            {
                TryMoveSelectedUnit(_board.WorldToCell(worldPosition));
            }
        }
    }
}
