using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionManager : MonoBehaviour
{
    //public static SkillSelectionManager Instance { get; private set; }

    //public enum State { Idle, Delay, Selecting, Ready, Closed }

    //[Header("References (để trống sẽ tự tìm)")]
    //public PocketTowPs pocket;
    //public CueStickController cueStick; // [NEW]

    //[Header("Timing")]
    //[Tooltip("Chờ thêm sau khi bi dừng. CueStickController đã tự chờ ~1s ổn định nên để 0.")]
    //public float delayBeforeWindow = 0f;
    //public float selectionDuration = 3f;

    //[Header("Khi nào có cửa sổ")]
    //[Tooltip("Cửa sổ chỉ mở từ cú đánh thứ N trở đi. 3 = bắt đầu ở cú thứ 3. 1 hoặc 2 = hiện ngay sau cú đánh đầu tiên.")]
    //public int firstShotWithWindow = 3;
    //[Tooltip("true = nếu không ai đủ stamina cho skill nào thì không mở cửa sổ.")]
    //public bool hideIfNobodyCanAfford = false;

    //[Header("Đồng hồ lượt")] // [NEW]
    //[Tooltip("true = đóng băng đồng hồ lượt trong lúc khóa input (Delay/Selecting), để người chơi không mất thời gian suy nghĩ.")]
    //public bool freezeTurnTimerWhileLocked = true;

    //[Header("Phạt khi giành skill mà không dùng")]
    //[Header("Phạt khi giành skill mà không dùng")]
    //[Tooltip("Số stamina bị trừ cố định nếu giành skill nhưng không dùng. 0 = tắt phạt.")]
    //public float unusedGrabPenaltyFlat = 20f;

    //[Header("Thời hạn dùng skill đã giành")]
    //[Tooltip("Số giây được dùng skill sau khi giành. Hết giờ chưa dùng -> bị phạt ngay và khóa skill. 0 = không giới hạn (chỉ phạt khi đánh bi hoặc hết lượt).")]
    //public float readyDuration = 10f;

    //[Header("Debug")]
    //public bool verboseLog = true;

    //// ===== Trạng thái cho UI đọc =====
    //public State CurrentState { get; private set; } = State.Idle;
    //public int TurnPlayer { get; private set; }
    //public float TimeLeft { get; private set; }
    //public int ReadyPlayer { get; private set; }
    //public int ReadySlot { get; private set; }

    //public float ReadyTimeLeft { get; private set; }

    //// ===== Phản hồi "skill đã khóa" cho UI =====
    //public bool LockFeedbackActive { get; private set; }
    //public int LockedPlayer { get; private set; }   // skill được tô đỏ (hết hạn / bị bấm sai lượt)
    //public int LockedSlot { get; private set; }
    //public float LockedPenalty { get; private set; }
    //public float LockFeedbackTime { get; private set; } = -999f; // unscaled time
    //public string LastMessage { get; private set; } = "";
    //public float LastMessageTime { get; private set; } = -999f;
    //public bool IsInDelay => CurrentState == State.Delay;
    //public bool IsSelecting => CurrentState == State.Selecting; // [NEW]

    //private int shotsStarted = 0;
    //public int ShotsStarted => shotsStarted;
    ///// <summary>true nếu cửa sổ sẽ mở cho cú đánh kế tiếp (dựa trên số cú đã đánh).</summary>
    //public bool WindowEnabledForNextShot => shotsStarted + 1 >= firstShotWithWindow;

    //private enum RequestKind { Grab, Use }
    //private struct Request { public int player; public int slot; public RequestKind kind; }
    //private readonly List<Request> queue = new List<Request>();
    //private readonly List<Request> workList = new List<Request>();
    //private int readyFrame = -1;
    //private bool timerFrozenByUs = false; // [NEW]

    //private float lastPenaltyTaken = 0f;

    //// ================================================================
    //private void Awake()
    //{
    //    Instance = this;
    //    if (pocket == null) pocket = FindFirstObjectByType<PocketTowPs>();
    //    if (pocket == null) Debug.LogError("[SkillSelect] Không tìm thấy PocketTowPs!");

    //    if (cueStick == null) cueStick = FindFirstObjectByType<CueStickController>(); // [NEW]
    //    if (cueStick == null) Debug.LogWarning("[SkillSelect] Không tìm thấy CueStickController (sẽ không đóng băng được đồng hồ lượt)");
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
    //    ClearLockFeedback();

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
    //    ReadyPlayer = 0;
    //    ReadySlot = 0;
    //    TurnPlayer = turnPlayerNumber;

    //    if (delayBeforeWindow > 0f)
    //    {
    //        CurrentState = State.Delay;
    //        TimeLeft = delayBeforeWindow;
    //        FreezeTurnTimer(true); // [NEW]
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
    //    ClearLockFeedback();

    //    shotsStarted++;
    //    Log($"SHOT STARTED | cú đánh thứ {shotsStarted}");

    //    bool hadReady = CurrentState == State.Ready;
    //    int readyP = ReadyPlayer;

    //    RevokeAllGrants();

    //    if (CurrentState == State.Delay || CurrentState == State.Selecting || CurrentState == State.Ready)
    //    {
    //        //CloseWindow(hadReady
    //        //    ? $"cú đánh đã thực hiện, P{readyP} không dùng skill (không trừ stamina)"
    //        //    : "cú đánh đã được thực hiện");

    //        CloseWindow(hadReady
    //            ? $"cú đánh đã thực hiện, P{readyP} không dùng skill"
    //            : "cú đánh đã được thực hiện",
    //            penalizeUnused: hadReady);
    //    }
    //}

    ///// <summary>Giành skill. slot = 1..3. (UI chạm, online hay AI đều gọi hàm này)</summary>
    //public void RequestSkill(int player, int slot)
    //{
    //    if (player < 1 || player > 2 || slot < 1 || slot > 3)
    //    {
    //        Debug.LogWarning($"[SkillSelect] Request không hợp lệ: P{player} slot{slot}");
    //        return;
    //    }

    //    queue.Add(new Request { player = player, slot = slot, kind = RequestKind.Grab });
    //    Log($"REQUEST SKILL P{player} slot{slot} | frame {Time.frameCount}");
    //}

    ///// <summary>Người đã giành skill chọn DÙNG (chạm chip READY).</summary>
    //public void RequestUse(int player)
    //{
    //    queue.Add(new Request { player = player, slot = 0, kind = RequestKind.Use });
    //    Log($"REQUEST USE P{player} | frame {Time.frameCount}");
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
    //    return CurrentState == State.Selecting && CanAfford(player, slot);
    //}

    //private bool AnyoneCanAfford()
    //{
    //    for (int p = 1; p <= 2; p++)
    //        for (int s = 1; s <= 3; s++)
    //            if (CanAfford(p, s)) return true;
    //    return false;
    //}

    //// ================================================================
    //// VÒNG LẶP CHÍNH (LateUpdate để gom hết input của frame: nhiều ngón chạm cùng lúc)
    //// ================================================================
    //private void LateUpdate()
    //{
    //    if (Time.timeScale <= 0f) // đang pause
    //    {
    //        queue.Clear();
    //        return;
    //    }

    //    ProcessQueue();
    //    Tick();
    //}

    //private void ProcessQueue()
    //{
    //    if (queue.Count == 0) return;

    //    if (CurrentState != State.Selecting && CurrentState != State.Ready)
    //    {
    //        Log($"Bỏ qua {queue.Count} request (state={CurrentState})");
    //        queue.Clear();
    //        return;
    //    }

    //    // Snapshot rồi xóa queue NGAY, để Resolve() có đóng cửa sổ cũng không làm hỏng vòng lặp
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
    //        if (r.kind != RequestKind.Grab)
    //        {
    //            Log($"P{r.player} {r.kind}: chưa có skill nào được giành -> bỏ qua");
    //            return;
    //        }

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
    //            //Post($"P{r.player} grabbed out of turn - nobody can use skills");
    //            //CloseWindow($"P{r.player} giành ngoài lượt -> không ai được dùng skill");
    //            //return;
    //            Post($"P{r.player} grabbed out of turn - nobody can use skills");
    //            CloseWindow($"P{r.player} giành ngoài lượt -> không ai được dùng skill");
    //            RaiseLockFeedback(r.player, r.slot, 0f);
    //            return;
    //        }

    //        MakeReady(r.player, r.slot);
    //    }
    //    else if (CurrentState == State.Ready)
    //    {
    //        if (r.kind == RequestKind.Grab)
    //        {
    //            Log($"P{r.player} slot{r.slot}: skill đã được giành rồi -> bỏ qua");
    //            return;
    //        }

    //        if (r.player != ReadyPlayer)
    //        {
    //            Log($"P{r.player} USE: không phải người đã giành (P{ReadyPlayer}) -> bỏ qua");
    //            return;
    //        }

    //        // Chặn USE trong chính frame vừa giành (phòng 1 lần chạm bị đăng ký 2 lần)
    //        if (Time.frameCount == readyFrame)
    //        {
    //            Log("USE cùng frame với lúc giành -> bỏ qua");
    //            return;
    //        }

    //        UseReady();
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

    //        case State.Ready:
    //            //ShouldAbort();
    //            //break;
    //            //if (ShouldAbort()) return;

    //            //if (readyDuration > 0f)
    //            //{
    //            //    ReadyTimeLeft -= Time.deltaTime;
    //            //    if (ReadyTimeLeft <= 0f)
    //            //    {
    //            //        ReadyTimeLeft = 0f;
    //            //        CloseWindow($"P{ReadyPlayer} giữ skill quá {readyDuration:0.#}s mà không dùng -> phạt và khóa skill",
    //            //                    penalizeUnused: true);
    //            //    }
    //            //}
    //            //break;

    //            if (ShouldAbort()) return;

    //            if (readyDuration > 0f)
    //            {
    //                ReadyTimeLeft -= Time.deltaTime;
    //                if (ReadyTimeLeft <= 0f)
    //                {
    //                    ReadyTimeLeft = 0f;
    //                    int expiredPlayer = ReadyPlayer;
    //                    int expiredSlot = ReadySlot;

    //                    CloseWindow($"P{expiredPlayer} giữ skill quá {readyDuration:0.#}s mà không dùng -> phạt và khóa skill",
    //                                penalizeUnused: true);
    //                    RaiseLockFeedback(expiredPlayer, expiredSlot, lastPenaltyTaken);
    //                }
    //            }
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
    //        //CloseWindow($"đổi lượt (P{TurnPlayer} -> P{pocket.currentPlayer})");
    //        CloseWindow($"đổi lượt (P{TurnPlayer} -> P{pocket.currentPlayer})", penalizeUnused: true);
    //        return true;
    //    }

    //    return false;
    //}

    //// ================================================================
    //// CÁC BƯỚC TRẠNG THÁI
    //// ================================================================
    //private void OpenWindow()
    //{
    //    if (hideIfNobodyCanAfford && !AnyoneCanAfford())
    //    {
    //        FreezeTurnTimer(false); // [NEW] (phòng trường hợp đi từ Delay sang)
    //        CurrentState = State.Idle;
    //        Log("Không ai đủ stamina cho skill nào -> không mở cửa sổ");
    //        return;
    //    }

    //    CurrentState = State.Selecting;
    //    TimeLeft = selectionDuration;
    //    FreezeTurnTimer(true); // [NEW]

    //    Log($"=== MỞ CỬA SỔ {selectionDuration}s | lượt P{TurnPlayer} | KHÓA xoay ngắm + đánh ===");
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

    //private void MakeReady(int player, int slot)
    //{
    //    ReadyPlayer = player;
    //    ReadySlot = slot;
    //    CurrentState = State.Ready;
    //    readyFrame = Time.frameCount;
    //    ReadyTimeLeft = readyDuration;
    //    FreezeTurnTimer(false); // [NEW] panel đóng -> mở khóa, đồng hồ chạy lại

    //    PlayerSkillController psc = GetPSC(player);
    //    if (psc != null) psc.GrantSlot(slot);

    //    BaseSkills sk = GetSkill(player, slot);
    //    Post($"P{player} grabbed {(sk != null ? sk.skillName : "skill")}");
    //    Log($"READY P{player} slot{slot} | panel đóng, MỞ KHÓA. Chưa trừ stamina: chạm chip để dùng, hoặc đánh bi để bỏ qua");
    //}

    //private void UseReady()
    //{
    //    PlayerSkillController psc = GetPSC(ReadyPlayer);
    //    if (psc == null)
    //    {
    //        Debug.LogError("[SkillSelect] Không tìm thấy PlayerSkillController để dùng skill!");
    //        CloseWindow("lỗi: thiếu PlayerSkillController");
    //        return;
    //    }

    //    bool ok = psc.TryUseSlot(ReadySlot);
    //    if (ok)
    //    {
    //        Post($"P{ReadyPlayer} used skill");
    //        CloseWindow("skill đã được dùng");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[SkillSelect] TryUseSlot thất bại (xem log [PSC]) -> đóng cửa sổ");
    //        CloseWindow("dùng skill thất bại");
    //    }
    //}

    //private void CloseWindow(string reason, bool penalizeUnused = false)
    //{
    //    lastPenaltyTaken = 0f;
    //    // Giành skill nhưng không dùng -> phạt (phải xét TRƯỚC khi đổi state)
    //    if (penalizeUnused && CurrentState == State.Ready)
    //        ApplyUnusedPenalty();

    //    RevokeAllGrants();
    //    queue.Clear();
    //    CurrentState = State.Closed;
    //    FreezeTurnTimer(false); // [NEW]
    //    Log($"=== ĐÓNG CỬA SỔ: {reason} ===");
    //}

    //private void ApplyUnusedPenalty()
    //{
    //    if (unusedGrabPenaltyFlat <= 0f) return;

    //    StaminaManager st = StaminaManagerRegistry.Get(ReadyPlayer);
    //    if (st == null) return;

    //    BaseSkills sk = GetSkill(ReadyPlayer, ReadySlot);
    //    string skillName = sk != null ? sk.skillName : "skill";

    //    float taken = st.ConsumeUpTo(unusedGrabPenaltyFlat);

    //    lastPenaltyTaken = taken;

    //    Post($"P{ReadyPlayer} lost {taken:0.#} stamina (skill not used)");
    //    Log($"PHẠT: P{ReadyPlayer} giành {skillName} nhưng không dùng -> trừ {taken:0.##}/{unusedGrabPenaltyFlat:0.##} stamina");
    //}

    //private void RaiseLockFeedback(int player, int slot, float penalty)
    //{
    //    LockFeedbackActive = true;
    //    LockedPlayer = player;
    //    LockedSlot = slot;
    //    LockedPenalty = penalty;
    //    LockFeedbackTime = Time.unscaledTime;
    //    Log($"LOCK FEEDBACK: skill P{player} slot{slot} bị khóa (phạt {penalty:0.##}) -> UI hiện trạng thái cooldown");
    //}

    //private void ClearLockFeedback()
    //{
    //    if (!LockFeedbackActive) return;
    //    LockFeedbackActive = false;
    //    Log("Tắt phản hồi khóa skill");
    //}

    //private void RevokeAllGrants()
    //{
    //    for (int p = 1; p <= 2; p++)
    //    {
    //        PlayerSkillController psc = GetPSC(p);
    //        if (psc != null) psc.RevokeGrant();
    //    }
    //}

    //// [NEW] ================================================================
    //// ĐỒNG HỒ LƯỢT: đóng băng trong lúc khóa input, chỉ chạy lại nếu chính mình đã đóng băng nó
    //// ================================================================
    //private void FreezeTurnTimer(bool freeze)
    //{
    //    if (cueStick == null) return;

    //    if (freeze)
    //    {
    //        if (!freezeTurnTimerWhileLocked || timerFrozenByUs) return;
    //        timerFrozenByUs = true;
    //        cueStick.stopTimer = true;
    //        Log("Đóng băng đồng hồ lượt");
    //    }
    //    else if (timerFrozenByUs)
    //    {
    //        timerFrozenByUs = false;

    //        // Game đã kết thúc thì PocketTowPs tự giữ stopTimer = true, không được ghi đè
    //        if (pocket == null || !pocket.gameEnd)
    //        {
    //            cueStick.stopTimer = false;
    //            Log("Chạy lại đồng hồ lượt");
    //        }
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

    //[ContextMenu("DEBUG: In trạng thái hiện tại")]
    //private void DebugDumpState()
    //{
    //    Debug.Log($"[SkillSelect] STATE={CurrentState} | TurnPlayer=P{TurnPlayer} | pocket.currentPlayer=P{(pocket != null ? pocket.currentPlayer : -1)} | shotsStarted={shotsStarted} | Ready=P{ReadyPlayer}/slot{ReadySlot} | LockFeedback={LockFeedbackActive} (P{LockedPlayer}/slot{LockedSlot})");

    //    for (int p = 1; p <= 2; p++)
    //    {
    //        PlayerSkillController psc = GetPSC(p);
    //        StaminaManager st = StaminaManagerRegistry.Get(p);
    //        Debug.Log($"[SkillSelect]   P{p}: stamina={(st != null ? st.GetCurrentStamina().ToString("F1") : "NULL")} | {(psc != null ? psc.DebugState() : "PSC NULL")}");

    //        for (int s = 1; s <= 3; s++)
    //        {
    //            BaseSkills sk = GetSkill(p, s);
    //            Debug.Log($"[SkillSelect]      slot{s}: {(sk != null ? sk.skillName : "NULL")} | cost={(sk != null ? sk.staminaCost : 0)} | đủ stamina={CanAfford(p, s)}");
    //        }
    //    }
    //}

    public static SkillSelectionManager Instance { get; private set; }

    public enum State { Idle, Delay, Selecting, Ready, Targeting, Closed }

    [Header("References (để trống sẽ tự tìm)")]
    public PocketTowPs pocket;
    public CueStickController cueStick;

    [Header("Timing")]
    [Tooltip("Chờ thêm sau khi bi dừng. CueStickController đã tự chờ ~1s ổn định nên để 0.")]
    public float delayBeforeWindow = 0f;
    public float selectionDuration = 3f;

    [Header("Khi nào có cửa sổ")]
    [Tooltip("Cửa sổ chỉ mở từ cú đánh thứ N trở đi. 3 = bắt đầu ở cú thứ 3.")]
    public int firstShotWithWindow = 3;
    [Tooltip("true = nếu không ai đủ stamina (hoặc không dùng được) cho skill nào thì không mở cửa sổ.")]
    public bool hideIfNobodyCanAfford = false;

    [Header("Đồng hồ lượt")]
    [Tooltip("true = đóng băng đồng hồ lượt trong lúc khóa input (Delay/Selecting/Targeting).")]
    public bool freezeTurnTimerWhileLocked = true;

    [Header("Phạt khi giành skill mà không dùng")]
    [Tooltip("Số stamina bị trừ cố định nếu giành skill nhưng không dùng. 0 = tắt phạt.")]
    public float unusedGrabPenaltyFlat = 20f;

    [Header("Thời hạn dùng skill đã giành")]
    [Tooltip("Số giây được dùng skill sau khi giành. Hết giờ chưa dùng -> bị phạt và khóa skill. 0 = không giới hạn.")]
    public float readyDuration = 5f;

    [Header("Chọn vùng / chọn mục tiêu")]
    [Tooltip("Thời gian tối đa ở chế độ chọn. Hết giờ = hủy. 0 = không giới hạn.")]
    public float targetingDuration = 10f;
    [Tooltip("true = bấm Hủy hoặc hết giờ ở chế độ chọn thì bị phạt như 'giành mà không dùng'.")]
    public bool cancelPenalizes = true;

    [Header("Debug")]
    public bool verboseLog = true;

    // ===== Trạng thái cho UI đọc =====
    public State CurrentState { get; private set; } = State.Idle;
    public int TurnPlayer { get; private set; }
    public float TimeLeft { get; private set; }
    public int ReadyPlayer { get; private set; }
    public int ReadySlot { get; private set; }
    public float ReadyTimeLeft { get; private set; }
    public string LastMessage { get; private set; } = "";
    public float LastMessageTime { get; private set; } = -999f;
    public bool IsInDelay => CurrentState == State.Delay;
    public bool IsSelecting => CurrentState == State.Selecting;
    public bool IsTargeting => CurrentState == State.Targeting;

    // Chọn vùng / mục tiêu
    public BaseSkills TargetingSkill { get; private set; }
    public float TargetingTimeLeft { get; private set; }

    // Logo phép Toggle (hiện dưới joystick tới khi bắt đầu cú đánh kế tiếp)
    public BaseSkills BadgeSkill { get; private set; }
    public int BadgePlayer { get; private set; }

    // Phản hồi "skill đã khóa" cho UI
    public bool LockFeedbackActive { get; private set; }
    public int LockedPlayer { get; private set; }
    public int LockedSlot { get; private set; }
    public float LockedPenalty { get; private set; }
    public float LockFeedbackTime { get; private set; } = -999f;

    private int shotsStarted = 0;
    public int ShotsStarted => shotsStarted;
    /// <summary>true nếu cửa sổ sẽ mở cho cú đánh kế tiếp (dựa trên số cú đã đánh).</summary>
    public bool WindowEnabledForNextShot => shotsStarted + 1 >= firstShotWithWindow;

    private enum RequestKind { Grab, Use, Confirm, Cancel }
    private struct Request
    {
        public int player;
        public int slot;
        public RequestKind kind;
        public SkillTargetData data;
    }

    private readonly List<Request> queue = new List<Request>();
    private readonly List<Request> workList = new List<Request>();
    private readonly bool[,] usable = new bool[3, 4]; // [player 1..2, slot 1..3]
    private int readyFrame = -1;
    private int targetingFrame = -1;
    private bool timerFrozenByUs = false;
    private float lastPenaltyTaken = 0f;

    private bool previewCommitted = false;

    // ================================================================
    private void Awake()
    {
        Instance = this;
        if (pocket == null) pocket = FindFirstObjectByType<PocketTowPs>();
        if (pocket == null) Debug.LogError("[SkillSelect] Không tìm thấy PocketTowPs!");

        if (cueStick == null) cueStick = FindFirstObjectByType<CueStickController>();
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
        ClearLockFeedback();

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
        TargetingSkill = null;
        TurnPlayer = turnPlayerNumber;

        if (delayBeforeWindow > 0f)
        {
            CurrentState = State.Delay;
            TimeLeft = delayBeforeWindow;
            FreezeTurnTimer(true);
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
        ClearLockFeedback();
        ClearBadge();

        shotsStarted++;
        Log($"SHOT STARTED | cú đánh thứ {shotsStarted}");

        bool hadSkill = CurrentState == State.Ready || CurrentState == State.Targeting;
        int readyP = ReadyPlayer;

        RevokeAllGrants();

        if (CurrentState == State.Delay || CurrentState == State.Selecting || hadSkill)
        {
            CloseWindow(hadSkill
                ? $"cú đánh đã thực hiện, P{readyP} không dùng skill"
                : "cú đánh đã được thực hiện",
                penalizeUnused: hadSkill);
        }
    }

    /// <summary>Giành skill. slot = 1..3.</summary>
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

    /// <summary>Người đã giành skill chọn DÙNG (chạm nút skill).</summary>
    public void RequestUse(int player)
    {
        queue.Add(new Request { player = player, slot = 0, kind = RequestKind.Use });
        Log($"REQUEST USE P{player} | frame {Time.frameCount}");
    }

    /// <summary>Bấm DUYỆT ở chế độ chọn vùng / mục tiêu.</summary>
    public void RequestConfirm(int player, SkillTargetData data)
    {
        queue.Add(new Request { player = player, slot = 0, kind = RequestKind.Confirm, data = data });
        //Log($"REQUEST CONFIRM P{player} | pos={data.position} | ball={(data.ball != null ? data.ball.name : "none")} | frame {Time.frameCount}");
        Log($"REQUEST CONFIRM P{player} | ball={(data.ball != null ? data.ball.name : "none")} | frame {Time.frameCount}");
    }

    /// <summary>Bấm HỦY ở chế độ chọn vùng / mục tiêu.</summary>
    public void RequestCancel(int player)
    {
        queue.Add(new Request { player = player, slot = 0, kind = RequestKind.Cancel });
        Log($"REQUEST CANCEL P{player} | frame {Time.frameCount}");
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
        if (st == null) return true;
        return st.GetCurrentStamina() >= skill.staminaCost;
    }

    /// <summary>Skill có dùng được lúc này không (vd: Target mà hết bi hợp lệ thì false). Tính 1 lần khi mở cửa sổ.</summary>
    public bool IsUsable(int player, int slot)
    {
        if (player < 1 || player > 2 || slot < 1 || slot > 3) return false;
        return usable[player, slot];
    }

    public bool IsSlotLit(int player, int slot)
    {
        return CurrentState == State.Selecting && CanAfford(player, slot) && IsUsable(player, slot);
    }

    private bool AnyoneCanAfford()
    {
        for (int p = 1; p <= 2; p++)
            for (int s = 1; s <= 3; s++)
                if (CanAfford(p, s) && usable[p, s]) return true;
        return false;
    }

    // ================================================================
    // VÒNG LẶP CHÍNH
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
        UpdateBadgeLifetime();
    }

    private void ProcessQueue()
    {
        if (queue.Count == 0) return;

        if (CurrentState != State.Selecting && CurrentState != State.Ready && CurrentState != State.Targeting)
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
        switch (CurrentState)
        {
            case State.Selecting: ResolveSelecting(r); break;
            case State.Ready: ResolveReady(r); break;
            case State.Targeting: ResolveTargeting(r); break;
        }
    }

    private void ResolveSelecting(Request r)
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

        if (!IsUsable(r.player, r.slot))
        {
            Log($"P{r.player} slot{r.slot}: không dùng được lúc này (không có mục tiêu hợp lệ) -> bỏ qua");
            return;
        }

        if (r.player != TurnPlayer)
        {
            Post($"P{r.player} grabbed out of turn - nobody can use skills");
            CloseWindow($"P{r.player} giành ngoài lượt -> không ai được dùng skill");
            RaiseLockFeedback(r.player, r.slot, 0f);
            return;
        }

        MakeReady(r.player, r.slot);
    }

    private void ResolveReady(Request r)
    {
        if (r.kind != RequestKind.Use)
        {
            Log($"P{r.player} {r.kind}: không hợp lệ ở trạng thái READY -> bỏ qua");
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

    private void ResolveTargeting(Request r)
    {
        if (r.kind != RequestKind.Confirm && r.kind != RequestKind.Cancel)
        {
            Log($"P{r.player} {r.kind}: không hợp lệ ở chế độ chọn -> bỏ qua");
            return;
        }

        if (r.player != ReadyPlayer)
        {
            Log($"P{r.player} {r.kind}: không phải người đang chọn (P{ReadyPlayer}) -> bỏ qua");
            return;
        }

        if (Time.frameCount == targetingFrame)
        {
            Log($"{r.kind} cùng frame với lúc vào chế độ chọn -> bỏ qua");
            return;
        }

        if (r.kind == RequestKind.Confirm) ConfirmTargeting(r.data);
        else CancelTargeting("người chơi bấm Hủy");
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
                if (ShouldAbort()) return;

                if (readyDuration > 0f)
                {
                    ReadyTimeLeft -= Time.deltaTime;
                    if (ReadyTimeLeft <= 0f)
                    {
                        ReadyTimeLeft = 0f;
                        int expiredPlayer = ReadyPlayer;
                        int expiredSlot = ReadySlot;

                        CloseWindow($"P{expiredPlayer} giữ skill quá {readyDuration:0.#}s mà không dùng -> phạt và khóa skill",
                                    penalizeUnused: true);
                        RaiseLockFeedback(expiredPlayer, expiredSlot, lastPenaltyTaken);
                    }
                }
                break;

            case State.Targeting:
                if (ShouldAbort()) return;

                if (targetingDuration > 0f)
                {
                    TargetingTimeLeft -= Time.deltaTime;
                    if (TargetingTimeLeft <= 0f)
                    {
                        TargetingTimeLeft = 0f;
                        CancelTargeting($"hết {targetingDuration:0.#}s chọn");
                    }
                }
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
            CloseWindow($"đổi lượt (P{TurnPlayer} -> P{pocket.currentPlayer})", penalizeUnused: true);
            return true;
        }

        return false;
    }

    // ================================================================
    // CÁC BƯỚC TRẠNG THÁI
    // ================================================================
    private void OpenWindow()
    {
        RefreshUsableCache();

        if (hideIfNobodyCanAfford && !AnyoneCanAfford())
        {
            FreezeTurnTimer(false);
            CurrentState = State.Idle;
            Log("Không ai đủ stamina / không dùng được skill nào -> không mở cửa sổ");
            return;
        }

        CurrentState = State.Selecting;
        TimeLeft = selectionDuration;
        FreezeTurnTimer(true);

        Log($"=== MỞ CỬA SỔ {selectionDuration}s | lượt P{TurnPlayer} | KHÓA xoay ngắm + đánh ===");
        for (int p = 1; p <= 2; p++)
        {
            for (int s = 1; s <= 3; s++)
            {
                BaseSkills sk = GetSkill(p, s);
                string nm = sk != null ? sk.skillName : "NULL";
                int cost = sk != null ? sk.staminaCost : 0;
                string type = sk != null ? sk.targetingType.ToString() : "-";
                Log($"   P{p} slot{s}: {nm} [{type}] | cost={cost} | đủ stamina={CanAfford(p, s)} | dùng được={usable[p, s]}");
            }
        }
    }

    private void RefreshUsableCache()
    {
        for (int p = 1; p <= 2; p++)
            for (int s = 1; s <= 3; s++)
                usable[p, s] = ComputeUsable(p, s);
    }

    private bool ComputeUsable(int player, int slot)
    {
        BaseSkills sk = GetSkill(player, slot);
        if (sk == null) return false;

        GlaszekManager g = PlayerManagerRegistry.Get(player);
        if (g == null) return true;
        return sk.CanUse(g.gameObject, g);
    }

    private void MakeReady(int player, int slot)
    {
        ReadyPlayer = player;
        ReadySlot = slot;
        CurrentState = State.Ready;
        readyFrame = Time.frameCount;
        ReadyTimeLeft = readyDuration;
        FreezeTurnTimer(false); // panel đóng -> mở khóa, đồng hồ chạy lại

        PlayerSkillController psc = GetPSC(player);
        if (psc != null) psc.GrantSlot(slot);

        BaseSkills sk = GetSkill(player, slot);
        Post($"P{player} grabbed {(sk != null ? sk.skillName : "skill")}");
        Log($"READY P{player} slot{slot} ({(sk != null ? sk.targetingType.ToString() : "?")}) | panel đóng, MỞ KHÓA. Chưa trừ stamina: chạm nút để dùng, hoặc đánh bi để bỏ qua");
    }

    private void UseReady()
    {
        BaseSkills skill = GetSkill(ReadyPlayer, ReadySlot);
        if (skill == null)
        {
            CloseWindow("lỗi: không có skill ở slot đang giữ");
            return;
        }

        //if (skill.targetingType == SkillTargetingType.Toggle)
        //{
        //    UseNow(skill, false, default(SkillTargetData));
        //}
        //else
        //{
        //    BeginTargeting(skill);
        //}

        if (!skill.IsTargetSkill)
        {
            UseNow(skill, false, default(SkillTargetData));
        }
        else
        {
            BeginTargeting(skill);
        }
    }

    private void BeginTargeting(BaseSkills skill)
    {
        GlaszekManager g = PlayerManagerRegistry.Get(ReadyPlayer);
        if (g != null && !skill.CanUse(g.gameObject, g))
        {
            Post("No valid target");
            Log($"{skill.skillName}: không còn mục tiêu hợp lệ -> không vào chế độ chọn (vẫn ở READY)");
            return;
        }

        CurrentState = State.Targeting;
        TargetingSkill = skill;
        TargetingTimeLeft = targetingDuration;
        targetingFrame = Time.frameCount;

        previewCommitted = false;

        FreezeTurnTimer(true);

        Log($"=== VÀO CHẾ ĐỘ CHỌN {skill.targetingType} | P{ReadyPlayer} | {skill.skillName} | tối đa {targetingDuration:0.#}s ===");
    }

    private void ConfirmTargeting(SkillTargetData data)
    {
        BaseSkills skill = GetSkill(ReadyPlayer, ReadySlot);
        GlaszekManager g = PlayerManagerRegistry.Get(ReadyPlayer);

        if (skill == null || g == null)
        {
            CancelTargeting("thiếu skill hoặc manager");
            return;
        }

        //bool valid;
        //if (skill.targetingType == SkillTargetingType.Area)
        //    valid = skill.IsValidArea(data.position, g);
        //else
        //    valid = data.ball != null && data.ball.gameObject.activeInHierarchy && skill.IsValidTarget(data.ball, g);
        bool valid = data.ball != null && data.ball.gameObject.activeInHierarchy && skill.IsValidTarget(data.ball, g);

        if (!valid)
        {
            Post("Invalid selection");
            Log($"CONFIRM không hợp lệ (ball={(data.ball != null ? data.ball.name : "none")}) -> vẫn ở chế độ chọn");
            return;
        }

        if (!valid)
        {
            Post("Invalid selection");
            Log($"CONFIRM không hợp lệ (pos={data.position}, ball={(data.ball != null ? data.ball.name : "none")}) -> vẫn ở chế độ chọn");
            return;
        }

        UseNow(skill, true, data);
    }

    private void CancelTargeting(string reason)
    {
        BaseSkills skill = GetSkill(ReadyPlayer, ReadySlot);
        Post($"P{ReadyPlayer} cancelled {(skill != null ? skill.skillName : "skill")}");
        CloseWindow($"P{ReadyPlayer} hủy chọn: {reason}", penalizeUnused: cancelPenalizes);
    }

    private void UseNow(BaseSkills skill, bool withTarget, SkillTargetData data)
    {
        PlayerSkillController psc = GetPSC(ReadyPlayer);
        if (psc == null)
        {
            Debug.LogError("[SkillSelect] Không tìm thấy PlayerSkillController để dùng skill!");
            CloseWindow("lỗi: thiếu PlayerSkillController");
            return;
        }

        bool ok = withTarget ? psc.TryUseSlotWithTarget(ReadySlot, data) : psc.TryUseSlot(ReadySlot);
        if (ok)
        {
            previewCommitted = true;
            // Phép Toggle muốn hiện logo dưới joystick tới cú đánh kế tiếp
            //if (skill.targetingType == SkillTargetingType.Toggle && skill.showLogoUntilNextShot)
            //    SetBadge(skill, ReadyPlayer);
            if (!skill.IsTargetSkill && skill.showLogoUntilNextShot)
                SetBadge(skill, ReadyPlayer);

            Post($"P{ReadyPlayer} used {skill.skillName}");
            CloseWindow("skill đã được dùng");
        }
        else
        {
            Debug.LogWarning("[SkillSelect] TryUse thất bại (xem log [PSC]) -> đóng cửa sổ");
            CloseWindow("dùng skill thất bại");
        }
    }

    private void CloseWindow(string reason, bool penalizeUnused = false)
    {
        lastPenaltyTaken = 0f;

        if (CurrentState == State.Targeting && TargetingSkill != null && TargetingSkill.livePreview && !previewCommitted)
        {
            TargetingSkill.CancelPreview(PlayerManagerRegistry.Get(ReadyPlayer));
            Log($"Thu hồi bản xem trước của {TargetingSkill.skillName}");
        }

        // Giành skill nhưng không dùng (đang READY hoặc đang chọn) -> phạt (xét TRƯỚC khi đổi state)
        if (penalizeUnused && (CurrentState == State.Ready || CurrentState == State.Targeting))
            ApplyUnusedPenalty();

        RevokeAllGrants();
        queue.Clear();
        TargetingSkill = null;
        CurrentState = State.Closed;
        FreezeTurnTimer(false);
        Log($"=== ĐÓNG CỬA SỔ: {reason} ===");
    }

    private void ApplyUnusedPenalty()
    {
        if (unusedGrabPenaltyFlat <= 0f) return;

        StaminaManager st = StaminaManagerRegistry.Get(ReadyPlayer);
        if (st == null) return;

        BaseSkills sk = GetSkill(ReadyPlayer, ReadySlot);
        string skillName = sk != null ? sk.skillName : "skill";

        float taken = st.ConsumeUpTo(unusedGrabPenaltyFlat);
        lastPenaltyTaken = taken;

        Post($"P{ReadyPlayer} lost {taken:0.#} stamina (skill not used)");
        Log($"PHẠT: P{ReadyPlayer} giành {skillName} nhưng không dùng -> trừ {taken:0.##}/{unusedGrabPenaltyFlat:0.##} stamina");
    }

    private void RaiseLockFeedback(int player, int slot, float penalty)
    {
        LockFeedbackActive = true;
        LockedPlayer = player;
        LockedSlot = slot;
        LockedPenalty = penalty;
        LockFeedbackTime = Time.unscaledTime;
        Log($"LOCK FEEDBACK: skill P{player} slot{slot} bị khóa (phạt {penalty:0.##})");
    }

    private void ClearLockFeedback()
    {
        if (!LockFeedbackActive) return;
        LockFeedbackActive = false;
        Log("Tắt phản hồi khóa skill");
    }

    // ===== Logo phép Toggle =====
    private void SetBadge(BaseSkills skill, int player)
    {
        BadgeSkill = skill;
        BadgePlayer = player;
        Log($"BADGE: logo {skill.skillName} hiện dưới joystick tới khi bắt đầu cú đánh kế tiếp");
    }

    private void ClearBadge()
    {
        if (BadgeSkill == null) return;
        Log($"BADGE: tắt logo {BadgeSkill.skillName}");
        BadgeSkill = null;
    }

    private void UpdateBadgeLifetime()
    {
        if (BadgeSkill == null || pocket == null) return;
        if (pocket.gameEnd || pocket.currentPlayer != BadgePlayer) ClearBadge();
    }

    private void RevokeAllGrants()
    {
        for (int p = 1; p <= 2; p++)
        {
            PlayerSkillController psc = GetPSC(p);
            if (psc != null) psc.RevokeGrant();
        }
    }

    // ================================================================
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

    [ContextMenu("DEBUG: In trạng thái hiện tại")]
    private void DebugDumpState()
    {
        Debug.Log($"[SkillSelect] STATE={CurrentState} | TurnPlayer=P{TurnPlayer} | pocket.currentPlayer=P{(pocket != null ? pocket.currentPlayer : -1)} | shotsStarted={shotsStarted} | Ready=P{ReadyPlayer}/slot{ReadySlot} | Targeting={(TargetingSkill != null ? TargetingSkill.skillName : "none")} | Badge={(BadgeSkill != null ? BadgeSkill.skillName : "none")}");

        for (int p = 1; p <= 2; p++)
        {
            PlayerSkillController psc = GetPSC(p);
            StaminaManager st = StaminaManagerRegistry.Get(p);
            Debug.Log($"[SkillSelect]   P{p}: stamina={(st != null ? st.GetCurrentStamina().ToString("F1") : "NULL")} | {(psc != null ? psc.DebugState() : "PSC NULL")}");

            for (int s = 1; s <= 3; s++)
            {
                BaseSkills sk = GetSkill(p, s);
                Debug.Log($"[SkillSelect]      slot{s}: {(sk != null ? sk.skillName : "NULL")} [{(sk != null ? sk.targetingType.ToString() : "-")}] | cost={(sk != null ? sk.staminaCost : 0)} | đủ stamina={CanAfford(p, s)} | dùng được={usable[p, s]}");
            }
        }
    }
}
