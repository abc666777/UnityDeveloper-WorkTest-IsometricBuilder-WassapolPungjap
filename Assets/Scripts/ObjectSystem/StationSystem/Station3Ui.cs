using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace ObjectSystem.StationSystem
{
    public class Station3Ui : MonoBehaviour
    {
        [SerializeField] private PlacementController placementController;
        [SerializeField] private RectTransform buttonRoot;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Button closeButton;

        public static UnityAction ShowMenu;
        
        private void Awake()
        {
            foreach (var data in placementController.furnitureDatas)
            {
                var newObj = Instantiate(buttonPrefab, buttonRoot);
                newObj.transform.GetChild(0).GetComponent<Image>().sprite = data.prefab.GetComponent<SpriteRenderer>().sprite;
                newObj.onClick.AddListener(() => SpawnObject(data));
            }

            ShowMenu += Show;
            rootObject.SetActive(false);
            closeButton.onClick.AddListener(Close);
        }

        private void Show()
        {
            rootObject.SetActive(true);
        }

        private void Close()
        {
            rootObject.SetActive(false);
            Helper.onCloseUi.Invoke();
        }
        
        private void SpawnObject(FurnitureData data)
        {
            if (placementController.ButtonTriedSpawnObject(data))
                Close();
            else
                Debug.LogError("THIS FURNITURE IS UNIQUE");
        }
    }
}
