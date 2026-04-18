using UnityEngine;

public class HoleLogic : MonoBehaviour
{
    private bool destroyOnBallEnter;
    private bool ballEntered = false;

    private bool isActive = false;

    public void Init(bool onBallEnter)
    {
        //destroyOnBallEnter = onBallEnter;
        //ballEntered = false;

        destroyOnBallEnter = onBallEnter;
        ballEntered = false;
        isActive = false; // chưa hoạt động
    }
    public void ActivateHole()
    {
        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return; // 🔥 CHẶN

        BallNo ball = other.GetComponentInParent<BallNo>();

        if (ball != null)
        {
            Debug.Log("Ball vào hole: " + ball.ballNumber);

            if (ball.isCueBall)
            {
                Debug.Log("Bi trắng vào lỗ!");
            }

            if (destroyOnBallEnter)
            {
                ballEntered = true;
            }
        }
    }

    public bool HasBallEntered()
    {
        return ballEntered;
    }

    //private bool destroyOnBallEnter;
    //private bool ballEntered = false;

    //public void Init(bool onBallEnter)
    //{
    //    destroyOnBallEnter = onBallEnter;
    //    ballEntered = false;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    BallNo ball = other.GetComponent<BallNo>();

    //    if (ball != null)
    //    {
    //        Debug.Log("Ball vào hole: " + ball.ballNumber);

    //        if (ball.isCueBall)
    //        {
    //            Debug.Log("Bi trắng vào lỗ!");
    //        }

    //        if (destroyOnBallEnter)
    //        {
    //            ballEntered = true;
    //        }
    //    }

    //    //    if (other.CompareTag("Ball"))
    //    //{
    //    //    Debug.Log("Ball vào hole");

    //    //    if (destroyOnBallEnter)
    //    //    {
    //    //        ballEntered = true;
    //    //        Debug.Log("SET ballEntered = TRUE"); // 🔥 thêm dòng này
    //    //    }
    //    //}
    //}

    //public bool HasBallEntered()
    //{
    //    return ballEntered;
    //}

    //private bool destroyOnBallEnter;

    //public void Init(bool onBallEnter)
    //{
    //    destroyOnBallEnter = onBallEnter;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Ball"))
    //    {
    //        Debug.Log("Ball vào hole");

    //        if (destroyOnBallEnter)
    //        {
    //            gameObject.SetActive(false);
    //        }
    //    }
    //}

    //public int rewardAmount;
    //public bool isHealthReward = false;

    //private bool destroyOnBallEnter = false;
    //private bool isActive = false;

    //// 🔥 Init từ SkillManager
    //public void Init(bool onBallEnter)
    //{
    //    destroyOnBallEnter = onBallEnter;
    //    isActive = true;

    //    //destroyOnBallEnter = onBallEnter;
    //    //isActive = true;
    //}



    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isActive) return;

    //    if (other.CompareTag("Ball"))
    //    {
    //        Debug.Log("Ball vào lỗ!");

    //        ApplyReward();

    //        // 🟢 Ý tưởng 1: biến mất khi ăn bi
    //        if (destroyOnBallEnter)
    //        {
    //            DisableHole();
    //        }
    //    }
    //}

    //public void DisableHole()
    //{
    //    isActive = false;

    //    CancelInvoke();

    //    // ❗ KHÔNG destroy nữa
    //    gameObject.SetActive(false);
    //}

    //// 🔵 Ý tưởng 2: tự biến mất sau thời gian
    //public void AutoDisable(float delay)
    //{
    //    Invoke(nameof(DisableHole), delay);

    //    //Invoke(nameof(DisableHole), delay);
    //}

    //void ApplyReward()
    //{
    //    // tạm bỏ qua theo yêu cầu
    //}

    // PHẢI ĐỂ PUBLIC để script RewardSkill có thể gán giá trị
    //public int rewardAmount;
    //public bool isHealthReward = false; // Phân biệt nhận bi hay nhận máu

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Kiểm tra nếu vật chạm vào là Bi (Ball)
    //    if (other.CompareTag("Ball"))
    //    {
    //        ApplyReward();

    //        // Xóa lỗ ngay sau khi nhận thưởng (theo yêu cầu của bạn)
    //        Destroy(gameObject);
    //    }
    //}

    //void ApplyReward()
    //{
    //    if (isHealthReward)
    //    {
    //        Debug.Log("Nhận " + rewardAmount + " máu!");
    //        // Gọi hàm cộng máu của Player tại đây
    //    }
    //    else
    //    {
    //        Debug.Log("Nhận " + rewardAmount + " bi!");
    //        // Gọi hàm cộng bi của Player tại đây
    //    }
    //}
}
