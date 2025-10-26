using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int, int> OnPlayerHealthChanged;
    public static void PlayerHealthChanged(int currentHealth, int maxHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public static event Action<EnemyAI> OnEnemyVisuallyDied;
    public static void EnemyVisuallyDied(EnemyAI enemy)
    {
        OnEnemyVisuallyDied?.Invoke(enemy);
    }

    public static event Action OnEnemyDeactivatedCompletely;
    public static void EnemyDeactivatedCompletely()
    {
        OnEnemyDeactivatedCompletely?.Invoke();
    }
    
    // --- CORRECCIÓN AQUÍ: El evento OnGameOver debe ser de tipo Action<bool> ---
    public static event Action<bool> OnGameOver; // <-- ¡Cambiado a Action<bool>!
    public static void GameOver(bool playerWon)
    {
        OnGameOver?.Invoke(playerWon);
    }
    // ----------------------------------------------------------------------

    public static event Action<ConsumableItem> OnItemPickedUp;
    public static void ItemPickedUp(ConsumableItem item)
    {
        OnItemPickedUp?.Invoke(item);
    }

    public static event Action OnHotbarUpdated;
    public static void HotbarUpdated()
    {
        OnHotbarUpdated?.Invoke();
    }
}