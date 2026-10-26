# 미니 택틱스 8차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson08/Scenes/Lesson08.unity`를 엽니다. 표시 이름은 `Mini Tactics Lesson 08 Student`입니다.
새 Teacher7 완성 코드 35파일(namespace만 Lesson08로 변경), 이번 차시 제공 코드 3파일과 다섯 개의 빈 선언이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 `Unit Spawner`가 병종 에셋(`UnitClasses`의 Swordsman·Archer·Healer·Lancer)으로 양 진영 넷씩 여덟 유닛을 만듭니다. 7차시 끝과 같은 동작입니다.
HP 14·9·7·16, 병종별 이동 범위와 사거리(궁수 2, 나머지 1), 이동 뒤 곧바로 뜨는 공격 대상 칸, Undo, 체력바·사망, 적 차례, 승패와 재시작이 동작합니다. 행동 메뉴와 스킬은 아직 없습니다.

`Skill`(빈 abstract ScriptableObject), `PowerStrikeSkill`·`HealSkill`(빈 ScriptableObject), `SkillState`·`ActionMenuState`(빈 class)의 본문은 수업에서 입력합니다.
두 스킬의 `: Skill`, 두 상태의 `: InputState`, `[CreateAssetMenu]`도 수업에서 씁니다.
`Unit`, `TurnManager`, `UnitClass`, `BattleController`, `AttackState`는 7차시 상태 그대로이며 수업에서 Subclass Sandbox에 맞게 줄을 추가합니다.

제공 코드: 새 `ActionMenu`(행동 메뉴 패널과 버튼 이벤트), 갱신된 `UnitAnimator`(`PlaySkill`)·`UnitLook`(스킬 프레임). 나머지 제공 코드(`CombatCalculator`, `FrameEffect`, `HealthBarView`, `MovementOverlayPool`, `PhaseHudView`)는 7차시와 같고 고치지 않습니다.
`UnitLooks` 여덟 에셋에 스킬 프레임이, 한글 글꼴에 메뉴 글자가 준비되어 있습니다. 씬의 `Lesson UI/Action Menu`(켜짐)와 자식 `Action Panel`(꺼짐)에 공격·스킬·대기 버튼이 있습니다.
스킬 에셋 폴더 `Assets/Lesson08/Skills`와 스킬 에셋은 없습니다. 수업에서 Create로 직접 만들고 값을 입력합니다. 병종 에셋의 Skill 칸과 `BattleController`의 Action Menu 칸도 수업에서 생기고 연결합니다.

교사용 진행안의 순서(회복·범위 조회·보너스를 받는 Attack → Skill → 두 스킬과 스킬 에셋 → 병종의 Skill 칸 →
스킬 처리와 SkillState → Action Menu 연결과 ActionMenuState → 메뉴 전환과 조건)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
에셋 값과 Inspector 연결은 필드를 저장해 컴파일한 직후 Play를 끈 상태에서 합니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Subclass Sandbox 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
