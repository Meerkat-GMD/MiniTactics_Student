# 미니 택틱스 4차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson04/Scenes/Lesson04.unity`를 엽니다.
새 Teacher3 완성 코드 16파일(namespace만 Lesson04로 변경), 네 개의 빈 State 선언,
검토된 완성 `UnitSpawner.cs`가 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Blue는 Play 때 (5,4)에 하나 생성되고 Black은 (6,4)에 고정되어 있습니다.
MapData 12×10, 이동 예산4, 평지1/숲2/산3/강 통행불가, 이동과 Undo는 이전 차시처럼 동작합니다.
거부된 이동은 선택·범위를 유지합니다. 유닛 루트는 즉시 옮겨지고 Visual만 따라옵니다.
Undo 버튼, EventSystem, TMP 폰트와 모든 필수 Inspector 참조가 연결되어 있습니다.

`InputState`, `InputStateMachine`, `IdleState`, `UnitSelectedState`의 본문은 수업에서 입력합니다.
InputState는 abstract 빈 선언이며 Idle/Selected의 상속도 수업 중 추가합니다.
Controller 도우미 추출 → 계약 → 머신 → Idle → Selected → Controller 연결 순서로
각 파일의 완성 블록을 저장할 때마다 Console 오류0개를 확인합니다.
교사용 진행안의 Inspector 단계에서 Play를 멈추고 Spawn Cells를3개
(2,2), (5,4), (8,6)으로 바꾸어 씬을 저장합니다. 공급된 Spawner 코드는 고치지 않습니다.
Tests/Editor/완성 State/DLL은 포함하지 않습니다. HTML은 State 이론 자료이고 전체 구현은 교사용 진행안에 있습니다.
