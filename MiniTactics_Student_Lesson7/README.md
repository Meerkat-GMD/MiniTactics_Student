# Mini Tactics Student — Lesson 07

Unity 6000.5.3f1에서 `Assets/Lesson07/Scenes/Lesson07.unity`를 엽니다.

시작은 검증된 새 Teacher6입니다. Play하면 Blue (2,2),(5,4),(8,6), Red (9,8),(9,3), 총 5명이 보입니다. 모두 HP10·공격6·방어1·이동4이며 인접 일반 공격을 합니다. 가중 이동/점유 경로/State/Command/Undo, 현재 포인터 UI 보호, 논리 루트/Visual 보간, 아군·적군 턴과 재시작, 활성 HP바·피격·사망 Observer와 전멸 승패가 동작합니다.

빈 신규 선언은 `UnitClass`, `UnitPlacement` 두 개입니다. 앞 차시의 35개 런타임 코드는 namespace만 바꿨습니다. UnitClass에는 ScriptableObject 상속만 있고 CreateAssetMenu/필드는 없으며 UnitPlacement의 Serializable도 수업에서 입력합니다. 새 컴포넌트를 붙이지 않습니다. 네 병종의 Blue/Red UnitLook과 사용 그림은 제공되지만 완성 병종 에셋·새 placement 배열·Default Class 참조는 없습니다. UnitHealthBar/UnitPresentationBinding과 활성 `Visual/Health Bar`는 이전 차시 그대로 연결되어 있습니다.

제공 준비: Blue/Red Idle/Run/Attack과 Dust 원본 프레임, HP bar geometry와 Fill 피벗, 기존 지형/MapData, TMP 필수 리소스, NotoSansKR 원본/OFL/SDF(내장 glyph/atlas, Clear Dynamic Data On Build=false), UniTask 2.5.11와 runtime asmdef 참조. 기존 UI와 listener는 각각 한 개씩 유지합니다. 지형 ClassroomResources 재임포트는 하지 않습니다. Tests/Editor C#나 완성 Teacher DLL을 배포하지 않습니다.

수업은 Teacher 저장소 `MiniTactics_Teacher_Lesson7/교사용_7차시_진행안.md`의 아홉 독립 저장과 필드 선언 직후 Inspector 연결을 따릅니다. 매 저장마다 오류0을 확인합니다.03 뒤 실제 Unit 프리팹의 Default Class를 연결한 뒤04로 전환하고,06 뒤 네 병종을 Create/Duplicate로 만들고4+4 배치를 저장한 뒤07로 전환합니다.08/09에서 옛 입력을 지웁니다. 최종에는 검사·궁수·힐러·창병의 공유 정의, 양 진영8명과 궁수 사거리2가 동작합니다. 힐러는 이번에는 일반 공격을 합니다. 패턴 HTML의 개편 승인은 별도 Task3이며 구현/Inspector 순서는 진행안에 있습니다.

Windows에서는 프로젝트를 `D:/Class/MT7`처럼 짧은 로컬 경로에 풉니다. 긴 중첩 경로에서 패키지 셰이더 임포트가 실패한 사례가 있습니다. 표시 이름은 `Mini Tactics Lesson 07 Student`입니다.
