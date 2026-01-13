using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Animator UpperUIAnimator, cameraButtonAnim,buttonPauseAnim;
    private Animator pausePanelAnim, backButtonAnim;

    public GameObject backButton, pausePanel, backGround;

    public bool UpperUIShift = true;

    public CueStickController cueStickController;


    private void Start()
    {
        //cueStickController = GetComponent<CueStickController>();

        pausePanelAnim = pausePanel.GetComponent<Animator>();
        backButtonAnim = backButton.GetComponent<Animator>();

        if (cueStickController == null)
        {
            //cueStickController = FindObjectOfType<CueStickController>();
            cueStickController = GetComponent<CueStickController>();
        }

        pausePanel.SetActive(false);
        backGround.SetActive(false);
        backButton.SetActive(false);

        UpperUIAnimator.SetBool("IsIldePlace", true);
        UpperUIAnimator.SetBool("IsGoBack", false);
    }

    public void OnPauseButtonClicked()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        backGround.SetActive(true);
        backButton.SetActive(true);
        cameraButtonAnim.SetBool("IsGoBack", true);
        buttonPauseAnim.SetBool("IsGoBack", true);
        cueStickController.powerSliderAnim.SetBool("IsGoBack", true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanelAnim.SetBool("IsGoBack", true);
        backGround.SetActive(false);
        backButtonAnim.SetBool("IsGoBack", true);

        if (cameraButtonAnim && buttonPauseAnim)
        {
            cameraButtonAnim.SetBool("IsGoBack", false);
            buttonPauseAnim.SetBool("IsGoBack", false);
            cueStickController.powerSliderAnim.SetBool("IsGoBack", false);
        }

        StartCoroutine(DeactivatePanel(0.4f));
    }

    private IEnumerator DeactivatePanel(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        pausePanel.SetActive(false);
        backButton.SetActive(false);
    }

    public void OnUpperUIButtonClicked()
    {
        //if (cueStickController == null || UpperUIAnimator == null)
        //{
        //    Debug.LogError("Thiếu Animator hoặc CueStickController trên GameManager!");
        //    return;
        //}

        if (!cueStickController.isOnTopCameraActive)
        {
            UpperUIShift = !UpperUIShift;
            UpperUIAnimator.SetBool("IsGoBack", !UpperUIShift);
        }
        else
        {
            UpperUIAnimator.SetBool("IsGoBack", false);
        }
    }
}
