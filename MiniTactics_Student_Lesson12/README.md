# 미니 택틱스 12차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson12/Scenes/Lesson12_Title.unity`를 엽니다. 표시 이름(Product Name)은 `미니 택틱스 성채 탈환`, 회사 이름은 `MiniTacticsCourse`입니다(11차시와 같은 제품 이름과 진행 저장 키).
새 Teacher11 완성 코드 58파일(namespace만 Lesson12로 변경), 이번 차시에 갱신·추가된 제공 코드 3파일과 네 개의 빈 선언이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 11차시 끝과 같은 Title → 전장 1~3 → Ending 캠페인입니다. 효과음·음소거·진행 저장·조작 안내가 동작하고, 피해 숫자와 죽음 먼지는 사건마다 새로 만들어졌다가 스스로 파괴됩니다(Hierarchy의 `Presentation Queue` > `Effects`). `Lesson12_PoolComparison`을 Play하면 Console에 안내 한 줄만 나옵니다.

`PoolLease`(빈 struct), `TransientPool`(빈 class), `TransientPresenter`·`PoolComparisonDriver`(빈 MonoBehaviour)의 본문은 수업에서 입력합니다.
11차시 파일 가운데 수업에서 고치는 것은 `PresentationQueue` 하나이고, 바뀌는 11차시 줄은 숫자·먼지를 만들고 지우던 일곱 줄뿐입니다(Presenter에게 보여 달라고 하는 줄로 바뀜).

제공 코드: 갱신된 `DamageNumber`·`FrameEffect`(시각 상태 초기화 `ResetVisual`, 한 프레임 진행 `Advance`, 자동 수명 선택 `SetAutomaticLifetime`; 자동 수명이 기본으로 켜져 있음)와 새 측정 도구 `PoolComparisonRunner`(실행 순서·시간·관리 할당 기록·결과 파일). 나머지 제공 코드(`ActionMenu`, `CampaignView`, `CombatCalculator`, `HealthBarView`, `MovementOverlayPool`, `PhaseHudView`, `StageCatalog`, `StageDefinition`, `UnitAnimator`, `UnitLook`)는 11차시와 같고 고치지 않습니다.
세 전장의 `Presentation Queue`에는 체크가 꺼진 빈 `TransientPresenter`가, `Lesson12_PoolComparison`의 `Pool Comparison`에는 완성 `PoolComparisonRunner`와 체크가 꺼진 빈 `PoolComparisonDriver`가 붙어 있습니다. 칸 연결과 켜기는 수업에서 합니다.
장면과 데이터는 11차시 끝 상태입니다. File > Build Profiles의 Scene List는 Title이 0번인 다섯 장면이 켜져 있고, 비교 장면은 목록에 있지만 꺼져 있습니다(실행 파일에 넣지 않음).

교사용 진행안의 순서(영수증과 풀 상태 → 빌려 주기·돌려받기 → 모두 회수·미리 만들기·닫기 → Presenter의 필드와 세 전장 연결 → 보여 주기·진행·회수·생명 주기 → 대기열 전환과 Presenter 켜기 → 비교 Driver의 필드와 연결 → 말단 동작·분배·구독 → Driver 켜기와 비교 Play)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
Inspector 연결은 필드를 저장해 컴파일한 직후 Play를 끈 상태에서 합니다. 모든 코드를 저장하기 전에는 Presenter와 Driver의 체크를 켜지 않습니다.
비교 결과 파일은 Runner의 Output Path에 적힌 폴더(기본은 이 컴퓨터의 `Application.persistentDataPath` 아래 `PoolComparison`)에 실행마다 새로 생깁니다. 진행 저장은 Editor의 PlayerPrefs(회사·제품 이름별)에 남고, 같은 컴퓨터에서 같은 제품 이름의 프로젝트(교사용 완성본, 11차시)는 같은 저장값을 씁니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Object Pool 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다. 실행 파일 조작은 `실행_조작_안내.md`, 출처와 사용 조건은 `배포_출처_고지.md`와 `배포고지` 폴더에 있습니다.
