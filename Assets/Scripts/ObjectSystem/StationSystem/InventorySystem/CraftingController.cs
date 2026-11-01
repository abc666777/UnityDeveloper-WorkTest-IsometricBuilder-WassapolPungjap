using System;
using System.Collections.Generic;
using SaveSystem;
using UnityEngine;

namespace ObjectSystem.StationSystem.InventorySystem
{
    public class CraftingController : MonoBehaviour
    {
        [SerializeField] private ItemData[] itemDatas;
        [SerializeField] private RecipeData[] recipeDatas;
        private Dictionary<int, RecipeData> _recipeDatas;
        private Dictionary<int, ItemData> _itemDatas;

        [SerializeField] private Station1Ui ui;

        private float _craftingDuration = 180f;
        private DateTime _startingTime;
        private RecipeData _currentRecipe;
        private bool _isCrafting;

        private void Awake()
        {
            //For Debugging
            foreach (var itemData in itemDatas)
                Inventory.Add(itemData.id, 99);
            
            _recipeDatas = new Dictionary<int, RecipeData>();
            _itemDatas = new Dictionary<int, ItemData>();

            foreach (var data in recipeDatas)
                _recipeDatas.Add(data.id, data);

            foreach (var data in itemDatas)
                _itemDatas.Add(data.id, data);

            var saveData = SaveController.LoadCraftingSaveData();

            if (saveData != null)
            {
                _startingTime = DateTime.FromOADate(saveData.startingTime);
                _currentRecipe = GetRecipeData(saveData.recipeId);
                _isCrafting = saveData.isCrafting;
            }

            var loadedInventory = SaveController.LoadInventory();
            if(loadedInventory != null)
                Inventory.ItemAmount = new Dictionary<int, int>(loadedInventory);

            ui.Setup(recipeDatas);
        }

        private void Update()
        {
            if (_isCrafting)
            {
                var currentTimeLeft = (DateTime.UtcNow - _startingTime).TotalSeconds;
                if (currentTimeLeft >= _craftingDuration)
                {
                    _isCrafting = false;
                    Debug.LogError("CRAFTING COMPLETE");
                    Inventory.Add(_currentRecipe.itemOutput.id, 1);
                    ui.UpdateTimeCounter(_currentRecipe, 0);
                    ui.UpdateInventory();
                    SaveController.SaveCraftingProgress(_isCrafting, _currentRecipe.id, 0);
                    SaveController.SaveInventory();
                    _currentRecipe = null;
                    return;
                }
                
                ui.UpdateTimeCounter(_currentRecipe, _craftingDuration - currentTimeLeft);
            }
        }

        public bool Craft(RecipeData recipeData)
        {
            if (_isCrafting)
            {
                Debug.LogError("ALREADY CRAFTING");
                return false;
            }

            for (var i = 0; i < recipeData.itemRequired.Length; i++)
            {
                if (Inventory.GetAmount(recipeData.itemRequired[i].id) < recipeData.amountRequired[i])
                {
                    Debug.LogError("NOT ENOUGH ITEM");
                    return false;
                }
            }

            for (var i = 0; i < recipeData.itemRequired.Length; i++)
                Inventory.Remove(recipeData.itemRequired[i].id, recipeData.amountRequired[i]);
            
            _isCrafting = true;
            _currentRecipe = recipeData;

            _startingTime = DateTime.UtcNow;
            ui.UpdateInventory();
            SaveController.SaveCraftingProgress(_isCrafting, recipeData.id, _startingTime.ToOADate());
            SaveController.SaveInventory();

            return true;
        }

        public RecipeData GetFirstRecipeData()
        {
            return recipeDatas[0];
        }

        private RecipeData GetRecipeData(int id)
        {
            return _recipeDatas[id];
        }

        public ItemData GetItemData(int id)
        {
            return _itemDatas[id];
        }
    }
}
