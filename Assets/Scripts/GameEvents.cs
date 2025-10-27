using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int, int> OnPlayerHealthChanged;
    public static void PlayerHealthChanged(int currentHealth, int maxHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public static event Action OnPlayerTookDamage;
    public static void PlayerTookDamage()
    {
        OnPlayerTookDamage?.Invoke();
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
    
    public static event Action<bool> OnGameOver; 
    public static void GameOver(bool playerWon)
    {
        OnGameOver?.Invoke(playerWon);
    }

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