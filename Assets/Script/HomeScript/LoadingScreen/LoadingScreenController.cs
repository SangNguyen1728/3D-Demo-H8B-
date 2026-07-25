using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LoadingScreenController : MonoBehaviour
{
    //public Slider progressBar;   // kéo Slider vào đây (Inspector)
    //public TMP_Text progressText;    // optional, có thể để trống

    //private void Start()
    //{
    //    StartCoroutine(LoadTargetScene());
    //}

    //private IEnumerator LoadTargetScene()
    //{
    //    if (SceneLoader.Instance == null)
    //    {
    //        Debug.LogError("[LoadingScreen] Không tìm thấy SceneLoader.Instance! " +
    //            "Chắc chắn SceneLoader đã được tạo từ HomeScene trước đó.");
    //        yield break;
    //    }

    //    string target = SceneLoader.Instance.GetSceneToLoad();
    //    if (string.IsNullOrEmpty(target))
    //    {
    //        Debug.LogError("[LoadingScreen] Chưa có scene nào được set để load!");
    //        yield break;
    //    }

    //    Debug.Log($"[LoadingScreen] Đang load: {target}");

    //    AsyncOperation operation = SceneManager.LoadSceneAsync(target);
    //    operation.allowSceneActivation = false;

    //    while (!operation.isDone)
    //    {
    //        float progress = Mathf.Clamp01(operation.progress / 0.9f);

    //        if (progressBar != null) progressBar.value = progress;
    //        if (progressText != null) progressText.text = (progress * 100f).ToString("F0") + "%";

    //        if (operation.progress >= 0.9f)
    //        {
    //            yield return new WaitForSeconds(0.2f); // delay nhẹ cho mượt, có thể bỏ
    //            operation.allowSceneActivation = true;
    //        }

    //        yield return null;
    //    }
    //}

    [Header("UI References")]
    public Slider progressBar;
    public TMP_Text progressText;

    [Header("Cấu hình thời gian")]
    [Tooltip("Thời gian tối thiểu (giây) để thanh loading chạy, kể cả khi scene load nhanh")]
    public float minLoadingDuration = 2.5f;

    [Tooltip("Tốc độ thanh bar đuổi theo % hiển thị (số càng lớn càng nhanh)")]
    public float fillSpeed = 3f;

    [Header("Hiệu ứng nhấp nháy % khi gần xong")]
    public float blinkStartThreshold = 0.95f; // bắt đầu nhấp nháy khi đạt 95%
    public float blinkSpeed = 8f;

    private float displayedProgress = 0f; // % đang hiển thị (chạy mượt)
    private float realProgress = 0f;      // % thật từ AsyncOperation

    private void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("[LoadingScreen] Không tìm thấy SceneLoader.Instance! " +
                "Chắc chắn SceneLoader đã được tạo từ HomeScene trước đó.");
            yield break;
        }

        string target = SceneLoader.Instance.GetSceneToLoad();
        if (string.IsNullOrEmpty(target))
        {
            Debug.LogError("[LoadingScreen] Chưa có scene nào được set để load!");
            yield break;
        }

        Debug.Log($"[LoadingScreen] Đang load: {target}");

        // Reset trạng thái hiển thị
        displayedProgress = 0f;
        realProgress = 0f;
        if (progressBar != null) progressBar.value = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(target);
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;
        bool readyToActivate = false;

        while (true)
        {
            elapsedTime += Time.deltaTime;

            // % thật từ Unity (0 -> 0.9 trong lúc load, 0.9 -> 1 khi cho phép activate)
            realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // % giả lập theo thời gian tối thiểu, đảm bảo không bao giờ chạy nhanh hơn minLoadingDuration
            float timeBasedProgress = Mathf.Clamp01(elapsedTime / minLoadingDuration);

            // % mục tiêu = giá trị NHỎ HƠN giữa (thời gian giả lập) và (thật) -> không vượt quá tiến trình thật,
            // nhưng cũng không chạy nhanh hơn thời gian tối thiểu mong muốn
            float targetProgress = Mathf.Min(timeBasedProgress, Mathf.Max(realProgress, timeBasedProgress >= 1f ? realProgress : 0f));

            // Cách đơn giản và ổn định hơn: mục tiêu luôn là timeBasedProgress,
            // trừ khi việc load thật sự chưa xong thì không vượt quá realProgress khi realProgress < 1
            targetProgress = (elapsedTime >= minLoadingDuration) ? realProgress : Mathf.Min(timeBasedProgress, 0.99f);

            // Thanh hiển thị đuổi mượt theo targetProgress
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, fillSpeed * Time.deltaTime);

            if (progressBar != null) progressBar.value = displayedProgress;
            if (progressText != null) progressText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";

            // Hiệu ứng nhấp nháy khi gần xong
            if (progressText != null && displayedProgress >= blinkStartThreshold)
            {
                float blinkAlpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0 -> 1 -> 0 liên tục
                Color c = progressText.color;
                c.a = Mathf.Lerp(0.4f, 1f, blinkAlpha);
                progressText.color = c;
            }

            // Điều kiện hoàn tất: đã đủ thời gian tối thiểu, load thật đã xong (>=0.9), và hiển thị đã chạy tới gần 100%
            if (elapsedTime >= minLoadingDuration && operation.progress >= 0.9f && displayedProgress >= 0.99f && !readyToActivate)
            {
                readyToActivate = true;

                // Đảm bảo hiển thị đúng 100% trước khi chuyển scene
                displayedProgress = 1f;
                if (progressBar != null) progressBar.value = 1f;
                if (progressText != null)
                {
                    progressText.text = "100%";
                    Color c = progressText.color;
                    c.a = 1f;
                    progressText.color = c;
                }

                yield return new WaitForSeconds(0.3f); // giữ lại 100% một chút cho người chơi kịp thấy
                operation.allowSceneActivation = true;
            }

            if (operation.isDone)
            {
                yield break;
            }

            yield return null;
        }
    }
}

