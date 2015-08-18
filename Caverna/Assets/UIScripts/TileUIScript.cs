using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.UIScripts
{
    public class TileUIScript : MonoBehaviour {

        public Sprite Tunnel;
        public Sprite Cave;
        public Sprite Field;
        public Sprite Clearing;
        public Sprite OreMine;
        public Sprite RubyMine;
        public Sprite DeepTunnel;
        public Sprite SmallFence;
        public Sprite BigFence1;
        public Sprite BigFence2;
        public Sprite Stable;
        public Sprite Dwelling;
        public Sprite SimpleDwelling1;
        public Sprite SimpleDwelling2;
        public Sprite MixedDwelling;
        public Sprite CoupleDwelling;
        public Sprite AdditionalDwelling;
        public Sprite Carpenter;
        public Sprite StoneCarver;
        public Sprite Miner;
        public Sprite Builder;
        public Sprite Blacksmith;
        public Sprite Trader;
        public Sprite CuddleRoom;
        public Sprite WorkRoom;
        public Sprite WoodSupplier;
        public Sprite DogSchool;
        public Sprite BreakfastRoom;
        public Sprite GuestRoom;
        public Sprite StoneSupplier;
        public Sprite Quarry;
        public Sprite StubbleRoom;
        public Sprite OfficeRoom;
        public Sprite RubySupplier;
        public Sprite Seam;
        public Sprite SlaughteringCave;
        public Sprite MiningCave;
        public Sprite StoneStorage;
        public Sprite MainStorage;
        public Sprite CookingCave;
        public Sprite BreedingCave;
        public Sprite OreStorage;
        public Sprite WeaponStorage;
        public Sprite WorkingCave;
        public Sprite PeacefulCave;
        public Sprite SparePartStorage;
        public Sprite SuppliesStorage;
        public Sprite WeavingParlour;
        public Sprite HuntingParlour;
        public Sprite BroomChamber;
        public Sprite PrayerChamber;
        public Sprite MilkingParlour;
        public Sprite BeerParlour;
        public Sprite TreasureChamber;
        public Sprite WritingChamber;
        public Sprite StateParlour;
        public Sprite BlacksmithingParlour;
        public Sprite FoodChamber;
        public Sprite FodderChamber;
        public Sprite Unavailable;
        
        public int X;
        public int Y;

        private bool _isCave;
        private bool _clickable;
        private bool _actionEnabled;
        private bool _useMouseOverHighlight;
        private string _tileType;

        private Sprite _activeSprite;
        private Color _activeColor;
        private Sprite _mouseOverSprite;
        private Sprite _mainSprite;
        private GameObject _icon;
        private GameObject _dogIcon;
        private GameObject _childTile;
        private CanvasRenderer _renderer;
        private Image _image;
        private readonly Color _hiddenColor = new Color(1f, 1f, 1f, 0f);


        public void SetVector(int x, int y)
        {
            X = x;
            Y = y;
        }

// ReSharper disable once UnusedMember.Local
        void Start()
        {
            _image = GetComponent<Image>();
            //_activeSprite = null;

            //set tile to hidden by default.
            _renderer = GetComponent<Image>().canvasRenderer;
            //_activeColor = _hiddenColor;

            var parentWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
            var parentHeight = transform.GetComponent<RectTransform>().sizeDelta.y;
            var spriteSize = new Vector2(parentWidth / 2f, parentHeight / 2f);
            var verticalOffset = transform.GetComponent<RectTransform>().anchoredPosition.y + 0*parentHeight;
            var horizontalOffset = transform.GetComponent<RectTransform>().anchoredPosition.x - 0*parentWidth;

            _icon = ResourceIcon.Create(ResourceTypes.Grain, false);
            _icon.transform.SetParent(transform, true);
            _icon.transform.localScale = transform.localScale;
            //_icon.transform.position = new Vector3(transform.position.x + 0.0f*parentWidth, transform.position.y + 0.5f*parentHeight);
            spriteSize.y = (spriteSize.x / _icon.GetComponent<RectTransform>().sizeDelta.x) *
                           _icon.GetComponent<RectTransform>().sizeDelta.y;
            _icon.GetComponent<RectTransform>().sizeDelta = spriteSize;
            _icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f*parentWidth, 0.05f*parentHeight);

            _dogIcon = ResourceIcon.Create(ResourceTypes.Dogs, false);
            _dogIcon.transform.SetParent(transform, true);
            _dogIcon.transform.localScale = transform.localScale;
            //_dogIcon.transform.position = new Vector3(transform.position.x +  0*parentWidth, transform.position.y + 0*parentHeight);
            spriteSize.y = (spriteSize.x / _dogIcon.GetComponent<RectTransform>().sizeDelta.x) *
                           _dogIcon.GetComponent<RectTransform>().sizeDelta.y;
            _dogIcon.GetComponent<RectTransform>().sizeDelta = spriteSize;
            _dogIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f*parentWidth, -0.5f*parentHeight); //new Vector2(horizontalOffset + 0.5f * parentWidth, verticalOffset - 0.8f*parentHeight);
        }

        public void SetIsCave()
        {
            _isCave = true;
        }

        public void SetType(List<string> tileTypes)
        {
            SetUnclickable();
            //SortingOrder = 2-tileTypes.Count;

            _tileType = tileTypes[0];

            _mainSprite = GetSpriteByType(_tileType);

            _activeSprite = _mainSprite;
            _activeColor = _activeSprite != null ? Color.white : _hiddenColor;
            
            if (_icon != null)
                _icon.GetComponent<ResourceIcon>().SetValue(0);
            else
                return;

            if (_tileType == TileTypes.Field3Grain)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Grain);
                _icon.GetComponent<ResourceIcon>().SetValue(3);
            }
            if (_tileType == TileTypes.Field2Grain)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Grain);
                _icon.GetComponent<ResourceIcon>().SetValue(2);
            }
            if (_tileType == TileTypes.Field1Grain)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Grain);
                _icon.GetComponent<ResourceIcon>().SetValue(1);
            }
            if (_tileType == TileTypes.Field2Veg)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Veg);
                _icon.GetComponent<ResourceIcon>().SetValue(2);
            }
            if (_tileType == TileTypes.Field1Veg)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Veg);
                _icon.GetComponent<ResourceIcon>().SetValue(1);
            }

            if (tileTypes.Count > 1)
            {
                if (_childTile != null)
                    Destroy(_childTile);
                InitChildTile(tileTypes[1]);
                if (_childTile != null)
                    _childTile.GetComponent<TileUIScript>().SetType(new List<string> { tileTypes[1] });
            }
        }

        public void SetActionActive()
        {
            gameObject.GetComponent<CanvasGroup>().interactable = true;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            _actionEnabled = true;
        }

        public void SetUnclickable()
        {
            _clickable = false;
            if (_childTile != null)
            {
                if (!_childTile.GetComponent<TileUIScript>().HasValue())
                    Destroy(_childTile);
                else
                    _childTile.GetComponent<TileUIScript>().SetUnclickable();
            }
            if (!_actionEnabled)
            {
                gameObject.GetComponent<CanvasGroup>().interactable = false;
                gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            //_isClickableBuilding = false;
        }

        public void SetClickable(string tileType, bool isParent, bool isOnBuildingsBoard, bool useMouseOverHighlight)
        {
            _tileType = tileType;
            _useMouseOverHighlight = useMouseOverHighlight;
            gameObject.GetComponent<CanvasGroup>().interactable = true;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //_renderSortingOrder = 5;

            if (tileType == TileTypes.Stable && isParent)
            {
                InitChildTile(tileType);
                _childTile.GetComponent<TileUIScript>().SetClickable(tileType, false, isOnBuildingsBoard, useMouseOverHighlight);
            }
            else if (IsBuildingTile(tileType) && isParent && !isOnBuildingsBoard)
            {
                InitChildTile(tileType);
                _childTile.GetComponent<TileUIScript>().SetClickable(tileType, false, isOnBuildingsBoard, useMouseOverHighlight);
            }
            else
            {
                _clickable = true;
                _mouseOverSprite = GetSpriteByType(tileType);
            }
        }

        private bool IsBuildingTile(string tileType)
        {
            return Array.Find(typeof(BuildingTypes).GetFields(), x => x.GetValue(null).ToString() == tileType) != null;
        }

        public void SetResources(string animalType, int count)
        {
            if (animalType == ResourceTypes.Dogs)
            {
                _dogIcon.GetComponent<ResourceIcon>().SetType(animalType);
                _dogIcon.GetComponent<ResourceIcon>().SetValue(count);
            }
            else
            {
                _icon.GetComponent<ResourceIcon>().SetType(animalType);
                _icon.GetComponent<ResourceIcon>().SetValue(count);
            }
        }

        public void SetRotation(int degrees)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, degrees));
        }

        private void InitChildTile(string tileType)
        {
            _childTile = (GameObject)Instantiate(Resources.Load("TileUI"));
            _childTile.transform.SetParent(transform, true);
            _childTile.transform.localScale = transform.localScale;
            _childTile.transform.position = transform.position;// new Vector2(parentPosition.x + x * boardWidth / 7.4f, parentPosition.y + y * boardHeight / 5.2f);

            //spriteSize.y = (spriteSize.x / _caveTiles[x, y].GetComponent<RectTransform>().sizeDelta.x) *
            //    _caveTiles[x, y].GetComponent<RectTransform>().sizeDelta.y;

            _childTile.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta*0.9f;
            //_caveTiles[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset + x * horizontalIncrement, verticalOffset + y * verticalIncrement);
                    

            //_childTile = (GameObject)Instantiate(Resources.Load("TileUI"));
            //_childTile.transform.position = new Vector2(transform.position.x, transform.position.y);
            _childTile.GetComponent<TileUIScript>().SetVector(X, Y);
            //_childTile.transform.localScale = transform.localScale;
            _childTile.GetComponent<TileUIScript>().SetClickable(tileType, false, false, false);
            if (_isCave)
                _childTile.GetComponent<TileUIScript>().SetIsCave();
        }

        private bool HasValue()
        {
            return _mainSprite != null;
        }

        private void Update()
        {
            _image.sprite = _activeSprite;
            _renderer.SetColor(_activeColor);
        }

        // ReSharper disable once UnusedMember.Local
        public void OnMouseDown()
        {
            //do action if possible
            if (_clickable)
            {
                ClientSocket.Instance.SetTileClicked("Player", new Vector2(X, Y), _tileType, _isCave, IsBuildingTile(_tileType));
            }
            if (_actionEnabled)
            {
                if (!CavernaGame.Instance.GetComponent<CavernaGame>().ModalExecuting)
                    ClientSocket.Instance.GetTileAction("Player", new Vector2(X, Y), _tileType);
            }
        }

        // ReSharper disable once UnusedMember.Local
        public void OnMouseEnter()
        {
            if (_clickable)
            {
                _activeSprite = _mouseOverSprite;
                if (_useMouseOverHighlight)
                    _activeColor = Color.green;
                else
                    _activeColor = new Color(10, 50, 10, 0.5f);
            }
        }

        public void OnMouseExit()
        {
            _activeSprite = _mainSprite;
            if (_mainSprite == null)
                _activeColor = _hiddenColor;
            else
                _activeColor = Color.white;
        }

        private Sprite GetSpriteByType(string tileType)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f);

            if (tileType == TileTypes.Tunnel)
            {
                return Tunnel;
            }
            if (tileType == TileTypes.Cavern)
            {
                return Cave;
            }
            if (tileType == TileTypes.Field)
            {
                return Field;
            }
            if (tileType == TileTypes.Clearing)
            {
                return Clearing;
            }
            if (tileType == TileTypes.OreMine)
            {
                return OreMine;
            }
            if (tileType == TileTypes.RubyMine)
            {
                return RubyMine;
            }
            if (tileType == TileTypes.DeepTunnel)
            {
                return DeepTunnel;
            }
            if (tileType == TileTypes.Field3Grain)
            {
                return Field;
            }
            if (tileType == TileTypes.Field2Grain)
            {
                return Field;
            }
            if (tileType == TileTypes.Field1Grain)
            {
                return Field;
            }
            if (tileType == TileTypes.Field2Veg)
            {
                return Field;
            }
            if (tileType == TileTypes.Field1Veg)
            {
                return Field;
            }
            if (tileType == TileTypes.SmallFence)
            {
                return SmallFence;
            }
            if (tileType == TileTypes.BigFence1)
            {
                return BigFence1;
            }
            if (tileType == TileTypes.BigFence2)
            {
                return BigFence2;
            }

            if (tileType == TileTypes.Stable)
            {
                gameObject.transform.localScale = new Vector3(0.4f, 0.4f);//gameObject.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta * 0.5f;
                var parentWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.3f*parentWidth, -0.3f*parentWidth);
                return Stable;
            }

            if (tileType == BuildingTypes.Dwelling)
                return Dwelling;
            if (tileType == BuildingTypes.MixedDwelling)
                return MixedDwelling;
            if (tileType == BuildingTypes.Carpenter)
                return Carpenter;
            if (tileType == BuildingTypes.Miner)
                return Miner;
            if (tileType == BuildingTypes.SimpleDwelling1)
                return SimpleDwelling1;
            if (tileType == BuildingTypes.CoupleDwelling)
                return CoupleDwelling;
            if (tileType == BuildingTypes.StoneCarver)
                return StoneCarver;
            if (tileType == BuildingTypes.Builder)
                return Builder;
            if (tileType == BuildingTypes.SimpleDwelling2)
                return SimpleDwelling2;
            if (tileType == BuildingTypes.AdditionalDwelling)
                return AdditionalDwelling;
            if (tileType == BuildingTypes.Blacksmith)
                return Blacksmith;
            if (tileType == BuildingTypes.Trader)
                return Trader;
            if (tileType == BuildingTypes.CuddleRoom)
                return CuddleRoom;
            if (tileType == BuildingTypes.WorkRoom)
                return WorkRoom;
            if (tileType == BuildingTypes.WoodSupplier)
                return WoodSupplier;
            if (tileType == BuildingTypes.StoneSupplier)
                return StoneSupplier;
            if (tileType == BuildingTypes.DogSchool)
                return DogSchool;
            if (tileType == BuildingTypes.BreakfastRoom)
                return BreakfastRoom;
            if (tileType == BuildingTypes.GuestRoom)
                return StoneSupplier;
            if (tileType == BuildingTypes.Quarry)
                return Quarry;
            if (tileType == BuildingTypes.StubbleRoom)
                return StubbleRoom;
            if (tileType == BuildingTypes.OfficeRoom)
                return OfficeRoom;
            if (tileType == BuildingTypes.RubySupplier)
                return RubySupplier;
            if (tileType == BuildingTypes.Seam)
                return Seam;
            if (tileType == BuildingTypes.SlaughteringCave)
                return SlaughteringCave;
            if (tileType == BuildingTypes.MiningCave)
                return MiningCave;
            if (tileType == BuildingTypes.StoneStorage)
                return StoneStorage;
            if (tileType == BuildingTypes.MainStorage)
                return MainStorage;
            if (tileType == BuildingTypes.CookingCave)
                return CookingCave;
            if (tileType == BuildingTypes.BreedingCave)
                return BreedingCave;
            if (tileType == BuildingTypes.OreStorage)
                return OreStorage;
            if (tileType == BuildingTypes.WeaponStorage)
                return WeaponStorage;
            if (tileType == BuildingTypes.WorkingCave)
                return WorkingCave;
            if (tileType == BuildingTypes.PeacefulCave)
                return PeacefulCave;
            if (tileType == BuildingTypes.SparePartStorage)
                return SparePartStorage;
            if (tileType == BuildingTypes.SuppliesStorage)
                return SuppliesStorage;
            if (tileType == BuildingTypes.WeavingParlour)
                return WeavingParlour;
            if (tileType == BuildingTypes.HuntingParlour)
                return HuntingParlour;
            if (tileType == BuildingTypes.BroomChamber)
                return BroomChamber;
            if (tileType == BuildingTypes.PrayerChamber)
                return PrayerChamber;
            if (tileType == BuildingTypes.MilkingParlour)
                return MilkingParlour;
            if (tileType == BuildingTypes.BeerParlour)
                return BeerParlour;
            if (tileType == BuildingTypes.TreasureChamber)
                return TreasureChamber;
            if (tileType == BuildingTypes.WritingChamber)
                return WritingChamber;
            if (tileType == BuildingTypes.StateParlour)
                return StateParlour;
            if (tileType == BuildingTypes.BlacksmithingParlour)
                return BlacksmithingParlour;
            if (tileType == BuildingTypes.FoodChamber)
                return FoodChamber;
            if (tileType == BuildingTypes.FodderChamber)
                return FodderChamber;
            
            if (tileType == BuildingTypes.Unavailable)
            {
                return Unavailable;
            }

            return null;
        }

        public string GetTileType()
        {
            return _tileType;
        }

        public void SetTaken()
        {
            //draw a dwarf icon on it
            GameObject dwarfToken = (GameObject)Instantiate(Resources.Load("DwarfIconUI"));
            dwarfToken.transform.SetParent(transform, true);
            dwarfToken.transform.localScale = transform.localScale;
            dwarfToken.transform.position = transform.position;
            var tileSize = transform.GetComponent<RectTransform>().rect.width;
            var spriteSize = new Vector2(0.5f*tileSize, 0.5f*tileSize);
            dwarfToken.GetComponent<RectTransform>().sizeDelta = spriteSize;
            dwarfToken.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }
}
