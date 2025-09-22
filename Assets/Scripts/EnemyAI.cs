using UnityEngine;
using UnityEngine.UI; 

public class EnemyAI : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;


    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 3f;
    private int currentPointIndex = 0;
    private bool facingRight = true; 

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [Header("Shooting")]
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public Transform firePoint; // El punto de origen del disparo
    public string enemyBulletTag = "EnemyBullet";
    private float nextFireTime = 0f;
    public int bulletDamage = 10;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Se registra en el GameManager y actualiza su barra de vida inicial
        GameManager.Instance.RegisterEnemy(this);
        
        UpdateHealthBar();
        
        // Direccion del enemigo
        if (patrolPoints.Length > 0 && patrolPoints[0].position.x < transform.position.x)
        {
            Flip(); 
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootingRange)
        {
            // Si el jugador está en rango, deja de patrullar, mira al jugador y dispara.
            AimAndShoot();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        
        // Moverse hacia el punto de patrulla
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);


        if (targetPoint.position.x > transform.position.x && !facingRight)
        {
            Flip(); 
            
        }
        else if (targetPoint.position.x < transform.position.x && facingRight)
        {
            Flip(); 
            
        }

        // Cambiar al siguiente punto de patrulla
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void AimAndShoot()
    
    {

        
        // Lógica de Flip para apuntar al jugador
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }

        // Disparar (si ha pasado el tiempo de recarga)
         if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            
            GameObject bulletObject = ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, firePoint.position, Quaternion.identity);
            if (bulletObject != null)
            {
                Bullet bulletScript = bulletObject.GetComponent<Bullet>();
                // Le decimos a la bala cuánto daño hacer
                bulletScript.damageAmount = bulletDamage; 
                
                Vector2 directionToPlayer = (player.position - firePoint.position).normalized;
                bulletScript.SetDirection(directionToPlayer);
            }
        }
    
    }


    public void DealDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy Health: " + currentHealth);
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El enemigo ha sido derrotado");
        GameManager.Instance.UnregisterEnemy(this);
        gameObject.SetActive(false); 
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

 

    private void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }
}