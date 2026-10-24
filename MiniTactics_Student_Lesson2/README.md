# 미니 택틱스 2차시 학생용 시작본

Unity 6000.5.3f1에서 `Assets/Lesson02/Scenes/Lesson02.unity`를 엽니다.
새 Teacher1의 이동·선택·점유 표시와 제공 그림/애니메이션을 그대로 이어받습니다.
Blue는 2칸 이동, Black은 이동 불가입니다. 선택 범위는 자기 칸 포함 13칸입니다.

수업에서 ICommand, IUndoableCommand, MoveUnitCommand, CommandHistory의 빈 선언을 완성하고 BattleController에 명령 실행·Undo·UI 입력 검사를 추가합니다. 새 타입의 using은 준비되어 있고 인터페이스 상속/구현은 수업에서 작성합니다.

`Lesson UI`는 처음에 비활성이며 Undo 버튼 On Click은 비어 있습니다.
교사용 진행안의 코드 저장 순서를 마쳐 OnUndoClicked와 UI 입력 검사까지 컴파일한 뒤 Canvas를 켜고 씬 BattleController.OnUndoClicked()를 연결합니다.
EventSystem, Canvas/버튼/TMP 라벨, 폰트 리소스와 패키지 참조는 준비되어 있습니다.

MovementOverlayPool, UnitAnimator, UnitLook는 이전 차시에서 이어받은 제공 렌더링입니다.
새 Command 완성 코드, Tests, Editor SceneBuilder는 배포본에 없습니다.
구현 순서와 전체 입력 코드는 Teacher2의 교사용_2차시_진행안.md를 따릅니다.
HTML 자료는 Command 패턴의 개념 설명에 사용합니다.
