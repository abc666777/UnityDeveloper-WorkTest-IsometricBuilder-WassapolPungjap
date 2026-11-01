using System;
using ObjectSystem.StationSystem.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace ObjectSystem.StationSystem
{
    public class Station1Ui : MonoBehaviour
    {
        [SerializeField] private CraftingController craftingController;
        [SerializeField] private GameObject recipePrefab;
        [SerializeField] private GameObject requiredItemPrefab;

        [SerializeField] private RectTransform recipeRoot;
        [SerializeField] private RectTransform inventoryRoot;

        [SerializeField] private GameObject rootObject;

        [SerializeField] private TextMeshProUGUI resultName;
        [SerializeField] private Image resultImage;
        [SerializeField] private Button craftButton;
        [SerializeField] private TextMeshProUGUI timeLeft;
        
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform requiredItemRoot;
        
        
        private RecipeData _currentRecipe;

        public static UnityAction ShowMenu;
        public void Setup(RecipeData[] recipeData)
        {
            foreach (var data in recipeData)
            {
                var newRecipeButton = Instantiate(recipePrefab, recipeRoot);
                newRecipeButton.GetComponent<Button>().onClick.AddListener(() => ShowDisplay(data));
                var newItemRequiredDisplay = Instantiate(requiredItemPrefab, newRecipeButton.transform);
                newItemRequiredDisplay.GetComponent<Image>().sprite = data.itemOutput.sprite;
                newItemRequiredDisplay.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            }

            ShowMenu += Show;
            craftButton.onClick.AddListener(Craft);
            closeButton.onClick.AddListener(Close);
            
            rootObject.SetActive(false);
        }
        
        private void Show()
        {
            rootObject.SetActive(true);
            ShowDisplay(_currentRecipe ? _currentRecipe : craftingController.GetFirstRecipeData());
            UpdateInventory();
        }
        
        private void Close()
        {
            rootObject.SetActive(false);
            Helper.onCloseUi.Invoke();
        }

        private void ShowDisplay(RecipeData recipeData)
        {
            resultName.text = recipeData.name;
            resultImage.sprite = recipeData.itemOutput.sprite;
            _currentRecipe = recipeData;
            timeLeft.text = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss"); 
            
            for (var i = requiredItemRoot.childCount - 1; i >= 0; i--)
                Destroy(requiredItemRoot.GetChild(i).gameObject);

            for (var i = 0; i < recipeData.itemRequired.Length; i++)
            {
                var newDisplayImage = Instantiate(requiredItemPrefab, requiredItemRoot);
                newDisplayImage.GetComponent<Image>().sprite = recipeData.itemRequired[i].sprite;
                newDisplayImage.GetComponentInChildren<TextMeshProUGUI>().text = recipeData.amountRequired[i].ToString();
            }
        }

        public void UpdateTimeCounter(RecipeData targetRecipe, double currentTimeLeft)
        {
            if (targetRecipe == _currentRecipe)
                timeLeft.text = TimeSpan.FromSeconds(currentTimeLeft).ToString(@"hh\:mm\:ss");
        }

        public void UpdateInventory()
        {
            for (var i = inventoryRoot.childCount - 1; i >= 0; i--)
                Destroy(inventoryRoot.GetChild(i).gameObject);

            foreach (var inventory in Inventory.ItemAmount)
            {
                var newItem = Instantiate(recipePrefab, inventoryRoot);
                var newDisplayImage = Instantiate(requiredItemPrefab, newItem.transform);
                newDisplayImage.GetComponent<Image>().sprite = craftingController.GetItemData(inventory.Key).sprite;
                newDisplayImage.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Value.ToString();
            }
        }

        private void Craft()
        {
            craftingController.Craft(_currentRecipe);
        }
    }
}
