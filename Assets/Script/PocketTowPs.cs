using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PocketTowPs : MonoBehaviour
{
    public int currentPlayer = 1;
    public int targetBallNumber = 1;
    public bool gameEnd = false;

    private bool ballPottedThisTurn = false;
    private bool foulCommittedThisTurn = false;
    private StringBuilder shotReport = new StringBuilder();
    private List<int> pottedBalls = new List<int>(); // Đã khởi tạo sẵn

    public void RegisterStartShot()
    {
        ballPottedThisTurn = false;
        foulCommittedThisTurn = false;
        shotReport.Clear();
        shotReport.AppendLine($"<color=white><b>--- LƯỢT PLAYER {currentPlayer} ---</b></color>");
    }

    // Hàm public để script PocketDetector gọi tới
    public void OnBallEnteredPocket(Collider ball)
    {
        if (gameEnd) return;

        if (ball.CompareTag("CueBall"))
        {
            foulCommittedThisTurn = true;
            shotReport.AppendLine("<color=red>  ! LỖI: Bi cái vào lỗ</color>");
            ball.transform.position = new Vector3(-0.9f, 1.18f, -0.17f);
            Rigidbody rb = ball.attachedRigidbody;
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
            return;
        }

        int nr = GetBallNumber(ball.tag);
        if (nr > 0)
        {
            ballPottedThisTurn = true;
            shotReport.AppendLine($"<color=yellow>  + Vào lỗ: Bi số {nr}</color>");
            if (!pottedBalls.Contains(nr)) pottedBalls.Add(nr);
            if (nr == targetBallNumber) UpdateNextTarget();
            if (nr == 9 && !foulCommittedThisTurn) gameEnd = true;
            Destroy(ball.gameObject);
        }
    }

    public void HandleStrokeResult(bool hitTargetFirst)
    {
        if (gameEnd) { Debug.Log("<color=cyan>PLAYER " + currentPlayer + " THẮNG!</color>"); return; }

        if (!hitTargetFirst)
        {
            foulCommittedThisTurn = true;
            shotReport.AppendLine($"<color=red>  ! LỖI: Không chạm bi mục tiêu {targetBallNumber} đầu tiên</color>");
        }

        // ĐIỀU KIỆN GIỮ LƯỢT: Không lỗi VÀ có bi vào lỗ
        if (!foulCommittedThisTurn && ballPottedThisTurn)
        {
            shotReport.AppendLine($"<color=green>=> GIỮ LƯỢT.</color>");
        }
        else
        {
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
            shotReport.AppendLine($"<color=orange>=> ĐỔI LƯỢT sang Player {currentPlayer}.</color>");
        }
        Debug.Log(shotReport.ToString());
    }

    private void UpdateNextTarget()
    {
        targetBallNumber++;
        while (targetBallNumber < 9 && pottedBalls.Contains(targetBallNumber)) targetBallNumber++;
    }

    private int GetBallNumber(string tag)
    {
        if (tag == "BallNo.9") return 9;
        if (tag.StartsWith("BallNo.") && int.TryParse(tag.Replace("BallNo.", ""), out int n)) return n;
        return 0;
    }

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
