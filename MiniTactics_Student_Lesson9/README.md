# 미니 택틱스 9차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson09/Scenes/Lesson09.unity`를 엽니다. 표시 이름은 `Mini Tactics Lesson 09 Student`입니다.
새 Teacher8 완성 코드 43파일(namespace만 Lesson09로 변경), 이번 차시 제공 코드 1파일과 두 개의 빈 선언이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 8차시 끝과 같은 map01 지도에 양 진영 넷씩 여덟 병종 유닛이 섭니다.
이동하거나 제자리에서 다시 누른 유닛에게 대상이 있으면 행동 메뉴 [공격] [스킬] [대기]가 뜹니다. 검사는 강타, 힐러는 치유를 쓰고 궁수·창병은 스킬이 없습니다. Undo, 체력바·사망, 적 차례, 승패와 재시작이 동작합니다.

`SkillProgram`(빈 class), `ScriptedSkill`(빈 ScriptableObject)의 본문은 수업에서 입력합니다. `ScriptedSkill`의 `: Skill`과 `[CreateAssetMenu]`도 수업에서 씁니다.
`Skill`, `TurnManager`는 8차시 상태 그대로이며 수업에서 줄을 추가만 합니다(`Skill`의 protected 속성 두 줄, `TurnManager.PushAway`). 8차시 줄은 고치지 않습니다.

제공 코드: 새 `StageDefinition`(전장 이름·지도·배치를 묶어 읽는 자료형, 장면을 자동으로 바꾸지 않음). 나머지 제공 코드(`ActionMenu`, `CombatCalculator`, `FrameEffect`, `HealthBarView`, `MovementOverlayPool`, `PhaseHudView`, `UnitAnimator`, `UnitLook`)는 8차시와 같고 고치지 않습니다.
제공 데이터: `Maps`의 stage01~03, `Stages`의 Stage01(전초기지)·Stage02(숲속 길목)·Stage03(성채 앞마당), 완성된 두 전투 장면 `Lesson09_Stage02`·`Lesson09_Stage03`, 한글 글꼴의 휩쓸기·충격 화살·대치유 글자. File > Build Profiles의 Scene List에 세 장면이 켜져 있습니다.
`Skills`에는 8차시의 `PowerStrike`·`Heal`만 있습니다. 데이터 스킬 `Cleave`·`PushArrow`·`GroupHeal`은 수업에서 Create로 직접 만들고 값을 입력합니다. 병종 에셋의 Skill 연결과 첫 장면 `Lesson09`의 지도(stage01)·배치 변경도 수업에서 합니다.

교사용 진행안의 순서(명령 데이터 → 한 줄 검사 → 원문 전체 검사 → 원문 필드와 읽기 → 문맥 속성과 밀기 → 해석기 →
스킬 에셋과 병종 연결 → 첫 장면을 전초기지로)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
에셋 값과 Inspector 연결은 코드를 저장해 컴파일한 직후 Play를 끈 상태에서 합니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Bytecode 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
