using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null; // El ícono que se mostrará en la hotbar
    public enum ItemType { HealthPotion, AttackSpeedStar } // Los tipos de objetos que existen
    public ItemType itemType;

    // Valores específicos para cada tipo de objeto
    [Header("Health Potion")]
    public int healthToRestore = 25;

    [Header("Attack Speed Star")]
    public float speedMultiplier = 1.5f; // Aumenta la velocidad de ataque en un 50%
    public float duration = 5f;          // Durante 5 segundos
}