using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPhisic : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float jumpForce = 7f;
    private bool isGrounded;

    private bool isJumping;
    private int moveX;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool facingRight = false;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void DebugCurrentAnimationState()
{
    // Obtenemos la información del estado actual de la capa 0 (la capa base).
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    // Comprobamos el nombre de los estados que tengas en tu Animator Controller.
    // Reemplaza "Idle", "Run", "Jump" con los nombres EXACTOS de tus animaciones.
    if (stateInfo.IsName("mario_idle"))
    {
        Debug.Log("Estado actual: Idle");
    }
    else if (stateInfo.IsName("Entry"))
    {
        Debug.Log("Estado actual: entry");
    }
    else if (stateInfo.IsName("mario_walk"))
    {
        Debug.Log("Estado actual: walk");
    }

    else if (stateInfo.IsName("mario_jump"))
    {
        Debug.Log("Estado actual: jump");
    }
    // Puedes añadir más 'else if' para otros estados.
}

    void FixedUpdate()
    {
        float moveX = 0f;

        // if (Input.GetKey(KeyCode.A))
        //     moveX = -1f;
        // else if (Input.GetKey(KeyCode.D))
        //     moveX = 1f;
        
        moveX = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

         float absMoveX = Mathf.Abs(moveX);

        int speedInt = Mathf.RoundToInt(absMoveX);
        animator.SetInteger("SpeedX", Mathf.Abs(speedInt));


    }
    

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        animator.SetBool("isJumping", isGrounded);
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded )
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Detectar flip visual
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX > 0 && !facingRight)
            Flip();
        else if (moveX < 0 && facingRight)
            Flip();

        DebugCurrentAnimationState(); 
        
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;  // Invierte la escala en X
        transform.localScale = scale;
    }
}
