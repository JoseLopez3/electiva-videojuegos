using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPhisic : MonoBehaviour
{
    private float moveSpeed = 14f;
    public int maxHealth;
    private int currentHealth;
    private bool gameOver = false;
    private int moveX;
    private float originalFireRate;
    private Coroutine attackSpeedCoroutine;

    private bool facingRight = false;
    [SerializeField] private Animator animator;

    public static MovementPhisic instance;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f; 
    [SerializeField] private int bulletDamage = 25;
    private float nextFireTime = 0f;
    public bool canMove = true; 

    private void Awake(){
        instance = this;
    }

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        GameEvents.PlayerHealthChanged(currentHealth, maxHealth);
        originalFireRate = fireRate;
    }



    private void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    public void DealDamage(int damage){

        if (gameOver)
        {
            return; 
        }


        currentHealth -= damage;
        GameEvents.PlayerHealthChanged(currentHealth, maxHealth);
        GameEvents.PlayerTookDamage(); 
        if(currentHealth <= 0){
            Debug.Log("Fin del juego");

            gameOver = true; 
            animator.SetBool("LeblancDeath", true);


            // Se desactiva el control para que no haya interferencias.
            this.enabled = false; 

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;


            float deactivetObjectDuration = 1.7f;

            Invoke("DeactivateObject",deactivetObjectDuration);
            GameEvents.GameOver(false);
        }
    }

 

     void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero; 
            animator.SetFloat("Speed", 0); 
            return; 
        }
        // Input de movimiento de ambos ejes
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical"); 

        //Se normaliza para que no se mueva mas rapido en diagonal.
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        rb.velocity = moveDirection * moveSpeed;

     
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }
    

     void Update()
    {

        if (!canMove) return;

        //Obtiene la posicion del mouse en el mundo del juego
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; 

        // Calcula la direccion desde el firePoint hasta el mouse
        Vector2 aimDirection = (mousePosition - firePoint.position).normalized;

        // Calcular el angulo para rotar el firePoint
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        firePoint.eulerAngles = new Vector3(0, 0, angle);

        // Detectar flip visual
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX > 0 && !facingRight)
            Flip();
        else if (moveX < 0 && facingRight)
            Flip();


        //Logica de disparo
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot(aimDirection); 
        }
    }

    void Shoot(Vector2 direction) 
    {
        GameObject bulletObject = ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, firePoint.rotation);
        if (bulletObject != null)
        {
            Bullet bulletScript = bulletObject.GetComponent<Bullet>();
            // Usamos la dirección que le pasamos al método
            bulletScript.SetDirection(direction);
            // Le decimos a la bala cuánto daño hacer
            bulletScript.damageAmount = bulletDamage;
            bulletScript.SetDirection(direction);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale;
    }

    void Shoot()
    {
        // Se pide una bala al pooler y se guarda una referencia a ella
        GameObject bulletObject = ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, firePoint.rotation);

        if (bulletObject != null)
        {
            Bullet bulletScript = bulletObject.GetComponent<Bullet>();

            // Se decide la direccion del disparo dependiendo de donde mire el jugador
            Vector2 shootDirection = facingRight ? Vector2.right : Vector2.left;

            bulletScript.SetDirection(shootDirection);
        }
    }



public void Heal(int amount)
{
    currentHealth += amount;
    if (currentHealth > maxHealth)
    {
        currentHealth = maxHealth;
    }
    Debug.Log("Jugador curado! Vida actual: " + currentHealth);
    GameEvents.PlayerHealthChanged(currentHealth, maxHealth);
}

public void ApplyAttackSpeedBuff(float multiplier, float duration)
{
    if (attackSpeedCoroutine != null)
    {
        StopCoroutine(attackSpeedCoroutine);
    }
    attackSpeedCoroutine = StartCoroutine(AttackSpeedBuffCoroutine(multiplier, duration));
}

private IEnumerator AttackSpeedBuffCoroutine(float multiplier, float duration)
{
    Debug.Log("Buff de velocidad de ataque activado!");
    fireRate *= multiplier; // Aumentamos la velocidad de ataque

    yield return new WaitForSeconds(duration);

    Debug.Log("Buff de velocidad de ataque terminado.");
    fireRate = originalFireRate; // Restauramos la velocidad de ataque original
    attackSpeedCoroutine = null;
}

}
