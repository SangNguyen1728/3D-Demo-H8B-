# Decision Log - 3ddemo-bida

## 2025-08-23: UI Skill Panel Optimization (PickCharacterPanel > SkillSelectUI)

**Context**: User wants to improve the skill selection UI layout in HomeScene. Current issues:
- Skills cramped together, no visual breathing room
- SelectionSummaryUI hidden at bottom of panel
- No way to preview skill details (description, stamina cost, cooldown) before selecting

**Decision**:
1. **Layout & Spacing**: Add `VerticalLayoutGroup` to `rowContainer` (spacing=15, padding=10), `HorizontalLayoutGroup` to `variantButtonContainer` (spacing=10, childForceExpandWidth=false)
2. **Move SummaryUI**: Reparent `SelectionSummaryUI` as last child of `rowContainer` so it renders below skill rows
3. **Extend SummaryUI**: Show selected skill's description, stamina cost, cooldown, type
4. **Add SkillDetailPopup**: New component for hover/click tooltip on variant buttons showing full skill info
5. **Prefab Updates**: Update `SkillSlotRow.prefab` and `SkillVariantButton.prefab` with LayoutGroups and LayoutElements

**Files to Modify**:
- `Assets/Script/HomeScript/HomeScreen/UI Layer/SkillSlotRowUI.cs`
- `Assets/Script/HomeScript/HomeScreen/UI Layer/SkillVariantButtonUI.cs`
- `Assets/Script/HomeScript/HomeScreen/UI Layer/SkillSelectUI.cs`
- `Assets/Script/HomeScript/SelectionSummaryUI.cs`
- `Assets/Prefabs/SkillSlotRow.prefab`
- `Assets/Prefabs/SkillVariantButton.prefab`

**New File**:
- `Assets/Script/HomeScript/HomeScreen/UI Layer/SkillDetailPopup.cs`

**Rationale**: 
- LayoutGroups are Unity-standard, performant, and handle dynamic content
- Keeping SummaryUI in same container ensures it stays below rows automatically
- BaseSkills already has `description`, `staminaCost`, `cooldown`, `SkillType` fields (BaseSkills.cs:38-48)
- Popup avoids cluttering main UI while providing detail-on-demand

**Open Questions**:
- Popup trigger: Hover (desktop) vs Click/Hold (mobile)? → Start with hover + click fallback
- Popup position: Follow mouse (tooltip) or fixed sidebar? → Tooltip near button
- Animation: Simple fade in/out (CanvasGroup alpha)

---

## 2025-08-23: Performance Optimization Priorities Identified

**Context**: Code review revealed critical performance issues in gameplay scene.

**Decision**: Document top 5 issues to fix before adding features:

1. `Aiming.DectectDangerousCircles()` - `FindObjectsOfType<Rigidbody>()` every frame + List rebuilds (Aiming.cs:43-64)
2. `PocketTowPs.UpdateNextTarget()` / `HasClearedGroup()` - `FindObjectsByType<BallNo>()` repeatedly (PocketTowPs.cs:1311, 749)
3. `GlaszekManager.NotifyBallStopped()` / `NotifyShotStarted()` - `FindObjectsByType` in callbacks (GlaszekManager.cs:131, 141)
4. `BeelzitaManager` - repeated `FindFirstObjectByType<PocketTowPs>` / `CueStickController` (BeelzitaManager.cs:164, 496)
5. `TargetBallFinder.GetTargetBallTransform()` - `GameObject.FindGameObjectWithTag()` with string concat (TargetBallFinder.cs:49)

**Rationale**: These run in hot paths (Update, physics callbacks) and cause GC spikes. Fix by caching references via registries or static lists updated on ball spawn/destroy.

---

## 2025-08-23: Project Documentation Created

**Decision**: Created `AGENTS.md` with verified project details for future agent sessions.

**Content**: Build/run instructions, architecture overview, code conventions, known performance issues, testing setup, common tasks, gotchas, directory structure, external dependencies.

**Source of Truth**: Verified against `Assembly-CSharp.csproj`, `Packages/manifest.json`, and actual script files.