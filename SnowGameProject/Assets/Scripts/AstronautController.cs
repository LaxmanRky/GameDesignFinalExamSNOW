using UnityEngine;
using System.Collections;

public class AstronautController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float horizontalBoundary = 8f; // Limit for how far the character can move left/right

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float leapForce = 5f;
    public float jumpDuration = 0.5f;
    public float gravity = 15f;
    public float horizontalMultiplier = 2.5f; // Multiplier for horizontal distance during leaps
    public LayerMask groundLayer;
    public LayerMask boundaryLayer; // Layer for left and right boundaries
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Animation Settings")]
    public float bobAmount = 0.1f; // How much the character bobs up and down
    public float bobSpeed = 5f; // How fast the character bobs
    
    [Header("Debug")]
    public bool showDebugRays = true;
    
    private Vector3 startPosition;
    private bool facingRight = true;
    private float horizontalInput;
    private float verticalInput;
    private bool isJumping = false;
    private bool isGrounded = true;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isTouching = false;
    private float jumpStartY;
    
    // Variables for key press/release detection
    private bool jumpKeyWasPressed = false;
    private bool canJump = true;
    
    // Variables for collision detection
    private bool hitLeftBoundary = false;
    private bool hitRightBoundary = false;
    
    // References to components
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private GameManager gameManager;
    
    void Start()
    {
        startPosition = transform.position;
        jumpStartY = transform.position.y;
        
        // Tag the player
        gameObject.tag = "Player";
        
        // Find the GameManager
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogWarning("GameManager not found! Some features may not work correctly.");
            }
        }
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = transform;
            checkObj.transform.localPosition = new Vector3(0, -0.5f, 0); // Position at the bottom of the character
            groundCheck = checkObj.transform;
        }
        
        // Get or add required components
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Configure rigidbody
        rb.gravityScale = 0; // We're handling gravity ourselves
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Prevent tunneling through colliders
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smoother movement
        
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 1.8f); // Adjust size to match your sprite
        }
    }
    
    void Update()
    {
        // Check if game is paused (timeScale == 0)
        if (Time.timeScale == 0f)
        {
            // Game is paused, don't process input
            return;
        }
        
        // Check if grounded
        CheckGrounded();
        
        // Get keyboard input
        horizontalInput = Input.GetAxis("Horizontal");
        
        // Get vertical input with key press/release detection
        bool jumpKeyIsPressed = Input.GetAxis("Vertical") > 0.1f;
        
        // Handle touch input for mobile
        HandleTouchInput();
        
        // Handle jumping with key press/release detection
        if ((jumpKeyIsPressed && !jumpKeyWasPressed) || (isTouching && IsSwipeUp()))
        {
            if (isGrounded && !isJumping && canJump)
            {
                if (horizontalInput != 0)
                {
                    // Diagonal leap (jump + horizontal movement)
                    StartCoroutine(PerformLeap());
                }
                else
                {
                    // Vertical jump
                    StartCoroutine(PerformJump());
                }
                
                // Prevent multiple jumps from holding the key
                canJump = false;
            }
        }
        
        // Reset canJump when key is released
        if (!jumpKeyIsPressed && jumpKeyWasPressed)
        {
            canJump = true;
        }
        
        // Update previous key state
        jumpKeyWasPressed = jumpKeyIsPressed;
    }
    
    void FixedUpdate()
    {
        // Check if game is paused (timeScale == 0)
        if (Time.timeScale == 0f)
        {
            // Game is paused, don't process movement
            rb.linearVelocity = Vector2.zero; // Stop any movement
            return;
        }
        
        // Only move horizontally if not jumping
        if (!isJumping)
        {
            // Check for collisions in the direction of movement
            CheckForBoundaryCollisions();
            
            // Don't move if hitting a boundary and trying to move further into it
            if ((hitLeftBoundary && horizontalInput < 0) || (hitRightBoundary && horizontalInput > 0))
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }
            
            // Calculate desired velocity
            Vector2 targetVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            
            // Apply movement
            rb.linearVelocity = targetVelocity;
            
            // Handle direction
            if (horizontalInput > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontalInput < 0 && facingRight)
            {
                Flip();
            }
            
            // Apply bobbing animation when moving on ground
            if (Mathf.Abs(horizontalInput) > 0.1f && isGrounded)
            {
                float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
                transform.position = new Vector3(transform.position.x, startPosition.y + bobOffset, transform.position.z);
            }
            else if (isGrounded)
            {
                // Return to original position when not moving and on ground
                transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
            }
        }
    }
    
    void HandleTouchInput()
    {
        // Check if game is paused (timeScale == 0)
        if (Time.timeScale == 0f)
        {
            // Game is paused, don't process touch input
            return;
        }
        
        // Handle touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                isTouching = false;
            }
            
            // Determine horizontal input from touch position
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // If touch is on the left side of the screen, move left
                if (touch.position.x < Screen.width / 2)
                {
                    horizontalInput = -1;
                }
                // If touch is on the right side of the screen, move right
                else
                {
                    horizontalInput = 1;
                }
            }
        }
        else
        {
            isTouching = false;
        }
    }
    
    bool IsSwipeUp()
    {
        if (touchEndPos.y - touchStartPos.y > Screen.height / 4)
        {
            return true;
        }
        return false;
    }
    
    void CheckGrounded()
    {
        // Check if the character is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // If we're grounded, update the start position for bobbing
        if (isGrounded && !isJumping)
        {
            startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
    
    void CheckForBoundaryCollisions()
    {
        // Reset boundary flags
        hitLeftBoundary = false;
        hitRightBoundary = false;
        
        // Get the bounds of the collider
        Bounds bounds = boxCollider.bounds;
        
        // Check for left boundary using direct collision detection (doesn't rely on layers)
        Vector2 leftOrigin = new Vector2(bounds.min.x - 0.05f, bounds.center.y);
        Collider2D[] leftColliders = Physics2D.OverlapPointAll(leftOrigin);
        foreach (Collider2D col in leftColliders)
        {
            if (col.gameObject != gameObject) // Ignore self-collision
            {
                hitLeftBoundary = true;
                Debug.Log($"Left boundary detected: {col.gameObject.name}");
                break;
            }
        }
        
        // Check for right boundary using direct collision detection (doesn't rely on layers)
        Vector2 rightOrigin = new Vector2(bounds.max.x + 0.05f, bounds.center.y);
        Collider2D[] rightColliders = Physics2D.OverlapPointAll(rightOrigin);
        foreach (Collider2D col in rightColliders)
        {
            if (col.gameObject != gameObject) // Ignore self-collision
            {
                hitRightBoundary = true;
                Debug.Log($"Right boundary detected: {col.gameObject.name}");
                break;
            }
        }
        
        // Also check using raycasts (both layer-based and all layers)
        RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(bounds.min.x, bounds.center.y), Vector2.left, 0.2f);
        RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(bounds.max.x, bounds.center.y), Vector2.right, 0.2f);
        
        if (leftHit.collider != null && leftHit.collider.gameObject != gameObject)
        {
            hitLeftBoundary = true;
            Debug.Log($"Left boundary detected by raycast: {leftHit.collider.gameObject.name}");
        }
        
        if (rightHit.collider != null && rightHit.collider.gameObject != gameObject)
        {
            hitRightBoundary = true;
            Debug.Log($"Right boundary detected by raycast: {rightHit.collider.gameObject.name}");
        }
        
        // Draw debug rays
        if (showDebugRays)
        {
            Debug.DrawRay(new Vector2(bounds.min.x, bounds.center.y), Vector2.left * 0.2f, leftHit.collider != null ? Color.red : Color.green);
            Debug.DrawRay(new Vector2(bounds.max.x, bounds.center.y), Vector2.right * 0.2f, rightHit.collider != null ? Color.red : Color.green);
            
            // Draw points for overlap checks
            Debug.DrawLine(leftOrigin + new Vector2(-0.1f, 0), leftOrigin + new Vector2(0.1f, 0), hitLeftBoundary ? Color.red : Color.green);
            Debug.DrawLine(leftOrigin + new Vector2(0, -0.1f), leftOrigin + new Vector2(0, 0.1f), hitLeftBoundary ? Color.red : Color.green);
            
            Debug.DrawLine(rightOrigin + new Vector2(-0.1f, 0), rightOrigin + new Vector2(0.1f, 0), hitRightBoundary ? Color.red : Color.green);
            Debug.DrawLine(rightOrigin + new Vector2(0, -0.1f), rightOrigin + new Vector2(0, 0.1f), hitRightBoundary ? Color.red : Color.green);
        }
    }
    
    // Called when this collider enters another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with: {collision.gameObject.name}, Layer: {collision.gameObject.layer}");
        
        // Check if we hit a boundary (using layer)
        if (((1 << collision.gameObject.layer) & boundaryLayer) != 0)
        {
            // Determine if it's a left or right boundary based on the contact point
            Vector2 contactPoint = collision.GetContact(0).point;
            
            if (contactPoint.x < transform.position.x)
            {
                hitLeftBoundary = true;
                Debug.Log("Hit left boundary - COLLISION");
                
                // Immediately stop horizontal movement
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                
                // Push the player slightly away from the boundary
                transform.position = new Vector3(transform.position.x + 0.05f, transform.position.y, transform.position.z);
            }
            else
            {
                hitRightBoundary = true;
                Debug.Log("Hit right boundary - COLLISION");
                
                // Immediately stop horizontal movement
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                
                // Push the player slightly away from the boundary
                transform.position = new Vector3(transform.position.x - 0.05f, transform.position.y, transform.position.z);
            }
        }
        // Also check by name as a fallback
        else if (collision.gameObject.name.Contains("Boundary") || 
                 collision.gameObject.name.Contains("Wall") || 
                 collision.gameObject.name.Contains("Border") ||
                 collision.gameObject.name.Contains("Collider"))
        {
            // Determine if it's a left or right boundary based on the contact point
            Vector2 contactPoint = collision.GetContact(0).point;
            
            if (contactPoint.x < transform.position.x)
            {
                hitLeftBoundary = true;
                Debug.Log($"Hit left boundary by name: {collision.gameObject.name}");
                
                // Immediately stop horizontal movement
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                
                // Push the player slightly away from the boundary
                transform.position = new Vector3(transform.position.x + 0.05f, transform.position.y, transform.position.z);
            }
            else
            {
                hitRightBoundary = true;
                Debug.Log($"Hit right boundary by name: {collision.gameObject.name}");
                
                // Immediately stop horizontal movement
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                
                // Push the player slightly away from the boundary
                transform.position = new Vector3(transform.position.x - 0.05f, transform.position.y, transform.position.z);
            }
        }
        
        // Check if we hit the ground
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
            Debug.Log("Hit ground");
        }
    }
    
    // Called when this collider stays in contact with another collider
    void OnCollisionStay2D(Collision2D collision)
    {
        // Check if we're still in contact with a boundary
        if (((1 << collision.gameObject.layer) & boundaryLayer) != 0)
        {
            // Determine if it's a left or right boundary based on the contact point
            Vector2 contactPoint = collision.GetContact(0).point;
            
            if (contactPoint.x < transform.position.x)
            {
                hitLeftBoundary = true;
                
                // Stop any leftward movement
                if (rb.linearVelocity.x < 0)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
            else
            {
                hitRightBoundary = true;
                
                // Stop any rightward movement
                if (rb.linearVelocity.x > 0)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
        }
    }
    
    // Called when this collider exits another collider
    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if we left a boundary
        if (((1 << collision.gameObject.layer) & boundaryLayer) != 0)
        {
            // Determine if it's a left or right boundary based on the contact point
            Vector2 contactPoint = collision.GetContact(0).point;
            if (contactPoint.x < transform.position.x)
            {
                hitLeftBoundary = false;
                Debug.Log("Left left boundary");
            }
            else
            {
                hitRightBoundary = false;
                Debug.Log("Left right boundary");
            }
        }
        
        // Check if we left the ground
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
            Debug.Log("Left ground");
        }
    }
    
    IEnumerator PerformJump()
    {
        isJumping = true;
        jumpStartY = transform.position.y;
        float jumpTime = 0;
        
        while (jumpTime < jumpDuration)
        {
            // Calculate jump height using a parabolic curve
            float jumpProgress = jumpTime / jumpDuration;
            float heightMultiplier = 4 * jumpProgress * (1 - jumpProgress); // Parabolic curve peaking at 0.5
            float newY = jumpStartY + jumpForce * heightMultiplier;
            
            // Apply the new position
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            
            jumpTime += Time.deltaTime;
            yield return null;
        }
        
        // Apply gravity to come back down
        float fallTime = 0;
        float maxFallTime = jumpDuration * 0.75f; // Fall a bit faster than the jump
        
        while (transform.position.y > startPosition.y && fallTime < maxFallTime && !isGrounded)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y - gravity * fallTime * Time.deltaTime,
                transform.position.z
            );
            
            fallTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we land exactly at the start position
        if (!isGrounded)
        {
            transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
        }
        
        isJumping = false;
        
        // Add a small delay before allowing another jump
        yield return new WaitForSeconds(0.1f);
    }
    
    IEnumerator PerformLeap()
    {
        isJumping = true;
        jumpStartY = transform.position.y;
        float jumpTime = 0;
        float direction = Mathf.Sign(horizontalInput);
        
        while (jumpTime < jumpDuration)
        {
            // Calculate jump height using a parabolic curve
            float jumpProgress = jumpTime / jumpDuration;
            float heightMultiplier = 4 * jumpProgress * (1 - jumpProgress); // Parabolic curve peaking at 0.5
            float newY = jumpStartY + leapForce * heightMultiplier;
            
            // Calculate horizontal movement
            float horizontalDistance = direction * leapForce * horizontalMultiplier * Time.deltaTime;
            
            // Check for boundary collisions before moving horizontally
            bool wouldHitBoundary = false;
            
            if (direction < 0)
            {
                // Check left boundary
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Abs(horizontalDistance) + 0.1f, boundaryLayer);
                if (hit.collider != null)
                {
                    wouldHitBoundary = true;
                    hitLeftBoundary = true;
                }
            }
            else
            {
                // Check right boundary
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Abs(horizontalDistance) + 0.1f, boundaryLayer);
                if (hit.collider != null)
                {
                    wouldHitBoundary = true;
                    hitRightBoundary = true;
                }
            }
            
            // Check if we hit a boundary
            if ((hitLeftBoundary && direction < 0) || (hitRightBoundary && direction > 0) || wouldHitBoundary)
            {
                horizontalDistance = 0;
            }
            
            // Apply the new position with both vertical and horizontal movement
            transform.position = new Vector3(
                transform.position.x + horizontalDistance,
                newY,
                transform.position.z
            );
            
            jumpTime += Time.deltaTime;
            yield return null;
        }
        
        // Apply gravity to come back down
        float fallTime = 0;
        float maxFallTime = jumpDuration * 0.75f; // Fall a bit faster than the jump
        
        while (transform.position.y > startPosition.y && fallTime < maxFallTime && !isGrounded)
        {
            // Continue horizontal movement during fall
            float horizontalDistance = direction * leapForce * horizontalMultiplier * Time.deltaTime;
            
            // Check for boundary collisions before moving horizontally
            bool wouldHitBoundary = false;
            
            if (direction < 0)
            {
                // Check left boundary
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Abs(horizontalDistance) + 0.1f, boundaryLayer);
                if (hit.collider != null)
                {
                    wouldHitBoundary = true;
                    hitLeftBoundary = true;
                }
            }
            else
            {
                // Check right boundary
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Abs(horizontalDistance) + 0.1f, boundaryLayer);
                if (hit.collider != null)
                {
                    wouldHitBoundary = true;
                    hitRightBoundary = true;
                }
            }
            
            // Check if we hit a boundary
            if ((hitLeftBoundary && direction < 0) || (hitRightBoundary && direction > 0) || wouldHitBoundary)
            {
                horizontalDistance = 0;
            }
            
            transform.position = new Vector3(
                transform.position.x + horizontalDistance,
                transform.position.y - gravity * fallTime * Time.deltaTime,
                transform.position.z
            );
            
            fallTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we land exactly at the start position (Y only)
        if (!isGrounded)
        {
            transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
        }
        
        isJumping = false;
        
        // Add a small delay before allowing another jump
        yield return new WaitForSeconds(0.1f);
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
