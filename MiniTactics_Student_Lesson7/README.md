# 미니 택틱스 7차시 학생 시작본

Unity 6000.5.3f1에서 `Assets/Lesson07/Scenes/Lesson07.unity`를 엽니다. 표시 이름은 `Mini Tactics Lesson 07 Student`입니다.
새 Teacher6 완성 코드 35파일(namespace만 Lesson07로 변경)과 두 개의 빈 선언이 있습니다. 시작본부터 오류 없이 Play할 수 있습니다.

Play하면 `Unit Spawner`가 Blue 셋을 (2,2)·(5,4)·(8,6)에, Red 둘을 (9,8)·(9,3)에 만듭니다. 6차시 끝과 같은 동작입니다.
다섯 유닛은 모두 검사 그림이고 HP 10·공격력 6·방어력 1·이동력 4이며, 이 값은 Unit 프리팹 하나에 들어 있습니다.
지형 비용 이동 범위, 한 번만 이동과 Undo, 체력바·피격·사망, 상하좌우 공격, 적 차례, 승패와 재시작이 동작합니다.

`UnitClass`(빈 ScriptableObject)와 `UnitPlacement`(빈 class)의 본문은 수업에서 입력합니다. `[CreateAssetMenu]`와 `[Serializable]`도 수업에서 붙입니다.
`Unit`, `UnitSpawner`, `TurnManager`는 6차시 상태 그대로이며 수업에서 Type Object에 맞게 고칩니다.
제공 코드(`CombatCalculator`, `FrameEffect`, `HealthBarView`, `MovementOverlayPool`, `PhaseHudView`, `UnitAnimator`, `UnitLook`)는 6차시와 같고 고치지 않습니다.

`Assets/Lesson07/UnitLooks`에 검사·궁수·힐러·창병의 Blue·Red 그림 데이터 여덟 개와 그 그림 파일이 준비되어 있습니다(힐러 그림 파일 이름은 `Monk_...`).
병종 에셋 폴더 `Assets/Lesson07/UnitClasses`와 병종 에셋은 없습니다. 수업에서 Create와 Duplicate로 직접 만들고 값을 입력합니다.
`Unit Spawner`에는 아직 Spawn Cells·Enemy Cells만 있고 병종 배치 값은 없습니다. 새 컴포넌트는 붙이지 않습니다.

교사용 진행안의 순서(UnitClass와 병종 에셋 → UnitPlacement → Unit의 두 인자 Initialize → UnitSpawner와 배치 입력 →
옛 Initialize·필드 삭제와 속성 전환 → 사거리)대로 블록마다 저장한 뒤 Console 오류 0개·경고 0개를 확인합니다.
에셋 값과 Inspector 연결은 필드를 저장해 컴파일한 직후 Play를 끈 상태에서 합니다.

Tests, Editor 씬 생성 도구, 완성 코드와 DLL은 포함하지 않습니다.
HTML은 Type Object 패턴 이론 자료이고, 구현 순서와 입력할 코드는 교사용 진행안에 있습니다.
