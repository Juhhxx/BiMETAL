using UnityEngine;

public class BombLoopController : MonoBehaviour
{
    public Animator animator;
    public float cooldownTime = 5f;
    private float cooldownTimer = 0f;
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Idle"))
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= cooldownTime)
            {
                cooldownTimer = 0f;
                animator.SetBool("Launch", true);
            }
        }
        else if (stateInfo.IsName("BombLaunch"))
        {
            animator.SetBool("Launch", false);
        }
    }
}
