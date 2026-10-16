using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Input")]
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Animation")]
    public Animator animator;
    public string swordParameter = "HasSword";
    public string idleState = "Sword Idle";
    public string[] attackStates = { "SwordAttack1", "SwordAttack2", "SwordAttack3" };
    public float fadeTime = 0.1f;

    [Header("Combo Timing (0 to 1 through the animation)")]
    [Range(0f, 1f)]
    public float comboOpen = 0.5f; // after this point, a buffered click starts the next attack

    private int comboIndex = -1;   // -1 means not attacking
    private bool inputBuffered = false;

    public bool IsAttacking
    {
        get { return comboIndex >= 0; }
    }

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // Can only attack with the sword drawn
        if (!animator.GetBool(swordParameter))
        {
            comboIndex = -1;
            inputBuffered = false;
            return;
        }

        if (Input.GetKeyDown(attackKey))
        {
            if (comboIndex == -1)
            {
                StartAttack(0);
            }
            else
            {
                inputBuffered = true; // remember the click for the next hit
            }
        }

        if (comboIndex >= 0)
        {
            UpdateAttack();
        }
    }

    void StartAttack(int index)
    {
        comboIndex = index;
        inputBuffered = false;
        animator.CrossFade(attackStates[comboIndex], fadeTime);
    }

    void UpdateAttack()
    {
        // Wait until the attack state is actually playing
        if (animator.IsInTransition(0))
            return;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if (!info.IsName(attackStates[comboIndex]))
            return;

        float progress = info.normalizedTime;

        // Chain into the next attack once the window is open
        if (inputBuffered && progress >= comboOpen)
        {
            if (comboIndex < attackStates.Length - 1)
            {
                StartAttack(comboIndex + 1);
                return;
            }

            inputBuffered = false; // already on the last hit
        }

        // Attack finished, go back to idle
        if (progress >= 1f)
        {
            EndCombo();
        }
    }

    void EndCombo()
    {
        comboIndex = -1;
        inputBuffered = false;
        animator.CrossFade(idleState, fadeTime);
    }

    // Call this from a dodge, hit reaction, etc. to cancel mid-swing
    public void CancelAttack()
    {
        comboIndex = -1;
        inputBuffered = false;
    }
}