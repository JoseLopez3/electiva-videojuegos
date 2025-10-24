// using UnityEngine;
// using System.Collections.Generic;
// using System.Linq; // Necesario para .Select

// public static class SaveManager
// {
//     private const string HealthKey = "PlayerHealth";
//     private const string ScoreKey = "PlayerScore"; // Suponiendo que tienes puntuación
//     private const string TimeKey = "GameTime";
//     private const string ItemsKey = "PlayerItems";

//     public static void SaveGame(MovementPhisic player, GameManager game, InventoryManager inventory)
//     {
//         PlayerPrefs.SetInt(HealthKey, player.GetCurrentHealth());
//         PlayerPrefs.SetFloat(TimeKey, game.GetCurrentTime());
//         // ... guardar puntuación si la tienes

//         // Guardar items es más complejo. Guardaremos sus nombres.
//         string itemNames = string.Join(",", inventory.items.Select(item => item.name));
//         PlayerPrefs.SetString(ItemsKey, itemNames);

//         PlayerPrefs.Save(); // Escribe los datos en el disco
//         Debug.Log("Juego guardado!");
//     }

//     public static void LoadGame(MovementPhisic player, GameManager game, InventoryManager inventory)
//     {
//         if (!PlayerPrefs.HasKey(HealthKey))
//         {
//             Debug.Log("No hay datos de guardado.");
//             return;
//         }

//         player.SetCurrentHealth(PlayerPrefs.GetInt(HealthKey));
//         game.SetCurrentTime(PlayerPrefs.GetFloat(TimeKey));

//         // Cargar items
//         string[] itemNames = PlayerPrefs.GetString(ItemsKey).Split(',');
//         inventory.items.Clear();
//         foreach (var itemName in itemNames)
//         {
//             if (!string.IsNullOrEmpty(itemName))
//             {
//                 // Necesitamos una forma de encontrar el ScriptableObject a partir de su nombre
//                 ConsumableItem item = Resources.Load<ConsumableItem>("GameItems/" + itemName);
//                 if (item != null)
//                 {
//                     inventory.items.Add(item);
//                 }
//             }
//         }
//         GameEvents.HotbarUpdated(); // Anunciamos que la hotbar debe actualizarse
        
//         Debug.Log("Juego cargado!");
//     }
    
//     // Un método para limpiar los datos guardados (útil para botones de "Nueva Partida")
//     public static void DeleteSaveData()
//     {
//         PlayerPrefs.DeleteKey(HealthKey);
//         PlayerPrefs.DeleteKey(TimeKey);
//         PlayerPrefs.DeleteKey(ItemsKey);
//         Debug.Log("Datos de guardado eliminados.");
//     }
// }