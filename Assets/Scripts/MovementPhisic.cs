using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPhisic : MonoBehaviour
{
    private float moveSpeed = 14f;
    private float jumpForce = 12f;
    private bool isGrounded;
    public int maxHealth;
    public int totalDamage;
    private int currentHealth;
    public int maxScore;
    private int currentScore;
    private bool gameOver = false;
    private int moveX;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool facingRight = false;
    [SerializeField] private Animator animator;

    public static MovementPhisic instance;

    private void Awake(){
        instance = this;
    }

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

    }



    private void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    public void DealDamage(){

        if (gameOver)
        {
            return; // Sale de la función inmediatamente, para prevenir a anystate en un bucle infinito.
        }


        currentHealth = currentHealth - totalDamage;
        
        if(currentHealth <= 0){
            Debug.Log("Game over");

            gameOver = true; 
            animator.SetBool("LeblancDeath", true);


            // Desactivamos el control para que no haya interferencias.
            this.enabled = false; 

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;


            float deactivetObjectDuration = 1.7f;

            Invoke("DeactivateObject",deactivetObjectDuration);

        }
    }

    public void AddScore(){

        

        currentScore++;
        
        if(currentScore >= maxScore){
            animator.SetBool("LeblancWin", true);
            // Desactivamos el control para que no haya interferencias.
            this.enabled = false; 

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            float deactivetObjectDuration = 1f;

            Debug.Log("You Win");
            Invoke("DeactivateObject",deactivetObjectDuration);
        }
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

        
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;  // Invierte la escala en X
        transform.localScale = scale;
    }
}
