using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class AnimationManager : MonoBehaviour
{
    Animator anim;
    EnemyController controlScript;
    public event Action<bool> OnToggleMovement;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controlScript = GetComponent<EnemyController>();
        //Debug.Log ($"AnimationManager: Control Script is set to {controlScript.GetType().Name}");
        if (anim == null) Debug.Log("Animation Manager: Animator assignment has failed");
    }

    public void LocoVel(float speed)
    {
        //Debug.Log($"Animation Manager: speed is " +  speed);
        anim.SetFloat("vel", speed); //Passes velocity to the animator to support anim blending.
    }

    public void Death(string targetHit)
    {
        anim.SetTrigger("IsDead");
        //Destroy(gameObject);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (targetHit == "Enemy")
        {
            Debug.Log($"AnimationManager: Death was triggered by {targetHit}");
            StartCoroutine(WaitThenDestroy());
        }

        if (targetHit == "Player")
        {
            Debug.Log($"AnimationManager: Death was triggered by {targetHit}");
            StartCoroutine(WaitAndReload());
        }
    }
    #region Coroutines
    IEnumerator WaitThenDestroy()
    {
        yield return null; // Wait a frame so the animation has time to transition
        controlScript.Death(); 
        yield return new WaitForSeconds(3f);
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 1f); // Wait for the animation to finish
        Debug.Log($"AnimationManager: WaitThenDestroy is destroying {gameObject.name}");
        Destroy(gameObject);
    }
    IEnumerator WaitAndReload()
    {
        OnToggleMovement?.Invoke(false); // Tell listeners to stop movement
        yield return null; // Wait a frame so the animation has time to transition
        yield return new WaitForSeconds(3f); // Wait for the animation to finish
        Debug.Log($"AnimationManager: WaitAndReload() is done waiting");
        GameManager.Instance.LoadGame();
        Destroy(gameObject);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Restart level
    }
    private IEnumerator DelayMovement()
    {
        OnToggleMovement?.Invoke(false); // Tell listeners to stop movement

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation to finish

        OnToggleMovement?.Invoke(true); // Tell listeners to resume movement
    }
    #endregion
    public void Attacking(string attackType) //Change to power attack once debugged, punch is left click and unarmed, kick is right click and unarmed. armed == quick attack/ power attack. or fire/ aim. Switch case?
    {
        if (attackType == "Primary")
        {
            anim.SetTrigger("IsSlashing");
        }
        else if (attackType == "Secondary")
        {
            anim.SetTrigger("IsKicking");
            StartCoroutine(DelayMovement());
        }
    }

    internal void EnemyAttack()
    {
        Debug.Log("AnimationManager: Some EnemyAttack has been called upon");
        anim.SetTrigger("IsAttacking");
    }
}
