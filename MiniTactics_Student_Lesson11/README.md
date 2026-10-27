# 미니 택틱스 11차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson11/Scenes/Lesson11.unity`를 엽니다. 표시 이름(Product Name)은 `미니 택틱스 성채 탈환`, 회사 이름은 `MiniTacticsCourse`입니다(12차시와 같은 제품 이름).
새 Teacher10 완성 코드 48파일(namespace만 Lesson11로 변경), 이번 차시의 새 제공 코드 2파일과 열 개의 빈 선언이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 10차시 끝과 같은 전초기지(stage01)에 양 진영 넷씩 여덟 병종 유닛이 섭니다.
이동·공격·스킬·HP 변화가 사건 대기열로 차례로 재생되고, 연출 중에는 바깥 입력이 막히며, 결과는 마지막 연출 뒤에 보입니다. `Lesson11_Stage02`·`Lesson11_Stage03`도 따로 실행됩니다. 소리는 아직 나지 않습니다.

`SoundCue`(빈 enum), `CampaignSave`(빈 struct), `IAudioService`·`ISaveService`(빈 interface), `NullAudioService`·`NullSaveService`·`ClipAudioService`·`PlayerPrefsSaveService`(빈 class), `Services`(빈 static class), `CampaignController`(빈 MonoBehaviour)의 본문은 수업에서 입력합니다.
10차시 파일 네 개(`BattleController`, `PresentationQueue`, `GameManager`, `TurnManager`)는 수업에서 Service Locator에 필요한 줄을 더합니다. 10차시 줄 가운데 바뀌는 것은 `TurnManager.CanAct`의 조건 덧붙임 한 줄뿐입니다.

제공 코드: 새 `CampaignView`(여섯 캠페인 화면과 버튼 명령을 알리는 그림 부품)와 `StageCatalog`(장면 이름과 전장 정의를 묶은 데이터). 나머지 제공 코드(`ActionMenu`, `CombatCalculator`, `DamageNumber`, `FrameEffect`, `HealthBarView`, `MovementOverlayPool`, `PhaseHudView`, `StageDefinition`, `UnitAnimator`, `UnitLook`)는 10차시와 같고 고치지 않습니다.
제공 장면·데이터: `Lesson11_Title`·`Lesson11_Ending` 장면, `Stages/Campaign.asset`, `Audio`의 효과음 다섯 개. 다섯 장면의 `Campaign Bootstrap`(Audio Source와 체크가 꺼진 빈 `CampaignController`)과 꺼진 `Campaign UI`가 들어 있고, 칸 연결과 켜기는 수업에서 합니다.
세 전장의 장면과 데이터는 10차시 끝 상태입니다(대기열·GameManager·UnitSpawner 연결, `UnitPresentationBinding` 없는 `Unit.prefab`). File > Build Profiles의 Scene List에는 다섯 장면이 켜져 있고, 수업에서 Title을 0번으로 옮깁니다.

교사용 진행안의 순서(값과 계약·Null Object → 공통 접근점 → 소비자의 요청 줄 → 실제 공급자 → GameManager 결과 알림·입력 차단 →
캠페인 소유자의 필드와 다섯 장면 연결 → 진행 저장·음소거·도움말 → 장면 이동과 명령 → 장면 연결·결과 표시·등록과 해제 → 활성화)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
Inspector 연결은 필드를 저장해 컴파일한 직후 Play를 끈 상태에서 다섯 장면 모두에 합니다. 모든 코드를 저장하기 전에는 `Campaign Bootstrap`의 체크와 `Campaign UI`를 켜지 않습니다.
진행 저장은 Editor의 PlayerPrefs(회사·제품 이름별)에 남습니다. 같은 컴퓨터에서 같은 제품 이름의 프로젝트(교사용 완성본, 12차시)는 같은 저장값을 씁니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Service Locator 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
