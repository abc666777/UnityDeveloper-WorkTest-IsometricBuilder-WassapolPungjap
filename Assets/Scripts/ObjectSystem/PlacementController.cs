using System;
using System.Collections.Generic;
using GridSystem;
using ObjectSystem.StationSystem;
using SaveSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace ObjectSystem
{
    public class PlacementController : MonoBehaviour
    {
        public FurnitureData[] furnitureDatas;
        [SerializeField] private GridController gridController;
        [SerializeField] private FurnitureData station3FurnitureData;
        private Dictionary<int, FurnitureData> _furnitureDatas;
        
        [SerializeField] private PlacementControllerUi ui;

        private GameObject _touchObject;
        private ObjectController _currentFurniture;
        private bool _isInitialize;

        private InteractionState _state = InteractionState.Idle;

        private float _holdTimer;
        private bool _originalFlip;
        private bool _isOpenUi;

        private List<ObjectController> _placedFurnitures = new List<ObjectController>();

        private void OnEnable()
        {
/*#if UNITY_EDITOR
            TouchSimulation.Enable();
#endif*/
            Helper.onCloseUi += () => _isOpenUi = false;
        }

        private void Start()
        {
            _furnitureDatas = new Dictionary<int, FurnitureData>();
            
            foreach (var data in furnitureDatas)
                _furnitureDatas.Add(data.id, data);
            
            var savedFurniture = SaveController.LoadFurniture();
            if(savedFurniture == null)
                ui.placeStationButton.onClick.AddListener(InitializeStation3);
            else
            {
                ui.placeStationButton.gameObject.SetActive(false);
                foreach (var furniture in savedFurniture)
                {
                    var newObj = SpawnObject(_furnitureDatas[furniture.furnitureId]);
                    FlipObject(newObj.GetComponent<ObjectController>(), furniture.isFlip);
                    newObj.transform.position = furniture.gridPosition.GridPositionToObjectPosition();
                    PlaceObject(furniture.gridPosition, newObj.GetComponent<ObjectController>());
                }
            }

            ui.placeButton.onClick.AddListener(ButtonPlaceObjectEvent);
            ui.flipButton.onClick.AddListener(ButtonFlipObjectEvent);
            ui.cancelButton.onClick.AddListener(ButtonCancelEditEvent);
            ui.destroyButton.onClick.AddListener(DestroyObject);
        }

        private void InitializeStation3()
        {
            ButtonTriedSpawnObject(station3FurnitureData);
            ui.placeStationButton.gameObject.SetActive(false);
        }
        private void Update()
        {
            var touchScreen = Touchscreen.current;
            var touch = touchScreen.primaryTouch;
            var screenPosition = touch.position.ReadValue();
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            if(_isOpenUi) return;

            switch (_state)
            {
                case InteractionState.Idle:
                    if (touch.press.isPressed)
                    {
                        _touchObject = HandleTouchScreen(worldPosition);
                        if (_touchObject)
                        {
                            _state = InteractionState.Holding;
                            _originalFlip = HandleTouchScreen(worldPosition).GetComponent<ObjectController>().isFlip;
                        }
                    }
                    return;
                case InteractionState.Holding:
                    _holdTimer += Time.deltaTime;
                    if (!touch.press.isPressed)
                    {
                        var currentStation = HandleTouchScreen(worldPosition)?.GetComponent<IStation>();
                        
                        _holdTimer = 0;
                        if (_currentFurniture)
                            _state = InteractionState.Editing;

                        else
                        {
                            _state = InteractionState.Idle;
                            if (currentStation != null)
                            {
                                currentStation?.ShowMenu();
                                _isOpenUi = true;
                            }
                        }
                        
                        return;
                    }
                    if (_holdTimer >= 0.3f && HandleTouchScreen(worldPosition))
                    {
                        _holdTimer = 0;
                        _state = InteractionState.Dragging;
                        _currentFurniture = HandleTouchScreen(worldPosition).GetComponent<ObjectController>();
                        
                        if(_currentFurniture.originalGridPosition.x >= 0)
                            gridController.UnOccupyGrid(_currentFurniture.originalGridPosition, _currentFurniture.Size());
                    }
                    return;
                case InteractionState.Dragging:
                    if (!touch.press.isPressed)
                        _state = InteractionState.Editing;
                    else
                    {
                        ui.HideEditUi();
                        gridController.ClearAllFootprint();
                        DragObject(worldPosition);
                    }
                    return;
                case InteractionState.Editing:
                    ui.ShowEditUi(_currentFurniture.transform.position, _currentFurniture is Station3Controller);
                    if (touch.press.isPressed)
                    {
                        if (HandleTouchScreen(worldPosition) == _currentFurniture.gameObject)
                        {
                            _state = InteractionState.Holding;
                            ui.HideEditUi();
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DragObject(Vector2 worldPosition)
        {
            var snappedPosToGrid = worldPosition.ObjectPositionToGridPosition();
            var gridToWorldPos = snappedPosToGrid.GridPositionToObjectPosition();
            _currentFurniture.transform.position = new Vector3(gridToWorldPos.x, gridToWorldPos.y, 0f);

            gridController.PaintFootprint(snappedPosToGrid, _currentFurniture.Size(), 
                _currentFurniture.furnitureData.allowedGridType);
        }

        private void ButtonFlipObjectEvent()
        {
            FlipObject(_currentFurniture, !_currentFurniture.isFlip);
            gridController.ClearAllFootprint();
            gridController.PaintFootprint(((Vector2)_currentFurniture.transform.position).ObjectPositionToGridPosition(), 
                _currentFurniture.Size(), _currentFurniture.furnitureData.allowedGridType);
        }

        private void FlipObject(ObjectController objectController, bool isFlip)
        {
            objectController.isFlip = isFlip;
            objectController.GetComponent<SpriteRenderer>().flipX = isFlip;
        }

        private void ButtonPlaceObjectEvent()
        {
            var position = _currentFurniture.gameObject.transform.position;
            var snappedPosition = ((Vector2) position).ObjectPositionToGridPosition();
            PlaceObject(snappedPosition);
        }

        private void PlaceObject(Vector2Int snappedPosToGrid)
        {
            if (gridController.CanPlaceObject(snappedPosToGrid, _currentFurniture.Size(), _currentFurniture.GetComponent<ObjectController>().furnitureData.allowedGridType))
            {
                gridController.ClearAllFootprint();
                PlaceObject(snappedPosToGrid, _currentFurniture);
                
                ui.HideEditUi();
                _currentFurniture = null;
                _state = InteractionState.Idle;
                SaveController.SaveFurniture(_placedFurnitures);
            }
            else
            {
                Debug.LogError("NOT ALLOW");
                _state = InteractionState.Editing;
                gridController.ClearAllFootprint();
                gridController.PaintFootprint(snappedPosToGrid, _currentFurniture.Size(),
                    _currentFurniture.GetComponent<ObjectController>().furnitureData.allowedGridType);
            }
        }

        private void PlaceObject(Vector2Int snappedPosToGrid, ObjectController objectController)
        {
            gridController.OccupyGrid(snappedPosToGrid, objectController.Size());
            objectController.originalGridPosition = snappedPosToGrid;
            
            if(!_placedFurnitures.Contains(objectController))
                _placedFurnitures.Add(objectController);
        }

        private void ButtonCancelEditEvent()
        {
            if(_currentFurniture.isFlip != _originalFlip)
                ButtonFlipObjectEvent();

            var originalPosition = _currentFurniture.originalGridPosition.GridPositionToObjectPosition();
            _currentFurniture.transform.position = new Vector3(originalPosition.x, originalPosition.y, 0f);
            PlaceObject(_currentFurniture.originalGridPosition);
            _originalFlip = false;
            
            ui.HideEditUi();
        }

        public bool ButtonTriedSpawnObject(FurnitureData furnitureData)
        {
            if (furnitureData.isUnique)
            {
                foreach (var placeFurniture in _placedFurnitures)
                {
                    if (placeFurniture.furnitureData == furnitureData)
                        return false;
                }
            }

            _currentFurniture = SpawnObject(furnitureData).GetComponent<ObjectController>();
            gridController.PaintFootprint(((Vector2) _currentFurniture.transform.position).ObjectPositionToGridPosition(),
                _currentFurniture.Size(), _currentFurniture.furnitureData.allowedGridType);
            
            _currentFurniture.originalGridPosition = new Vector2Int(-1,-1);
            _state = InteractionState.Editing;
            return true;
        }

        private GameObject SpawnObject(FurnitureData furnitureData)
        {
            var newObj = Instantiate(furnitureData.prefab, new Vector2Int(0, 0).GridPositionToObjectPosition(),
                Quaternion.identity);
            newObj.GetComponent<ObjectController>().furnitureData = furnitureData;

            return newObj;
        }

        private void DestroyObject()
        {
            _placedFurnitures.Remove(_currentFurniture);
            
            Destroy(_currentFurniture.gameObject);
            gridController.ClearAllFootprint();
            ui.HideEditUi();
            _currentFurniture = null;
            _state = InteractionState.Idle;
            
            SaveController.SaveFurniture(_placedFurnitures);
        }

        private GameObject HandleTouchScreen(Vector2 worldPosition)
        {
            var hit = Physics2D.OverlapPoint(worldPosition);
            return hit == null ? null : hit.gameObject;
        }

        private enum InteractionState
        {
            Idle,
            Holding,
            Dragging,
            Editing,
        }
    }
}
