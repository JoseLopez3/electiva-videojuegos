
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ConsumableItem> items = new List<ConsumableItem>();
    public int hotbarSize = 3;

    [Header("Hotbar UI References")]
    public Image[] slotIcons; // Arrastra aquí las imágenes "ItemIcon" de cada slot
    public GameObject[] slots;    // Arrastra aquí los GameObjects "Slot_1", "Slot_2", etc.

    private MovementPhisic playerMovement;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Encontramos la referencia al jugador para poder aplicarle los efectos
        playerMovement = FindObjectOfType<MovementPhisic>();
        UpdateHotbarUI();
    }

    private void Update()
    {
        // Input para usar los items
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
    }

    public bool AddItem(ConsumableItem item)
    {
        if  (items.Count >= slots.Length)
        {
            Debug.Log("Inventario lleno!");
            return false; // No se pudo añadir
        }

        items.Add(item);
        UpdateHotbarUI();
        return true; // Se añadió con éxito
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex >= items.Count || items[slotIndex] == null)
        {
            Debug.Log("Slot vacío.");
            return; // No hay item en ese slot
        }

        ConsumableItem itemToUse = items[slotIndex];
        Debug.Log("Usando item: " + itemToUse.itemName);

        // Lógica para aplicar el efecto del item
        switch (itemToUse.itemType)
        {
            case ConsumableItem.ItemType.HealthPotion:
                playerMovement.Heal(itemToUse.healthToRestore);
                break;
            case ConsumableItem.ItemType.AttackSpeedStar:
                playerMovement.ApplyAttackSpeedBuff(itemToUse.speedMultiplier, itemToUse.duration);
                break;
        }

        // Remover el item de la lista
        items.RemoveAt(slotIndex);
        UpdateHotbarUI();
    }

    void UpdateHotbarUI()
    {
        for (int i = 0; i <  slots.Length; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                // Hay un item en este slot
                slotIcons[i].sprite = items[i].icon;
                slotIcons[i].enabled = true;
            }
            else
            {
                // El slot está vacío
                slotIcons[i].sprite = null;
                slotIcons[i].enabled = false;
            }
        }
    }
}