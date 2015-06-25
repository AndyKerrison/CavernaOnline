﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.UIScripts
{
    public class PlayerBoardScript : MonoBehaviour {

        private GameObject _woodCount;
        private GameObject _stoneCount;
        private GameObject _oreCount;
        private GameObject _foodCount;
        private GameObject _goldCount;
        private GameObject _grainCount;
        private GameObject _vegCount;
        private GameObject _rubyCount;
        private GameObject _beggingCount;
        private GameObject _scoreCount;

        private int _food;
        private int _wood;
        private int _stone;
        private int _ore;
        private int _gold;
        private int _grain;
        private int _veg;
        private int _rubies;
        private int _begging;
        private int _score;

        private bool _showResourcesOnBoard = false;

        private List<GameObject> _dwarves;
        private GameObject[,] _caveTiles;
        private GameObject[,] _forestTiles;


        // Use this for initialization
// ReSharper disable once UnusedMember.Local
        void Start () {
            _dwarves = new List<GameObject>();
            var parentWidth = transform.GetComponent<RectTransform>().rect.width;
            var parentHeight = transform.GetComponent<RectTransform>().rect.height;
            var spriteSize = new Vector2(parentWidth / 16f, parentHeight / 16f);
            var horizontalIncrement = - parentHeight / 12f;
            var verticalOffset = transform.GetComponent<RectTransform>().anchoredPosition.y + 0.000f*parentHeight;
            var horizontalOffset = transform.GetComponent<RectTransform>().anchoredPosition.x - 0.02f*parentWidth;

            if (_showResourcesOnBoard)
            {
                _woodCount = InitIcon(ResourceTypes.Wood, spriteSize);
                _woodCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 9*horizontalIncrement, verticalOffset);

                _stoneCount = InitIcon(ResourceTypes.Stone, spriteSize);
                _stoneCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 8*horizontalIncrement, verticalOffset);

                _oreCount = InitIcon(ResourceTypes.Ore, spriteSize);
                _oreCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 7*horizontalIncrement, verticalOffset);

                _foodCount = InitIcon(ResourceTypes.Food, spriteSize);
                _foodCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 6*horizontalIncrement, verticalOffset);

                _goldCount = InitIcon(ResourceTypes.Gold, spriteSize);
                _goldCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 5*horizontalIncrement, verticalOffset);

                _grainCount = InitIcon(ResourceTypes.Grain, spriteSize);
                _grainCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 4*horizontalIncrement, verticalOffset);

                _vegCount = InitIcon(ResourceTypes.Veg, spriteSize);
                _vegCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 3*horizontalIncrement, verticalOffset);

                _rubyCount = InitIcon(ResourceTypes.Ruby, spriteSize);
                _rubyCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 2*horizontalIncrement, verticalOffset);

                _beggingCount = InitIcon(ResourceTypes.Begging, spriteSize);
                _beggingCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 1*horizontalIncrement, verticalOffset);

                _scoreCount = InitIcon(ResourceTypes.ScoreMarker, spriteSize);
                _scoreCount.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(horizontalOffset + 0*horizontalIncrement, verticalOffset);
            }

            verticalOffset = transform.GetComponent<RectTransform>().anchoredPosition.y + -0.27f * parentHeight;
            horizontalOffset = transform.GetComponent<RectTransform>().anchoredPosition.x +0.1f * parentWidth;
            horizontalIncrement = 0.14f*parentWidth;
            var verticalIncrement = 0.195f * parentHeight;
            spriteSize = new Vector2(parentWidth / 7.5f, parentHeight / 7.5f);
            
            _caveTiles = new GameObject[3, 4];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    _caveTiles[x, y] = (GameObject)Instantiate(Resources.Load("TileUI"));
                    _caveTiles[x, y].transform.SetParent(transform, true);
                    _caveTiles[x, y].transform.localScale = transform.localScale;
                    _caveTiles[x, y].transform.position = transform.position;// new Vector2(parentPosition.x + x * boardWidth / 7.4f, parentPosition.y + y * boardHeight / 5.2f);

                    spriteSize.y = (spriteSize.x / _caveTiles[x, y].GetComponent<RectTransform>().sizeDelta.x) *
                        _caveTiles[x, y].GetComponent<RectTransform>().sizeDelta.y;

                    _caveTiles[x, y].GetComponent<RectTransform>().sizeDelta = spriteSize;
                    _caveTiles[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset + x*horizontalIncrement, verticalOffset + y*verticalIncrement);
                    
                    _caveTiles[x, y].GetComponent<TileUIScript>().SetVector(x, y);
                    _caveTiles[x, y].GetComponent<TileUIScript>().SetIsCave();
                    //_caveTiles[x, y].GetComponent<TileUIScript>().SetType(new List<string>() {});
                }
            }

            verticalOffset = transform.GetComponent<RectTransform>().anchoredPosition.y + -0.27f * parentHeight;
            horizontalOffset = transform.GetComponent<RectTransform>().anchoredPosition.x - 0.38f * parentWidth;
            _forestTiles = new GameObject[3, 4];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    _forestTiles[x, y] = (GameObject)Instantiate(Resources.Load("TileUI"));
                    _forestTiles[x, y].transform.SetParent(transform, true);
                    _forestTiles[x, y].transform.localScale = transform.localScale;
                    _forestTiles[x, y].transform.position = transform.position;// new Vector2(parentPosition.x + x * boardWidth / 7.4f, parentPosition.y + y * boardHeight / 5.2f);

                    spriteSize.y = (spriteSize.x / _forestTiles[x, y].GetComponent<RectTransform>().sizeDelta.x) *
                        _forestTiles[x, y].GetComponent<RectTransform>().sizeDelta.y;

                    _forestTiles[x, y].GetComponent<RectTransform>().sizeDelta = spriteSize;
                    _forestTiles[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset + x * horizontalIncrement, verticalOffset + y * verticalIncrement);

                    _forestTiles[x, y].GetComponent<TileUIScript>().SetVector(x, y);
                    //_forestTiles[x, y].GetComponent<TileUIScript>()..SetIsCave();
                    //_caveTiles[x, y].GetComponent<TileUIScript>().SetType(new List<string>() {});
                }
            }
        }
	
        // Update is called once per frame
// ReSharper disable once UnusedMember.Local
        void Update () {
            if (_showResourcesOnBoard)
            {
                _woodCount.GetComponent<ResourceIcon>().SetValue(_wood);
                _foodCount.GetComponent<ResourceIcon>().SetValue(_food);
                _stoneCount.GetComponent<ResourceIcon>().SetValue(_stone);
                _oreCount.GetComponent<ResourceIcon>().SetValue(_ore);
                _foodCount.GetComponent<ResourceIcon>().SetValue(_food);
                _goldCount.GetComponent<ResourceIcon>().SetValue(_gold);
                _grainCount.GetComponent<ResourceIcon>().SetValue(_grain);
                _vegCount.GetComponent<ResourceIcon>().SetValue(_veg);
                _rubyCount.GetComponent<ResourceIcon>().SetValue(_rubies);
                _beggingCount.GetComponent<ResourceIcon>().SetValue(_begging);
                _scoreCount.GetComponent<ResourceIcon>().SetValue(_score);
            }
        }

        private GameObject InitIcon(string resourceType, Vector2 spriteSize)
        {
            GameObject icon = ResourceIcon.Create(resourceType, true);
            icon.transform.SetParent(transform, true);
            icon.transform.localScale = transform.localScale;
            icon.transform.position = transform.position;
            spriteSize.y = (spriteSize.x / icon.GetComponent<RectTransform>().sizeDelta.x) *
                           icon.GetComponent<RectTransform>().sizeDelta.y;
            icon.GetComponent<RectTransform>().sizeDelta = spriteSize;
            return icon;
        }

        public void SetResources(string resourceName, int count)
        {
            if (!_showResourcesOnBoard)
                return;

            if (resourceName == ResourceTypes.Wood)
                _wood = count;

            if (resourceName == ResourceTypes.Stone)
                _stone = count;

            if (resourceName == ResourceTypes.Ore)
                _ore = count;
        
            if (resourceName == ResourceTypes.Food)
                _food = count;

            if (resourceName == ResourceTypes.Gold)
                _gold = count;

            if (resourceName == ResourceTypes.Grain)
                _grain = count;
        
            if (resourceName == ResourceTypes.Veg)
                _veg = count;

            if (resourceName == ResourceTypes.Ruby)
                _rubies = count;

            if (resourceName == ResourceTypes.Begging)
                _begging = count;
        
            if (resourceName == ResourceTypes.ScoreMarker)
                _score = count;
        }

        public void SetTileType(Vector2 position, List<string> tileType, bool isCave)
        {
            if (isCave)
                _caveTiles[(int)position.x, (int)position.y].GetComponent<TileUIScript>().SetType(tileType);
            else
                _forestTiles[(int)position.x, (int)position.y].GetComponent<TileUIScript>().SetType(tileType);
        }

        public void SetClickableTile(Vector2 validSpot, string tileType, bool isCave)
        {
            if (isCave)
                _caveTiles[(int)validSpot.x, (int)validSpot.y].GetComponent<TileUIScript>().SetClickable(tileType, true);
            else
                _forestTiles[(int)validSpot.x, (int)validSpot.y].GetComponent<TileUIScript>().SetClickable(tileType, true);
        }

        public void SetClickableBuildingTile(Vector2 validSpot, string buildingType)
        {
            _caveTiles[(int)validSpot.x, (int)validSpot.y].GetComponent<TileUIScript>().SetClickable(buildingType, true);
        }

        public void SetTileAnimals(Vector2 position, bool isCave, string animalType, int count)
        {
            if (isCave)
                _caveTiles[(int)position.x, (int)position.y].GetComponent<TileUIScript>()
                    .SetResources(animalType, count);
            else
                _forestTiles[(int)position.x, (int)position.y].GetComponent<TileUIScript>()
                    .SetResources(animalType, count);
        }

        public void SetTilesUnclickable()
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    _caveTiles[x, y].GetComponent<TileUIScript>().SetUnclickable();
                    _forestTiles[x, y].GetComponent<TileUIScript>().SetUnclickable();
                }
            }
        }

        public void SetDoubleFencedPastures(List<Vector2> doubleFencedPastures)
        {
            for (int i = 0; i < doubleFencedPastures.Count; i += 2)
            {
                if (doubleFencedPastures[i + 1].x > doubleFencedPastures[i].x) //default! no rotation
                {
                }

                //90 and 270 are flipped as unity seems to rotate anticlockwise
                if (doubleFencedPastures[i + 1].y < doubleFencedPastures[i].y) //90
                {
                    _forestTiles[(int)doubleFencedPastures[i].x, (int)doubleFencedPastures[i].y]
                        .GetComponent<TileUIScript>().SetRotation(270);
                    _forestTiles[(int)doubleFencedPastures[i + 1].x, (int)doubleFencedPastures[i + 1].y]
                        .GetComponent<TileUIScript>().SetRotation(270);
                }
                if (doubleFencedPastures[i + 1].x < doubleFencedPastures[i].x) //180
                {
                    _forestTiles[(int)doubleFencedPastures[i].x, (int)doubleFencedPastures[i].y]
                        .GetComponent<TileUIScript>().SetRotation(180);
                    _forestTiles[(int)doubleFencedPastures[i + 1].x, (int)doubleFencedPastures[i + 1].y]
                        .GetComponent<TileUIScript>().SetRotation(180);
                }
                if (doubleFencedPastures[i + 1].y > doubleFencedPastures[i].y) //270
                {
                    _forestTiles[(int)doubleFencedPastures[i].x, (int)doubleFencedPastures[i].y]
                        .GetComponent<TileUIScript>().SetRotation(90);
                    _forestTiles[(int)doubleFencedPastures[i + 1].x, (int)doubleFencedPastures[i + 1].y]
                        .GetComponent<TileUIScript>().SetRotation(90);
                }
            }
        }

        //draw dwarf icons (mark used & weapon level) on the player board
        public void SetDwarves(List<string> dwarfStatus)
        {
            if (_dwarves != null)
            {
                foreach (var dwarf in _dwarves)
                {
                    Destroy(dwarf);
                }
            }
            _dwarves = new List<GameObject>();
            /*
            Vector2 parentPosition = gameObject.transform.position;
            float boardHeight = _renderer.bounds.size.y;
            float boardWidth = _renderer.bounds.size.x;

            float x = parentPosition.x - boardWidth / 2.2f;
            float y = parentPosition.y - boardHeight / 2.2f;

            //float x = gameObject.transform.position.x;
            //float y = gameObject.transform.position.y;

            for (int i = 0; i < dwarfStatus.Count; i++)
            {
                GameObject dwarfToken = (GameObject)Instantiate(Resources.Load("DwarfToken"));
                dwarfToken.transform.parent = transform;
                //DwarfToken.transform.position = new Vector2(x, y);
                //DwarfToken.SetActive(false);



                //GameObject DwarfToken = (GameObject)Instantiate(Resources.Load("DwarfToken"));
                //DwarfToken.transform.parent = transform;
                dwarfToken.transform.localScale = new Vector3(transform.localScale.x * 0.5f, transform.localScale.y * 0.5f, transform.localScale.z * 0.75f);
                dwarfToken.transform.position = new Vector2(x + i * boardWidth / 12, y);
                dwarfToken.SetActive(true);

                DwarfGameObject dwarf = dwarfToken.GetComponent<DwarfGameObject>();
                dwarf.SetWeaponLevel(dwarfStatus[i].Split(new[] { '_' })[0]);
                dwarf.SetUsed(dwarfStatus[i].Split(new[] { '_' })[1]);

                if (Boolean.Parse(dwarfStatus[i].Split(new[] { '_' })[2]))
                {
                    dwarfToken.transform.localScale = new Vector3(dwarfToken.transform.localScale.x * 0.6f,
                        dwarfToken.transform.localScale.y * 0.6f, dwarfToken.transform.localScale.z * 0.6f);
                }
                _dwarves.Add(dwarfToken);
            }*/
        }
    }
}
