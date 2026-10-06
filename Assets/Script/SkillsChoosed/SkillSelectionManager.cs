using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionManager : MonoBehaviour
{
    //public static SkillSelectionManager Instance { get; private set; }

    //public enum State { Idle, Delay, Selecting, Grabbed, Closed }

    //[Header("References (để trống sẽ tự tìm)")]
    //public PocketTowPs pocket;

    //[Header("Timing")]
    //[Tooltip("Chờ thêm sau khi bi dừng. CueStickController đã tự chờ 1s ổn định nên để 0.")]
    //public float delayBeforeWindow = 0f;
    //public float selectionDuration = 3f;

    //[Header("Khi nào bắt đầu có cửa sổ")]
    //[Tooltip("Cửa sổ chỉ mở từ cú đánh thứ N trở đi. 3 = bắt đầu ở cú thứ 3 (cú 1 và 2 không hiện). 1 hoặc 2 = hiện ngay sau cú đánh đầu tiên.")]
    //public int firstShotWithWindow = 3;

    //[Header("Hotkeys (2 người 1 máy)")]
    //public bool useHotkeys = true;
    //public KeyCode[] player1Keys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    //public KeyCode[] player2Keys = { KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    //[Header("Debug")]
    //public bool verboseLog = true;

    //// ===== Trạng thái cho UI đọc =====
    //public State CurrentState { get; private set; } = State.Idle;
    //public int TurnPlayer { get; private set; }
    //public float TimeLeft { get; private set; }
    //public int GrabbedPlayer { get; private set; }
    //public int GrabbedSlot { get; private set; }
    //public string LastMessage { get; private set; } = "";
    //public float LastMessageTime { get; private set; } = -999f;
    //public bool IsInDelay => CurrentState == State.Delay;

    //private int shotsStarted = 0;
    //public int ShotsStarted => shotsStarted;

    ///// <summary>true nếu cửa sổ sẽ mở cho cú đánh kế tiếp (dựa trên số cú đã đánh).</summary>
    //public bool WindowEnabledForNextShot => shotsStarted + 1 >= firstShotWithWindow;

    //// private struct Request { public int player; public int slot; }
    //private enum RequestKind { Skill, Use, Skip }
    //private struct Request { public int player; public int slot; public RequestKind kind; }
    //private int grabFrame = -1;
    //private readonly List<Request> queue = new List<Request>();
    //private readonly List<Request> workList = new List<Request>();

    //// ================================================================
    //private void Awake()
    //{
    //    Instance = this;
    //    if (pocket == null) pocket = FindFirstObjectByType<PocketTowPs>();
    //    if (pocket == null) Debug.LogError("[SkillSelect] Không tìm thấy PocketTowPs!");
    //}

    //private void OnDestroy()
    //{
    //    if (Instance == this) Instance = null;
    //}

    //// ================================================================
    //// API GỌI TỪ BÊN NGOÀI
    //// ================================================================

    ///// <summary>Gọi cuối CueStickController.OnAllBallsStoppedAction (sau khi đã xử lý đổi lượt).</summary>
    //public void BeginSelection(int turnPlayerNumber)
    //{
    //    if (pocket != null && pocket.gameEnd)
    //    {
    //        Log("BeginSelection bị bỏ qua: game đã kết thúc");
    //        return;
    //    }

    //    if (!WindowEnabledForNextShot)
    //    {
    //        CurrentState = State.Idle;
    //        Log($"BeginSelection bị bỏ qua: mới đánh {shotsStarted} cú, cửa sổ bắt đầu từ cú thứ {firstShotWithWindow}");
    //        return;
    //    }

    //    RevokeAllGrants();
    //    queue.Clear();
    //    GrabbedPlayer = 0;
    //    GrabbedSlot = 0;
    //    TurnPlayer = turnPlayerNumber;

    //    if (delayBeforeWindow > 0f)
    //    {
    //        CurrentState = State.Delay;
    //        TimeLeft = delayBeforeWindow;
    //        Log($"DELAY {delayBeforeWindow}s trước khi mở cửa sổ | lượt P{TurnPlayer}");
    //    }
    //    else
    //    {
    //        OpenWindow();
    //    }
    //}

    ///// <summary>Gọi trong CueStickController.HitCueBall khi cú đánh bắt đầu.</summary>
    //public void OnShotStarted()
    //{
    //    shotsStarted++;
    //    Log($"SHOT STARTED | cú đánh thứ {shotsStarted}");

    //    RevokeAllGrants();

    //    if (CurrentState == State.Delay || CurrentState == State.Selecting || CurrentState == State.Grabbed)
    //        CloseWindow("cú đánh đã được thực hiện");
    //}

    ///// <summary>Mọi nguồn input gọi hàm này. slot = 1..3</summary>
    //public void RequestSkill(int player, int slot)
    //{
    //    if (player < 1 || player > 2 || slot < 1 || slot > 3)
    //    {
    //        Debug.LogWarning($"[SkillSelect] Request không hợp lệ: P{player} slot{slot}");
    //        return;
    //    }

    //    queue.Add(new Request { player = player, slot = slot });
    //    Log($"REQUEST P{player} slot{slot} | frame {Time.frameCount}");
    //}

    //// ===== Truy vấn cho UI =====
    //public BaseSkills GetSkill(int player, int slot)
    //{
    //    PlayerSkillController psc = GetPSC(player);
    //    return psc != null ? psc.GetSkill(slot) : null;
    //}

    //public bool CanAfford(int player, int slot)
    //{
    //    BaseSkills skill = GetSkill(player, slot);
    //    if (skill == null) return false;

    //    StaminaManager st = StaminaManagerRegistry.Get(player);
    //    if (st == null) return true; // giống PlayerSkillController: không có stamina manager thì không chặn
    //    return st.GetCurrentStamina() >= skill.staminaCost;
    //}

    //public bool IsSlotLit(int player, int slot)
    //{
    //    switch (CurrentState)
    //    {
    //        case State.Selecting: return CanAfford(player, slot);
    //        case State.Grabbed: return player == GrabbedPlayer && slot == GrabbedSlot;
    //        default: return false;
    //    }
    //}

    //// ================================================================
    //// VÒNG LẶP CHÍNH (LateUpdate để gom hết input của frame: UI click + phím)
    //// ================================================================
    //private void LateUpdate()
    //{
    //    if (Time.timeScale <= 0f) // đang pause
    //    {
    //        queue.Clear();
    //        return;
    //    }

    //    if (useHotkeys) PollHotkeys();
    //    ProcessQueue();
    //    Tick();
    //}

    //private void PollHotkeys()
    //{
    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (i < player1Keys.Length && Input.GetKeyDown(player1Keys[i])) RequestSkill(1, i + 1);
    //        if (i < player2Keys.Length && Input.GetKeyDown(player2Keys[i])) RequestSkill(2, i + 1);
    //    }
    //}

    //private void ProcessQueue()
    //{
    //    //if (queue.Count == 0) return;

    //    //if (CurrentState != State.Selecting && CurrentState != State.Grabbed)
    //    //{
    //    //    Log($"Bỏ qua {queue.Count} request (state={CurrentState})");
    //    //    queue.Clear();
    //    //    return;
    //    //}

    //    //// Cùng 1 frame: người có lượt xử lý trước
    //    //bool hasTurn = false, hasOther = false;
    //    //foreach (Request r in queue)
    //    //{
    //    //    if (r.player == TurnPlayer) hasTurn = true; else hasOther = true;
    //    //}
    //    //if (hasTurn && hasOther)
    //    //    Log("SAME-FRAME TIE -> ưu tiên người đang có lượt");

    //    //for (int pass = 0; pass < 2; pass++)
    //    //{
    //    //    foreach (Request r in queue)
    //    //    {
    //    //        bool isTurnPlayer = r.player == TurnPlayer;
    //    //        if ((pass == 0) == isTurnPlayer) Resolve(r);
    //    //    }
    //    //}

    //    //queue.Clear();

    //    if (queue.Count == 0) return;

    //    if (CurrentState != State.Selecting && CurrentState != State.Grabbed)
    //    {
    //        Log($"Bỏ qua {queue.Count} request (state={CurrentState})");
    //        queue.Clear();
    //        return;
    //    }

    //    // Chụp snapshot rồi xóa queue NGAY, để Resolve() có đóng cửa sổ
    //    // (CloseWindow gọi queue.Clear) cũng không làm hỏng vòng lặp
    //    workList.Clear();
    //    workList.AddRange(queue);
    //    queue.Clear();

    //    // Cùng 1 frame: người có lượt xử lý trước
    //    bool hasTurn = false, hasOther = false;
    //    for (int i = 0; i < workList.Count; i++)
    //    {
    //        if (workList[i].player == TurnPlayer) hasTurn = true; else hasOther = true;
    //    }
    //    if (hasTurn && hasOther)
    //        Log("SAME-FRAME TIE -> ưu tiên người đang có lượt");

    //    for (int pass = 0; pass < 2; pass++)
    //    {
    //        for (int i = 0; i < workList.Count; i++)
    //        {
    //            Request r = workList[i];
    //            bool isTurnPlayer = r.player == TurnPlayer;
    //            if ((pass == 0) == isTurnPlayer) Resolve(r);
    //        }
    //    }
    //}

    //private void Resolve(Request r)
    //{
    //    if (CurrentState == State.Selecting)
    //    {
    //        if (GetSkill(r.player, r.slot) == null)
    //        {
    //            Log($"P{r.player} slot{r.slot}: không có skill -> bỏ qua");
    //            return;
    //        }

    //        if (!CanAfford(r.player, r.slot))
    //        {
    //            Log($"P{r.player} slot{r.slot}: thiếu stamina (đang tối) -> bỏ qua");
    //            return;
    //        }

    //        if (r.player != TurnPlayer)
    //        {
    //            Post($"P{r.player} pressed out of turn - skills locked");
    //            CloseWindow($"P{r.player} bấm sai lượt");
    //            return;
    //        }

    //        Grab(r.player, r.slot);
    //    }
    //    else if (CurrentState == State.Grabbed)
    //    {
    //        if (r.player == GrabbedPlayer && r.slot == GrabbedSlot)
    //            UseGrabbed();
    //        else
    //            Log($"P{r.player} slot{r.slot}: đã có skill được giành -> bỏ qua");
    //    }
    //}

    //private void Tick()
    //{
    //    switch (CurrentState)
    //    {
    //        case State.Delay:
    //            if (ShouldAbort()) return;
    //            TimeLeft -= Time.deltaTime;
    //            if (TimeLeft <= 0f) OpenWindow();
    //            break;

    //        case State.Selecting:
    //            if (ShouldAbort()) return;
    //            TimeLeft -= Time.deltaTime;
    //            if (TimeLeft <= 0f)
    //            {
    //                TimeLeft = 0f;
    //                CloseWindow("hết giờ, không ai giành");
    //            }
    //            break;

    //        case State.Grabbed:
    //            ShouldAbort();
    //            break;
    //    }
    //}

    //private bool ShouldAbort()
    //{
    //    if (pocket == null) return false;

    //    if (pocket.gameEnd)
    //    {
    //        CloseWindow("game kết thúc");
    //        return true;
    //    }

    //    if (pocket.currentPlayer != TurnPlayer)
    //    {
    //        CloseWindow($"đổi lượt (P{TurnPlayer} -> P{pocket.currentPlayer})");
    //        return true;
    //    }

    //    return false;
    //}

    //// ================================================================
    //// CÁC BƯỚC TRẠNG THÁI
    //// ================================================================
    //private void OpenWindow()
    //{
    //    CurrentState = State.Selecting;
    //    TimeLeft = selectionDuration;

    //    Log($"=== MỞ CỬA SỔ {selectionDuration}s | lượt P{TurnPlayer} ===");
    //    for (int p = 1; p <= 2; p++)
    //    {
    //        for (int s = 1; s <= 3; s++)
    //        {
    //            BaseSkills sk = GetSkill(p, s);
    //            string nm = sk != null ? sk.skillName : "NULL";
    //            int cost = sk != null ? sk.staminaCost : 0;
    //            Log($"   P{p} slot{s}: {nm} | cost={cost} | sáng={CanAfford(p, s)}");
    //        }
    //    }
    //}

    //private void Grab(int player, int slot)
    //{
    //    GrabbedPlayer = player;
    //    GrabbedSlot = slot;
    //    CurrentState = State.Grabbed;

    //    PlayerSkillController psc = GetPSC(player);
    //    if (psc != null) psc.GrantSlot(slot);

    //    BaseSkills sk = GetSkill(player, slot);
    //    Post($"P{player} grabbed {(sk != null ? sk.skillName : "skill")}");
    //    Log($"GRABBED P{player} slot{slot} | chưa trừ stamina, chờ người chơi bấm lại để dùng");
    //}

    //private void UseGrabbed()
    //{
    //    PlayerSkillController psc = GetPSC(GrabbedPlayer);
    //    if (psc == null)
    //    {
    //        Debug.LogError("[SkillSelect] Không tìm thấy PlayerSkillController để dùng skill!");
    //        CloseWindow("lỗi: thiếu PlayerSkillController");
    //        return;
    //    }

    //    bool ok = psc.TryUseSlot(GrabbedSlot);
    //    if (ok)
    //    {
    //        Post($"P{GrabbedPlayer} used skill");
    //        CloseWindow("skill đã được dùng");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[SkillSelect] TryUseSlot thất bại (xem log [PSC]) -> đóng cửa sổ");
    //        CloseWindow("dùng skill thất bại");
    //    }
    //}

    //private void CloseWindow(string reason)
    //{
    //    RevokeAllGrants();
    //    queue.Clear();
    //    CurrentState = State.Closed;
    //    Log($"=== ĐÓNG CỬA SỔ: {reason} ===");
    //}

    //private void RevokeAllGrants()
    //{
    //    for (int p = 1; p <= 2; p++)
    //    {
    //        PlayerSkillController psc = GetPSC(p);
    //        if (psc != null) psc.RevokeGrant();
    //    }
    //}

    //// ================================================================
    //// HELPERS
    //// ================================================================
    //private PlayerSkillController GetPSC(int player)
    //{
    //    GlaszekManager g = PlayerManagerRegistry.Get(player);
    //    return g != null ? g.GetComponent<PlayerSkillController>() : null;
    //}

    //private void Post(string msg)
    //{
    //    LastMessage = msg;
    //    LastMessageTime = Time.unscaledTime;
    //}

    //private void Log(string msg)
    //{
    //    if (verboseLog) Debug.Log($"[SkillSelect] {msg}");
    //}

    //// ================================================================
    //// DEBUG (chuột phải vào component trong Play Mode)
    //// ================================================================
    //[ContextMenu("DEBUG: +100 stamina cho cả 2 player")]
    //private void DebugAddStamina()
    //{
    //    for (int p = 1; p <= 2; p++)
    //    {
    //        StaminaManager st = StaminaManagerRegistry.Get(p);
    //        if (st != null) st.AddStamina(100f);
    //    }
    //}

    //[ContextMenu("DEBUG: Mở cửa sổ ngay (cho lượt hiện tại)")]
    //private void DebugOpenWindow()
    //{
    //    if (pocket != null) BeginSelection(pocket.currentPlayer);
    //}

    public static SkillSelectionManager Instance { get; private set; }

    public enum State { Idle, Delay, Selecting, Ready, Closed }

    [Header("References (để trống sẽ tự tìm)")]
    public PocketTowPs pocket;
    public CueStickController cueStick; // [NEW]

    [Header("Timing")]
    [Tooltip("Chờ thêm sau khi bi dừng. CueStickController đã tự chờ ~1s ổn định nên để 0.")]
    public float delayBeforeWindow = 0f;
    public float selectionDuration = 3f;

    [Header("Khi nào có cửa sổ")]
    [Tooltip("Cửa sổ chỉ mở từ cú đánh thứ N trở đi. 3 = bắt đầu ở cú thứ 3. 1 hoặc 2 = hiện ngay sau cú đánh đầu tiên.")]
    public int firstShotWithWindow = 3;
    [Tooltip("true = nếu không ai đủ stamina cho skill nào thì không mở cửa sổ.")]
    public bool hideIfNobodyCanAfford = false;

    [Header("Đồng hồ lượt")] // [NEW]
    [Tooltip("true = đóng băng đồng hồ lượt trong lúc khóa input (Delay/Selecting), để người chơi không mất thời gian suy nghĩ.")]
    public bool freezeTurnTimerWhileLocked = true;

    [Header("Debug")]
    public bool verboseLog = true;

    // ===== Trạng thái cho UI đọc =====
    public State CurrentState { get; private set; } = State.Idle;
    public int TurnPlayer { get; private set; }
    public float TimeLeft { get; private set; }
    public int ReadyPlayer { get; private set; }
    public int ReadySlot { get; private set; }
    public string LastMessage { get; private set; } = "";
    public float LastMessageTime { get; private set; } = -999f;
    public bool IsInDelay => CurrentState == State.Delay;
    public bool IsSelecting => CurrentState == State.Selecting; // [NEW]

    private int shotsStarted = 0;
    public int ShotsStarted => shotsStarted;
    /// <summary>true nếu cửa sổ sẽ mở cho cú đánh kế tiếp (dựa trên số cú đã đánh).</summary>
    public bool WindowEnabledForNextShot => shotsStarted + 1 >= firstShotWithWindow;

    private enum RequestKind { Grab, Use }
    private struct Request { public int player; public int slot; public RequestKind kind; }
    private readonly List<Request> queue = new List<Request>();
    private readonly List<Request> workList = new List<Request>();
    private int readyFrame = -1;
    private bool timerFrozenByUs = false; // [NEW]

    // ================================================================
    private void Awake()
    {
        Instance = this;
        if (pocket == null) pocket = FindFirstObjectByType<PocketTowPs>();
        if (pocket == null) Debug.LogError("[SkillSelect] Không tìm thấy PocketTowPs!");

        if (cueStick == null) cueStick = FindFirstObjectByType<CueStickController>(); // [NEW]
        if (cueStick == null) Debug.LogWarning("[SkillSelect] Không tìm thấy CueStickController (sẽ không đóng băng được đồng hồ lượt)");
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ================================================================
    // API GỌI TỪ BÊN NGOÀI
    // ================================================================

    /// <summary>Gọi cuối CueStickController.OnAllBallsStoppedAction (sau khi đã xử lý đổi lượt).</summary>
    public void BeginSelection(int turnPlayerNumber)
    {
        if (pocket != null && pocket.gameEnd)
        {
            Log("BeginSelection bị bỏ qua: game đã kết thúc");
            return;
        }

        if (!WindowEnabledForNextShot)
        {
            CurrentState = State.Idle;
            Log($"BeginSelection bị bỏ qua: mới đánh {shotsStarted} cú, cửa sổ bắt đầu từ cú thứ {firstShotWithWindow}");
            return;
        }

        RevokeAllGrants();
        queue.Clear();
        ReadyPlayer = 0;
        ReadySlot = 0;
        TurnPlayer = turnPlayerNumber;

        if (delayBeforeWindow > 0f)
        {
            CurrentState = State.Delay;
            TimeLeft = delayBeforeWindow;
            FreezeTurnTimer(true); // [NEW]
            Log($"DELAY {delayBeforeWindow}s trước khi mở cửa sổ | lượt P{TurnPlayer}");
        }
        else
        {
            OpenWindow();
        }
    }

    /// <summary>Gọi trong CueStickController.HitCueBall khi cú đánh bắt đầu.</summary>
    public void OnShotStarted()
    {
        shotsStarted++;
        Log($"SHOT STARTED | cú đánh thứ {shotsStarted}");

        bool hadReady = CurrentState == State.Ready;
        int readyP = ReadyPlayer;

        RevokeAllGrants();

        if (CurrentState == State.Delay || CurrentState == State.Selecting || CurrentState == State.Ready)
        {
            CloseWindow(hadReady
                ? $"cú đánh đã thực hiện, P{readyP} không dùng skill (không trừ stamina)"
                : "cú đánh đã được thực hiện");
        }
    }

    /// <summary>Giành skill. slot = 1..3. (UI chạm, online hay AI đều gọi hàm này)</summary>
    public void RequestSkill(int player, int slot)
    {
        if (player < 1 || player > 2 || slot < 1 || slot > 3)
        {
            Debug.LogWarning($"[SkillSelect] Request không hợp lệ: P{player} slot{slot}");
            return;
        }

        queue.Add(new Request { player = player, slot = slot, kind = RequestKind.Grab });
        Log($"REQUEST SKILL P{player} slot{slot} | frame {Time.frameCount}");
    }

    /// <summary>Người đã giành skill chọn DÙNG (chạm chip READY).</summary>
    public void RequestUse(int player)
    {
        queue.Add(new Request { player = player, slot = 0, kind = RequestKind.Use });
        Log($"REQUEST USE P{player} | frame {Time.frameCount}");
    }

    // ===== Truy vấn cho UI =====
    public BaseSkills GetSkill(int player, int slot)
    {
        PlayerSkillController psc = GetPSC(player);
        return psc != null ? psc.GetSkill(slot) : null;
    }

    public bool CanAfford(int player, int slot)
    {
        BaseSkills skill = GetSkill(player, slot);
        if (skill == null) return false;

        StaminaManager st = StaminaManagerRegistry.Get(player);
        if (st == null) return true; // giống PlayerSkillController: không có stamina manager thì không chặn
        return st.GetCurrentStamina() >= skill.staminaCost;
    }

    public bool IsSlotLit(int player, int slot)
    {
        return CurrentState == State.Selecting && CanAfford(player, slot);
    }

    private bool AnyoneCanAfford()
    {
        for (int p = 1; p <= 2; p++)
            for (int s = 1; s <= 3; s++)
                if (CanAfford(p, s)) return true;
        return false;
    }

    // ================================================================
    // VÒNG LẶP CHÍNH (LateUpdate để gom hết input của frame: nhiều ngón chạm cùng lúc)
    // ================================================================
    private void LateUpdate()
    {
        if (Time.timeScale <= 0f) // đang pause
        {
            queue.Clear();
            return;
        }

        ProcessQueue();
        Tick();
    }

    private void ProcessQueue()
    {
        if (queue.Count == 0) return;

        if (CurrentState != State.Selecting && CurrentState != State.Ready)
        {
            Log($"Bỏ qua {queue.Count} request (state={CurrentState})");
            queue.Clear();
            return;
        }

        // Snapshot rồi xóa queue NGAY, để Resolve() có đóng cửa sổ cũng không làm hỏng vòng lặp
        workList.Clear();
        workList.AddRange(queue);
        queue.Clear();

        // Cùng 1 frame: người có lượt xử lý trước
        bool hasTurn = false, hasOther = false;
        for (int i = 0; i < workList.Count; i++)
        {
            if (workList[i].player == TurnPlayer) hasTurn = true; else hasOther = true;
        }
        if (hasTurn && hasOther)
            Log("SAME-FRAME TIE -> ưu tiên người đang có lượt");

        for (int pass = 0; pass < 2; pass++)
        {
            for (int i = 0; i < workList.Count; i++)
            {
                Request r = workList[i];
                bool isTurnPlayer = r.player == TurnPlayer;
                if ((pass == 0) == isTurnPlayer) Resolve(r);
            }
        }
    }

    private void Resolve(Request r)
    {
        if (CurrentState == State.Selecting)
        {
            if (r.kind != RequestKind.Grab)
            {
                Log($"P{r.player} {r.kind}: chưa có skill nào được giành -> bỏ qua");
                return;
            }

            if (GetSkill(r.player, r.slot) == null)
            {
                Log($"P{r.player} slot{r.slot}: không có skill -> bỏ qua");
                return;
            }

            if (!CanAfford(r.player, r.slot))
            {
                Log($"P{r.player} slot{r.slot}: thiếu stamina (đang tối) -> bỏ qua");
                return;
            }

            if (r.player != TurnPlayer)
            {
                Post($"P{r.player} grabbed out of turn - nobody can use skills");
                CloseWindow($"P{r.player} giành ngoài lượt -> không ai được dùng skill");
                return;
            }

            MakeReady(r.player, r.slot);
        }
        else if (CurrentState == State.Ready)
        {
            if (r.kind == RequestKind.Grab)
            {
                Log($"P{r.player} slot{r.slot}: skill đã được giành rồi -> bỏ qua");
                return;
            }

            if (r.player != ReadyPlayer)
            {
                Log($"P{r.player} USE: không phải người đã giành (P{ReadyPlayer}) -> bỏ qua");
                return;
            }

            // Chặn USE trong chính frame vừa giành (phòng 1 lần chạm bị đăng ký 2 lần)
            if (Time.frameCount == readyFrame)
            {
                Log("USE cùng frame với lúc giành -> bỏ qua");
                return;
            }

            UseReady();
        }
    }

    private void Tick()
    {
        switch (CurrentState)
        {
            case State.Delay:
                if (ShouldAbort()) return;
                TimeLeft -= Time.deltaTime;
                if (TimeLeft <= 0f) OpenWindow();
                break;

            case State.Selecting:
                if (ShouldAbort()) return;
                TimeLeft -= Time.deltaTime;
                if (TimeLeft <= 0f)
                {
                    TimeLeft = 0f;
                    CloseWindow("hết giờ, không ai giành");
                }
                break;

            case State.Ready:
                ShouldAbort();
                break;
        }
    }

    private bool ShouldAbort()
    {
        if (pocket == null) return false;

        if (pocket.gameEnd)
        {
            CloseWindow("game kết thúc");
            return true;
        }

        if (pocket.currentPlayer != TurnPlayer)
        {
            CloseWindow($"đổi lượt (P{TurnPlayer} -> P{pocket.currentPlayer})");
            return true;
        }

        return false;
    }

    // ================================================================
    // CÁC BƯỚC TRẠNG THÁI
    // ================================================================
    private void OpenWindow()
    {
        if (hideIfNobodyCanAfford && !AnyoneCanAfford())
        {
            FreezeTurnTimer(false); // [NEW] (phòng trường hợp đi từ Delay sang)
            CurrentState = State.Idle;
            Log("Không ai đủ stamina cho skill nào -> không mở cửa sổ");
            return;
        }

        CurrentState = State.Selecting;
        TimeLeft = selectionDuration;
        FreezeTurnTimer(true); // [NEW]

        Log($"=== MỞ CỬA SỔ {selectionDuration}s | lượt P{TurnPlayer} | KHÓA xoay ngắm + đánh ===");
        for (int p = 1; p <= 2; p++)
        {
            for (int s = 1; s <= 3; s++)
            {
                BaseSkills sk = GetSkill(p, s);
                string nm = sk != null ? sk.skillName : "NULL";
                int cost = sk != null ? sk.staminaCost : 0;
                Log($"   P{p} slot{s}: {nm} | cost={cost} | sáng={CanAfford(p, s)}");
            }
        }
    }

    private void MakeReady(int player, int slot)
    {
        ReadyPlayer = player;
        ReadySlot = slot;
        CurrentState = State.Ready;
        readyFrame = Time.frameCount;
        FreezeTurnTimer(false); // [NEW] panel đóng -> mở khóa, đồng hồ chạy lại

        PlayerSkillController psc = GetPSC(player);
        if (psc != null) psc.GrantSlot(slot);

        BaseSkills sk = GetSkill(player, slot);
        Post($"P{player} grabbed {(sk != null ? sk.skillName : "skill")}");
        Log($"READY P{player} slot{slot} | panel đóng, MỞ KHÓA. Chưa trừ stamina: chạm chip để dùng, hoặc đánh bi để bỏ qua");
    }

    private void UseReady()
    {
        PlayerSkillController psc = GetPSC(ReadyPlayer);
        if (psc == null)
        {
            Debug.LogError("[SkillSelect] Không tìm thấy PlayerSkillController để dùng skill!");
            CloseWindow("lỗi: thiếu PlayerSkillController");
            return;
        }

        bool ok = psc.TryUseSlot(ReadySlot);
        if (ok)
        {
            Post($"P{ReadyPlayer} used skill");
            CloseWindow("skill đã được dùng");
        }
        else
        {
            Debug.LogWarning("[SkillSelect] TryUseSlot thất bại (xem log [PSC]) -> đóng cửa sổ");
            CloseWindow("dùng skill thất bại");
        }
    }

    private void CloseWindow(string reason)
    {
        RevokeAllGrants();
        queue.Clear();
        CurrentState = State.Closed;
        FreezeTurnTimer(false); // [NEW]
        Log($"=== ĐÓNG CỬA SỔ: {reason} ===");
    }

    private void RevokeAllGrants()
    {
        for (int p = 1; p <= 2; p++)
        {
            PlayerSkillController psc = GetPSC(p);
            if (psc != null) psc.RevokeGrant();
        }
    }

    // [NEW] ================================================================
    // ĐỒNG HỒ LƯỢT: đóng băng trong lúc khóa input, chỉ chạy lại nếu chính mình đã đóng băng nó
    // ================================================================
    private void FreezeTurnTimer(bool freeze)
    {
        if (cueStick == null) return;

        if (freeze)
        {
            if (!freezeTurnTimerWhileLocked || timerFrozenByUs) return;
            timerFrozenByUs = true;
            cueStick.stopTimer = true;
            Log("Đóng băng đồng hồ lượt");
        }
        else if (timerFrozenByUs)
        {
            timerFrozenByUs = false;

            // Game đã kết thúc thì PocketTowPs tự giữ stopTimer = true, không được ghi đè
            if (pocket == null || !pocket.gameEnd)
            {
                cueStick.stopTimer = false;
                Log("Chạy lại đồng hồ lượt");
            }
        }
    }

    // ================================================================
    // HELPERS
    // ================================================================
    private PlayerSkillController GetPSC(int player)
    {
        GlaszekManager g = PlayerManagerRegistry.Get(player);
        return g != null ? g.GetComponent<PlayerSkillController>() : null;
    }

    private void Post(string msg)
    {
        LastMessage = msg;
        LastMessageTime = Time.unscaledTime;
    }

    private void Log(string msg)
    {
        if (verboseLog) Debug.Log($"[SkillSelect] {msg}");
    }

    // ================================================================
    // DEBUG (chuột phải vào component trong Play Mode)
    // ================================================================
    [ContextMenu("DEBUG: +100 stamina cho cả 2 player")]
    private void DebugAddStamina()
    {
        for (int p = 1; p <= 2; p++)
        {
            StaminaManager st = StaminaManagerRegistry.Get(p);
            if (st != null) st.AddStamina(100f);
        }
    }

    [ContextMenu("DEBUG: Mở cửa sổ ngay (cho lượt hiện tại)")]
    private void DebugOpenWindow()
    {
        if (pocket != null) BeginSelection(pocket.currentPlayer);
    }
}
