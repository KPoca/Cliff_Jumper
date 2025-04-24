using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 movementInput;
    Rigidbody2D playerRigidbody;
    Animator playerAnimation;
    CapsuleCollider2D playerCollider;
    BoxCollider2D feetCollider;
    
    [SerializeField] float playerGravityScale = 2f;
    [SerializeField] float horizontalMovementSpeed = 2300f;
    [SerializeField] float verticalMovementSpeed = 2300f;
    [SerializeField] float jumpMovementSpeed = 2800f;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform arrowSpawnPoint;
    [SerializeField] float cooldownLength = 1f;

    bool isDeath = false;
    bool onCooldown = false;
    float lastAttackTime = -Mathf.Infinity;
    bool reachedJumpApex = false;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();

        playerRigidbody.gravityScale = playerGravityScale;

    }
    
    void Update()
    {
        if (isDeath) {
            return;
        }
        
        CheckJumpApex();
        Run();
        FlipSprite();
        Climb();

        if (onCooldown && (Time.time >= (lastAttackTime + cooldownLength))){
            onCooldown = false;
            lastAttackTime = Time.time;
        }
    }

    void OnMove(InputValue value)
    {
        if (isDeath) { return; }
        movementInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (isDeath) { return; }

        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            return;
        }
        playerRigidbody.linearVelocity += new Vector2(0f, jumpMovementSpeed);

        reachedJumpApex = false;
    }

    void CheckJumpApex()
    {
        if (playerRigidbody.linearVelocity.y <= 0){
            reachedJumpApex = true;
        }
    }


    void Run()
    {
        if (isDeath) return; // prevent overriding velocity after knockback
        
        Vector2 playerVelocity = new Vector2(movementInput.x * horizontalMovementSpeed, playerRigidbody.linearVelocity.y);
        playerRigidbody.linearVelocity = playerVelocity;

        if (Mathf.Abs(playerRigidbody.linearVelocity.x) > Mathf.Epsilon){
            playerAnimation.SetBool("isRunning", true);
        } else {
            playerAnimation.SetBool("isRunning", false);
        }
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRigidbody.linearVelocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.linearVelocity.x), 1f);
        }
    }

    void Climb()
    {
        bool onLadder = playerCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        bool onGround = playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool hasVerticalInput = Mathf.Abs(movementInput.y) > Mathf.Epsilon;

        if (onLadder && reachedJumpApex){
            playerRigidbody.gravityScale = 0;

            Vector2 verticalVelocity = new Vector2(playerRigidbody.linearVelocity.x, movementInput.y * verticalMovementSpeed);
            playerRigidbody.linearVelocity = verticalVelocity;
        } else {
            playerRigidbody.gravityScale = playerGravityScale;
        }

        bool playClimbAnimation = onLadder && (!onGround || hasVerticalInput);
        playerAnimation.SetBool("isClimbing", playClimbAnimation);
    }

    void OnAttack()
    {
        if (!onCooldown){
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
            Vector3 flippedScale = arrow.transform.localScale;
            flippedScale.x = transform.localScale.x;
            arrow.transform.localScale = flippedScale;
            playerAnimation.SetTrigger("onShoot");
            Invoke("ReturnToIdle", 0.17f); // Adjust time to match shoot animation
            onCooldown = true;
        }
    }

    void ReturnToIdle()
    {
        playerAnimation.SetTrigger("ReturnToIdle");
    }

    public bool GetIsDeath(){ return isDeath; }
    public void SetIsDeath(bool value) { isDeath = value; }
}
