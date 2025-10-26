using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 650f;
    public int damageAmount; 
    private Rigidbody2D rb;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 velocityVector)
    {
        rb.velocity = velocityVector;
    }

    public void SetDirection(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
    }
    void OnEnable()
    {
        Invoke("Deactivate", lifeTime);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bala impactada con " + collision.gameObject.name);

        // Se obtiene el script del enemigo del objeto con el que chocamos y posteriormente del jugador.
        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Se le asigna el daño recibido
            enemy.DealDamage(damageAmount);
        }

        MovementPhisic player = collision.GetComponent<MovementPhisic>();
        if (player != null)
        {
            player.DealDamage(damageAmount);
        }

        if (enemy != null || player != null)
        {
            CancelInvoke();
            Deactivate();
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}