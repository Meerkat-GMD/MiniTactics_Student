# Mini Tactics Student project instructions

## Presentation HTML: preserve the user's chosen format

Before creating, revising or reviewing any lesson presentation HTML, read
[docs/presentation-format.md](docs/presentation-format.md) and open the actual
Lesson 2 reference linked there. This is a persistent user requirement recorded
on 2026-09-15 and applies to future lessons, including work in new sessions or
other checkouts.

- Reuse the Lesson 2 screen layout, HTML/CSS and bottom navigation.
- Follow its problem → first attempt → limitation → responsibility boundary →
  concept/pattern → short code and worked result → judgment teaching sequence.
- Make the visible slides sufficient for beginner self-study. A formatting
  change alone does not satisfy a request to improve the explanation.
- Do not add practice appendices, hidden notes, an explanation mode or a sidebar
  unless the user requests them. Keep the separate textbook deliverable and the
  course's implementation work; this rule concerns presentation composition.
- Preserve current-source accuracy and conceptual scope. Check rendered desktop
  and mobile layouts against the reference before calling the work complete.

Use the user's later explicit instructions if they change this decision, and
update the guide accordingly. Do not reopen the format choice as a routine
approval question.

## Materials explain only the lesson's pattern (user decision, 2026-09-20)

The user asked that the materials explain which pattern is used, with code, and
not what the lesson implements: "5차시의 경우 SINGLETON에 대한 내용만 있어야 해."
This applies to both the presentation and the textbook-style lecture HTML and
overrides older clauses that conflict with it. The rule text is the section
"내용 범위" in [docs/presentation-format.md](docs/presentation-format.md).

- Cover the problem the pattern solves, a limited first attempt, structure and
  roles, code, a traced run, costs and when to apply it.
- Leave out the gameplay features the lesson adds (turn rules, enemy AI, damage
  formula, attack input, HUD, key bindings, UniTask). Implementation order, full
  code and Inspector wiring belong only in the Teacher repository's
  `교사용_<N>차시_진행안.md`.
- Always show code. Every real excerpt must match the current Teacher scripts
  line by line; mark abridged excerpts and label invented first-attempt code as
  "설계 비교용 가상 코드".
- Theory-corner patterns the engine provides (Game Loop, Update Method,
  Component, Prototype) get one or two slides at the end.
- For a lesson that was already taught, quote the code the students wrote.
  Lessons 3 and 4 therefore quote `TerrainType`/`RuntimeTerrain` and the
  `StateMachine` property.
- Do not recreate code-writing-order HTML pages; the lesson 3 and 4 pages were
  deleted at the user's request.

## Student starter projects (user decision, 2026-09-20)

The user found too much finished code in the starters. The starter for lesson N
must behave exactly like the end of lesson N-1, and the class types every line
of lesson N together.

- Scripts = Teacher lesson N-1 scripts with only the namespace changed, plus one
  skeleton per file that is new in lesson N. Do not build a starter by deleting
  parts of the Teacher lesson N solution.
- A skeleton holds the finished file's `using` lines, the namespace and the empty
  type declaration, with no members and no TODO comments. Keep
  `: MonoBehaviour` / `: ScriptableObject`; type every other base type in class.
- No `Tests/` folder and no `Editor/LessonNNSceneBuilder.cs`. They reference API
  the skeletons lack, and a compile error blocks Play Mode.
- New components are attached but unwired. Make the scene by copying the Teacher
  lesson N scene and re-saving it with the starter scripts. Keep `.meta` GUIDs
  identical to the Teacher lesson N project.
- Generate skeletons with `MiniTactics_Teacher/tools/lesson-tools/skeleton.py`,
  copy inherited files with `port.py`, then run `student-baseline.sh <N>` and
  `MiniTactics_Teacher/tools/Verify-Lessons.ps1`.

## Code the students already typed is frozen (user decision, 2026-09-21)

Lessons 1-4 have been taught. The user does not want code the students typed to
change much, and allowed changes only to provided code they never typed.

- Lessons 1-4 stay exactly as on `main`. Do not apply simplifications to them.
- A later starter must contain the taught code unchanged. Later lessons add only
  the lines a feature needs to those files; simplification rules apply to the
  classes that are new in that lesson.
- Before editing a lesson, check whether it has been taught and treat it the
  same way. The fully simplified lessons 1-6 live at the tag
  `simplified-all-lessons-20260920` for a future course run from lesson 1.

The complete rules and the progress log are in the Teacher repository:
`docs/teaching-code-rules.md` and `docs/simplify-progress.md`. The production
rules for all three workspaces are in `D:\UnityProjects\AGENTS.md`.
