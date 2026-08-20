using UnityEngine;
using System.Collections;

public class HoleLogic : MonoBehaviour
{

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
    [HideInInspector] public int ownerPlayer = -1; // MỚI
    [HideInInspector] public System.Action onBallEntered;

    private bool respawnOnce = false;
    private bool hasRespawned = false;
    private bool respawnOnHit = false;
    private float respawnHPCost = 0f;
    private bool immuneToEnemy = false;
    private PlacementZone placementZone = PlacementZone.Free;



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
        ownerPlayer = -1; // MỚI — reset mỗi lần init
        onBallEntered = null;
        respawnOnce = false;
        hasRespawned = false;
        respawnOnHit = false;
        respawnHPCost = 0f;
        immuneToEnemy = false;
        placementZone = PlacementZone.Free;
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
        // 🔥 SKILL CALLBACKS
        // =========================

        // Gọi callback skill (Chuột 1.0, Chuột Tơ 1.1, Thỏ Già 2.2)
        onBallEntered?.Invoke();

        // Chuột Già 1.2: móc lên 1 lần, giữ nguyên HP
        if (respawnOnce && !hasRespawned)
        {
            hasRespawned = true;
            StartCoroutine(RespawnBallKeepHP(other.gameObject));
            return;
        }

        // Thỏ Tơ 2.1: nổi lại, trừ HP
        if (respawnOnHit && !hasRespawned)
        {
            hasRespawned = true;
            StartCoroutine(RespawnBallLoseHP(other.gameObject, respawnHPCost));
            return;
        }

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

    public void SetRespawnOnce(bool val) => respawnOnce = val;

    public void SetRespawnOnHit(bool val, float hpCost)
    {
        respawnOnHit = val;
        respawnHPCost = hpCost;
    }

    public void SetImmuneToEnemy(bool val) => immuneToEnemy = val;
    public void SetPlacementZone(PlacementZone zone) => placementZone = zone;

    private IEnumerator RespawnBallKeepHP(GameObject ball)
    {
        BallHealth bh = ball.GetComponent<BallHealth>();
        float savedHP = (bh != null) ? bh.GetCurrentHealth() : 0f;

        yield return new WaitForSeconds(1f);

        ball.transform.position = GetRespawnPosition();

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ball.SetActive(true);

        if (bh != null) bh.BlinkOnRespawn(3f);

        if (bh != null) bh.SetHealthDirect(savedHP);

        Debug.Log("[Chuột Già] Bi móc lên, giữ HP: " + savedHP);
    }

    private IEnumerator RespawnBallLoseHP(GameObject ball, float hpCost)
    {
        yield return new WaitForSeconds(1f);

        ball.transform.position = GetRespawnPosition();

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ball.SetActive(true);

        

        BallHealth bh = ball.GetComponent<BallHealth>();
        if (bh != null) bh.BlinkOnRespawn(3f);
        if (bh != null) bh.TakeDamage(hpCost);

        Debug.Log("[Thỏ Tơ] Bi nổi lại, mất " + hpCost + " HP");
    }

    private Vector3 GetRespawnPosition()
    {
        // 🔥 Chỉnh lại tọa độ theo scene của bạn
        return new Vector3(0f, 0.75f, 0f);
    }
}
