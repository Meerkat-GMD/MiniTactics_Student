# 미니 택틱스 4차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson04/Scenes/Lesson04.unity`를 엽니다.
새 Teacher3 완성 코드 16파일(namespace만 Lesson04로 변경), 네 개의 빈 State 선언,
제공 코드 `UnitSpawner.cs`가 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 `Unit Spawner`가 Blue 하나(`Unit 1`)를 (5,4)에 만들고, Black은 (6,4)에 고정되어 있습니다.
MapData 12×10과 공유 TileData 네 개(평지 1, 숲 2, 산 3, 강 통행 불가), 이동력 4의 범위는 3차시 끝과 같습니다.
Blue를 고르면 24칸이 초록으로 보입니다. 다른 유닛 칸은 지나가지도 멈추지도 못하고, 제자리 목적지는 거부됩니다.
이동과 Undo는 이전 차시처럼 동작하고, 거부된 클릭은 선택과 범위를 유지합니다.
유닛 루트는 즉시 옮겨지고 Visual만 따라옵니다.
Undo 버튼, EventSystem, TMP 폰트와 모든 필수 Inspector 참조가 연결되어 있습니다.

`InputState`(빈 abstract class), `InputStateMachine`, `IdleState`, `UnitSelectedState`의 본문과
Idle·Selected의 `: InputState` 상속 표시는 수업에서 입력합니다.
교사용 진행안의 순서(계약 → 머신 → Controller의 작업 → Idle → Selected → Controller 연결)대로
한 파일씩 저장할 때마다 Console 오류 0개·경고 0개를 확인합니다.
코드가 끝나면 Play를 끈 상태에서 `Unit Spawner`의 Spawn Cells를 세 칸
(2,2), (5,4), (8,6)으로 바꾸고 씬을 저장합니다. 제공된 `UnitSpawner.cs`는 고치지 않습니다.

Tests, Editor 씬 생성 도구, 완성 State 코드와 DLL은 포함하지 않습니다.
HTML은 State 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
