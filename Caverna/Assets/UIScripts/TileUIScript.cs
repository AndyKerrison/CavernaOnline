using System.Collections.Generic;
using UnityEngine;
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
        
        public int X;
        public int Y;

        private bool _isCave;
        private bool _clickable;

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

            var parentWidth = transform.GetComponent<RectTransform>().rect.width;
            var parentHeight = transform.GetComponent<RectTransform>().rect.height;
            var spriteSize = new Vector2(parentWidth / 2f, parentHeight / 2f);
            //var verticalOffset = transform.GetComponent<RectTransform>().anchoredPosition.y + 0*parentHeight;
            //var horizontalOffset = transform.GetComponent<RectTransform>().anchoredPosition.x - 0*parentWidth;

            _icon = ResourceIcon.Create(ResourceTypes.Grain, false);
            _icon.transform.SetParent(transform, true);
            _icon.transform.localScale = transform.localScale;
            _icon.transform.position = new Vector3(transform.position.x + parentWidth/2f, transform.position.y + parentHeight/2f);
            spriteSize.y = (spriteSize.x / _icon.GetComponent<RectTransform>().sizeDelta.x) *
                           _icon.GetComponent<RectTransform>().sizeDelta.y;
            _icon.GetComponent<RectTransform>().sizeDelta = spriteSize;

            _dogIcon = ResourceIcon.Create(ResourceTypes.Dogs, false);
            _dogIcon.transform.SetParent(transform, true);
            _dogIcon.transform.localScale = transform.localScale;
            _dogIcon.transform.position = new Vector3(transform.position.x +  0*parentWidth, transform.position.y + 0*parentHeight);
            spriteSize.y = (spriteSize.x / _dogIcon.GetComponent<RectTransform>().sizeDelta.x) *
                           _dogIcon.GetComponent<RectTransform>().sizeDelta.y;
            _dogIcon.GetComponent<RectTransform>().sizeDelta = spriteSize;
            //_icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset);
        }

        public void SetIsCave()
        {
            _isCave = true;
        }

        public void SetType(List<string> tileTypes)
        {
            SetUnclickable();
            //SortingOrder = 2-tileTypes.Count;

            string tileType = tileTypes[0];

            _mainSprite = GetSpriteByType(tileType);

            _activeSprite = _mainSprite;
            _activeColor = _activeSprite != null ? Color.white : _hiddenColor;
            
            if (_icon != null)
                _icon.GetComponent<ResourceIcon>().SetValue(0);
            else
                return;

            if (tileType == TileTypes.Field3Grain)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Grain);
                _icon.GetComponent<ResourceIcon>().SetValue(3);
            }
            if (tileType == TileTypes.Field2Grain)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Grain);
                _icon.GetComponent<ResourceIcon>().SetValue(2);
            }
            if (tileType == TileTypes.Field1Grain)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Grain);
                _icon.GetComponent<ResourceIcon>().SetValue(1);
            }
            if (tileType == TileTypes.Field2Veg)
            {
                //will already have field sprite.... draw 3 grain on top
                _icon.GetComponent<ResourceIcon>().SetType(ResourceTypes.Veg);
                _icon.GetComponent<ResourceIcon>().SetValue(2);
            }
            if (tileType == TileTypes.Field1Veg)
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
            gameObject.GetComponent<CanvasGroup>().interactable = false;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            //_isClickableBuilding = false;
        }

        public void SetClickable(string tileType, bool isParent)
        {
            gameObject.GetComponent<CanvasGroup>().interactable = true;
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //_renderSortingOrder = 5;

            if (tileType == TileTypes.Stable && isParent)
            {
                InitChildTile(tileType);
                _childTile.GetComponent<TileUIScript>().SetClickable(tileType, false);
            }
            else if (tileType == BuildingTypes.Dwelling && isParent)
            {
                InitChildTile(tileType);
                _childTile.GetComponent<TileUIScript>().SetClickable(tileType, false);
            }
            else
            {
                _clickable = true;
                _mouseOverSprite = GetSpriteByType(tileType);
            }
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
                var parentWidth = transform.GetComponent<RectTransform>().rect.width;
                var parentHeight = transform.GetComponent<RectTransform>().rect.height;
                _icon.transform.position = new Vector3(transform.position.x +0.4f*parentWidth / 2f, transform.position.y + 1.6f*parentHeight / 2f);
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
            _childTile.GetComponent<TileUIScript>().SetClickable(tileType, false);
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
                bool isBuilding = _mouseOverSprite == Dwelling;
                ClientSocket.Instance.SetTileClicked("Player", new Vector2(X, Y), _isCave, isBuilding);
            }
        }

        // ReSharper disable once UnusedMember.Local
        public void OnMouseEnter()
        {
            if (_clickable)
            {
                _activeSprite = _mouseOverSprite;
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
                return Stable;
            }

            if (tileType == BuildingTypes.Dwelling)
            {
                return Dwelling;
            }

            return null;
        }
    }
}
