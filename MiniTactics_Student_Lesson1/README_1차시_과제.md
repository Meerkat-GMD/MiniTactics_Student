# 미니 택틱스 1차시 시작본

Unity 6000.5.3f1에서 이 폴더를 프로젝트로 엽니다.
씬은 `Assets/Lesson01/Scenes/Lesson01.unity`입니다.
첫 수업에는 이전 Lesson0 프로젝트가 없습니다.

시작 시 Ground Tilemap은 비어 있고 두 검사 그림만 Idle을 재생합니다.
클릭 선택과 이동은 아직 없습니다. Play 진입 시 오류가 없어야 합니다.

수업에서 입력하는 파일은 `Scripts/BoardView.cs`, `Unit.cs`, `MovementRange.cs`,
`RangeHighlighter.cs`, `BattleController.cs` 다섯 개입니다. 현재 using, namespace, 빈 타입만 있습니다.
`MovementRange.cs`에는 빈 `MovementRangeResult`와 빈 `MovementRange` 두 타입이 있습니다.
완성 제공 `MovementOverlayPool`, `UnitLook`, `UnitAnimator`와 그림 에셋은 유지합니다.
Tests와 Editor/SceneBuilder는 학생 시작본에 포함하지 않습니다.

교사용 1차시 진행안 순서대로 보드 좌표·칸 비용 → Unit → 범위 결과 → 범위 계산 →
보드의 다른 유닛 칸·범위·목적지 판정 → 범위 표시 → Controller 초기화/선택/이동 → Update를 작성합니다.
완성 블록 하나를 저장할 때마다 Console 오류 0개를 확인하고 새 필드가 나타난 다음 Inspector를 연결합니다.
이전 파일을 통째로 다시 입력하지 않고 안내한 멤버를 추가합니다.

Unit 필드를 작성한 뒤 두 검사의 Move Budget=4를 확인하고, 파란 Can Move=true,
**검은 Can Move=false**를 직접 설정합니다.
검은 검사도 새 필드의 초기값 true를 받으므로 이름만으로는 고정되지 않습니다.
제공 UnitAnimator의 Visual/Body/Look과 Visual 자식의 SpriteRenderer 연결을 유지합니다.

마지막에는 제공 Terrain Palette로 Ground Tilemap의 셀 x=0..11, y=0..9를 채웁니다.
Plain 전체 → Forest x=1,y=2..7 → Mountain x=9,y=6..9 → River y=1,x=7..11.
Grid 위치는 (-0.5,-0.5,0), 파란 위치는 (5,4,0), 검은 위치는 (6,4,0)입니다.

완료 목표는 파란 선택 시 시작 칸을 포함한 38칸 표시, 검은 검사 칸 (6,4) 제외,
돌아가는 비용 4로 (7,4) 이동, 비용이 4를 넘는 (8,4)·(9,4) 거절,
성공 후 선택/표시 해제, 그림의 이동 애니메이션입니다.
이동력은 4이고 네 지형은 오늘 모두 한 칸 비용 1입니다. 다른 유닛이 있는 칸은 지나갈 수 없습니다.
보드 가장자리와 빈 구멍은 범위에서 빠집니다.
구체적인 전체 입력 코드와 Inspector 연결표는 교사용 진행안을 따릅니다.
HTML 자료는 객체지향 책임 분리의 이론 자료입니다.
