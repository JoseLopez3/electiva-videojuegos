using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ConsumableItem item; // Referencia al ScriptableObject que representa este item

   private void Start()
{
    // --- LÍNEAS DE DEPURACIÓN ---
    if (item == null)
    {
        Debug.LogError("ERROR: ¡No hay ningún ConsumableItem asignado en el Inspector de " + gameObject.name + "!");
        return; // Detenemos la ejecución para evitar más errores
    }

    if (item.icon == null)
    {
        Debug.LogWarning("ADVERTENCIA: El ConsumableItem '" + item.name + "' no tiene un Sprite asignado en su campo 'Icon'.");
    }
    else
    {
        Debug.Log("ÉXITO: Asignando el sprite '" + item.icon.name + "' al objeto " + gameObject.name);
    }
    // --- FIN LÍNEAS DE DEPURACIÓN ---

    // Asigna el sprite del item al objeto en la escena para que se vea correcto
    GetComponent<SpriteRenderer>().sprite = item.icon;
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Intenta añadir el item al inventario (que crearemos ahora)
            // bool wasPickedUp = InventoryManager.Instance.AddItem(item);
            GameEvents.ItemPickedUp(item); // Anuncia que se intentó recoger un item
        // El InventoryManager decidirá si puede o no añadirlo.
            Destroy(gameObject); // Asumimos que siempre se recoge.
        }
    }
}
