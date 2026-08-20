using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : MonoBehaviour
{
    [Header("Player Identity")]
    public int playerNumber = 1;

    [Header("Stamina")]
    public float maxStamina = 100f;
    private float currentStamina;

    [Header("UI")]
    public Slider staminaSlider;

    [Header("Cân bằng công thức hồi (chia thêm để quy đổi thang HP -> thang Stamina)")]
    public float rewardDivisor = 50f;

    private void Awake()
    {
        StaminaManagerRegistry.Register(playerNumber, this);
    }

    private void Start()
    {
        //currentStamina = maxStamina;
        currentStamina = 0f;
        UpdateUI();
    }

    public bool TryConsume(int cost)
    {
        if (currentStamina < cost)
        {
            Debug.LogWarning($"[Stamina P{playerNumber}] Không đủ năng lượng! Cần {cost}, hiện có {currentStamina}");
            return false;
        }

        currentStamina -= cost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        Debug.Log($"[Stamina P{playerNumber}] -{cost} -> còn {currentStamina}");
        UpdateUI();
        return true;
    }

    public void AddStamina(float amount)
    {
        if (amount <= 0) return;

        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        Debug.Log($"[Stamina P{playerNumber}] +{amount:F1} -> {currentStamina:F1}");
        UpdateUI();
    }

    public float GetCurrentStamina() => currentStamina;

    private void UpdateUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina / maxStamina;
    }
}
