using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PocketTowPs : MonoBehaviour
{
    public CueStickController cueStickController;
    public GameManager gameManager;
    public Aiming aiming;

    public TextMeshProUGUI assignBallDisplayText_1, assignBallDisplayText_2, foulText, selectedGroupText, winPlayerText, bottomMessagerText; //losePlayerText,
    public TextMeshProUGUI targetBallID_1, targetBallID_2;
    public GameObject noGroupImage_01, noGroupImage_02, stripesImage_01, stripesImage_02, solidImage_01,solidImage_02, nineBalls_01, nineBalls_02;

    public Animator bottomMessagerAnimator;

    public Image playerID_01, playerID_02;
   // public Sprite activeSprite, simpleSprite;

    public int currentPlayer = 1;
    public int targetBallNumber = 1;
    public bool gameEnd = false, groupAssigned = false, correctBallPotted = false;

    private bool ballPottedThisTurn = false;
    private bool foulCommittedThisTurn = false;
    private bool isNineBallPotted = false;
    private bool hitTargetFirstFromController = false;
    private bool anyColoredBallDestroyed = false;

    private List<int> pottedBalls = new List<int>();
    private StringBuilder shotReport = new StringBuilder();
    private NineBallRules rules;

    private List<Collider> player01PottedBalls = new List<Collider> ();
    private List<Collider> player02PottedBalls = new List<Collider> ();

    public string player01Name, player02Name;
    private string player01Group = "", player02Group = "";
    public TextMeshProUGUI player01NameText, player02NameText, totalTimetext;
    public Slider highlightSlider_01, highlightSlider_02;

    public int totalPottedBallsCount, totalRacksCount;

    public float totalTimeInput, timeRemaining;

    public TextMeshProUGUI currentPottedText01, currentPottedText02, player01NameInWinPanel, player02NameInWinPanel, totalPottedBallText, totalRackText;

    void Start()
    {
        ////cueStickController = GetComponent<CueStickController>();
        ////gameManager = GetComponent<GameManager>();
        ////aiming = gameManager.GetComponent<Aiming>();

        ////bottomMessagerText = bottomMessagerAnimator.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        ////noGroupImage_01.SetActive(true);
        ////solidImage_01.SetActive(false);
        ////stripesImage_01.SetActive(false);
        ////nineBalls_01.SetActive(false);

        ////noGroupImage_02.SetActive(true);
        ////solidImage_02.SetActive(false);
        ////stripesImage_02.SetActive(false);
        ////nineBalls_02.SetActive(false);

        ////bottomMessagerAnimator.gameObject.SetActive(true);
        ////foulText.gameObject.SetActive(true);
        ////selectedGroupText.gameObject.SetActive(true);
        ////winPlayerText.gameObject.SetActive(false);

        ////rules = GetComponent<NineBallRules>();
        ////if (rules == null) rules = gameObject.AddComponent<NineBallRules>();

        ////LoadPlayersInfoData();
        ////ResetGame();
        ////LoadRacksAndBallsCount();
        ////UpdateNextTarget();

        //if (gameManager == null)
        //    Debug.LogError("GameManager not assigned!");

        //if (cueStickController == null)
        //    Debug.LogError("CueStickController not assigned!");

        //if (aiming == null && gameManager != null)
        //    aiming = gameManager.GetComponent<Aiming>();

        //if (bottomMessagerAnimator != null)
        //    bottomMessagerText = bottomMessagerAnimator.GetComponentInChildren<TextMeshProUGUI>();

        //noGroupImage_01.SetActive(true);
        //solidImage_01.SetActive(false);
        //stripesImage_01.SetActive(false);
        //nineBalls_01.SetActive(false);

        //noGroupImage_02.SetActive(true);
        //solidImage_02.SetActive(false);
        //stripesImage_02.SetActive(false);
        //nineBalls_02.SetActive(false);

        ////bottomMessagerAnimator.gameObject.SetActive(true);
        ////foulText.gameObject.SetActive(true);
        ////selectedGroupText.gameObject.SetActive(true);
        ////winPlayerText.gameObject.SetActive(false);

        //ResetUI();

        //rules = GetComponent<NineBallRules>();
        //if (rules == null)
        //    rules = gameObject.AddComponent<NineBallRules>();

        //LoadPlayersInfoData();
        //ResetGame();
        //LoadRacksAndBallsCount();
        //UpdateNextTarget();

        if (gameManager == null)
            Debug.LogError("GameManager not assigned!");

        if (cueStickController == null)
            Debug.LogError("CueStickController not assigned!");

        if (aiming == null && gameManager != null)
            aiming = gameManager.GetComponent<Aiming>();

        if (bottomMessagerAnimator != null)
            bottomMessagerText = bottomMessagerAnimator.GetComponentInChildren<TextMeshProUGUI>();

        // ===== UI DEFAULT =====
        noGroupImage_01.SetActive(true);
        solidImage_01.SetActive(false);
        stripesImage_01.SetActive(false);
        nineBalls_01.SetActive(false);

        noGroupImage_02.SetActive(true);
        solidImage_02.SetActive(false);
        stripesImage_02.SetActive(false);
        nineBalls_02.SetActive(false);

        ResetUI();

        rules = GetComponent<NineBallRules>();
        if (rules == null)
            rules = gameObject.AddComponent<NineBallRules>();

        LoadPlayersInfoData();
        ResetGame();
        LoadRacksAndBallsCount();
        UpdateNextTarget();

        // 🔥🔥🔥 FIX QUAN TRỌNG: FORCE HIỆN UI
        ForceShowUI();
    }

    private void Update()
    {
        if(currentPlayer == 1)
        {
            highlightSlider_01.gameObject.SetActive(true);
            highlightSlider_02.gameObject.SetActive(false);
        }
        else if(currentPlayer == 2)
        {
            highlightSlider_01.gameObject.SetActive(false);
            highlightSlider_02.gameObject.SetActive(true);
        }

        UpdateTimer();
    }
    private void ResetUI()
    {
        bottomMessagerAnimator.gameObject.SetActive(true);

        foulText.gameObject.SetActive(false);
        selectedGroupText.gameObject.SetActive(false);
        winPlayerText.gameObject.SetActive(false);
    }

    private void ForceShowUI()
    {
        Debug.Log("FORCE SHOW UI");

        // TEXT
        if (player01NameText) player01NameText.gameObject.SetActive(true);
        if (player02NameText) player02NameText.gameObject.SetActive(true);
        if (totalTimetext) totalTimetext.gameObject.SetActive(true);

        if (targetBallID_1) targetBallID_1.gameObject.SetActive(true);
        if (targetBallID_2) targetBallID_2.gameObject.SetActive(true);

        if (assignBallDisplayText_1) assignBallDisplayText_1.gameObject.SetActive(true);
        if (assignBallDisplayText_2) assignBallDisplayText_2.gameObject.SetActive(true);

        // SLIDER TURN
        if (highlightSlider_01) highlightSlider_01.gameObject.SetActive(true);
        if (highlightSlider_02) highlightSlider_02.gameObject.SetActive(false);

        // MESSAGE UI
        if (bottomMessagerAnimator)
        {
            bottomMessagerAnimator.gameObject.SetActive(true);

            // ❗ disable animator để tránh bị ẩn UI
            //bottomMessagerAnimator.enabled = false;
            bottomMessagerAnimator.gameObject.SetActive(true);
        }

        // WIN TEXT (ẩn)
        if (winPlayerText)
            winPlayerText.gameObject.SetActive(false);
    }

    private void ResetGame()
    {
        groupAssigned = false;
        correctBallPotted = false;
        currentPlayer = 1;
        player01Group = "";
        player02Group = "";
        player01PottedBalls.Clear();
        player02PottedBalls.Clear();

        highlightSlider_01.gameObject.SetActive(true);
        highlightSlider_02.gameObject.SetActive(false);

        targetBallID_1.text = "Target Balls";
        targetBallID_2.text = "Target Balls";

        timeRemaining = totalTimeInput;
        cueStickController.stopTimer = false;
    }

    public void LoadPlayersInfoData()
    {
        player01Name = "PLAYER 1";
        player02Name = "PLAYER 2";
        totalTimeInput = 30;

        player01NameText.text = player01Name;
        player02NameText.text = player02Name;
        totalTimetext.text = totalTimeInput.ToString();

        player01NameInWinPanel .text = player01Name;
        player02NameInWinPanel .text = player02Name;
    }

    private void LoadRacksAndBallsCount()
    {
        //totalPottedBallsCount = PlayerPrefs.GetInt("PottedBallsSaveed", 0);
        //totalRacksCount = PlayerPrefs.GetInt("RacksCountSaved", 0);

        //totalPottedBallText.text = totalPottedBallsCount.ToString();
        //totalRackText.text = totalRackText.ToString();  

        totalPottedBallsCount = PlayerPrefs.GetInt("PocketedBallsSaved", 0);
        totalRacksCount = PlayerPrefs.GetInt("RacksCountSaved", 0);

        totalPottedBallText.text = totalPottedBallsCount.ToString();
        totalRackText.text = totalRacksCount.ToString(); // ✅ FIX BUG
    }

    private void UpdateTimer()
    {
        if (cueStickController.stopTimer) return;

        if (currentPlayer == 1)
            highlightSlider_01.value = timeRemaining / totalTimeInput;
        if (currentPlayer == 2)
            highlightSlider_02.value = timeRemaining / totalTimeInput;

        if(timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            SwitchTurn();    
        }

        int seconds = Mathf.Clamp(Mathf.FloorToInt(timeRemaining), 0, 99);

        totalTimetext.text = string.Format("{0:00}", seconds);
    }
    public void SavePocketedBalls()
    {
        //totalPottedBallsCount += player01PottedBalls.Count + player02PottedBalls.Count;
        //PlayerPrefs.SetInt("PocketedBallsSaved", totalPottedBallsCount);
        //LoadRacksAndBallsCount();

        totalPottedBallsCount += player01PottedBalls.Count + player02PottedBalls.Count;
        PlayerPrefs.SetInt("PocketedBallsSaved", totalPottedBallsCount); // ✅ đồng bộ key
        LoadRacksAndBallsCount();
    }
    //private IEnumerator OnTriggerEnter(Collider ball)
    //{
    //    //string ballTag = ball.tag;

    //    //// Handle Cue Ball 
    //    //if (ballTag == "CueBall");
    //    //{
    //    //    ball.transform.position = new Vector3(2, 2.57469f, 0);
    //    //    ball.attachedRigidbody.angularVelocity = Vector3.zero;
    //    //    yield break;
    //    //}

    //    string ballTag = ball.tag;

    //    // Đã xóa dấu ";" dư thừa sau câu lệnh if
    //    if (ballTag == "CueBall")
    //    {
    //        // Reset bi cái về vị trí chỉ định khi rơi xuống lỗ
    //        ball.transform.position = new Vector3(-0.9f, 1.18f, 0);
    //        //ball.transform.position = new Vector3(2, 1.57469f, 0);
    //        ball.attachedRigidbody.linearVelocity = Vector3.zero;
    //        ball.attachedRigidbody.angularVelocity = Vector3.zero;
    //        StartCoroutine(HandleCueBallPotted());
    //        foulCommittedThisTurn = true; // Ghi nhận lỗi
    //        yield break;
    //    }

    //    if(ballTag == "BallNo.9")
    //    {
    //        StartCoroutine(HandleNineBallPottedRoutine());
    //        HandlePottedBall(ball, (currentPlayer == 1 )? player01PottedBalls : player02PottedBalls);
    //        yield break;
    //    }

    //    // if no group have been assigned yet
    //    if(!groupAssigned)
    //    {
    //        selectedGroupText.GetComponent<Animator>().SetTrigger("ShowTrigger");
    //        AssignGroup(ballTag);
    //    }




    //    if(groupAssigned)
    //    {
    //        if (currentPlayer == 1 && ballTag == player01Group + "Ball")
    //        {
    //            HandlePottedBall (ball, player01PottedBalls);
    //            correctBallPotted = true;
    //        }
    //        else if(currentPlayer == 2 && ballTag == player02Group + "Ball")
    //        {
    //             HandlePottedBall(ball, player02PottedBalls);
    //            correctBallPotted = true;
    //        }
    //        else
    //        {
    //            while (bottomMessagerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Button Mesage In") && bottomMessagerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    //            {
    //                yield return null;
    //            }

    //            bottomMessagerAnimator.SetTrigger("ShowTrigger");
    //            bottomMessagerText.text = "Turn to Oppement";

    //            HandlePottedBall(ball, (currentPlayer == 1) ? player02PottedBalls : player01PottedBalls);
    //        }

    //        assignBallDisplayText_1.text = player01PottedBalls.Count + "9";
    //        assignBallDisplayText_2.text = player02PottedBalls.Count + "9";
    //    }
        
    //}

    private void AssignGroup(string ballTag)
    {
        groupAssigned = true;
        Debug.Log("Group Assigned: " + ballTag);

    }

    private void HandlePottedBall(Collider ball, List<Collider> pocketBalls)
    {
        cueStickController.balls.Remove(ball.attachedRigidbody);
        aiming.ballObjects.Remove(ball.gameObject);
        pocketBalls.Add(ball);
        //Destroy(ball.gameObject);
        ball.gameObject.SetActive(false);
    }
    private IEnumerator HandleNineBallPotted()
    {
        if(currentPlayer  == 1)
        {
            if(player01PottedBalls.Count  == 9)
            {
                winPlayerText.text = player01Name + "WON THE MATCH";
            }
            else
            {
                winPlayerText.text = player02Name + "WON THE MATCH";
            }
        }
        else if(currentPlayer == 2)
        {
            if (player02PottedBalls.Count == 9)
            {
                winPlayerText.text = player02Name + "WON THE MATCH";
            }
            else
            {
                winPlayerText.text = player01Name + "WON THE MATCH";
            }
        }

        winPlayerText.gameObject.SetActive(true);
        gameEnd = true;
        cueStickController.stopTimer = true;
        gameManager.cameraButtonAnim.SetBool("IsGoBack", true);
        gameManager.buttonPauseAnim.SetBool("IsGoBack", true);
        cueStickController.powerSliderAnim.SetBool("IsGoBack", true);

        yield return new WaitForSecondsRealtime(3.3f);
        gameManager.ShowWinPanel();
        currentPottedText01.text = player01PottedBalls.Count.ToString();
        currentPottedText02.text = player02PottedBalls.Count.ToString();

        totalPottedBallsCount += player01PottedBalls.Count + player02PottedBalls.Count;
        totalRacksCount++;
        PlayerPrefs.SetInt("PocketdBallsSaved", totalPottedBallsCount);
        PlayerPrefs.SetInt("RacksCountSaved", totalRacksCount);
    }
    public void SetHitResult(bool isCorrectHit) => hitTargetFirstFromController = isCorrectHit;

    public void RegisterStartShot()
    {
        ballPottedThisTurn = false;
        foulCommittedThisTurn = false;
        isNineBallPotted = false;
        correctBallPotted = false;

        shotReport.Clear();
        shotReport.AppendLine($"<color=white><b>--- LƯỢT PLAYER {currentPlayer} ---</b></color>");

        anyColoredBallDestroyed = false;
    }

    public void OnBallEnteredPocket(Collider ball)
    {
        if (gameEnd) return;

        if (ball.CompareTag("CueBall"))
        {
            //foulCommittedThisTurn = true;
            //shotReport.AppendLine("<color=red>  ! LỖI: Bi cái vào lỗ</color>");
            //ball.transform.position = new Vector3(-0.9f, 1.18f, -0.17f);
            //Rigidbody rb = ball.attachedRigidbody;
            //if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
            //return;

            //foulCommittedThisTurn = true;
            //ballPottedThisTurn = false;

            //shotReport.AppendLine("<color=red> ! FOUL: Cue Ball vào lỗ</color>");

            //// reset vị trí bi cái
            //ball.transform.position = new Vector3(-0.9f, 1.18f, -0.17f);

            //Rigidbody rb = ball.attachedRigidbody;

            //if (rb != null)
            //{
            //    rb.linearVelocity = Vector3.zero;
            //    rb.angularVelocity = Vector3.zero;
            //}

            //// HIỆN UI FOUL
            //foulText.gameObject.SetActive(true);
            //foulText.text = "FOUL";

            //bottomMessagerText.text = "Opponent's Turn";
            //bottomMessagerAnimator.gameObject.SetActive(true);

            //// ĐỔI LƯỢT
            //SwitchTurn();

            //return;

            foulCommittedThisTurn = true;
            ballPottedThisTurn = false;
            anyColoredBallDestroyed = true;

            shotReport.AppendLine("<color=red> ! FOUL: Cue Ball vào lỗ</color>");

            // reset vị trí bi cái
            ball.transform.position = new Vector3(-0.9f, 1.18f, -0.17f);

            Rigidbody rb = ball.attachedRigidbody;

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // UI FOUL
            foulText.gameObject.SetActive(true);
            foulText.text = "FOUL";

            StartCoroutine(HideFoulUI());

            // message
            bottomMessagerText.text = "Opponent's Turn";
            bottomMessagerAnimator.gameObject.SetActive(true);

            // đổi lượt
            SwitchTurn();

            return;
        }

        //int nr = GetBallNumber(ball.tag);
        //if (nr > 0)
        //{
        //    ballPottedThisTurn = true;
        //    shotReport.AppendLine($"<color=yellow>  + Vào lỗ: Bi số {nr}</color>");
        //    if (!pottedBalls.Contains(nr)) pottedBalls.Add(nr);
        //    if (nr == 9) isNineBallPotted = true;
        //    Destroy(ball.gameObject);
        //}

        int nr = GetBallNumber(ball.tag);

        if (nr > 0)
        {
            // ĐÁNH DẤU CÓ BI VÀO LỖ
            ballPottedThisTurn = true;

            shotReport.AppendLine($"<color=yellow>  + Vào lỗ: Bi số {nr}</color>");

            // lưu bi đã vào
            if (!pottedBalls.Contains(nr))
                pottedBalls.Add(nr);

            // KIỂM TRA BI MỤC TIÊU
            if (nr == targetBallNumber)
            {
                correctBallPotted = true;

                Debug.Log("<color=green>Đã vào đúng bi mục tiêu -> GIỮ LƯỢT</color>");

                UpdateNextTarget();
            }

            // BI 9
            if (nr == 9)
            {
                isNineBallPotted = true;
            }

            //Destroy(ball.gameObject);
            ball.gameObject.SetActive(false);
        }
    }

    public void OnBallDestroyed(GameObject ballObj)
    {
        //if (gameEnd) return;

        //int nr = GetBallNumber(ballObj.tag);

        //if (nr <= 0) return;

        //Debug.Log("<color=yellow>Bi bị destroy: " + nr + "</color>");

        //// đánh dấu có bi biến mất
        //ballPottedThisTurn = true;

        //// lưu bi đã clear
        //if (!pottedBalls.Contains(nr))
        //{
        //    pottedBalls.Add(nr);
        //}

        //// kiểm tra đúng bi mục tiêu
        //if (nr == targetBallNumber)
        //{
        //    correctBallPotted = true;

        //    Debug.Log("<color=green>Đúng bi mục tiêu -> giữ lượt</color>");
        //}

        //// bi 9
        //if (nr == 9)
        //{
        //    isNineBallPotted = true;
        //}

        if (gameEnd) return;

        int nr = GetBallNumber(ballObj.tag);

        if (nr <= 0) return;

        Debug.Log("<color=yellow>Bi bị destroy: " + nr + "</color>");

        // Có bi màu chết
        anyColoredBallDestroyed = true;

        // đánh dấu có bi vào
        ballPottedThisTurn = true;

        // lưu list
        if (!pottedBalls.Contains(nr))
        {
            pottedBalls.Add(nr);
        }

        // =========================
        // WIN NGAY KHI BI 9 CHẾT
        // =========================
        if (nr == 9)
        {
            isNineBallPotted = true;

            StartCoroutine(HandleNineBallPottedRoutine());

            return;
        }

        // update target mới
        UpdateNextTarget();
    }

    private IEnumerator HandleNineBallPottedRoutine()
    {
        if (gameEnd) yield break;

        // Đợi một chút để xem bi cái có rơi xuống lỗ sau bi 9 không
        //yield return new WaitForSeconds(1.5f);
        yield return new WaitForSecondsRealtime(1.5f);

        // Kiểm tra: Nếu bi 9 vào lỗ mà KHÔNG phạm lỗi
        if (!foulCommittedThisTurn && hitTargetFirstFromController)
        {
            gameEnd = true;

            // Hiển thị UI thắng cuộc
            winPlayerText.text = (currentPlayer == 1 ? player01Name : player02Name) + " CHIẾN THẮNG!";
            winPlayerText.gameObject.SetActive(true);

            // Lưu dữ liệu
            SavePocketedBalls();

            Debug.Log($"*********** Player {currentPlayer} ĐÃ THẮNG (Bi 9 hợp lệ)! ***********");
        }
        else
        {
            // TRƯỜNG HỢP LỖI: Bi 9 vào nhưng phạm lỗi
            shotReport.AppendLine("<color=red>! Bi 9 vào lỗ nhưng PHẠM LỖI.</color>");

            // Theo luật chuẩn: Bi 9 vào lỗ mà lỗi thì bi 9 được đặt lại điểm cuối bàn (Foot Spot)
            // Hoặc đơn giản là chuyển lượt cho đối thủ (Ball-in-hand)
            isNineBallPotted = false; // Reset trạng thái để game tiếp tục

            SwitchTurn(); // Đổi lượt sang người kia
        }
    }
    public void HandleStrokeResult()
    {
        //if (gameEnd) return;

        //// 1. Kiểm tra lỗi chạm bi mục tiêu
        //if (!hitTargetFirstFromController)
        //{
        //    foulCommittedThisTurn = true;
        //    shotReport.AppendLine($"<color=red>  ! LỖI: Không chạm bi {targetBallNumber} đầu tiên</color>");
        //}

        //// 2. Kiểm tra điều kiện thắng bi 9
        //if (isNineBallPotted && !foulCommittedThisTurn)
        //{
        //    gameEnd = true;
        //    shotReport.AppendLine($"<color=cyan><b>*** PLAYER {currentPlayer} THẮNG TRẬN! ***</b></color>");
        //    Debug.Log(shotReport.ToString());
        //    return;
        //}

        //// 3. Logic đổi lượt hoặc tiếp tục
        //if (!foulCommittedThisTurn && ballPottedThisTurn)
        //{
        //    shotReport.AppendLine($"<color=green>=> GIỮ LƯỢT.</color>");
        //}
        //else
        //{
        //    currentPlayer = (currentPlayer == 1) ? 2 : 1;
        //    shotReport.AppendLine($"<color=orange>=> ĐỔI LƯỢT sang Player {currentPlayer}.</color>");
        //}

        //UpdateNextTarget();
        //Debug.Log(shotReport.ToString());

        // đoạn chỉnh sửa 
        //if (gameEnd) return;

        //if (!hitTargetFirstFromController)
        //{
        //    foulCommittedThisTurn = true;
        //    shotReport.AppendLine("<color=red> ! LỖI: Chạm sai bi mục tiêu</color>");
        //}

        //if (isNineBallPotted && !foulCommittedThisTurn)
        //{
        //    gameEnd = true;
        //    shotReport.AppendLine("<color=cyan><b>PLAYER " + currentPlayer + " THẮNG!</b></color>");
        //}
        //else
        //{
        //    if (!foulCommittedThisTurn && ballPottedThisTurn) shotReport.AppendLine("=> GIỮ LƯỢT");
        //    else currentPlayer = (currentPlayer == 1) ? 2 : 1;
        //}
        //UpdateNextTarget();
        //Debug.Log(shotReport.ToString());

        //if (gameEnd) return;

        //// 1. Kiểm tra lỗi chạm bi (giữ nguyên logic của bạn)
        //if (!hitTargetFirstFromController)
        //{
        //    foulCommittedThisTurn = true;
        //    shotReport.AppendLine("<color=red> ! LỖI: Chạm sai bi mục tiêu đầu tiên</color>");
        //}

        //// 2. Xử lý bi 9 (Gọi hàm mới ở đây)
        //if (isNineBallPotted)
        //{
        //    //StartCoroutine(HandleNineBallPottedRoutine());
        //    //return; // Dừng hàm ở đây để Coroutine xử lý tiếp

        //    if (!gameEnd)
        //    {
        //        StartCoroutine(HandleNineBallPottedRoutine());
        //    }

        //    return;
        //}

        //// 3. Nếu không phải bi 9, xử lý lượt đánh bình thường
        //if (!foulCommittedThisTurn && /*ballPottedThisTurn*/ correctBallPotted)
        //{
        //    //shotReport.AppendLine("=> GIỮ LƯỢT");
        //    //timeRemaining = totalTimeInput;
        //    shotReport.AppendLine("<color=green>=> GIỮ LƯỢT</color>");

        //    timeRemaining = totalTimeInput;
        //}
        //else
        //{
        //    SwitchTurn();
        //}

        ////UpdateNextTarget();
        //UpdateNextTarget();
        //Debug.Log(shotReport.ToString());

        if (gameEnd) return;

        // =================================
        // 1. KIỂM TRA CHẠM BI ĐẦU TIÊN
        // =================================

        if (!hitTargetFirstFromController)
        {
            foulCommittedThisTurn = true;

            shotReport.AppendLine(
                "<color=red> ! LỖI: Không chạm bi mục tiêu đầu tiên</color>"
            );
        }

        // =================================
        // 2. BI 9 => WIN
        // =================================

        if (isNineBallPotted)
        {
            if (!gameEnd)
            {
                StartCoroutine(HandleNineBallPottedRoutine());
            }

            return;
        }

        // =================================
        // 3. GIỮ LƯỢT
        // =================================
        // LUẬT:
        // - chạm đúng bi mục tiêu đầu tiên
        // - và có ít nhất 1 bi chết/lọt lỗ
        // =================================

        bool keepTurn =
            hitTargetFirstFromController
            &&
            anyColoredBallDestroyed;

        if (keepTurn)
        {
            shotReport.AppendLine(
                "<color=green>=> GIỮ LƯỢT</color>"
            );

            Debug.Log(
                "<color=green>PLAYER GIỮ LƯỢT</color>"
            );

            timeRemaining = totalTimeInput;
        }
        else
        {
            shotReport.AppendLine(
                "<color=orange>=> ĐỔI LƯỢT</color>"
            );

            Debug.Log(
                "<color=orange>ĐỔI LƯỢT</color>"
            );

            SwitchTurn();
        }

        // =================================
        // 4. UPDATE TARGET
        // =================================

        UpdateNextTarget();

        Debug.Log(shotReport.ToString());

    }

    public IEnumerator HandleCueBallPotted()
    {
        cueStickController.moveCueBallAllow = true;

        if(selectedGroupText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            foulText.GetComponent<Animator>().SetTrigger("ShowTrigger");
        }

        while (bottomMessagerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Button Mesage In") && bottomMessagerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        bottomMessagerAnimator.SetTrigger("ShowTrigger");
        bottomMessagerText.text = "Drag the cue ball to position it anywhere on the table";
    }

    private IEnumerator HideFoulUI()
    {
        yield return new WaitForSecondsRealtime(2f);

        foulText.gameObject.SetActive(false);
    }

    public IEnumerator CannotMoveCueBall()
    {
        while (bottomMessagerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Button Mesage In") &&  bottomMessagerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield break ;
        }

        bottomMessagerAnimator.SetTrigger("ShowTrigger");
        bottomMessagerText.text = "The Cue Ball is not ready to be move";
    }
    private void SwitchTurn()
    {
        // 1. Đổi ID người chơi (Nếu là 1 thì thành 2, nếu là 2 thì thành 1)
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        Debug.Log(currentPlayer + "is Playing now");
        // 2. Reset lại thời gian cho lượt mới
        timeRemaining = totalTimeInput;

        // 3. Cập nhật UI: Bật/Tắt thanh Slider hiển thị lượt của mỗi người
        highlightSlider_01.gameObject.SetActive(currentPlayer == 1);
        highlightSlider_02.gameObject.SetActive(currentPlayer == 2);

        // 4. Thông báo vào báo cáo lượt đánh
        shotReport.AppendLine($"<color=orange>=> ĐỔI LƯỢT sang Player {currentPlayer}.</color>");
    }
    private void UpdateNextTarget()
    {
        //targetBallNumber = rules != null ? rules.GetCurrentTargetBall(pottedBalls) : 1;

        //BallNo[] balls = FindObjectsOfType<BallNo>();
        BallNo[] balls = FindObjectsByType<BallNo>(FindObjectsSortMode.None);

        int minBall = int.MaxValue;

        foreach (BallNo b in balls)
        {
            if (b.isCueBall) continue;

            if (!b.gameObject.activeInHierarchy) continue;

            if (b.ballNumber < minBall)
            {
                minBall = b.ballNumber;
            }
        }

        if (minBall != int.MaxValue)
        {
            targetBallNumber = minBall;
        }

        Debug.Log("<color=cyan>Target mới: " + targetBallNumber + "</color>");
    }

    private int GetBallNumber(string tag)
    {
        if (tag == "BallNo.9") return 9;
        if (tag.StartsWith("BallNo.") && int.TryParse(tag.Replace("BallNo.", ""), out int n)) return n;
        return 0;
    }

    //public int currentPlayer = 1;
    //public int targetBallNumber = 1;
    //public bool gameEnd = false;

    //private bool ballPottedThisTurn = false;
    //private bool foulCommittedThisTurn = false;
    //private StringBuilder shotReport = new StringBuilder();
    //private List<int> pottedBalls = new List<int>(); // Đã khởi tạo sẵn

    //public void RegisterStartShot()
    //{
    //    ballPottedThisTurn = false;
    //    foulCommittedThisTurn = false;
    //    shotReport.Clear();
    //    shotReport.AppendLine($"<color=white><b>--- LƯỢT PLAYER {currentPlayer} ---</b></color>");
    //}

    //// Hàm public để script PocketDetector gọi tới
    //public void OnBallEnteredPocket(Collider ball)
    //{
    //    if (gameEnd) return;

    //    if (ball.CompareTag("CueBall"))
    //    {
    //        foulCommittedThisTurn = true;
    //        shotReport.AppendLine("<color=red>  ! LỖI: Bi cái vào lỗ</color>");
    //        ball.transform.position = new Vector3(-0.9f, 1.18f, -0.17f);
    //        Rigidbody rb = ball.attachedRigidbody;
    //        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    //        return;
    //    }

    //    int nr = GetBallNumber(ball.tag);
    //    if (nr > 0)
    //    {
    //        ballPottedThisTurn = true;
    //        shotReport.AppendLine($"<color=yellow>  + Vào lỗ: Bi số {nr}</color>");
    //        if (!pottedBalls.Contains(nr)) pottedBalls.Add(nr);
    //        if (nr == targetBallNumber) UpdateNextTarget();
    //        if (nr == 9 && !foulCommittedThisTurn) gameEnd = true;
    //        Destroy(ball.gameObject);
    //    }
    //}

    //public void HandleStrokeResult(bool hitTargetFirst)
    //{
    //    if (gameEnd) { Debug.Log("<color=cyan>PLAYER " + currentPlayer + " THẮNG!</color>"); return; }

    //    if (!hitTargetFirst)
    //    {
    //        foulCommittedThisTurn = true;
    //        shotReport.AppendLine($"<color=red>  ! LỖI: Không chạm bi mục tiêu {targetBallNumber} đầu tiên</color>");
    //    }

    //    // ĐIỀU KIỆN GIỮ LƯỢT: Không lỗi VÀ có bi vào lỗ
    //    if (!foulCommittedThisTurn && ballPottedThisTurn)
    //    {
    //        shotReport.AppendLine($"<color=green>=> GIỮ LƯỢT.</color>");
    //    }
    //    else
    //    {
    //        currentPlayer = (currentPlayer == 1) ? 2 : 1;
    //        shotReport.AppendLine($"<color=orange>=> ĐỔI LƯỢT sang Player {currentPlayer}.</color>");
    //    }
    //    Debug.Log(shotReport.ToString());
    //}

    //private void UpdateNextTarget()
    //{
    //    targetBallNumber++;
    //    while (targetBallNumber < 9 && pottedBalls.Contains(targetBallNumber)) targetBallNumber++;
    //}

    //private int GetBallNumber(string tag)
    //{
    //    if (tag == "BallNo.9") return 9;
    //    if (tag.StartsWith("BallNo.") && int.TryParse(tag.Replace("BallNo.", ""), out int n)) return n;
    //    return 0;
    //}

    //[Header("Dependencies")]
    //public CueStickController cueStickController;

    //[Header("Game State")]
    //public int currentPlayer = 1;
    //public int targetBallNumber = 1; // Bi mục tiêu hiện tại (số nhỏ nhất trên bàn)

    //// Trạng thái Lỗi/Giữ lượt trong một cú đánh
    //private bool ballPottedThisTurn = false;    // Bi có vào lỗ không?
    //private bool foulCommittedThisTurn = false; // Có phạm lỗi (bi cái vào lỗ, đánh sai bi mục tiêu...) không?

    //public bool gameEnd = false;

    //// Hằng số Tag
    //private const string CUE_BALL_TAG = "CueBall";
    //private const string NINE_BALL_TAG = "BallNo.9";
    //private const string BALL_NO_PREFIX = "BallNo."; // Dùng cho bi 1-8

    //// Danh sách bi đã vào lỗ (để theo dõi trạng thái bàn)
    //private readonly List<int> pottedBallNumbers = new List<int>();

    //private void Start()
    //{
    //    if (cueStickController == null)
    //    {
    //        Debug.LogError("Lỗi Cài Đặt: CueStickController chưa được gán trong Inspector! Tắt script.");
    //        enabled = false;
    //        return;
    //    }

    //    ResetGame();
    //}

    //// --- XỬ LÝ TRẠNG THÁI GAME ---

    //public void ResetGame()
    //{
    //    ballPottedThisTurn = false;
    //    foulCommittedThisTurn = false;
    //    currentPlayer = 1;
    //    targetBallNumber = 1;
    //    pottedBallNumbers.Clear();
    //    gameEnd = false;

    //    Debug.Log("Game đã Reset. Player 1 chơi trước. Mục tiêu: Bi số 1.");
    //}

    //private void SwitchTurn()
    //{
    //    if (foulCommittedThisTurn)
    //    {
    //        Debug.Log($"Phạm lỗi! Player {(currentPlayer == 1 ? 2 : 1)} có bi cái trong tay.");
    //        // Logic đặt bi cái trong tay (cần được xử lý trong script quản lý bi cái)
    //    }

    //    currentPlayer = (currentPlayer == 1) ? 2 : 1;
    //    foulCommittedThisTurn = false; // Reset trạng thái lỗi
    //    ballPottedThisTurn = false;    // Reset trạng thái bi vào lỗ

    //    Debug.Log($"Lượt mới: Player {currentPlayer} đang chơi. Mục tiêu: Bi số {targetBallNumber}.");
    //}

    ///// <summary>
    ///// Hàm này được gọi bởi CueStickController sau khi mọi bi đã dừng hoàn toàn.
    ///// </summary>
    //public void HandleStrokeResult(bool hitTargetBallFirst)
    //{
    //    if (gameEnd) return;

    //    // 1. Kiểm tra Lỗi Đánh Sai Bi Mục Tiêu (Hit-First Foul)
    //    if (!hitTargetBallFirst)
    //    {
    //        foulCommittedThisTurn = true;
    //        Debug.LogWarning($"LỖI: Bi mục tiêu số {targetBallNumber} không phải là bi đầu tiên bị đánh trúng.");
    //    }

    //    // 2. Quyết định Chuyển Lượt
    //    if (foulCommittedThisTurn || !ballPottedThisTurn)
    //    {
    //        // Mất lượt khi: Có lỗi (Scratch/Hit-First) HOẶC không có bi nào vào lỗ.
    //        SwitchTurn();
    //    }
    //    else
    //    {
    //        // Giữ lượt (Không lỗi VÀ có bi vào lỗ)
    //        Debug.Log($"Player {currentPlayer} giữ lượt.");
    //    }

    //    // Đặt lại cờ cho lượt tiếp theo
    //    foulCommittedThisTurn = false;
    //    ballPottedThisTurn = false;
    //}

    //// --- XỬ LÝ BI RƠI XUỐNG LỖ (HÀM ONTRIGGERENTER) ---

    //private void OnTriggerEnter(Collider ball)
    //{
    //    if (gameEnd) return;

    //    // 1. Xử lý Bi Cái (Cue Ball)
    //    if (ball.CompareTag(CUE_BALL_TAG))
    //    {
    //        // Đặt lại vị trí bi cái
    //        ball.transform.position = new Vector3(-0.9048982f, 1.185f, -0.175406f);
    //        Rigidbody rb = ball.attachedRigidbody;
    //        if (rb != null)
    //        {
    //            rb.angularVelocity = Vector3.zero;
    //            rb.linearVelocity = Vector3.zero;
    //        }

    //        // Lỗi: Bi cái vào lỗ (Scratch)
    //        foulCommittedThisTurn = true;
    //        Debug.Log("LỖI: Bi cái đã rơi vào lỗ (Scratch).");
    //        return;
    //    }

    //    // 2. Xử lý Bi Số (Numbered Balls)
    //    int ballNumber = GetBallNumberFromTag(ball.tag);

    //    if (ballNumber == 0 || pottedBallNumbers.Contains(ballNumber))
    //    {
    //        return; // Bi không hợp lệ hoặc đã vào lỗ trước đó
    //    }

    //    HandlePottedBall(ball, ballNumber);
    //}

    //private void HandlePottedBall(Collider ball, int ballNumber)
    //{
    //    // Loại bỏ bi khỏi danh sách trên bàn (quan trọng để vật lý dừng)
    //    if (cueStickController != null && cueStickController.balls != null)
    //    {
    //        Rigidbody rb = ball.attachedRigidbody;
    //        if (rb != null)
    //        {
    //            // Giả định `cueStickController.balls` là List<Rigidbody>
    //            cueStickController.balls.Remove(rb);
    //        }
    //    }

    //    // Thêm vào danh sách bi đã vào lỗ và hủy GameObject
    //    pottedBallNumbers.Add(ballNumber);
    //    Destroy(ball.gameObject);
    //    ballPottedThisTurn = true; // Bi đã vào lỗ

    //    Debug.Log($"Bi số {ballNumber} đã được đưa vào lỗ.");

    //    // --- Kiểm tra thắng/thua ---
    //    if (ballNumber == 9)
    //    {
    //        // Trong 9 bi, nếu bi 9 vào lỗ, trò chơi kết thúc ngay lập tức.
    //        HandleNineBallPotted();
    //        return;
    //    }

    //    // --- Cập nhật Bi Mục Tiêu ---
    //    if (ballNumber == targetBallNumber)
    //    {
    //        // Tìm bi số nhỏ nhất tiếp theo chưa vào lỗ
    //        int nextTarget = targetBallNumber + 1;
    //        while (nextTarget <= 9 && pottedBallNumbers.Contains(nextTarget))
    //        {
    //            nextTarget++;
    //        }
    //        targetBallNumber = nextTarget;

    //        Debug.Log($"Bi mục tiêu kế tiếp: Bi số {targetBallNumber}.");
    //    }
    //}

    //// --- HÀM HỖ TRỢ VÀ KẾT QUẢ ---

    //private void HandleNineBallPotted()
    //{
    //    gameEnd = true;

    //    // Giả định thắng: Bi số 9 vào lỗ, và không có lỗi (được kiểm tra khi bi dừng).
    //    // Nếu có lỗi (foulCommittedThisTurn) thì bi 9 vào lỗ là không hợp lệ (mất lượt, đối thủ có bi trong tay).

    //    // Vì ta kiểm tra lỗi đánh sai bi sau khi bi dừng, nên ở đây chỉ ghi nhận bi 9 đã vào.
    //    // Quyết định thắng/thua cuối cùng nằm trong HandleStrokeResult()

    //    if (!foulCommittedThisTurn)
    //    {
    //        Debug.Log($"*********** Player {currentPlayer} ĐÃ THẮNG TRẬN ĐẤU (Bi 9 vào lỗ)! ***********");
    //    }
    //    else
    //    {
    //        int winner = (currentPlayer == 1) ? 2 : 1;
    //        Debug.LogWarning($"LỖI: Bi 9 vào lỗ khi có lỗi khác đang xảy ra. Player {winner} thắng.");
    //    }
    //}

    //private int GetBallNumberFromTag(string tag)
    //{
    //    if (tag.CompareTo(NINE_BALL_TAG) == 0)
    //    {
    //        return 9;
    //    }

    //    if (tag.StartsWith(BALL_NO_PREFIX))
    //    {
    //        string numberString = tag.Substring(BALL_NO_PREFIX.Length);
    //        if (int.TryParse(numberString, out int number) && number >= 1 && number <= 8)
    //        {
    //            return number;
    //        }
    //    }
    //    return 0; // Bi không hợp lệ
    //}

    //int currentPlayer;
    //string player1Gp = "", player2GP = "";
    //public bool groupAssigned = false, correctBallPotted = false, gameEnd = false;

    //List<Collider> player1PottedBalls = new List<Collider>();
    //List<Collider> player2PottedBalls = new List<Collider>();

    //public CueStickController cueStickController;

    //private void Start()
    //{
    //    //cueStickController = GetComponent<CueStickController>();
    //    ResetGame();
    //}
    //private void ResetGame()
    //{
    //    groupAssigned = false;
    //    correctBallPotted = false;
    //    player1Gp = "";
    //    player2GP = "";
    //    currentPlayer = 1;
    //    player1PottedBalls.Clear();
    //    player2PottedBalls.Clear();
    //}

    //private IEnumerator OnTriggerEnter(Collider ball)
    //{
    //   string ballTag = ball.tag;

    //    if(ballTag == "CueBall")
    //    {
    //        ball.transform.position = new Vector3(-0.9048982f, 1.185f, -0.175406f);
    //        ball.attachedRigidbody.angularVelocity =  Vector3.zero;
    //        StartCoroutine(HandleCueBallPotted());
    //        yield break;
    //    }

    //    if(ballTag == "BallNo.9")
    //    {
    //        StartCoroutine(HandleBlackBallPotted());
    //        HandlePootedBall(ball, (currentPlayer == 1) ? player1PottedBalls : player2PottedBalls);
    //        yield break;
    //    }

    //    if(!groupAssigned)
    //    {
    //        AssignGroup(ballTag);
    //    }

    //    if(groupAssigned)
    //    {
    //        if(currentPlayer == 1 && ballTag == player1Gp + "Ball")
    //        {
    //            HandlePootedBall(ball, player1PottedBalls);
    //            correctBallPotted = true;
    //        }
    //        else if (currentPlayer == 2 && ballTag == player2GP + "Ball")
    //        {
    //            HandlePootedBall(ball, player2PottedBalls);
    //            correctBallPotted = true;
    //        }
    //        else
    //        {
    //            HandlePootedBall(ball, (currentPlayer == 1) ? player2PottedBalls : player1PottedBalls);
    //        }

    //    }
    //}

    //private void HandlePootedBall(Collider ball, List<Collider> pottedBalls)
    //{
    //    cueStickController.balls.Remove(ball.attachedRigidbody);
    //    pottedBalls.Add(ball);
    //    Destroy(ball.gameObject);
    //}

    //private void AssignGroup(string ballTag)
    //{
    //    if(ballTag == "SolidBall" || ballTag == "Ball")
    //    {
    //        player1Gp = (ballTag == "SolidBall") ? "Solid" : "Ball";
    //        player2GP = (player1Gp == "Ball") ? "Ball" : "Solid";
    //    }
    //    else
    //    {
    //        player2GP = (ballTag == "SolidBall") ? "Solid" : "Ball";
    //        player1Gp = (player2GP == "Ball") ? "Ball" : "Solid";
    //    }

    //    groupAssigned = true;

    //    Debug.Log("Player 1 is : " + player1Gp + "Player2 : " + player2GP);
    //}

    //private IEnumerator HandleCueBallPotted()
    //{
    //    if(currentPlayer == 1)
    //    {
    //        Debug.Log("CueBall Potted by Player1");

    //    }
    //    else
    //    {
    //        Debug.Log("CueBall Potted by Player2");
    //    }
    //    yield return null;
    //}

    //private IEnumerator HandleBlackBallPotted()
    //{
    //    if(currentPlayer == 1)
    //    {
    //        if(player1PottedBalls.Count == 8)
    //        {
    //            Debug.Log("player 1 won the game");
    //        }
    //        else
    //        {
    //            Debug.Log("player 2 won the game");
    //        }
    //    }
    //    else
    //    {
    //        if (player2PottedBalls.Count == 8)
    //        {
    //            Debug.Log("player 2 won the game");
    //        }
    //        else
    //        {
    //            Debug.Log("player 1 won the game");
    //        }
    //    }

    //    gameEnd = true;
    //    yield return null;
    //}

    //private void SwitchTurn()
    //{
    //    currentPlayer = (currentPlayer == 1) ? 2 : 1;
    //    Debug.Log(currentPlayer + "is Playing Now");
    //}

    //public IEnumerator HitMissedOrNot()
    //{
    //    if(!correctBallPotted)
    //    {
    //        SwitchTurn();
    //    }

    //    correctBallPotted = false;
    //    yield break;
    //}
}
