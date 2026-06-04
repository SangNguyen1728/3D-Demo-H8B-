using UnityEngine;

public class HoleLogic : MonoBehaviour
{
    //private bool destroyOnBallEnter;
    //private bool ballEntered = false;
    //private bool isActive = false;

    //// HP SYSTEM
    //private bool useHP = false;
    //private float currentHP = 0;
    //private float maxHP = 0;

    //private Collider[] colliders;

    //// 🎨 RENDER
    //private Renderer holeRenderer;

    //void Awake()
    //{
    //    colliders = GetComponentsInChildren<Collider>();
    //    holeRenderer = GetComponentInChildren<Renderer>();
    //}

    //public void Init(bool onBallEnter, bool useHP = false, float hp = 0)
    //{
    //    destroyOnBallEnter = onBallEnter;
    //    ballEntered = false;
    //    isActive = false;

    //    this.useHP = useHP;
    //    currentHP = hp;
    //    maxHP = hp;

    //    SetCollider(false);

    //    // 🎨 reset màu
    //    UpdateColor();

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

    //    // 🟢 Skill 1.1 / 2.1
    //    if (destroyOnBallEnter)
    //    {
    //        ballEntered = true;
    //    }

    //    // 🔥 Skill 1.2 / 2.2 (HP)
    //    if (useHP)
    //    {
    //        float damage = 500f;
    //        currentHP -= damage;

    //        Debug.Log($"💥 Hole HP: {currentHP}");

    //        UpdateColor(); // 🎨 update màu mỗi lần bị hit

    //        if (currentHP <= 0)
    //        {
    //            Debug.Log("🔥 Hole hết HP");
    //            ballEntered = true;
    //        }
    //    }
    //}

    //// 🎨 COLOR LOGIC
    //void UpdateColor()
    //{
    //    if (!useHP || holeRenderer == null) return;

    //    float percent = currentHP / maxHP;

    //    if (percent > 0.5f)
    //    {
    //        // 🟡 Vàng
    //        holeRenderer.material.color = Color.yellow;
    //    }
    //    else if (percent > 0.15f)
    //    {
    //        // 🟣 Tím đậm
    //        holeRenderer.material.color = new Color(0.4f, 0f, 0.6f);
    //    }
    //    else
    //    {
    //        // 🟠 Cam đậm
    //        holeRenderer.material.color = new Color(1f, 0.4f, 0f);
    //    }
    //}

    //public void EnableTrigger()
    //{
    //    Collider col = GetComponent<Collider>();
    //    if (col != null)
    //        col.enabled = true;
    //}

    //public bool HasBallEntered()
    //{
    //    return ballEntered;
    //}


    private bool destroyOnBallEnter;
    private bool isActive = false;

    // =========================
    // 🎯 BALL COUNT SYSTEM
    // =========================
    private int ballsRequired = 1;
    private int currentBalls = 0;

    // =========================
    // 🎯 TURN SYSTEM
    // =========================
    private int remainTurns = 1;

    // =========================
    // 🎯 HP SYSTEM
    // =========================
    private bool useHP = false;
    private float currentHP = 0;
    private float maxHP = 0;

    private bool completed = false;

    private Collider[] colliders;

    private Renderer holeRenderer;

    [HideInInspector] public int ownerSkillID = -1;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        holeRenderer = GetComponentInChildren<Renderer>();
    }

    public void Init(
        bool onBallEnter,
        bool useHP = false,
        float hp = 0,
        int ballsRequired = 1,
        int turns = 1)
    {
        destroyOnBallEnter = onBallEnter;

        this.useHP = useHP;

        currentHP = hp;
        maxHP = hp;

        this.ballsRequired = ballsRequired;
        currentBalls = 0;

        remainTurns = turns;

        completed = false;

        isActive = false;

        SetCollider(false);

        UpdateColor();

        Debug.Log($"{name} INIT");
        ownerSkillID = -1; // reset mỗi lần init
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

        // =========================
        // 🔥 HP MODE
        // =========================
        if (useHP)
        {
            float damage = 500f;

            currentHP -= damage;

            Debug.Log($"💥 Hole HP: {currentHP}");

            UpdateColor();

            if (currentHP <= 0)
            {
                completed = true;
            }

            return;
        }

        // =========================
        // 🎯 BALL COUNT MODE
        // =========================
        currentBalls++;

        Debug.Log($"BALL COUNT: {currentBalls}/{ballsRequired}");

        if (currentBalls >= ballsRequired)
        {
            completed = true;
        }
    }

    // =========================
    // 🎨 COLOR LOGIC
    // =========================
    void UpdateColor()
    {
        if (!useHP || holeRenderer == null) return;

        float percent = currentHP / maxHP;

        if (percent > 0.5f)
        {
            holeRenderer.material.color = Color.yellow;
        }
        else if (percent > 0.15f)
        {
            holeRenderer.material.color = new Color(0.4f, 0f, 0.6f);
        }
        else
        {
            holeRenderer.material.color = new Color(1f, 0.4f, 0f);
        }
    }

    public void EnableTrigger()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = true;
    }

    // =========================
    // 🎯 TURN COUNT
    // =========================
    public void ReduceTurn()
    {
        remainTurns--;

        Debug.Log($"{name} remain turns: {remainTurns}");
    }

    public bool IsExpired()
    {
        return remainTurns <= 0;
    }

    public bool IsCompleted()
    {
        return completed;
    }
}
