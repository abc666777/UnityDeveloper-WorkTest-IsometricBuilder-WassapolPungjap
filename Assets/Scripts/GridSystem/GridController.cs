using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridSystem
{
    public class GridController : MonoBehaviour
    {
        private GridType[,] _zoneTypes;
        private bool[,] _isGridOccupied;
        private TileBase[,] _originTileBase;

        private List<Vector2Int> _currentlyPaintedFootprint;
        
        [SerializeField] private Tilemap originTileMap;
        [SerializeField] private TileBase noPlacementTile;
        [SerializeField] private AreaData[] areaDatas;
        
        [SerializeField] private TileBase validateTile;
        [SerializeField] private TileBase invalidateTile;

        public int width = 30;
        public int height = 30;

        private void Awake()
        {
            _zoneTypes = new GridType[width,height];
            _isGridOccupied = new bool[width,height];
            _originTileBase = new TileBase[width,height];
            _currentlyPaintedFootprint = new List<Vector2Int>();
            
            Camera.main.transform.position = new Vector3(0, height * 0.25f, -10f);
            
            GenerateGridMap();
        }
        
        private void GenerateGridMap()
        {
            originTileMap.ClearAllTiles(); 

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    _zoneTypes[x, y] = GridType.NoPlacement;
                    var tilePosition = new Vector3Int(x, y, 0);
                    originTileMap.SetTile(tilePosition, noPlacementTile);
                    _originTileBase[x, y] = noPlacementTile;
                }
            }
            
            foreach (var areaData in areaDatas)
            {
                for (var x = areaData.startPos.x; x <= areaData.endPos.x; x++)
                {
                    for (var y = areaData.startPos.y; y <= areaData.endPos.y; y++)
                    {
                        _zoneTypes[x, y] = areaData.gridType;
                        var tilePosition = new Vector3Int(x, y, 0);
                        originTileMap.SetTile(tilePosition, areaData.tileToGenerate);
                        _originTileBase[x, y] = areaData.tileToGenerate;
                    }
                }
            }
        }

        private bool IsInsideGrid(int x, int y)
        {
            return (x >= 0 && x < width) && (y >= 0 && y < height);
        }

        private bool IsGridOccupied(int x, int y)
        {
            if (!IsInsideGrid(x, y))
                throw new IndexOutOfRangeException();

            return  _isGridOccupied[x, y];
        }

        private GridType GetGridType(int x, int y)
        {
            if (!IsInsideGrid(x, y))
                throw new IndexOutOfRangeException();
            
            return _zoneTypes[x, y];
        }

        private bool IsGridValid(Vector2Int position, GridType[] allowedGridTypes)
        {
            if (!IsInsideGrid(position.x, position.y))
                return false;

            if (IsGridOccupied(position.x, position.y))
                return false;

            if (!allowedGridTypes.Contains(GetGridType(position.x, position.y)))
                return false;

            return true;
        }

        public bool CanPlaceObject(Vector2Int position, Vector2Int size, GridType[] allowedGridTypes)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var targetGrid = new Vector2Int(position.x + x, position.y + y);
                    
                    if (!IsGridValid(targetGrid, allowedGridTypes))
                        return false;
                }
            }

            return true;
        }

        public void OccupyGrid(Vector2Int position, Vector2Int size)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var targetGrid = new Vector2Int(position.x + x, position.y + y);
                    _isGridOccupied[targetGrid.x, targetGrid.y] = true;
                }
            }
        }
        
        public void UnOccupyGrid(Vector2Int position, Vector2Int size)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var targetGrid = new Vector2Int(position.x + x, position.y + y);
                    _isGridOccupied[targetGrid.x, targetGrid.y] = false;
                }
            }
        }

        public void PaintFootprint(Vector2Int position, Vector2Int size,GridType[] allowedGridTypes)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var targetGrid = new Vector2Int(position.x + x, position.y + y);
                    _currentlyPaintedFootprint.Add(targetGrid);
                    originTileMap.SetTile((Vector3Int) targetGrid, !IsGridValid(targetGrid, allowedGridTypes) ? invalidateTile : validateTile);
                }
            }
        }

        public void ClearAllFootprint()
        {
            foreach (var paintedFootprint in _currentlyPaintedFootprint)
            {
                if(!IsInsideGrid(paintedFootprint.x, paintedFootprint.y))
                    originTileMap.SetTile((Vector3Int) paintedFootprint, null);
                else
                    originTileMap.SetTile((Vector3Int) paintedFootprint, _originTileBase[paintedFootprint.x, paintedFootprint.y]);
            }

            _currentlyPaintedFootprint.Clear();
        }
    }
}
