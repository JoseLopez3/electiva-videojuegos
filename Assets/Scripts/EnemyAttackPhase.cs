[System.Serializable]
public class EnemyAttackPhase
{
    public string phaseName; // Para identificarlo en el inspector
    public BulletPatternSO pattern;
    public float triggerTimeInSeconds; // El tiempo del cronómetro para activar esta fase
}