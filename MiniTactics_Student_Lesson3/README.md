# 미니 택틱스 3차시 시작본

Unity **6000.5.3f1**에서 `Assets/Lesson03/Scenes/Lesson03.unity`를 엽니다.
시작하면 새2차시의 수동12×10지도, Blue(5,4)/고정 Black(6,4), 거리2/13칸 범위,
이동 Command와 활성 Undo가 동작합니다. Blue의 콜라이더 중앙 재클릭은 재선택입니다.

런타임17파일 중11파일은 검증된 Teacher2에서 namespace만 바꿨습니다.
새 TileData/Board/MapData/MapLoader/MovementRange/RangeHighlighter의6파일7타입은
using·namespace·빈 선언뿐입니다. 이번 수업에서 본문과 연결을 직접 작성합니다.
MovementCell은 이전 BFS의 실제 소비자가 사라진 뒤 삭제합니다.
독립 렌더링 MovementOverlayPool/UnitAnimator/UnitLook와 Command4파일은 이어받습니다.
학생 배포본에는 Tests/Editor C#이 없습니다.

교사용 `교사용_3차시_진행안.md`의21개 한 파일 저장을 순서대로 따릅니다.
매 완성 블록 저장마다 컴파일 종료/Console 오류0개를 확인합니다.
진행안은 Teacher 저장소의 `MiniTactics_Teacher_Lesson3`에 있습니다.

**데이터 가져오기는 TileData와 MapData 전체가 컴파일된 다음입니다.**
초기 Assets에는 네 지형 데이터와 map01이 없습니다. 프로젝트 루트의
`ClassroomResources/Lesson03TerrainData.unitypackage`를
Assets > Import Package > Custom Package로 가져오고 완료를 기다립니다.
빈 ScriptableObject 상태에서 먼저 가져오거나 저장하지 않습니다.
패키지는 다섯 데이터 에셋과 두 폴더 메타만 포함하며 코드/완성 씬/그림 의존성이 없습니다.
크기1607bytes, SHA256 `bd63e6198922f7afcca31792a339b4e7c740aea7f39f6490df5840f4f52f9118`입니다.

가져온 네 규칙은 평지1/숲2/산3/강1(통행불가), 각각 같은 이름의 `Tiles` 에셋을 참조합니다.
map01은 Width12/Height10/Cells120이며 위쪽 행부터 저장한 공유 참조입니다.
진행안의120개 참조 확인을 끝내고 BoardView 필드/질의를 작성한 뒤
Board의 Map Data에 map01을 연결하고, 다음 저장에서 Awake를 추가합니다.
완성 Teacher 씬이나 빌더로 학생 씬을 교체하지 않습니다.
Play 밖에서는 이전 수동 Tilemap 미리보기가 남을 수 있고,
Play에 들어갈 때 Awake가 MapData로120칸을 다시 그립니다.

Controller를 가중 표시로 바꾼 직후부터 BoardView 이동 허가를 바꾸기 전까지는
**컴파일 전용**입니다. 허가까지 바꾼 뒤 플레이합니다.
마지막 Unit 필드를 `_moveBudget`으로 이관한 다음에는
Unit 프리팹과 Blue/Black 두 씬 인스턴스의 **Move Budget을 모두4**로 명시 설정합니다.
Blue Can Move는켜짐, Black은꺼짐, 기존 Undo 콜백 한 개는 유지합니다.
최종 결과는16파일, 가중30칸 범위/강·점유 거부/점유 너머 탐색과 Undo입니다.

Tiny Swords 그림과 라이선스 고지는 `Assets/Lesson03/Art`에서 확인합니다.
발표/강의 HTML 자료는 Flyweight의 개념과 코드 설명용이며 실습 순서는 교사용 진행안을 따릅니다.
