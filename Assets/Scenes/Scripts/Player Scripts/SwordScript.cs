using UnityEngine;

public class SwordToggle : MonoBehaviour
{
    [Header("Input")]
    public KeyCode toggleKey = KeyCode.E;

    [Header("Animation")]
    public Animator animator;
    public string swordParameter = "HasSword";

    [Header("Timing")]
    public float toggleCooldown = 0.5f; // stops E spam from interrupting the draw/put away animation

    private bool swordDrawn = false;
    private float nextToggleTime = 0f;

    public SwordAttack swordAttack;

    void Start()
    {
        // If you didn't drag an Animator in, look on this object or its children
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && Time.time >= nextToggleTime && !swordAttack.IsAttacking)
        {
            swordDrawn = !swordDrawn;
            animator.SetBool(swordParameter, swordDrawn);
            nextToggleTime = Time.time + toggleCooldown;
        }
    }
}