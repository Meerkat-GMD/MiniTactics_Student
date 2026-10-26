# 미니 택틱스 5차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson05/Scenes/Lesson05.unity`를 엽니다.
새 Teacher4 완성 코드 21파일(namespace만 Lesson05로 변경), 여섯 개의 빈 선언,
제공 코드 `PhaseHudView.cs`가 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 `Unit Spawner`가 Blue 셋을 (2,2)·(5,4)·(8,6)에 만들고, 고정 Black은 (6,4)에 있습니다.
4차시 끝과 같은 동작입니다. 지형 비용 범위(Blue (5,4)는 24칸), State 선택·이동·거부,
여러 단계 Undo가 동작합니다. 아직 차례·진영·적은 없습니다.

`Team`, `BattlePhase`(값이 없는 빈 enum), `GameManager`, `TurnManager`, `EnemyAI`, `PhaseHud`의
본문은 수업에서 입력합니다. 4차시에 제공 코드로 받은 `UnitSpawner.cs`는 오늘부터 수업에서 고칩니다.
`PhaseHudView.cs`는 글자와 턴 종료 버튼만 표시하는 제공 코드이며 고치지 않습니다.
`Game Manager`, `Turn Manager`, `Enemy AI`, `Lesson UI`의 새 컴포넌트는 붙어만 있고 참조가 비어 있습니다.
`Lesson UI` 아래 Phase Text, End Turn Button, Restart Button, Instructions는 꺼져 있고
새 버튼의 On Click은 비어 있습니다.

교사용 진행안의 순서(값 → GameManager → Unit → UnitSpawner → BoardView → 기록 → EnemyAI →
TurnManager → Controller → 행동 완료 → HUD)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
새 필드는 저장해 컴파일한 직후 Play를 끈 상태에서 Inspector로 연결하고, 고정 Black은 진행안의 단계에서 지웁니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Singleton 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
