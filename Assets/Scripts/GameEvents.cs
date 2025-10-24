using System;

using UnityEngine;

public static class GameEvents
{
    // Evento que se dispara cuando la vida del jugador cambia.
    // Envía la vida actual y la vida máxima para que la UI pueda actualizarse.
    public static event Action<int, int> OnPlayerHealthChanged;
    public static void PlayerHealthChanged(int currentHealth, int maxHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Evento que se dispara cuando un enemigo muere.
    // Podríamos añadir parámetros si quisiéramos dar puntos, por ejemplo.
    public static event Action OnEnemyDied;
    public static void EnemyDied()
    {
        OnEnemyDied?.Invoke();
    }

    // Evento que se dispara para indicar el fin del juego.
    // Envía un booleano: true si el jugador ganó, false si perdió.
    public static event Action<bool> OnGameOver;
    public static void GameOver(bool playerWon)
    {
        OnGameOver?.Invoke(playerWon);
    }

    // Evento para cuando el jugador recoge un item
    public static event Action<ConsumableItem> OnItemPickedUp;
    public static void ItemPickedUp(ConsumableItem item)
    {
        OnItemPickedUp?.Invoke(item);
    }

    // Evento para actualizar la UI de la hotbar
    public static event Action OnHotbarUpdated;
    public static void HotbarUpdated()
    {
        OnHotbarUpdated?.Invoke();
    }
}
