using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Splines;
using static Aiming.Circle;

public class Aiming : MonoBehaviour
{
    public GameObject cueStickPivot;
    public GameObject imaginationBall;
    public LineRenderer aimDirectionLine;
    public LineRenderer cueBallMoveDirectionLine;
    public LineRenderer targetBallMoveDirectionLine;
    public float lineLength = 1.5f, cueBallRadius = 0.1f, whiteEmission, redEmission;

    private List<Circle> dangerousCircles = new List<Circle>();
    public List<GameObject> ballObjects = new List<GameObject>();

    public static bool lineIsDisplaying = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!lineIsDisplaying)
        {
            DisableAimVisual();
            return;
        }

        UpdateAimLine();
        DectectDangerousCircles();
    }

    private void DectectDangerousCircles()
    {
        // clear previous dangerous circles
        dangerousCircles.Clear();
        ballObjects.Clear();

        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        for(int i  = 0; i < rigidbodies.Length; i++)
        {
            GameObject ball = rigidbodies[i].gameObject;
            ballObjects.Add(ball);
            Vector2 ballCenter2D = To2D(ball.transform.position);
            float radius = cueBallRadius * 2;

            Circle ballCircle = new Circle(ballCenter2D, radius);
            dangerousCircles.Add(ballCircle);
        }

        // Sort circle by distance from the cue ball
        dangerousCircles =  SortCirclesByDistanceWithCueBall(dangerousCircles);
    }

    private void UpdateAimLine()
    {
        cueBallMoveDirectionLine.enabled = true;
        targetBallMoveDirectionLine.enabled = true;
        imaginationBall.SetActive(true);
        aimDirectionLine.positionCount = 2;

        Vector3 cueBallPos = cueStickPivot.transform.position;
        Vector3 aimDirection = cueStickPivot.transform.forward;

        aimDirectionLine.SetPosition(0, cueBallPos);
        aimDirectionLine.SetPosition(1, cueBallPos + aimDirection * 10f);

        HandleCircleInteractions(cueBallPos, aimDirection);
    }

    private void DisableAimVisual()
    {
        aimDirectionLine.enabled = false;
        cueBallMoveDirectionLine.enabled = false;
        targetBallMoveDirectionLine.enabled = false;
        imaginationBall.SetActive(false);
    }

    private void HandleCircleInteractions(Vector3 cueBallPos, Vector3 aimDirection)
    {
        Vector2 cueBall2D = To2D(cueBallPos);
        Vector2 aimDirection2D = To2D(aimDirection);

        StraightRay2D aimRay = new StraightRay2D(cueBall2D,aimDirection2D);

        bool hitCircle = false;

        foreach(var circle in dangerousCircles)
        {
            Vector2? cutPoint = circle.CutPoint(aimRay);

            if(cutPoint != null)
            {
                Vector3 hitpoint = To3D((Vector2)cutPoint, cueBallPos.y);
                aimDirectionLine.SetPosition(1, hitpoint);

                imaginationBall.transform.position = hitpoint;
                imaginationBall.SetActive(true);

                // For Finding closetBall to the hitPoint
                GameObject closestBall = null;
                float closestDistantSqr = Mathf.Infinity;

                foreach(var ball in ballObjects)
                {
                    if(ball != null)
                    {
                        Collider ballCollider = ball.GetComponent<Collider>();

                        if(ballCollider != null)
                        {
                            Vector3 ballCenter = ballCollider.bounds.center;
                            float distanceSqr = (hitpoint - ballCenter).sqrMagnitude;

                            if(distanceSqr < closestDistantSqr)
                            {
                                closestDistantSqr = distanceSqr;
                                closestBall = ball;
                            }
                        }
                    }
                }

                if (closestBall != null)
                {
                    Collider closestBallCollider = closestBall.GetComponent<Collider>();

                    if(closestBallCollider != null)
                    {
                        Vector2 cueBallPotentialDirection = Vector2.Perpendicular
                            (new Vector2(hitpoint.x, hitpoint.z) - new Vector2(closestBallCollider.bounds.center.x, closestBallCollider.bounds.center.z));
                        Vector2 cueBallDirection = (aimDirection2D + cueBallPotentialDirection).magnitude > (aimDirection2D - cueBallPotentialDirection).magnitude ? cueBallPotentialDirection : -cueBallPotentialDirection;
                        cueBallDirection.Normalize();

                        //Vector2 impactNormal = (new Vector2(closestBallCollider.bounds.center.x, closestBallCollider.bounds.center.z) - new Vector2(hitpoint.x, hitpoint.z)).normalized;
                        //Vector2 targetBallDirection = impactNormal;
                        //Vector2 incomingVector = aimDirection2D.normalized;
                        //Vector2 cueBallDirections = incomingVector - Vector2.Dot(incomingVector, targetBallDirection) * targetBallDirection;
                        //cueBallDirections.Normalize();
                        Vector2 targetBallDirection = - (new Vector2(hitpoint.x, hitpoint.z) - new Vector2(closestBallCollider.bounds.center.x, closestBallCollider.bounds.center.z)).normalized;

                        UpdateAimVisualizeComponent(hitpoint, closestBallCollider.bounds.center, cueBallDirection, targetBallDirection);
                    }
                }

                hitCircle = true;
                break;
            }
        }

        if(!hitCircle)
        {
            RaycastHit tableHit;
            if(Physics.Raycast(cueBallPos, aimDirection, out tableHit, 100f))
            {
                Vector3 adjustedHitPoint = tableHit.point - aimDirection * cueBallRadius;

                aimDirectionLine.SetPosition(1, adjustedHitPoint);

                // Calculate reflection for cue ball
                imaginationBall.transform.position = adjustedHitPoint;
                imaginationBall.SetActive(true);

                Vector2 adjustedHitPoint2D = To2D(adjustedHitPoint);
                Vector2 tableNormal2D = To2D( tableHit.normal);

                //Approximate the table adge as a line segment perpendicalar to the normal
                Vector2 tableEdgeDirection = Vector2.Perpendicular(tableNormal2D);
                LineSegment2D tableEdgeSegment = new LineSegment2D(
                    adjustedHitPoint2D - tableEdgeDirection * 0.5f,
                    adjustedHitPoint2D + tableEdgeDirection * 0.5f);

                Vector2 cueBallReflection2D = tableEdgeSegment.ReflectVector(aimDirection2D);

                if (tableHit.collider.gameObject.CompareTag("Table"))
                    UpdateAimVisualizeComponent(adjustedHitPoint, Vector3.zero, cueBallReflection2D, Vector2.zero);

                if(tableHit.collider.gameObject.CompareTag("TablePocket"))
                    UpdateAimVisualizeComponent(adjustedHitPoint, Vector3.zero, aimDirection2D * 0.001f, Vector2.zero);
            }
        }

       
    }

    private void UpdateAimVisualizeComponent(Vector3 hitPosition, Vector3 targetBallPosition, Vector2 cueBallDirection, Vector2 targetBallDirection)
    {
        // Determine alignmet whit the aim direction
        Vector2 aimdirection2D = To2D(cueStickPivot.transform.forward);

        // Calculate alignment factors
        float cueBallAlignment = Mathf.Abs(Vector2.Dot(cueBallDirection.normalized , aimdirection2D));
        float targetBallAlignemt = Mathf.Abs(Vector2.Dot(targetBallDirection.normalized, aimdirection2D));

        // Normalize alignment factors to ensure 
        float totalAlignment = cueBallAlignment + targetBallAlignemt;
        cueBallAlignment /= totalAlignment;
        targetBallAlignemt /= totalAlignment;

        // Scale line lengths base on alignment
        float cueBallLineLength = lineLength * cueBallAlignment;
        float targetBallLineLength = lineLength * targetBallAlignemt;

        // Adjust cue ball move direction line length
        LineRenderer lr = cueBallMoveDirectionLine;
        lr.positionCount = 2;
        lr.SetPosition(0, hitPosition);
        lr .SetPosition(1, hitPosition + new Vector3(cueBallDirection.x, 0, cueBallDirection.y) * cueBallLineLength);

        // Adjust target ball move direction line length
        lr = targetBallMoveDirectionLine;
        lr.positionCount = 2;
        lr.SetPosition(0, targetBallPosition);
        Vector3 targetBallDirection3D = new Vector3(targetBallDirection.x, 0, targetBallDirection.y);
        lr.SetPosition(1, targetBallPosition + new Vector3(targetBallPosition.x, 0, targetBallPosition.z) * targetBallLineLength);

        // Adjust the aim direction line (unchanged)
        lr = aimDirectionLine;
        lr.positionCount = 2;
        lr.SetPosition(0, cueStickPivot.transform.position);
        lr.SetPosition(1, hitPosition);


        // Position the imaginationBall at the hit point
        imaginationBall.transform.position = hitPosition;
    }

    public static Vector2 To2D(Vector3 vec)
    {
        return new Vector2 (vec.x, vec.z);
    }

    public static Vector3 To3D(Vector2 vec, float y)
    {
        return new Vector3(vec.x, y, vec.y);
    }

    public List<Circle> SortCirclesByDistanceWithCueBall(List<Circle> dangerousCircle)
    {
        Vector2 cueBall2D  = To2D(cueStickPivot.transform.position);
        return SortCirclesByDistanceWithPoint(dangerousCircle, cueBall2D);
    }

    public List<Circle> SortCirclesByDistanceWithPoint(List<Circle> dangerousCircle, Vector3 rawPosition)
    {
        dangerousCircle.Sort((c1, c2) =>
        Vector2.Distance(c1.center, rawPosition). CompareTo(Vector2.Distance(c2.center, rawPosition)));
        return dangerousCircle;
    }
    public struct Circle
    {
        public Vector2 center;
        public float radius;

        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool IsContain(Vector2 point)
        {
            return Vector2.Distance(center, point) <= radius;
        }

        public Vector2? CutPoint(StraightRay2D ray)
        {
            Vector2 prjCenterOnAimLine = (Vector2)Vector3.Project(center - ray.start,
                ray.direction) + ray.start;
            if(IsContain(prjCenterOnAimLine))
            {
                float disFromCenterToPrj = Vector2.Distance(prjCenterOnAimLine, center);
                float disFromPrjToHitPoint = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(disFromCenterToPrj, 2));
                Vector2 hitPosition = prjCenterOnAimLine - (ray.direction.normalized * disFromPrjToHitPoint);
                if(ray.IsContain(hitPosition)) // Hit
                {
                    return hitPosition;
                }
            }
            return null;
        }

        public class StraightRay2D
        {
            public Vector2 start;
            public Vector2 direction;

            public StraightRay2D(Vector2 start, Vector2 direction)
            {
                this.start = start;
                this.direction = direction;
            }

            public bool IsContain(Vector2 point)
            {
                Vector2 toPoint = point - start;
                float angleErrorMargin = 0.1f;
                return Vector2.Angle(toPoint, direction) < angleErrorMargin;
            }

        }

        public class LineSegment2D
        {
            public Vector2 start;
            public Vector2 end;

            public LineSegment2D(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;
            }

            public Vector2 ReflectVector(Vector2 vec)
            {
                Vector2 prj = (Vector2)Vector3.Project(vec, end - start);
                return vec + 2 * (prj - vec);
            }
        }

    }

}
