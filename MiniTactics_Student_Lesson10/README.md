# 미니 택틱스 10차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson10/Scenes/Lesson10.unity`를 엽니다. 표시 이름은 `Mini Tactics Lesson 10 Student`입니다.
새 Teacher9 완성 코드 44파일(namespace만 Lesson10으로 변경), 이번 차시의 새 제공 코드 1파일·갱신된 제공 코드 2파일과 두 개의 빈 선언이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 9차시 끝과 같은 전초기지(stage01)에 양 진영 넷씩 여덟 병종 유닛이 섭니다.
창병 휩쓸기·궁수 충격 화살·힐러 대치유·검사 강타, Undo, 적 차례, 체력바·사망, 승패와 재시작이 동작하고, `Lesson10_Stage02`·`Lesson10_Stage03`도 따로 실행됩니다. 공격·스킬 그림과 체력바는 규칙이 바뀌는 순간 바로 바뀌고, 피해 숫자는 아직 없습니다.

`PresentationEvent`(빈 readonly struct), `PresentationQueue`(빈 MonoBehaviour)의 본문은 수업에서 입력합니다. 세 장면의 `Presentation Queue` 오브젝트에 빈 `PresentationQueue`와 자식 `Effects`가 붙어 있고, 칸 연결은 수업에서 합니다.
9차시 파일 열한 개(`UnitHealthBar`, `Unit`, `GameManager`, `UnitSpawner`, `TurnManager`, `BattleController`, `AttackState`, `SkillState`, `ActionMenuState`, `UnitSelectedState`, `PhaseHud`)는 수업에서 Event Queue에 필요한 줄을 더하거나 그 줄만 바꿉니다. `UnitPresentationBinding.cs`는 9차시 그대로 들어 있으며, 수업에서 `Prefabs/Unit.prefab`의 컴포넌트를 먼저 떼고 파일을 삭제합니다.

제공 코드: 새 `DamageNumber`(피해·회복 숫자를 잠시 띄우는 그림 부품)와 `Prefabs/DamageNumber.prefab`. 갱신된 `UnitAnimator`(수동 재생 도구와 시간 상수, 9차시 자동 재생도 그대로)와 `FrameEffect`(부모를 정해 효과를 만드는 `SpawnOwned`). 나머지 제공 코드(`ActionMenu`, `CombatCalculator`, `HealthBarView`, `MovementOverlayPool`, `PhaseHudView`, `StageDefinition`, `UnitLook`)는 9차시와 같고 고치지 않습니다.
장면·데이터는 9차시 끝 상태입니다(스킬 에셋 다섯 개와 병종 연결, 첫 장면의 전초기지 지도·배치, Stage02·03, Build Profiles의 세 장면). 오늘은 새 에셋을 만들지 않습니다.

교사용 진행안의 순서(체력바 수동 표시 → 사건 → 대기열 → 재생 → 소비자와 등록 입구 → GameManager → 유닛에게 대기열 전달 →
생산자 연결과 옛 관찰자 삭제 → 행동 뒤 정리와 바깥 입력 나누기 → 연출 중 입력 차단과 결과 표시)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
Inspector 연결은 필드를 저장해 컴파일한 직후 Play를 끈 상태에서 세 장면 모두에 합니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Event Queue 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
