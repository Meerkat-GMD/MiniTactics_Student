# Mini Tactics Student — Lesson 05

Unity 6000.5.3f1에서 `Assets/Lesson05/Scenes/Lesson05.unity`를 엽니다.

시작은 완성 Teacher4입니다. Play하면 Blue (2,2),(5,4),(8,6)과 고정 Black (6,4), 총 4명이 보입니다. 이전 가중 이동/State/Command/Undo와 현재 포인터 UI 보호, 논리 루트/Visual 보간이 동작합니다. 새 차례 동작은 아직 없습니다.

`Team`, `BattlePhase`, `GameManager`, `EnemyAI`, `TurnManager`, `PhaseHud`는 빈 선언입니다. `PhaseHudView`만 독립 TMP/Button 표시 제공 코드이며 gameplay API를 호출하지 않습니다. 신규 End Turn/Restart/Phase Text/Instructions는 숨김이고 신규 버튼 listener는 비어 있습니다. 신규 필수 참조는 교사용 진행안의 코드 저장 직후 Inspector에서 직접 연결합니다.

제공 준비: 기존 그림/지형/MapData와 UnitLook, Red 그림, TMP 필수 리소스, NotoSansKR 원본/OFL/SDF(내장 glyph/atlas, Clear Dynamic Data On Build=false), UniTask 2.5.11와 runtime asmdef 참조. 지형 ClassroomResources 재임포트는 하지 않습니다. Tests/Editor C#나 완성 Teacher DLL을 배포하지 않습니다.

수업은 Teacher 저장소 `MiniTactics_Teacher_Lesson5/교사용_5차시_진행안.md`의 25개 독립 저장을 따릅니다. 최종에는 고정 Black을 삭제하고 3 Blue/2 Red만 spawner가 등록합니다. 패턴 HTML은 Singleton 이론 자료이며 구현/Inspector 순서는 진행안에 있습니다. Lesson6에서 공격·HP·전멸 승리를 이어갑니다.
