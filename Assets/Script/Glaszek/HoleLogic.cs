using UnityEngine;

public class HoleLogic : MonoBehaviour
{
    private bool destroyOnBallEnter;
    private bool ballEntered = false;
    private bool isActive = false;

    // HP SYSTEM
    private bool useHP = false;
    private float currentHP = 0;
    private float maxHP = 0;

    private Collider[] colliders;

    // 🎨 RENDER
    private Renderer holeRenderer;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        holeRenderer = GetComponentInChildren<Renderer>();
    }

    public void Init(bool onBallEnter, bool useHP = false, float hp = 0)
    {
        destroyOnBallEnter = onBallEnter;
        ballEntered = false;
        isActive = false;

        this.useHP = useHP;
        currentHP = hp;
        maxHP = hp;

        SetCollider(false);

        // 🎨 reset màu
        UpdateColor();

        Debug.Log($"{name} INIT | HP: {currentHP}");
    }

    public void ActivateHole()
    {
        isActive = true;
        SetCollider(true);

        Debug.Log(name + " → ACTIVE");
    }

    void SetCollider(bool state)
    {
        foreach (var c in colliders)
            c.enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        BallNo ball = other.GetComponentInParent<BallNo>();
        if (ball == null) return;

        Debug.Log("🎯 Ball vào hole: " + ball.ballNumber);

        // 🟢 Skill 1.1 / 2.1
        if (destroyOnBallEnter)
        {
            ballEntered = true;
        }

        // 🔥 Skill 1.2 / 2.2 (HP)
        if (useHP)
        {
            float damage = 500f;
            currentHP -= damage;

            Debug.Log($"💥 Hole HP: {currentHP}");

            UpdateColor(); // 🎨 update màu mỗi lần bị hit

            if (currentHP <= 0)
            {
                Debug.Log("🔥 Hole hết HP");
                ballEntered = true;
            }
        }
    }

    // 🎨 COLOR LOGIC
    void UpdateColor()
    {
        if (!useHP || holeRenderer == null) return;

        float percent = currentHP / maxHP;

        if (percent > 0.5f)
        {
            // 🟡 Vàng
            holeRenderer.material.color = Color.yellow;
        }
        else if (percent > 0.15f)
        {
            // 🟣 Tím đậm
            holeRenderer.material.color = new Color(0.4f, 0f, 0.6f);
        }
        else
        {
            // 🟠 Cam đậm
            holeRenderer.material.color = new Color(1f, 0.4f, 0f);
        }
    }

    public bool HasBallEntered()
    {
        return ballEntered;
    }

    //// ALL SKILLS DONE
    //private bool destroyOnBallEnter;
    //private bool ballEntered = false;
    //private bool isActive = false;

    //// 🆕 HP SYSTEM
    //private bool useHP = false;
    //private float currentHP = 0;

    //private Collider[] colliders;

    //void Awake()
    //{
    //    colliders = GetComponentsInChildren<Collider>();
    //}

    //public void Init(bool onBallEnter, bool useHP = false, float hp = 0)
    //{
    //    destroyOnBallEnter = onBallEnter;
    //    ballEntered = false;
    //    isActive = false;

    //    // 🆕 HP
    //    this.useHP = useHP;
    //    currentHP = hp;

    //    SetCollider(false);

    //    Debug.Log($"{name} INIT | HP: {currentHP}");
    //}

    //public void ActivateHole()
    //{
    //    isActive = true;
    //    SetCollider(true);

    //    Debug.Log(name + " → ACTIVE");
    //}

    //void SetCollider(bool state)
    //{
    //    foreach (var c in colliders)
    //        c.enabled = state;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isActive) return;

    //    BallNo ball = other.GetComponentInParent<BallNo>();
    //    if (ball == null) return;

    //    Debug.Log("🎯 Ball vào hole: " + ball.ballNumber);

    //    // =========================
    //    // 🟢 SKILL 1.1 / 2.1
    //    // =========================
    //    if (destroyOnBallEnter)
    //    {
    //        ballEntered = true;
    //    }

    //    // =========================
    //    // 🔥 SKILL 1.2 / 2.2 (HP)
    //    // =========================
    //    if (useHP)
    //    {
    //        float damage = 500f; // 👉 mỗi bi trừ 500 (bạn chỉnh tùy ý)

    //        currentHP -= damage;

    //        Debug.Log($"💥 Hole HP: {currentHP}");

    //        if (currentHP <= 0)
    //        {
    //            Debug.Log("🔥 Hole hết HP");
    //            ballEntered = true; // reuse logic cũ
    //        }
    //    }
    //}

    //public bool HasBallEntered()
    //{
    //    return ballEntered;
    //}
    ///
    //private bool destroyOnBallEnter;
    //private bool ballEntered = false;
    //private bool isActive = false;

    //private Collider[] colliders;

    //void Awake()
    //{
    //    colliders = GetComponentsInChildren<Collider>(); // 🔥 lấy TẤT CẢ
    //}

    //public void Init(bool onBallEnter)
    //{
    //    destroyOnBallEnter = onBallEnter;
    //    ballEntered = false;
    //    isActive = false;

    //    SetCollider(false);

    //    Debug.Log(name + " INIT → collider OFF");
    //}

    //public void ActivateHole()
    //{
    //    isActive = true;

    //    SetCollider(true);

    //    Debug.Log(name + " → ACTIVE");
    //}

    //void SetCollider(bool state)
    //{
    //    foreach (var c in colliders)
    //    {
    //        c.enabled = state;
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isActive) return;

    //    BallNo ball = other.GetComponentInParent<BallNo>();

    //    if (ball != null)
    //    {
    //        Debug.Log("🎯 Ball vào hole: " + ball.ballNumber);

    //        if (destroyOnBallEnter)
    //        {
    //            ballEntered = true;
    //        }
    //    }
    //}

    //public bool HasBallEntered()
    //{
    //    return ballEntered;
    //}

    //private bool destroyOnBallEnter;
    //private bool ballEntered = false;

    //private bool isActive = false;

    //public void Init(bool onBallEnter)
    //{
    //    //destroyOnBallEnter = onBallEnter;
    //    //ballEntered = false;

    //    destroyOnBallEnter = onBallEnter;
    //    ballEntered = false;
    //    isActive = false; // chưa hoạt động
    //}
    //public void ActivateHole()
    //{
    //    isActive = true;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isActive) return; // 🔥 CHẶN

    //    BallNo ball = other.GetComponentInParent<BallNo>();

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
    //}

    //public bool HasBallEntered()
    //{
    //    return ballEntered;
    //}

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
