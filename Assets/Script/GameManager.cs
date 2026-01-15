using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

public class GameManager : MonoBehaviour
{
    public Animator UpperUIAnimator, cameraButtonAnim,buttonPauseAnim;
    private Animator pausePanelAnim, backButtonAnim, settingPanelAnim,audioPanelAnim,infoPanelAnim,
        displayPanelAnim;

    public Animator[] selectShapeAnimator;

    [NonSerialized] public GameObject previousPanel, previousOptionPanel;

    public GameObject backButton, pausePanel, backGround, settingPanel, audioPanel, infoPanel,displayPanel;

    public bool UpperUIShift = true;

    public CueStickController cueStickController;


    private void Start()
    {
        //cueStickController = GetComponent<CueStickController>();

        pausePanelAnim = pausePanel.GetComponent<Animator>();
        backButtonAnim = backButton.GetComponent<Animator>();
        settingPanelAnim = settingPanel.GetComponent<Animator>();
        audioPanelAnim = audioPanel.GetComponent<Animator>();
        infoPanelAnim = infoPanel.GetComponent<Animator>();
        displayPanelAnim = displayPanel.GetComponent<Animator>();

        if (cueStickController == null)
        {
            //cueStickController = FindObjectOfType<CueStickController>();
            cueStickController = GetComponent<CueStickController>();
        }

        pausePanel.SetActive(false);
        backGround.SetActive(false);
        backButton.SetActive(false);

        settingPanel.SetActive(false);
        audioPanel.SetActive(false);
        displayPanel.SetActive(false);
        infoPanel.SetActive(false);

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

    private IEnumerator SettingPanelSelectActive()
    {
        if (pausePanel && pausePanel.activeSelf)
        {
            pausePanelAnim.SetBool("IsGoBack", true);
            StartCoroutine(DeactivatePanel(0.35f));
            previousPanel = pausePanel;
            yield return new WaitForSecondsRealtime(0.3f);

            settingPanel.SetActive(true);
            foreach(Animator selected in selectShapeAnimator)
            {
                selected.gameObject.SetActive(true);
                selected.SetBool("IsSelectOut", true);
            }

            if(!backButton.activeSelf)
            {
                backButton.SetActive(true);
            }

            selectShapeAnimator[0].SetBool("IsSelectOut", false);
            audioPanel.SetActive(true);
            previousOptionPanel = audioPanel;
        }
    }

    private IEnumerator InforPanelSetActive()
    {
        if(previousOptionPanel == audioPanel)
        {
            audioPanelAnim.SetBool("IsGoBack", true);
            selectShapeAnimator[0].SetBool("IsSelectOut", true);
        }
        if(previousOptionPanel == displayPanel)
        {
            displayPanelAnim.SetBool("IsGoBack", true);
            selectShapeAnimator[2].SetBool("IsSelectOut", true);
        }

        StartCoroutine(DeactivateOptionPanel(0.35f));
        selectShapeAnimator[1].SetBool("IsSelectOut", false);
        yield return new WaitForSecondsRealtime(0.15f);
        infoPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        previousOptionPanel = infoPanel;
    }

    private IEnumerator AudioPanelSetActive()
    {
        if(previousOptionPanel == infoPanel)
        {
            infoPanelAnim.SetBool("IsGoBack", true);
            selectShapeAnimator[1].SetBool("IsSelectOut", true);
        }
        if(previousOptionPanel == displayPanel)
        {
            displayPanelAnim.SetBool("IsGoBack", true);
            selectShapeAnimator[2].SetBool("IsSelectOut", true);
        }

        StartCoroutine(DeactivateOptionPanel(0.35f));
        selectShapeAnimator[0].SetBool("IsSelectOut", false);
        yield return new WaitForSecondsRealtime(0.15f);
        audioPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        previousOptionPanel = audioPanel;
    }

    private IEnumerator DisplayPanelSetActive()
    {
        if(previousOptionPanel == infoPanel)
        {
            infoPanelAnim.SetBool("IsGoBack", true);
            selectShapeAnimator[1].SetBool("IsSelectOut", true);
        }
        if(previousOptionPanel == audioPanel)
        {
            audioPanelAnim.SetBool("isGoBack", true);
            selectShapeAnimator[0].SetBool("IsSelectOut", true);
        }

        StartCoroutine(DeactivateOptionPanel(0.35f));
        selectShapeAnimator[2].SetBool("IsSelectOut", false);
        yield return new WaitForSecondsRealtime(0.15f);
        displayPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        previousOptionPanel = displayPanel;
    }

    private IEnumerator BackPanelActive()
    {
        if(settingPanel.activeSelf)
        {
            settingPanelAnim.SetBool("IsGoBack", true);
            StartCoroutine(DeactivateSettingPanel(0.35f));

            if(previousOptionPanel == audioPanel)
            {
                audioPanelAnim.SetBool("IsGoBack", true);
                selectShapeAnimator[0].SetBool("IsSelectOut", true);
            }
            if( previousOptionPanel == infoPanel)
            {
                infoPanelAnim.SetBool("IsGoBack", true);
                selectShapeAnimator[1].SetBool("IsSelectOut", true);
            }
            if(previousOptionPanel == displayPanel)
            {
                displayPanelAnim.SetBool("IsGoBack", true);
                selectShapeAnimator[2].SetBool("IsSelectOut", true);
            }
        }

        StartCoroutine(DeactivateOptionPanel(0.35f));

        // Back from pause panel
        if(pausePanel && pausePanel.activeSelf)
        {
            ResumeGame();
        }

        yield return new WaitForSecondsRealtime(0.3f);

        if(pausePanel && previousPanel == pausePanel)
        {
            pausePanel.SetActive(true);

            if(!backButton.activeSelf)
            {
                backButton.SetActive(true);
            }
        }
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
        if(!settingPanel.activeSelf)
        {
            backButton.SetActive(false);
        }
    }

    private IEnumerator DeactivateSettingPanel(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        settingPanel.SetActive(false);
    }

    private IEnumerator DeactivateOptionPanel(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if(previousOptionPanel == audioPanel)
            audioPanel.SetActive(false);
        if(previousOptionPanel == infoPanel)
            infoPanel.SetActive(false);
        if(previousOptionPanel == displayPanel)
            displayPanel.SetActive(false);
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

    public void OnSettingButtonClicked()
    {
        StartCoroutine(SettingPanelSelectActive());
    }
    public void OnInfoButtonClickedd()
    {
        StartCoroutine(InforPanelSetActive());
    }
    public void OnAdudioButtonClicked()
    {
        StartCoroutine(AudioPanelSetActive());
    }
    public void OnDisplayButtonClicked()
    {
        StartCoroutine(DisplayPanelSetActive());
    }
    public void OnBackButtonClicked()
    {
        StartCoroutine(BackPanelActive());
    }
}
