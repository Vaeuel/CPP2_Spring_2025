using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]

public class EnemyPathFinder : MonoBehaviour
{
    public enum EnemyState { Chase, Patrol, Freeze, Dead }
    public EnemyState curState;
    private bool isWaiting = false;

    public Transform player;
    public Transform[] path; //Is populated with empty game objects that act as waypoints, passing transform values one at a time for the enemy to navigate to.
    public int pathIndex;
    public float distThreshold = .2f; //Floating point math is inexact, this allows us to get close enough to a waypoint and move to the next one.
                                      //**Sometimes Floating point math can lead to deadlocks if you're trying to get Equal to or greater than.
    private Transform target;
    private NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); //Agent is a kinomatic movement type that allows the use of a nav mesh.
    }

    void FixedUpdate()
    {
        switch (curState)
        {
            case EnemyState.Chase:
                target = player;

                if (!isWaiting)
                {
                    StartCoroutine(WaitAndResume());
                    isWaiting = true;
                }
                break;

            case EnemyState.Patrol:
                target = path[pathIndex];

                if (agent.remainingDistance < distThreshold)
                {
                    pathIndex++;
                    pathIndex %= path.Length;
                    target = path[pathIndex];
                }
                break;

            case EnemyState.Freeze:
                target = null;
                agent.ResetPath();
                break;

            case EnemyState.Dead:
                target = null;
                break;
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    //{
    //    //if (!player) return;

    //    if (curState == EnemyState.Chase)
    //    {
    //        target = player;
    //        StartCoroutine(WaitAndResume());
    //    }

    //    if (curState == EnemyState.Patrol)
    //    {
    //        target = path[pathIndex]; //List of transform paths from the path array. They present as empty game objects in the hierarchy 

    //        if (agent.remainingDistance < distThreshold)
    //        {
    //            pathIndex++;
    //            pathIndex %= path.Length; //0 mod 4 = return 0 && 4 mod 4 returns 0
    //            target = path[pathIndex];
    //        }
    //    }

    //    if (curState == EnemyState.Freeze) target = null;

    //    agent.SetDestination(target.position);
    //}
    public void SetPlayer(Transform p) => player = p;
    public void SetState(EnemyState state) => curState = state;

    private IEnumerator WaitAndResume()
    {
        yield return new WaitForSeconds(5f);
        SetState(EnemyState.Patrol);
        isWaiting = false;
    }
}
