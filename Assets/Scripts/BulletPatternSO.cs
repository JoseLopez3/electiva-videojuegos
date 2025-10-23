using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Pattern", menuName = "Bullet Hell/Bullet Pattern")]
public class BulletPatternSO : ScriptableObject
{
    // Añadimos los nuevos tipos de patrones aquí
    public enum PatternType { Circle, Spiral, AimedBurst, SequentialBurst, DoubleSpiralFlower } 
    public PatternType patternType;

    [Header("General Settings")]
    public int numberOfProjectiles = 10;
    public float projectileSpeed = 5f;
    public float timeBetweenBursts = 1f;

    [Header("Circle Settings")]
    public float startAngleOffset = 0f;

    [Header("Spiral / Double Spiral Settings")]
    public float angleStep = 10f;       // Grados de rotación en cada disparo para la espiral
    public float timeBetweenShots = 0.1f; // Tiempo entre cada bala de la espiral

    [Header("Aimed Burst / Sequential Burst Settings")]
    public int burstCount = 5;
    public float angleBetweenBursts = 15f;
    public float angleSpread = 20f;
    public float timeBetweenProjectilesInBurst = 0.05f;
    [Header("Double Spiral Flower Specific")]
    public int petals = 6; // Número de "pétalos" o brazos de la flor
}