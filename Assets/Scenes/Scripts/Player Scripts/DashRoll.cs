using UnityEngine;

public class DashRoll : MonoBehaviour
{
    [Header("Input")]
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Movement")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.4f;
    public float cooldown = 1f;

    [Header("I-Frames")]
    public float iFrameStart = 0.05f;
    public float iFrameDuration = 0.25f;

    [Header("Animation")]
    public Animator animator;
    public string rollForwardState = "RollForward";
    public string rollBackState = "RollBack";
    public string rollLeftState = "RollLeft";
    public string rollRightState = "RollRight";
    public float fadeTime = 0.05f;

    [Header("Return State")]
    public string idleState = "Player Idle";
    public string swordIdleState = "Sword Idle";
    public string hasSwordParameter = "HasSword";

    [Header("References")]
    public CharacterController controller;
    public IsometricCamera cameraController;
    public Transform cameraTransform;
    public SwordAttack swordAttack;     // same one PlayerMovement uses

    // for the I frame Stuff
    public bool IsDashing { get; private set; }
    public bool IsInvincible { get; private set; }

    // how long till next dash
    private Vector3 dashDirection;
    private float dashTimer = 0f;
    private float iFrameTimer = 0f;
    private float nextDashTime = 0f;

    void Start()
    {



        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(dashKey) && !IsDashing && Time.time >= nextDashTime && CanDash())
        {
            StartDash();
        }

        if (IsDashing)
        {
            UpdateDash();
        }
    }

    bool CanDash()
    {
        bool lockedOn = cameraController != null && cameraController.isLocked;

        if (!lockedOn)
            return true; // no restriction when not locked on

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        return new Vector3(horizontal, 0f, vertical).sqrMagnitude > 0.01f; // must hold a direction to dash while locked on
    }

    #region Dash
    void StartDash()
    {
        // making sure the attack gets canceled to not cause problems

        if (swordAttack != null)
        {
            swordAttack.CancelAttack();
        }

        

        // these are all there to know what direction, if im locked on and the main dashing that does the dash

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 worldMoveDir = cameraForward * vertical + cameraRight * horizontal;

        bool lockedOn = cameraController != null && cameraController.isLocked;

        if (worldMoveDir.sqrMagnitude > 0.01f)
        {
            dashDirection = worldMoveDir.normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        if (lockedOn)
        {
            // Convert world direction into local space relative to the player's current facing,
            // so the roll picks forward/back/left/right correctly while staying locked on the target
            Vector3 localMove = transform.InverseTransformDirection(worldMoveDir);
            PlayDirectionalRoll(localMove);
        }
        else
        {
            // No lock-on, so just turn to face the roll direction
            transform.rotation = Quaternion.LookRotation(dashDirection);

            if (animator != null)
            {
                animator.CrossFade(rollForwardState, fadeTime);
            }
        }

        IsDashing = true;
        dashTimer = 0f;
        iFrameTimer = 0f;
        IsInvincible = false;

        nextDashTime = Time.time + cooldown;
    }
    #endregion

    void PlayDirectionalRoll(Vector3 moveInput)
    {

        // this controlls the animator in the player for the rolling and call the nessary Animation direction with crossfade
        if (animator == null)
            return;

        // No input means dash forward by default
        if (moveInput.sqrMagnitude < 0.01f)
        {
            animator.CrossFade(rollForwardState, fadeTime);
            return;
        }

        // Compare forward/back vs left/right to decide which clip fits best
        if (Mathf.Abs(moveInput.z) >= Mathf.Abs(moveInput.x))
        {
            animator.CrossFade(moveInput.z >= 0 ? rollForwardState : rollBackState, fadeTime);
        }
        else
        {
            animator.CrossFade(moveInput.x >= 0 ? rollRightState : rollLeftState, fadeTime);
        }
    }

    void UpdateDash()
    {

        // this section is to have I Frames during a sec peroid of time and the code below is touse in other code when i take damageand if i take damage


        /*
        public DashRoll dashRoll;

        void TakeDamage(int amount)
        {
            if (dashRoll != null && dashRoll.IsInvincible)
            {
               return; // dodged it
            }

            // your existing damage code
        }*/

        dashTimer += Time.deltaTime;

        if (dashTimer <= dashDuration && controller != null)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
        }

        iFrameTimer += Time.deltaTime;

        if (!IsInvincible && iFrameTimer >= iFrameStart)
        {
            IsInvincible = true;
        }

        if (IsInvincible && iFrameTimer >= iFrameStart + iFrameDuration)
        {
            IsInvincible = false;
        }

        if (dashTimer >= dashDuration)
        {
            IsDashing = false;

            if (animator != null)
            {
                bool hasSword = animator.GetBool(hasSwordParameter);
                animator.CrossFade(hasSword ? swordIdleState : idleState, fadeTime);
            }
        }
    }
}