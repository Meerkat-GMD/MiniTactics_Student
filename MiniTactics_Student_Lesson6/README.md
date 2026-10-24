# Mini Tactics Student — Lesson 06

Unity 6000.5.3f1에서 `Assets/Lesson06/Scenes/Lesson06.unity`를 엽니다.

시작은 검증된 새 Teacher5입니다. Play하면 Blue (2,2),(5,4),(8,6), Red (9,8),(9,3), 총 5명이 보입니다. 가중 이동/점유 경로/State/Command/Undo, 현재 포인터 UI 보호, 논리 루트/Visual 보간, 아군·적군 턴과 재시작이 동작합니다. 아직 HP 막대·공격·승패는 없습니다.

빈 신규 선언은 `BattleResult`, `AttackState`, `UnitHealthBar`, `UnitPresentationBinding` 네 개입니다. 앞 차시의 모든 게임 코드는 namespace만 바꿨습니다. HealthBarView/FrameEffect와 독립 UnitAnimator/UnitLook 갱신은 그림 제공 코드이며 CombatCalculator는 숫자만 계산합니다. Unit의 HP/이벤트와 Observer 구독, 전투/결과 연동은 수업에서 입력합니다. `Visual/Health Bar`는 비활성이고 새 Observer는 부착만 되어 있습니다. 필수 참조는 각 코드 저장 뒤 Inspector에서 연결합니다.

제공 준비: Blue/Red Idle/Run/Attack과 Dust 원본 프레임, HP bar geometry와 Fill 피벗, 기존 지형/MapData, TMP 필수 리소스, NotoSansKR 원본/OFL/SDF(내장 glyph/atlas, Clear Dynamic Data On Build=false), UniTask 2.5.11와 runtime asmdef 참조. 기존 UI와 listener는 각각 한 개씩 유지합니다. 지형 ClassroomResources 재임포트는 하지 않습니다. Tests/Editor C#나 완성 Teacher DLL을 배포하지 않습니다.

수업은 Teacher 저장소 `MiniTactics_Teacher_Lesson6/교사용_6차시_진행안.md`의 19개 독립 저장과 필드 선언 직후 Inspector 연결을 따릅니다. 매 저장마다 오류0을 확인합니다. 최종에는 HP10·공격6·방어1·이동4, 지형 방어0/1/2/0, 인접 공격, HP/피격/사망 Observer와 전멸 승패가 연결됩니다. 패턴 HTML의 배포 승인은 별도 단계이며 구현/Inspector 순서는 진행안에 있습니다.
