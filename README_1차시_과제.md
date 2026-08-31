# 미니 택틱스 1차시 학생용 과제

## 오늘 만드는 것

이번 차시에는 Tile Palette로 SRPG 전장을 직접 그리고, 마우스로 유닛을 선택해 이동시키는 핵심 규칙을 완성합니다.

- Strategy 0~3으로 구성한 Ground Tilemap
- Strategy 4~6으로 구성한 Decoration Tilemap
- 이동 가능한 유닛 클릭 선택
- 이동거리 2 범위 계산
- 이동 가능 셀은 초록색, 다른 유닛이 있는 셀은 빨간색 표시
- 초록색 목적지 타일 클릭 이동

유닛을 클릭해 선택한 뒤 이동할 타일을 클릭합니다.

## 프로젝트 열기

1. Unity Hub에서 이 폴더를 프로젝트로 추가합니다.
2. Unity `6000.5.3f1`로 엽니다.
3. `Assets/Lesson01/Scenes/Lesson01.unity`를 엽니다.
4. `Window > 2D > Tile Palette`와 `Window > General > Test Runner`를 엽니다.

## 제공된 구조

### Scene

- `Grid`: 셀 크기와 좌표계
- `Ground Tilemap`: 캐릭터가 설 수 있는 바닥
- `Decoration Tilemap`: 나무와 숲 장식
- `Board`: 실제 Ground 셀 판정과 이동 가능 범위 계산
- `BattleController`: 마우스 클릭에 따른 선택과 이동 처리
- `MovementOverlayRoot`: SpriteRenderer 오버레이의 부모
- `PlayerUnit`: 선택하고 이동할 수 있는 유닛
- `FixedUnit`: 통과해서 탐색하지만 도착할 수 없는 점유 유닛
- `Main Camera`: 전장을 표시

### Asset

- `Assets/Lesson01/Tiles/Strategy_0.asset` ~ `Strategy_6.asset`
- `Assets/Lesson01/Prefabs/Unit.prefab`
- 완성된 좌표 변환 코드
- 완성된 SpriteRenderer 오버레이 생성·재사용 코드
- BFS 시작 셀과 자료구조를 준비하는 코드

## 실습 1 — Tile Palette 생성과 맵 페인팅

1. `Window > 2D > Tile Palette`를 엽니다.
2. 새 Palette를 만들고 `Strategy_0~6` Tile Asset을 등록합니다.
3. Active Tilemap을 `Ground Tilemap`으로 선택합니다.
4. Strategy 0을 기본 잔디로 사용하고 1~3을 섞어 12×10 바닥을 칠합니다.
5. Active Tilemap을 `Decoration Tilemap`으로 바꿉니다.
6. Strategy 4~6 나무를 원하는 위치에 배치합니다.

Ground와 Decoration은 같은 Grid의 같은 셀에 겹쳐 칠할 수 있습니다. 나무 타일은 투명 배경이므로 Decoration에 칠해야 나무 아래의 Ground가 유지됩니다.

`BoardView`는 `(0,0)`부터 시작하는 사각형을 가정하지 않고 실제로 칠해진 Ground 셀을 확인합니다. 심화 실습에서는 `(-1,-3)`처럼 음수 셀부터 맵을 그려도 됩니다.

## 제공 코드 — 셀 좌표와 월드 좌표

`BoardView.CellToWorld()`와 `BoardView.WorldToCell()`은 완성된 상태로 제공합니다.

- Tile Palette로 칠한 셀 좌표를 게임 규칙의 좌표로 사용합니다.
- 셀 `(0,0)`의 중심은 월드 `(0,0,0)`입니다.
- 캐릭터와 오버레이는 `CellToWorld()`가 반환한 셀 중심에 배치됩니다.

수업에서는 이 변환이 필요한 이유를 확인하지만, Unity API 호출을 다시 작성하지는 않습니다.

## 실습 2 — 클릭으로 유닛 선택과 이동 구분

파일: `Assets/Lesson01/Scripts/BattleController.cs`

`TODO(학생) 1`에서 클릭 결과를 두 경우로 나눕니다.

1. `clickedUnit`이 존재하고 움직일 수 있으면 `Select(clickedUnit)`을 호출합니다.
2. 선택 처리가 목적지 이동으로 이어지지 않도록 즉시 반환합니다.
3. 클릭된 이동 가능 유닛이 없고 `SelectedUnit`이 존재하면 클릭한 월드 위치를 셀 좌표로 바꿉니다.
4. 변환한 셀로 `TryMoveSelectedUnit()`을 호출합니다.

마우스 버튼 입력, 화면 좌표에서 월드 좌표로 변환, 클릭된 유닛 탐색 코드는 제공되어 있습니다.

## 실습 3 — 실제로 칠해진 Ground 셀 판정

파일: `Assets/Lesson01/Scripts/BoardView.cs`

`TODO(학생) 2`에서 해당 셀에 실제 Ground Tile이 있는지 반환합니다.

- `CellBounds` 안에 있는지만 검사하면 중간에 뚫린 구멍도 이동 가능한 셀이 됩니다.
- `_tilemap.HasTile()`을 사용해 실제로 칠해진 셀만 통과시킵니다.
- `Vector2Int` 셀 좌표를 `Vector3Int`로 바꿀 때 Z는 0입니다.

## 실습 4 — 이동거리 2 BFS 완성

파일: `Assets/Lesson01/Scripts/BoardView.cs`

`TODO(학생) 3` 앞뒤에는 다음 코드가 이미 준비되어 있습니다.

- 선택 유닛의 위치로부터 `startCell` 계산
- 시작 셀에 Ground가 없을 때 빈 결과 반환
- 다른 유닛의 셀을 모은 `occupiedCells`
- 탐색 대기열 `frontier`
- 시작 셀을 등록한 방문 집합 `visited`

이 자료구조를 사용해 다음 규칙을 완성합니다.

1. `frontier`에 시작 셀과 거리 0을 넣습니다.
2. 대기열이 빌 때까지 앞의 항목을 하나씩 꺼냅니다.
3. 꺼낸 셀이 다른 유닛에게 점유됐는지 확인해 `MovementCell`을 결과에 추가합니다.
4. 현재 거리가 `MoveDistance` 이상이면 더 먼 이웃은 넣지 않습니다.
5. 상하좌우 이웃 중 처음 방문했고 실제 Ground가 있는 셀만 대기열에 넣습니다.
6. 점유 셀도 결과에는 빨간색으로 포함하고, 그 셀의 이웃 탐색은 계속합니다.

시작 셀도 초록색으로 표시되어야 합니다.

## 검증

- 이동 가능한 유닛을 클릭하면 SpriteRenderer 오버레이가 나타나는가?
- 시작 셀이 초록색인가?
- 이동거리 2 안의 실제 Ground 셀이 초록색인가?
- 다른 유닛의 셀이 빨간색인가?
- 빨간 셀 뒤쪽의 유효한 셀도 계산되는가?
- 빨간 셀을 클릭하면 이동하지 않는가?
- 초록 셀을 클릭하면 캐릭터가 해당 셀 중심으로 이동하는가?
- Ground의 빈 구멍과 Ground 밖 셀에는 오버레이가 나타나지 않는가?

Test Runner의 실패 이름과 메시지는 아직 완성하지 않은 규칙을 찾는 단서로 사용합니다.

## 완료 체크리스트

- [ ] 직접 만든 Tile Palette에 Strategy 0~6을 등록했다.
- [ ] Ground를 Strategy 0~3으로 12×10 칠했다.
- [ ] Decoration에 Strategy 4~6을 배치했다.
- [ ] `TODO(학생) 1`의 선택/이동 분기를 완성했다.
- [ ] `TODO(학생) 2`의 실제 Ground 셀 판정을 완성했다.
- [ ] `TODO(학생) 3`의 BFS를 완성했다.
- [ ] 시작 셀과 이동 가능 셀은 초록색으로 보인다.
- [ ] 다른 유닛이 있는 셀은 빨간색이며 그 뒤쪽 셀도 탐색된다.
- [ ] 초록색 목적지를 클릭하면 캐릭터가 한 타일 중심에 정확히 선다.

## 심화 과제

1. Ground 중간 셀 하나를 지우고 이동 범위에서 제외되는지 확인합니다.
2. 맵을 음수 셀부터 그린 뒤 셀 좌표와 캐릭터 위치를 확인합니다.
3. 고정 유닛을 옮기고 빨간 셀과 그 뒤의 초록 셀이 함께 바뀌는지 확인합니다.
