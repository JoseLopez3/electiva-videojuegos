using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Asegúrate de tener DOTween importado

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public List<EnemyAttackPhase> attackPhases;
    public Transform firePoint;
    public string enemyBulletTag = "EnemyBullet";
    public int bulletDamage = 10;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private Slider healthBar;

    [Header("Visuals - NEW")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer del enemigo
    [SerializeField] private Color deathColor = Color.red; // Color al "morir" visualmente
    private Color originalColor;
    public bool isVisuallyDead { get; private set; } = false; // Estado si el enemigo está "muerto visualmente"
    public bool hasDiedCompletely { get; private set; } = false; // Estado si el enemigo ha sido completamente procesado para desactivación.

    private int currentPhaseIndex = -1;
    private BulletPatternSO currentPattern;
    private float nextFireTime = 0f;
    private float currentSpiralAngle = 0f;
    private bool isShooting = false; // Bloqueo para evitar que las corrutinas se solapen

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("EnemyAI: No se encontró SpriteRenderer en el GameObject o no está asignado. Asigna uno en el Inspector.");
            }
        }
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        isVisuallyDead = false;
        hasDiedCompletely = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // Restaurar color original al activarse
        }
        
        // Asegúrate de resetear el estado de los patrones de disparo
        currentPhaseIndex = -1; 
        nextFireTime = 0f; 
        StopAllCoroutines(); // Detiene cualquier corrutina de disparo antigua
        isShooting = false;

        // Resetear la colisión y barra de vida por si se reutiliza del pool
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;
        if (healthBar != null) healthBar.gameObject.SetActive(true);

        GameManager.Instance?.RegisterEnemy(this);
        Debug.Log($"Enemigo {name} activado y registrado. Salud: {currentHealth}");
    }

    void OnDisable()
    {
        GameManager.Instance?.UnregisterEnemy(this);
        DOTween.Kill(spriteRenderer); // Detener animaciones de DOTween
        Debug.Log($"Enemigo {name} desactivado y desregistrado.");
    }

    void Update()
    {
        // La lógica de CheckForPhaseSwitch y disparo AÚN SE EJECUTA
        // incluso si el enemigo está isVisuallyDead, porque ahora debe seguir atacando.
        CheckForPhaseSwitch();

        if (Time.time >= nextFireTime && !isShooting)
        {
            if (currentPattern != null)
            {
                StartCoroutine(ShootPatternCoroutine(currentPattern));
                nextFireTime = Time.time + currentPattern.timeBetweenBursts;
            }
        }
    }
    
    void CheckForPhaseSwitch()
    {
        if (currentPhaseIndex == attackPhases.Count - 1) return;
        if (GameManager.Instance == null) return; 

        float gameTimeLeft = GameManager.Instance.GetCurrentTime();
        int nextPhaseIndexToCheck = currentPhaseIndex + 1;

        if (nextPhaseIndexToCheck < attackPhases.Count && gameTimeLeft <= attackPhases[nextPhaseIndexToCheck].triggerTimeInSeconds)
        {
            SwitchToPhase(nextPhaseIndexToCheck);
        }
    }

    void SwitchToPhase(int phaseIndex)
    {
        if (phaseIndex >= attackPhases.Count) return;
        currentPhaseIndex = phaseIndex;
        currentPattern = attackPhases[phaseIndex].pattern;
        nextFireTime = Time.time;
        currentSpiralAngle = 0;
        StopAllCoroutines(); // Detenemos cualquier patrón anterior
        isShooting = false;
        Debug.Log("Enemigo entrando en fase: " + attackPhases[phaseIndex].phaseName);
    }

    IEnumerator ShootPatternCoroutine(BulletPatternSO pattern)
    {
        isShooting = true;

        switch (pattern.patternType)
        {
            case BulletPatternSO.PatternType.Circle:
                float angleStepCircle = 360f / pattern.numberOfProjectiles;
                for (int i = 0; i < pattern.numberOfProjectiles; i++)
                {
                    float angle = (i * angleStepCircle) + pattern.startAngleOffset;
                    SpawnBullet(angle, pattern.projectileSpeed);
                }
                break;

            case BulletPatternSO.PatternType.Spiral:
                for(int i = 0; i < pattern.numberOfProjectiles; i++)
                {
                    currentSpiralAngle += pattern.angleStep;
                    SpawnBullet(currentSpiralAngle + pattern.startAngleOffset, pattern.projectileSpeed);
                    yield return new WaitForSeconds(pattern.timeBetweenShots);
                }
                break;

            case BulletPatternSO.PatternType.AimedBurst:
                for (int i = 0; i < pattern.burstCount; i++)
                {
                    float burstBaseAngle = i * pattern.angleBetweenBursts;
                    for (int j = 0; j < pattern.numberOfProjectiles; j++)
                    {
                        float projectileAngle;
                        if (pattern.numberOfProjectiles > 1)
                            projectileAngle = burstBaseAngle - (pattern.angleSpread / 2) + (pattern.angleSpread / (pattern.numberOfProjectiles - 1)) * j;
                        else
                            projectileAngle = burstBaseAngle;
                        SpawnBullet(projectileAngle, pattern.projectileSpeed);
                    }
                }
                break;

            case BulletPatternSO.PatternType.SequentialBurst:
                float currentBurstAngle = pattern.startAngleOffset - (pattern.angleBetweenBursts * (pattern.burstCount -1) / 2f);
                for(int i = 0; i < pattern.burstCount; i++)
                {
                    for(int j = 0; j < pattern.numberOfProjectiles; j++)
                    {
                        float projectileAngle;
                        if (pattern.numberOfProjectiles > 1 && pattern.angleSpread > 0)
                            projectileAngle = currentBurstAngle - (pattern.angleSpread / 2) + (pattern.angleSpread / (pattern.numberOfProjectiles - 1)) * j;
                        else
                            projectileAngle = currentBurstAngle;
                        
                        SpawnBullet(projectileAngle, pattern.projectileSpeed);
                        yield return new WaitForSeconds(pattern.timeBetweenProjectilesInBurst); 
                    }
                    currentBurstAngle += pattern.angleBetweenBursts;
                    yield return new WaitForSeconds(pattern.timeBetweenShots);
                }
                break;

            case BulletPatternSO.PatternType.DoubleSpiralFlower:
                float anglePerPetal = 360f / pattern.petals;
                for(int i = 0; i < pattern.numberOfProjectiles; i++)
                {
                    currentSpiralAngle += pattern.angleStep;
                    for(int j = 0; j < pattern.petals; j++)
                    {
                        float petalBaseAngle = j * anglePerPetal;
                        SpawnBullet(petalBaseAngle + currentSpiralAngle, pattern.projectileSpeed);
                        SpawnBullet(petalBaseAngle - currentSpiralAngle, pattern.projectileSpeed);
                    }
                    yield return new WaitForSeconds(pattern.timeBetweenShots);
                }
                break;
        }
        
        isShooting = false;
    }

    void SpawnBullet(float angle, float speed)
    {
        GameObject bulletObject = ObjectPooler.Instance.SpawnFromPool(enemyBulletTag, firePoint.position, Quaternion.identity);
        if (bulletObject != null)
        {
            Bullet bulletScript = bulletObject.GetComponent<Bullet>();
            bulletScript.damageAmount = bulletDamage;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = rotation * Vector2.up;
            Vector2 finalVelocity = direction * speed; 
    
            bulletScript.SetVelocity(finalVelocity);
        }
    }
    
    public void DealDamage(int damage)
    {
        if (hasDiedCompletely) return; // Si ya ha sido desactivado, no recibe más daño

        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0 && !isVisuallyDead) // Si la vida llega a 0 y aún no está visualmente muerto
        {
            DieVisual(); // Llama al nuevo método de muerte visual
        }
    }

    private void DieVisual() // NUEVO MÉTODO PARA LA "MUERTE VISUAL"
    {
        if (isVisuallyDead) return; // Evitar procesar dos veces la muerte visual

        isVisuallyDead = true;
        Debug.Log($"Enemigo {name} ha llegado a 0 HP y está visualmente muerto.");
        GameEvents.EnemyVisuallyDied(this); // Notifica al GameManager

        // Cambiar a color rojo fijo, sin animación de flash si quieres que sea instantáneo y permanente
        if (spriteRenderer != null)
        {
            spriteRenderer.color = deathColor;
        }

        // Opcional: Desactivar la barra de vida cuando está "muerto"
        if (healthBar != null) healthBar.gameObject.SetActive(false);

        // Opcional: Desactivar colisiones para que las balas no lo golpeen más (pero el jugador sí podría atravesarlo)
        // Collider2D collider = GetComponent<Collider2D>();
        // if (collider != null) collider.enabled = false;
        
        // IMPORTANTE: NO detener corrutinas de disparo ni deshabilitar este script aquí.
        // El enemigo DEBE seguir atacando.
    }

    // Método que el GameManager llamará cuando sea el momento de desactivar por completo
    public void DeactivateCompletely()
    {
        if (hasDiedCompletely) return;

        hasDiedCompletely = true;
        Debug.Log($"Enemigo {name} desactivado completamente.");
        gameObject.SetActive(false); // Ahora sí, se desactiva
        // GameEvents.EnemyDeactivatedCompletely(); // Este evento ya no es estrictamente necesario, el GM ya lo sabe por la lista.
    }

    private void UpdateHealthBar()
    {
        if(healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            // Solo muestra la barra de vida si no está visualmente muerto y tiene menos de maxHealth
            healthBar.gameObject.SetActive(!isVisuallyDead && currentHealth < maxHealth);
        }
    }

    // Este método se llama desde el ObjectPooler al sacar un enemigo para reutilizarlo
    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        isVisuallyDead = false;
        hasDiedCompletely = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true); // Asegúrate de reactivar la barra de vida
        }
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true; // Reactivar colisiones
        currentPhaseIndex = -1;
        StopAllCoroutines();
        isShooting = false;
        nextFireTime = 0f;
        currentSpiralAngle = 0f;
    }
}