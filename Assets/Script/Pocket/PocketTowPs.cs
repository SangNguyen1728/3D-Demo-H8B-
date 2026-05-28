using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PocketTowPs : MonoBehaviour
{
    public enum PoolGameMode
    {
        NineBall,
        EightBall
    }

    public PoolGameMode gameMode = PoolGameMode.EightBall;

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

    public bool shotAlreadyResolved = false;

    private bool targetBallScoredThisTurn = false;

    private bool ballPottedThisTurn = false;
    private bool foulCommittedThisTurn = false;
    private bool cueBallPocketedThisTurn = false;
    private bool isNineBallPotted = false;
    private bool hitTargetFirstFromController = false;
    private bool anyColoredBallDestroyed = false;
    private bool anyObjectBallScored = false;

    private bool hitCorrectTargetThisShot = false;

    private bool scoredColoredBallThisShot = false;
    private bool scoredOwnGroupBallThisShot = false;

    private int pendingEvents = 0;

    private bool switchingTurn = false;

    public int shotEventVersion = 0;
    public float lastBallEventTime = 0f;

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

    private bool player1Assigned;
    private bool player2Assigned;

    private string player1Group = "";
    private string player2Group = "";
    void Start()
    {
        

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
            //SwitchTurn();    
            if (!switchingTurn && !cueStickController.hitPeriod)
            {
                shotAlreadyResolved = true;
                SwitchTurn();
            }
        }

        int seconds = Mathf.Clamp(Mathf.FloorToInt(timeRemaining), 0, 99);

        totalTimetext.text = string.Format("{0:00}", seconds);
    }
    public void SavePocketedBalls()
    {
        

        totalPottedBallsCount += player01PottedBalls.Count + player02PottedBalls.Count;
        PlayerPrefs.SetInt("PocketedBallsSaved", totalPottedBallsCount); // ✅ đồng bộ key
        LoadRacksAndBallsCount();
    }
   

    private void AssignGroup(string ballTag)
    {
        //groupAssigned = true;
        //Debug.Log("Group Assigned: " + ballTag);
        int nr = GetBallNumber(ballTag);

        if (nr >= 1 && nr <= 7)
        {
            player1Group = currentPlayer == 1 ? "SOLID" : "STRIPE";
            player2Group = currentPlayer == 1 ? "STRIPE" : "SOLID";
        }
        else if (nr >= 9 && nr <= 15)
        {
            player1Group = currentPlayer == 1 ? "STRIPE" : "SOLID";
            player2Group = currentPlayer == 1 ? "SOLID" : "STRIPE";
        }
        else
        {
            return;
        }

        groupAssigned = true;

        Debug.Log("GROUP ASSIGNED");

        Debug.Log("P1 = " + player1Group);

        Debug.Log("P2 = " + player2Group);
    }

    public bool IsPlayersBall(BallNo ball)
    {
        //string group =
        //    currentPlayer == 1
        //    ? player1Group
        //    : player2Group;

        //if (group == "SOLID")
        //{
        //    return nr >= 1 && nr <= 7;
        //}

        //if (group == "STRIPE")
        //{
        //    return nr >= 9 && nr <= 15;
        //}

        //return false;

        BallOwnership owner =ball.GetComponent<BallOwnership>();

        // =========================
        // 🎯 CONVERTED BALL
        // =========================
        if (owner != null && owner.isConverted)
        {
            return owner.ownerPlayer == currentPlayer;
        }

        int nr = ball.ballNumber;

        string group =
            currentPlayer == 1
            ? player1Group
            : player2Group;

        if (group == "SOLID")
        {
            return nr >= 1 && nr <= 7;
        }

        if (group == "STRIPE")
        {
            return nr >= 9 && nr <= 15;
        }

        return false;
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
    public void SetHitResult(bool isCorrectHit) 
    {
        //hitTargetFirstFromController = isCorrectHit;
        hitTargetFirstFromController = isCorrectHit;

        hitCorrectTargetThisShot = isCorrectHit;

        Debug.Log("SET HIT RESULT = " + isCorrectHit);
    } 

    public void RegisterStartShot()
    {
        
        hitTargetFirstFromController = false;

        ballPottedThisTurn = false;

        foulCommittedThisTurn = false;

        cueBallPocketedThisTurn = false;

        isNineBallPotted = false;

        anyColoredBallDestroyed = false;

        anyObjectBallScored = false;

        correctBallPotted = false;

        targetBallScoredThisTurn = false;

        shotAlreadyResolved = false;

        scoredOwnGroupBallThisShot = false;

        switchingTurn = false;

        pendingEvents = 0;

        shotReport.Clear();

        shotReport.AppendLine( $"<color=white><b>--- PLAYER {currentPlayer} TURN ---</b></color>" );
    }

    public void OnBallEnteredPocket(Collider ball)
    {
        AddPendingEvent();

        lastBallEventTime = Time.time;

        if (gameEnd) return;

        if (ball.CompareTag("CueBall"))
        {
            

            Debug.Log("CUE BALL POCKETED");

            cueBallPocketedThisTurn = true;

            foulCommittedThisTurn = true;

            shotReport.AppendLine("<color=red>FOUL : Cue Ball Pocketed</color>");

            // reset vị trí bi trắng
            //ball.transform.position = new Vector3(-0.9f, 1.18f, -0.17f);
            StartCoroutine(RespawnCueBall(ball));

            Rigidbody rb = ball.attachedRigidbody;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            foulText.gameObject.SetActive(true);

            foulText.text = "FOUL";

            StartCoroutine(HideFoulUI());

            // QUAN TRỌNG:
            // KHÔNG SwitchTurn ở đây
            CompletePendingEvent();
            return;
        }

        

        int nr = GetBallNumber(ball.tag);

        if (nr > 0)
        {
            anyObjectBallScored = true;
            scoredColoredBallThisShot = true;

            // ĐÁNH DẤU CÓ BI VÀO LỖ
            ballPottedThisTurn = true;

            shotReport.AppendLine($"<color=yellow>  + Vào lỗ: Bi số {nr}</color>");

            // lưu bi đã vào
            if (!pottedBalls.Contains(nr))
                pottedBalls.Add(nr);

            // KIỂM TRA BI MỤC TIÊU


            //if (gameMode == PoolGameMode.NineBall)
            //{
            //    if (nr == targetBallNumber)
            //    {
            //        correctBallPotted = true;

            //        hitTargetFirstFromController = true;
            //    }
            //}
            //else
            //{
            //    if (!groupAssigned)
            //    {
            //        AssignGroup(ball.tag);

            //        correctBallPotted = true;

            //        hitTargetFirstFromController = true;
            //    }
            //    else
            //    {
            //        if (IsPlayersBall(nr))
            //        {
            //            correctBallPotted = true;

            //            hitTargetFirstFromController = true;
            //        }
            //    }
            //}

            if (gameMode == PoolGameMode.NineBall)
            {
                if (nr == targetBallNumber)
                {
                    correctBallPotted = true;

                    hitTargetFirstFromController = true;

                    scoredOwnGroupBallThisShot = true;
                }
            }
            else
            {
                if (!groupAssigned)
                {
                    AssignGroup(ball.tag);

                    correctBallPotted = true;

                    hitTargetFirstFromController = true;

                    scoredOwnGroupBallThisShot = true;
                }

                else
                {
                    if (IsPlayersBall(ball.GetComponent<BallNo>()))
                    {
                        correctBallPotted = true;

                        hitTargetFirstFromController = true;

                        scoredOwnGroupBallThisShot = true;

                        Debug.Log("<color=green>OWN GROUP BALL POTTED</color>");
                    }
                    else
                    {
                        Debug.Log("<color=red>WRONG GROUP BALL POTTED</color>");
                    }
                }
            }

            // BI 9
            if ((gameMode == PoolGameMode.NineBall && nr == 9)||(gameMode == PoolGameMode.EightBall && nr == 8))
            {
                //isNineBallPotted = true;
                isNineBallPotted = true;

                Debug.Log("<color=yellow>WINNING BALL POCKETED</color>");
            }

            //Destroy(ball.gameObject);
            ball.gameObject.SetActive(false);
        }

        CompletePendingEvent();
    }

    private IEnumerator RespawnCueBall(Collider ball)
    {
        yield return new WaitForSeconds(0.2f);

        ball.transform.position =
            new Vector3(-0.9f, 1.18f, -0.17f);

        Rigidbody rb = ball.attachedRigidbody;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public bool IsValidFirstHit8Ball(int nr)
    {
        if (!groupAssigned)
        {
            return nr != 8;
        }

        string group =
            currentPlayer == 1
            ? player1Group
            : player2Group;

        bool clearedGroup = HasClearedGroup(currentPlayer);

        // được đánh bi 8
        if (clearedGroup)
        {
            return nr == 8;
        }

        if (group == "SOLID")
        {
            return nr >= 1 && nr <= 7;
        }

        if (group == "STRIPE")
        {
            return nr >= 9 && nr <= 15;
        }

        return false;
    }
    private bool HasClearedGroup(int player)
    {
        string group =
            player == 1
            ? player1Group
            : player2Group;

        BallNo[] balls =
            FindObjectsByType<BallNo>(
                FindObjectsSortMode.None);

        foreach (BallNo b in balls)
        {
            if (!b.gameObject.activeInHierarchy)
                continue;

            int nr = b.ballNumber;

            if (group == "SOLID")
            {
                if (nr >= 1 && nr <= 7)
                    return false;
            }

            if (group == "STRIPE")
            {
                if (nr >= 9 && nr <= 15)
                    return false;
            }
        }

        return true;
    }
    public bool HasClearedCurrentPlayerGroup()
    {
        return HasClearedGroup(currentPlayer);
    }

    private bool CanPocketEightBall(int player)
    {
        return HasClearedGroup(player);
    }

    public void OnBallDestroyed(GameObject ballObj)
    {
        AddPendingEvent();

        lastBallEventTime = Time.time;

        Debug.Log("DESTROY DURING SHOT");
        if (gameEnd) return;

        int nr = GetBallNumber(ballObj.tag);

        if (nr <= 0) return;

        anyObjectBallScored = true;
        scoredColoredBallThisShot = true;

        Debug.Log("<color=yellow>Bi bị destroy: " + nr + "</color>");

        // Có bi màu chết
        anyColoredBallDestroyed = true;

        // đánh dấu có bi vào
        ballPottedThisTurn = true;
        //if (nr == targetBallNumber || hitTargetFirstFromController)
        //{
        //    correctBallPotted = true;

        //    // FIX KEEP TURN
        //    hitTargetFirstFromController = true;

        //    Debug.Log("TARGET BALL DESTROYED");
        //}
        if (gameMode == PoolGameMode.NineBall)
        {
            if (nr == targetBallNumber)
            {
                correctBallPotted = true;

                hitTargetFirstFromController = true;

                //scoredOwnGroupBallThisShot = true;
                if (IsPlayersBall(ballObj.GetComponent<BallNo>()))
                {
                    scoredOwnGroupBallThisShot = true;
                }

                Debug.Log("TARGET BALL DESTROYED");
            }
        }
        else
        {
            if (!groupAssigned)
            {
                scoredOwnGroupBallThisShot = true;
            }
            else
            {
                if (IsPlayersBall(ballObj.GetComponent<BallNo>()))
                {
                    correctBallPotted = true;

                    hitTargetFirstFromController = true;

                    scoredOwnGroupBallThisShot = true;

                    Debug.Log("<color=green>OWN GROUP BALL DESTROYED</color>");
                }
                else
                {
                    Debug.Log("<color=red>WRONG GROUP BALL DESTROYED</color>");
                }
            }
        }

        // lưu list
        if (!pottedBalls.Contains(nr))
        {
            pottedBalls.Add(nr);
        }

        // =========================
        // WIN NGAY KHI BI 9 CHẾT
        // =========================
        //if (nr == 9)
        if ((gameMode == PoolGameMode.NineBall && nr == 9) || (gameMode == PoolGameMode.EightBall && nr == 8))
        {
            //isNineBallPotted = true;
            //CompletePendingEvent();
            //StartCoroutine(HandleNineBallPottedRoutine());

            isNineBallPotted = true;

            Debug.Log("<color=yellow>WINNING BALL DESTROYED</color>");

            CompletePendingEvent();

            return;
        }

        // update target mới
        //UpdateNextTarget();
        CompletePendingEvent();
    }

    public void AddPendingEvent()
    {
        pendingEvents++;
    }

    public void CompletePendingEvent()
    {
        pendingEvents = Mathf.Max(0, pendingEvents - 1);
    }

    public bool HasPendingEvents()
    {
        return pendingEvents > 0;
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

           // SwitchTurn(); // Đổi lượt sang người kia
        }
    }
    public void HandleStrokeResult()
    {

        if (shotAlreadyResolved)
        {
            Debug.Log("SHOT ALREADY RESOLVED");
            return;
        }

        shotAlreadyResolved = true;

        if (gameEnd) return;

        Debug.Log("========== HANDLE STROKE RESULT ==========");

        Debug.Log("hitTargetFirstFromController = " + hitTargetFirstFromController);

        Debug.Log("ballPottedThisTurn = " + ballPottedThisTurn);

        Debug.Log("cueBallPocketedThisTurn = " + cueBallPocketedThisTurn);

        Debug.Log("anyObjectBallScored = " + anyObjectBallScored);

        // =========================
        // BI 9 WIN
        // =========================

        if (isNineBallPotted)
        {
            //gameEnd = true;

            //winPlayerText.text =
            //    (currentPlayer == 1
            //    ? player01Name
            //    : player02Name)
            //    + " YOU WIN";

            //winPlayerText.gameObject.SetActive(true);

            //return;

            // =========================
            // 8 BALL MODE
            // =========================
            if (gameMode == PoolGameMode.EightBall)
            {
                bool cleared =
                    HasClearedGroup(currentPlayer);

                // =========================
                // THẮNG HỢP LỆ
                // =========================
                if (cleared &&
                    hitTargetFirstFromController &&
                    !cueBallPocketedThisTurn)
                {
                    gameEnd = true;

                    winPlayerText.text =
                        (currentPlayer == 1
                        ? player01Name
                        : player02Name)
                        + " WIN";

                    winPlayerText.gameObject.SetActive(true);

                    Debug.Log("<color=green>LEGAL 8 BALL WIN</color>");
                }
                else
                {
                    // =========================
                    // THUA DO ĐÁNH 8 SỚM
                    // =========================
                    gameEnd = true;

                    int loser = currentPlayer;

                    int winner =
                        loser == 1 ? 2 : 1;

                    string winnerName =
                        winner == 1
                        ? player01Name
                        : player02Name;

                    winPlayerText.text =
                        winnerName + " WIN";

                    winPlayerText.gameObject.SetActive(true);

                    Debug.Log("<color=red>ILLEGAL 8 BALL -> LOSE MATCH</color>");
                }

                return;
            }

            // =========================
            // 9 BALL MODE
            // =========================
            gameEnd = true;

            winPlayerText.text =
                (currentPlayer == 1
                ? player01Name
                : player02Name)
                + " YOU WIN";

            winPlayerText.gameObject.SetActive(true);

            return;
        }

        // =========================
        // FOUL
        // =========================

        if (cueBallPocketedThisTurn || foulCommittedThisTurn)
        {
            Debug.Log("<color=red>FOUL -> SWITCH TURN</color>");

            SwitchTurn();

            UpdateNextTarget();

            return;
        }

        if (gameMode == PoolGameMode.EightBall)
        {
            //if (isNineBallPotted)
            //{
            //    bool cleared =
            //        HasClearedGroup(currentPlayer);

            //    if (cleared &&
            //        hitTargetFirstFromController &&
            //        !cueBallPocketedThisTurn)
            //    {
            //        gameEnd = true;

            //        winPlayerText.text =
            //            "PLAYER " + currentPlayer + " WIN";

            //        winPlayerText.gameObject.SetActive(true);
            //    }
            //    else
            //    {
            //        gameEnd = true;

            //        int loser = currentPlayer;

            //        int winner =
            //            loser == 1 ? 2 : 1;

            //        winPlayerText.text =
            //            "PLAYER " + winner + " WIN";

            //        winPlayerText.gameObject.SetActive(true);
            //    }

            //    return;
            //}

            if (isNineBallPotted)
            {
                bool clearedGroup =
                    CanPocketEightBall(currentPlayer);

                bool validWin =
                    clearedGroup &&
                    hitTargetFirstFromController &&
                    !cueBallPocketedThisTurn;

                gameEnd = true;

                if (validWin)
                {
                    Debug.Log("<color=green>VALID 8 BALL WIN</color>");

                    winPlayerText.text =
                        (currentPlayer == 1
                        ? player01Name
                        : player02Name)
                        + " WON THE MATCH";

                    winPlayerText.gameObject.SetActive(true);

                    SavePocketedBalls();
                }
                else
                {
                    Debug.Log("<color=red>EARLY 8 BALL = LOSE</color>");

                    int winner =
                        currentPlayer == 1 ? 2 : 1;

                    string winnerName =
                        winner == 1
                        ? player01Name
                        : player02Name;

                    winPlayerText.text =
                        winnerName + " WON THE MATCH";

                    winPlayerText.gameObject.SetActive(true);

                    foulText.gameObject.SetActive(true);

                    foulText.text = "LOST THE MATCH";

                    SavePocketedBalls();
                }

                cueStickController.stopTimer = true;

                return;
            }
        }

        // =========================
        // KEEP TURN LOGIC
        // =========================
        //bool scoredAnyObjectBall = anyObjectBallScored|| anyColoredBallDestroyed || ballPottedThisTurn;

        //bool keepTurn = hitTargetFirstFromController &&scoredAnyObjectBall &&!cueBallPocketedThisTurn;

        //bool keepTurn = hitCorrectTargetThisShot && scoredColoredBallThisShot;

        bool keepTurn = hitTargetFirstFromController && scoredOwnGroupBallThisShot &&!cueBallPocketedThisTurn;


        Debug.Log("KEEP TURN = " + keepTurn);

        if (keepTurn)
        {
            Debug.Log("<color=green>PLAYER GIỮ LƯỢT</color>");

            timeRemaining = totalTimeInput;
        }
        else
        {
            Debug.Log("<color=orange>ĐỔI LƯỢT</color>");

            SwitchTurn();
        }

        // UPDATE TARGET SAU KHI XỬ LÝ XONG
        UpdateNextTarget();
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

        if (switchingTurn)
        {
            Debug.Log("TURN SWITCH BLOCKED");
            return;
        }

        switchingTurn = true;

        int previousPlayer = currentPlayer;

        currentPlayer = (currentPlayer == 1) ? 2 : 1;

        Debug.Log(
            $"<color=orange>TURN SWITCH: P{previousPlayer} -> P{currentPlayer}</color>"
        );

        timeRemaining = totalTimeInput;

        highlightSlider_01.gameObject.SetActive(currentPlayer == 1);
        highlightSlider_02.gameObject.SetActive(currentPlayer == 2);

        shotReport.AppendLine(
            $"<color=orange>=> ĐỔI LƯỢT sang Player {currentPlayer}</color>"
        );

        StartCoroutine(UnlockSwitchTurn());
    }

    private IEnumerator UnlockSwitchTurn()
    {
        yield return new WaitForSeconds(0.5f);

        switchingTurn = false;
    }
    private void UpdateNextTarget()
    {

        //if (gameMode == PoolGameMode.EightBall)
        //{
        //    return;
        //}

        //BallNo[] balls = FindObjectsByType<BallNo>(FindObjectsSortMode.None);

        //int minBall = int.MaxValue;

        //foreach (BallNo b in balls)
        //{
        //    if (b.isCueBall) continue;

        //    if (!b.gameObject.activeInHierarchy) continue;

        //    if (b.ballNumber < minBall)
        //    {
        //        minBall = b.ballNumber;
        //    }
        //}

        //if (minBall != int.MaxValue)
        //{
        //    targetBallNumber = minBall;
        //}

        //Debug.Log("<color=cyan>Target mới: " + targetBallNumber + "</color>");

        // =========================
        // 8 BALL MODE
        // =========================
        if (gameMode == PoolGameMode.EightBall)
        {
            string group =
                currentPlayer == 1
                ? player1Group
                : player2Group;

            bool cleared = HasClearedGroup(currentPlayer);

            // =====================================
            // ĐÃ CLEAR HẾT -> TARGET BI 8
            // =====================================
            if (cleared)
            {
                targetBallNumber = 8;

                Debug.Log("<color=green>TARGET = BALL 8</color>");

                return;
            }

            // =====================================
            // CHƯA CLEAR -> TARGET BI NHÓM MÌNH
            // =====================================
            BallNo[] balls =
                FindObjectsByType<BallNo>(FindObjectsSortMode.None);

            foreach (BallNo b in balls)
            {
                if (b.isCueBall)
                    continue;

                if (!b.gameObject.activeInHierarchy)
                    continue;

                int nr = b.ballNumber;

                // SOLID
                if (group == "SOLID")
                {
                    if (nr >= 1 && nr <= 7)
                    {
                        targetBallNumber = nr;

                        Debug.Log("<color=cyan>TARGET SOLID = "
                            + nr + "</color>");

                        return;
                    }
                }

                // STRIPE
                if (group == "STRIPE")
                {
                    if (nr >= 9 && nr <= 15)
                    {
                        targetBallNumber = nr;

                        Debug.Log("<color=cyan>TARGET STRIPE = "
                            + nr + "</color>");

                        return;
                    }
                }
            }

            return;
        }

        // =========================
        // 9 BALL MODE
        // =========================
        BallNo[] nineBalls =
            FindObjectsByType<BallNo>(FindObjectsSortMode.None);

        int minBall = int.MaxValue;

        foreach (BallNo b in nineBalls)
        {
            if (b.isCueBall)
                continue;

            if (!b.gameObject.activeInHierarchy)
                continue;

            if (b.ballNumber < minBall)
            {
                minBall = b.ballNumber;
            }
        }

        if (minBall != int.MaxValue)
        {
            targetBallNumber = minBall;
        }

        Debug.Log("<color=cyan>9BALL TARGET = "
            + targetBallNumber + "</color>");

        targetBallID_1.text = "TARGET : " + targetBallNumber;

        targetBallID_2.text = "TARGET : " + targetBallNumber;
    }

    private int GetBallNumber(string tag)
    {
        if (tag == "BallNo.9") return 9;
        if (tag.StartsWith("BallNo.") && int.TryParse(tag.Replace("BallNo.", ""), out int n)) return n;
        return 0;
    }

    
}
