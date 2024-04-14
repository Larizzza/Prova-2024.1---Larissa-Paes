using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float quadrupedSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float climbForce;

    private enum CharacterState { Idle, Walking, Running, Sliding, QuadrupedRunning, Rolling, Jumping, AttackSide, AttackUp, AttackDown, Crounch, Sneak, ClimbIdle, Climbing, ClimbSlide }
    private CharacterState currentState;
    private bool isGrounded;
    private bool isClimbing;

    void Start()
    {
        currentState = CharacterState.Idle;
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        UpdateWalkingState(horizontalInput);
        UpdateVerticalState(verticalInput);
        UpdateRunningState(horizontalInput);
        UpdateQuadrupedRunningState(horizontalInput);
        UpdateSlidingState(horizontalInput);
        UpdateRollState(horizontalInput);
        UpdateAttack(verticalInput);
        UpdateSneak(horizontalInput);
        UpdateClimb(verticalInput);
        CheckForInputs();

        UpdateAnimation(horizontalInput);
    }

    private void UpdateWalkingState(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            if(isGrounded) SetState(CharacterState.Walking);
            rb.position += new Vector2(horizontalInput * moveSpeed * Time.fixedDeltaTime, 0);
        }
    }

    private void UpdateRunningState(float horizontalInput)
    {  
        if(!isGrounded) return; 

        if (horizontalInput == 0) return;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            SetState(CharacterState.Running);
            rb.position += new Vector2(horizontalInput * runSpeed * Time.fixedDeltaTime, 0);
        }
    }

    private void UpdateRollState(float horizontalInput)
    {
        if(!isGrounded) return; 
        if (horizontalInput == 0) return;

        if (Input.GetKey(KeyCode.R))
        {
            SetState(CharacterState.Rolling);
            rb.position += new Vector2(horizontalInput * rollSpeed * Time.fixedDeltaTime, 0);
        }
    }

    private void UpdateSlidingState(float horizontalInput)
    {
        if(!isGrounded) return; 
        if (horizontalInput == 0) return;

        if (Input.GetKey(KeyCode.S))
        {
            SetState(CharacterState.Sliding);
            rb.position += new Vector2(horizontalInput * slideSpeed * Time.fixedDeltaTime, 0);
        }
    }

    private void UpdateVerticalState(float verticalInput)
    {
        if (verticalInput > 0)
        {
            if (isGrounded && !isClimbing)
            {
                SetState(CharacterState.Jumping);
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
            else
            {
                rb.position += new Vector2(0, verticalInput * climbForce * Time.fixedDeltaTime);
            }
        }
        else
        {
            if(isClimbing)
            {
                rb.position += new Vector2(0, verticalInput * climbForce * Time.fixedDeltaTime);
            }
        }

    }

    void UpdateClimb(float verticalInput)
    {
        if (isClimbing)
        {            
            if (verticalInput == 0)
            {
                currentState = CharacterState.ClimbIdle;
            }
            if (verticalInput > 0)
            {
                currentState = CharacterState.Climbing;
            }

            if (Input.GetKey(KeyCode.LeftShift) && verticalInput < 0 || Input.GetKey(KeyCode.RightShift) && verticalInput < 0)
            {
                currentState = CharacterState.ClimbSlide;
            }
        }
    }

    private void UpdateQuadrupedRunningState(float horizontalInput)
    {
        if(!isGrounded) return; 

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            SetState(CharacterState.QuadrupedRunning);
            rb.position += new Vector2(horizontalInput * quadrupedSpeed * Time.fixedDeltaTime, 0);
        }
    }

    private void CheckForInputs()
    {
        if (!Input.anyKey && currentState != CharacterState.Jumping)
        {
            currentState = CharacterState.Idle;
        }
    }

    private void SetState(CharacterState characterState)
    {
        currentState = characterState;
    }

    private void UpdateAttack(float verticalInput)
    {
        if (Input.GetKey(KeyCode.J)) //ataque lateral
        {
            currentState = CharacterState.AttackSide;
        }

        if (Input.GetKey(KeyCode.J) && verticalInput > 0) //ataque Cima
        {
            currentState =  CharacterState.AttackUp;
        }

         if (Input.GetKey(KeyCode.J) && verticalInput < 0) //ataque Baixo
        {
            currentState =  CharacterState.AttackDown;
        }
    }

    private void UpdateSneak(float horizontalInput)
    {
        if (Input.GetKey(KeyCode.K))
        {
            currentState = CharacterState.Crounch;
        }

         if (Input.GetKey(KeyCode.K) && horizontalInput > 0)
        {
            currentState = CharacterState.Sneak;
        }

         if (Input.GetKey(KeyCode.K) && horizontalInput < 0)
        {
            currentState = CharacterState.Sneak;
        }
    }
    
    void UpdateAnimation(float horizontalInput)
    {
        if (horizontalInput < 0) transform.rotation = Quaternion.Euler(new Vector3(0f, 0, 0f));
        else if (horizontalInput > 0) transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        switch (currentState)
        {
            case CharacterState.Idle:
                animator.Play("Idle");
                break;
            case CharacterState.Walking:
                animator.Play("Walking");
                break;
            case CharacterState.Running:
                animator.Play("Running");
                break;
            case CharacterState.Sliding:
                animator.Play("Slide on floor");
                break;
            case CharacterState.QuadrupedRunning:
                animator.Play("Running On 4");
                break;
            case CharacterState.Rolling:
                animator.Play("Roll");
                break;
            case CharacterState.Jumping:
                animator.Play("Jump");
                break;
            case CharacterState.AttackSide:
                animator.Play("Attack Side");
                break;
            case CharacterState.AttackUp:
                animator.Play("Attack Up");
                break;
            case CharacterState.AttackDown:
                animator.Play("Attack Down");
                break;
            case CharacterState.Crounch:
                animator.Play("Crounch Idle");
                break;
            case CharacterState.Sneak:
                animator.Play("Sneak");
                break;
            case CharacterState.ClimbIdle:
                animator.Play("Climb Idle");
                break;
            case CharacterState.Climbing:
                animator.Play("Climb");
                break;
            case CharacterState.ClimbSlide:
                animator.Play("Wall Slider");
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            currentState = CharacterState.Idle;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            isClimbing = true;
            isGrounded = false;
            rb.gravityScale = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            isClimbing = false;
            rb.gravityScale = 10f;
            currentState = CharacterState.Idle;
        }
    }
}