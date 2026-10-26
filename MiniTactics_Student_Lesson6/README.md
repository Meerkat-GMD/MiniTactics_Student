# 미니 택틱스 6차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson06/Scenes/Lesson06.unity`를 엽니다.
새 Teacher5 완성 코드 26파일(namespace만 Lesson06으로 변경), 네 개의 빈 선언,
제공 코드 다섯 파일이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 `Unit Spawner`가 Blue 셋을 (2,2)·(5,4)·(8,6)에, Red 둘을 (9,8)·(9,3)에 만듭니다.
5차시 끝과 같은 동작입니다. 지형 비용 범위(Blue (5,4)는 30칸), 한 번만 이동과 Undo,
Enter·턴 종료 버튼의 적 차례, R·재시작 버튼이 동작합니다. 아직 HP·체력바·공격·승패는 없습니다.

`BattleResult`(값이 없는 빈 enum), `AttackState`(상속 표시가 없는 빈 class),
`UnitHealthBar`, `UnitPresentationBinding`(빈 MonoBehaviour)의 본문은 수업에서 입력합니다.
`Unit`, `GameManager`, `TurnManager`, `BattleController`, `UnitSelectedState`, `BoardView`, `EnemyAI`,
`RangeHighlighter`, `PhaseHud`, `TileData`는 5차시 상태 그대로이며 수업에서 줄을 추가하거나 조건을 덧붙입니다.

제공 코드는 고치지 않습니다. `HealthBarView`(막대 비율 표시), `FrameEffect`(한 번 재생되는 그림 연출),
`CombatCalculator`(숫자 셋으로 피해 계산)는 새 제공 코드이고, `UnitAnimator`(공격·피격·사망 그리기 추가)와
`UnitLook`(공격 그림 추가)은 갱신된 제공 그리기 코드입니다.

Unit 프리팹의 `Visual/Health Bar`는 꺼져 있고, 그 `UnitHealthBar`와 루트의 `UnitPresentationBinding`은
붙어만 있고 참조가 비어 있습니다. `Game Manager`의 새 Spawner 칸과 지형 TileData의 방어 값도 아직 없습니다.

교사용 진행안의 순서(결과·지형 방어 → Unit → 체력바·연출 구독자 → 승패 판정 → 적의 공격 →
아군 공격 상태 → 제자리 공격)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
새 필드는 저장해 컴파일한 직후 Play를 끈 상태에서 Inspector로 연결합니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Observer 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
