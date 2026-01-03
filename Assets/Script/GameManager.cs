using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Animator UpperUIAnimator;

    public bool UpperUIShift = true;

    public CueStickController cueStickController;

    private void Start()
    {
        //cueStickController = GetComponent<CueStickController>();

        if (cueStickController == null)
        {
            cueStickController = FindObjectOfType<CueStickController>();
        }

        UpperUIAnimator.SetBool("IsIldePlace", true);
        UpperUIAnimator.SetBool("IsGoBack", false);
    }

    public void OnUpperUIButtonClicked()
    {
        if (cueStickController == null || UpperUIAnimator == null)
        {
            Debug.LogError("Thiếu Animator hoặc CueStickController trên GameManager!");
            return;
        }

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
