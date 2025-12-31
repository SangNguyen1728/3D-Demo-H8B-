using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ball : MonoBehaviour
{
    //public GameObject cueBall;
    [SerializeField] public Rigidbody cueBallRigitBody;
    [SerializeField] public Transform cueStickPivot, stickTransform;

    public float amountOfForce;
    public int numberOfForces = 12;
    public bool switch1;
    public string name = "oldName";
    public Vector3 newVector, stickEndPosition;
    public Camera mainCamera;

    public List<GameObject> Balls;
    public List<string> familyMembers;
    public float[] digits = { 0.2f, 0.5f, 5, 12.5f };
   
    public void Start()
    {
        //switch1 = false;
        numberOfForces = 20;

       StartCoroutine(AssignBallsToList());
    }
    public void Update()
    {
        AddForceOnCueBall(newVector);
        //switch1 = SwitchBool();
        amountOfForce = ForceNumber();

        MouseInputRay();
    }


    private IEnumerator AssignBallsToList()
    {
        GameObject[] ballWithTags = GameObject.FindGameObjectsWithTag("Ball");

        foreach (GameObject gameObject in ballWithTags)
        {
            Balls.Add(gameObject);
            Debug.Log(gameObject);
            yield return new WaitForSeconds(0.1f);

        }

        Vector3 startPosition = stickTransform.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y,startPosition.z - 0.5f);

        float lerpSpeed = 0f, duration = 1f;
        while (lerpSpeed < duration)
        {
            //stickTransform.localPosition = Vector3.MoveTowards(stickTransform.localPosition, stickEndPosition, lerpSpeed);
            //lerpSpeed += Time.deltaTime * speed;
            stickTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, lerpSpeed / duration);
            lerpSpeed += Time.deltaTime;
            yield return null;
           
        }

       // stickTransform.localPosition = targetPosition;
    }

    public void AddForceOnCueBall(Vector3 cueBallForward)
    {
        if(switch1 == true && numberOfForces == 13)
        {
            cueBallForward = cueStickPivot.forward;
            cueBallRigitBody.AddForce(cueBallForward * 10, ForceMode.Impulse);
        }
        else if(amountOfForce != 1.5f)
        {
            Debug.Log("Switch off");
            return;
        }
        else
        {
            Debug.Log("amountOfForce = 1.5");
        }

    }

    public bool SwitchBool()
    {
        //if(amountOfForce == 2)
        //{
        //    return true;
        //}
        return false;
    }

    public float ForceNumber()
    {
        if(numberOfForces == 15)
        {
            return 100.5f;
        }

        return 50.5f;
    }


    //private void AssignBallsToList()
    //{
    //    GameObject[] ballWithTags = GameObject.FindGameObjectsWithTag("Ball");

    //    foreach (GameObject gameObject in ballWithTags)
    //    {
    //        Balls.Add(gameObject);
    //        Debug.Log(gameObject);
    //    }
    //}

    private void MouseInputRay()
    {
        if(Input.GetMouseButton(0))
        {

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.blue);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                cueStickPivot.LookAt(hit.point);
            }
        }
    }
}
