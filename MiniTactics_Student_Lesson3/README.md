# 미니 택틱스 3차시 학생용 시작본

Unity 6000.5.3f1에서 `Assets/Lesson03/Scenes/Lesson03.unity`를 엽니다.
2차시가 끝난 상태에서 출발합니다. 직접 칠한 12×10 지도, Blue(5,4)·고정 Black(6,4), 이동력 4 안의 범위 표시,
이동 Command와 여러 단계 Undo, 오른쪽 위 Undo 버튼이 동작합니다. 아직 모든 칸의 이동 비용은 1입니다.
Blue를 선택하면 시작 칸을 포함한 38칸이 초록으로 표시됩니다. Black이 선 칸은 지나가지도 멈추지도 못하고,
지금 서 있는 칸을 목적지로 고르면 이동하지 않습니다.

이전 차시의 12파일(BoardView, Unit, MovementRange, RangeHighlighter, BattleController, Command 네 파일,
제공 렌더링 MovementOverlayPool·UnitAnimator·UnitLook)은 Teacher2에서 namespace만 바꿨습니다.

수업에서 TileData, MapData, Board, MapLoader의 빈 선언을 완성합니다. 새 타입의 using은 준비되어 있고,
TileData와 MapData의 `[CreateAssetMenu]`와 모든 멤버는 수업에서 작성합니다.
이전 파일 가운데 고치는 것은 BoardView 하나입니다. Map Data 필드와 Awake를 추가하고,
`IsInside`와 `TryGetMovementCost`의 본문만 공유 지형 데이터를 읽게 바꿉니다. 이동 범위 탐색 코드는 고치지 않습니다.

**데이터 가져오기는 TileData와 MapData가 모두 컴파일된 다음입니다.**
처음 Assets에는 네 지형 데이터와 map01이 없습니다. 프로젝트 루트의
`ClassroomResources/Lesson03TerrainData.unitypackage`를 Assets > Import Package > Custom Package로 가져오고 완료를 기다립니다.
패키지는 다섯 데이터 에셋과 두 폴더 메타만 포함하며 코드·씬·그림이 없습니다.
크기 1607바이트, SHA256 `bd63e6198922f7afcca31792a339b4e7c740aea7f39f6490df5840f4f52f9118`입니다.
네 규칙은 평지 1·숲 2·산 3·강 1(통행 불가)이고 각각 같은 이름의 `Tiles` 에셋을 참조합니다.
map01은 Width 12·Height 10·Cells 120이며, 위쪽 행부터 저장한 네 에셋의 공유 참조입니다.

BoardView의 Map Data 필드를 입력해 컴파일한 뒤 Board 오브젝트의 Map Data에 map01을 연결하고 씬을 저장합니다.
그다음 Awake를 추가합니다. Play 밖에서는 직접 칠한 이전 지도가 보이고, Play에 들어가면 Awake가 MapData로 다시 그립니다.
Play를 끄면 직접 칠한 지도로 돌아오는 것이 정상입니다. 다시 칠하지 않습니다.
완성하면 Blue의 범위는 지형 비용에 맞는 24칸이 됩니다.
Play 중에 TileData 값을 바꾸면 Play를 꺼도 남으므로, 수업 실험 뒤에는 원래 값으로 되돌리고 저장합니다.

Tests와 Editor SceneBuilder는 배포본에 없습니다.
구현 순서와 전체 입력 코드는 Teacher3의 `교사용_3차시_진행안.md`를 따릅니다.
Tiny Swords 그림과 라이선스 고지는 `Assets/Lesson03/Art`에서 확인합니다.
HTML 자료는 Flyweight 패턴의 개념 설명에 사용합니다.
