using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private int currentPhaseIndex = -1;
    private BulletPatternSO currentPattern;
    private float nextFireTime = 0f;
    private float currentSpiralAngle = 0f;
    private bool isShooting = false; // Bloqueo para evitar que las corrutinas se solapen

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        // SwitchToPhase(0); 
    }

    void Update()
    {
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
    // Si ya estamos en la última fase, no hay más cambios que hacer.
    if (currentPhaseIndex == attackPhases.Count - 1) return;

    float gameTimeLeft = GameManager.Instance.GetCurrentTime();
    
    // El índice de la siguiente fase potencial que vamos a comprobar.
    // Si estamos inactivos (índice -1), la siguiente fase es la 0.
    int nextPhaseIndexToCheck = currentPhaseIndex + 1;

    // Comprobamos si el tiempo ha alcanzado el trigger de la siguiente fase.
    if (gameTimeLeft <= attackPhases[nextPhaseIndexToCheck].triggerTimeInSeconds)
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
                for(int i = 0; i < pattern.burstCount; i++) // Este bucle controla las 3 ráfagas principales
                {
                    for(int j = 0; j < pattern.numberOfProjectiles; j++) // Este bucle controla las 5 balas de CADA ráfaga
                    {
                        float projectileAngle;
                        if (pattern.numberOfProjectiles > 1 && pattern.angleSpread > 0)
                            projectileAngle = currentBurstAngle - (pattern.angleSpread / 2) + (pattern.angleSpread / (pattern.numberOfProjectiles - 1)) * j;
                        else
                            projectileAngle = currentBurstAngle;
                        
                        SpawnBullet(projectileAngle, pattern.projectileSpeed);

                        // ¡AQUÍ ESTÁ LA LÍNEA MÁGICA!
                        // Añadimos una pequeña pausa después de CADA bala.
                        yield return new WaitForSeconds(pattern.timeBetweenProjectilesInBurst); 
                    }
                    currentBurstAngle += pattern.angleBetweenBursts;
                    // Esta pausa es para esperar entre una ráfaga y la siguiente (ej. entre la de 0° y la de 30°)
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
                        // Espiral 1 (sentido horario)
                        SpawnBullet(petalBaseAngle + currentSpiralAngle, pattern.projectileSpeed);
                        // Espiral 2 (sentido anti-horario)
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
    
            // Y se lo pasamos a la bala
            bulletScript.SetVelocity(finalVelocity);
        }
    }
    
    // El resto de los métodos (DealDamage, Die, UpdateHealthBar) no cambian.
    public void DealDamage(int damage) { currentHealth -= damage; UpdateHealthBar(); if (currentHealth <= 0) Die(); }
    private void Die() {  GameEvents.EnemyDied(); gameObject.SetActive(false); }
    private void UpdateHealthBar() { if(healthBar != null) { healthBar.maxValue = maxHealth; healthBar.value = currentHealth; } }
}