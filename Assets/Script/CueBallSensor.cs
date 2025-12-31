using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueBallSensor : MonoBehaviour
{
    private CueStickController controller;
    void Start() { controller = FindObjectOfType<CueStickController>(); }

    private void OnCollisionEnter(Collision collision)
    {
        if (controller != null) controller.NotifyFirstCollision(collision.gameObject);
    }
}
