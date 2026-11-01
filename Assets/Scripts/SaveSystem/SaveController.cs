using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using ObjectSystem;
using ObjectSystem.StationSystem.InventorySystem;

namespace SaveSystem
{
    public static class SaveController
    {
        private static string _craftingSavePath = Path.Combine(Application.persistentDataPath, "craftingSave.json");
        private static string _inventorySavePath = Path.Combine(Application.persistentDataPath, "inventorySave.json");
        private static string _furnitureSavePath = Path.Combine(Application.persistentDataPath, "furnitureSave.json");
        private static string _quizSavePath = Path.Combine(Application.persistentDataPath, "quizSave.json");
        
        [Serializable]
        public class CraftingSaveData
        {
            public bool isCrafting;
            public int recipeId;
            public double startingTime;
        }
        
        [Serializable]
        public class FurnitureSaveData
        {
            public Vector2Int gridPosition;
            public int furnitureId;
            public bool isFlip;
        }

        public static void SaveCraftingProgress(bool isCrafting, int recipeId, double startingTime)
        {
            var craftingSave = new CraftingSaveData
            {
                isCrafting = isCrafting,
                recipeId = recipeId,
                startingTime = startingTime
            };

            var craftingSaveJson = JsonUtility.ToJson(craftingSave, false);
            File.WriteAllText(_craftingSavePath, craftingSaveJson);
        }

        public static CraftingSaveData LoadCraftingSaveData()
        {
            if (!File.Exists(_craftingSavePath))
                return null;

            var craftingSaveJson = File.ReadAllText(_craftingSavePath);

            return JsonUtility.FromJson<CraftingSaveData>(craftingSaveJson);
        }

        public static void SaveInventory()
        {
            var inventoryJson = JsonConvert.SerializeObject(Inventory.ItemAmount);
            File.WriteAllText(_inventorySavePath, inventoryJson);
        }

        public static Dictionary<int, int> LoadInventory()
        {
            if (!File.Exists(_inventorySavePath))
                return null;
            
            var inventorySaveJson = File.ReadAllText(_inventorySavePath);
            return JsonConvert.DeserializeObject<Dictionary<int, int>>(inventorySaveJson);
        }

        public static void SaveFurniture(List<ObjectController> furnitures)
        {
            var savedFurniture = new List<FurnitureSaveData>();
            foreach (var furniture in furnitures)
            {
                savedFurniture.Add(new FurnitureSaveData
                {
                    gridPosition = furniture.originalGridPosition,
                    furnitureId = furniture.furnitureData.id,
                    isFlip = furniture.isFlip
                });
            }

            var savedFurnitureJson = JsonConvert.SerializeObject(savedFurniture);
            File.WriteAllText(_furnitureSavePath, savedFurnitureJson);
        }

        public static List<FurnitureSaveData> LoadFurniture()
        {
            if (!File.Exists(_furnitureSavePath))
                return null;

            var furnitureSaveJson = File.ReadAllText(_furnitureSavePath);
            return JsonConvert.DeserializeObject<List<FurnitureSaveData>>(furnitureSaveJson);
        }

        public static void SaveQuiz(Dictionary<int, bool> quizCorrect)
        {
            var saveQuizJson = JsonConvert.SerializeObject(quizCorrect);
            File.WriteAllText(_quizSavePath, saveQuizJson);
        }

        public static Dictionary<int, bool> LoadQuiz()
        {
            if (!File.Exists(_quizSavePath))
                return null;
            
            var quizSaveJson = File.ReadAllText(_quizSavePath);
            return JsonConvert.DeserializeObject<Dictionary<int, bool>>(quizSaveJson);
        }
    }
}
