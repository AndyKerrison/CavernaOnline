using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIScripts
{
    public class ActionSpace : MonoBehaviour {

        public int ActionID;

        public Sprite SkipRound;

        public Sprite DriftMining;
        public Sprite Excavation;
        public Sprite StartingPlayer;
        public Sprite Logging;
        public Sprite Supplies;
        public Sprite OreMining;
        public Sprite WoodGathering;
        public Sprite Clearing;
        public Sprite Sustenance;
        public Sprite RubyMining;
        public Sprite HouseWork;
        public Sprite SlashAndBurn;

        public Sprite Blacksmithing;
        public Sprite OreMineConstruction;
        public Sprite SheepFarming;

        public Sprite DonkeyFarming;
        public Sprite RubyMine;
        public Sprite WishForChildren;
        public Sprite UrgentWish;
        
        public Sprite Exploration;
        public Sprite FamilyLife;
        public Sprite OreDelivery;
        
        public Sprite Adventure;
        public Sprite OreTrading;
        public Sprite RubyDelivery;

        private GameObject _dwarfToken;

        public GameObject woodSprite;
        private GameObject _stoneSprite;
        private GameObject _oreSprite;
        private GameObject _foodSprite;
        private GameObject _rubySprite;
        private GameObject _sheepSprite;
        private GameObject _donkeySprite;

        private int _wood;
        private int _stone;
        private int _ore;
        private int _food;
        private int _rubies;
        private int _sheep;
        private int _donkeys;

        public string _type;

        private bool _spritesInit;
        private bool _isActive;
        private bool _isMouseOver;

        public static GameObject Create(int actionID, string type)
        {
            GameObject instance = (GameObject)Instantiate(Resources.Load("ActionCardUI"));
            instance.GetComponent<ActionSpace>()._type = type;
            instance.GetComponent<ActionSpace>().ActionID = actionID;

            return instance;
        }

        // Use this for initialization
// ReSharper disable once UnusedMember.Local
        void Start () {
            SetType ();
            InitialiseSprites();
        }

        private void SetType()
        {
            switch (_type) {
                case "driftMining":
                {
                    SetSprite(DriftMining);
                    break;
                }
                case "excavation":
                {
                    SetSprite(Excavation);
                    break;
                }
                case "startingPlayer":
                {
                    SetSprite(StartingPlayer);
                    break;
                }
                case "logging":
                {
                    SetSprite(Logging);
                    break;
                }
                case "supplies":
                {
                    SetSprite(Supplies);
                    break;
                }
                case "oreMining":
                {
                    SetSprite(OreMining);
                    break;
                }
                case "woodGathering":
                {
                    SetSprite(WoodGathering);
                    break;
                }
                case "clearing":
                {
                    SetSprite(Clearing);
                    break;
                }
                case "sustenance":
                {
                    SetSprite(Sustenance);
                    break;
                }
                case "rubyMining":
                {
                    SetSprite(RubyMining);
                    break;
                }
                case "housework":
                {
                    SetSprite(HouseWork);
                    break;
                }
                case "slashAndBurn":
                {
                    SetSprite(SlashAndBurn);
                    break;
                }
                case ActionSpaceTypes.Blacksmithing:
                {
                    SetSprite(Blacksmithing);
                    break;
                }
                case ActionSpaceTypes.OreMineConstruction:
                {
                    SetSprite(OreMineConstruction);
                    break;
                }
                case ActionSpaceTypes.SheepFarming:
                {
                    SetSprite(SheepFarming);
                    break;
                }
                case ActionSpaceTypes.DonkeyFarming:
                {
                    SetSprite(DonkeyFarming);
                    break;
                }
                case ActionSpaceTypes.RubyMine:
                {
                    SetSprite(RubyMine);
                    break;
                }
                case ActionSpaceTypes.WishForChildren:
                {
                    SetSprite(WishForChildren);
                    break;
                }
                case ActionSpaceTypes.UrgentWish:
                {
                    SetSprite(UrgentWish);
                    break;
                }
                case ActionSpaceTypes.Exploration:
                {
                    SetSprite(Exploration);
                    break;
                }
                case ActionSpaceTypes.FamilyLife:
                {
                    SetSprite(FamilyLife);
                    break;
                }
                case ActionSpaceTypes.OreDelivery:
                {
                    SetSprite(OreDelivery);
                    break;
                }
                case ActionSpaceTypes.Adventure:
                {
                    SetSprite(Adventure);
                    break;
                }
                case ActionSpaceTypes.OreTrading:
                {
                    SetSprite(OreTrading);
                    break;
                }
                case ActionSpaceTypes.RubyDelivery:
                {
                    SetSprite(RubyDelivery);
                    break;
                }
                case ActionSpaceTypes.SkipRound:
                {
                    SetSprite(SkipRound);
                    break;
                }
            }
        }

        private void SetSprite(Sprite sprite)
        {
            GetComponent<Image>().sprite = sprite;
        }

        private void InitialiseSprites()
        {
            //float x = gameObject.transform.position.x;
            //float y = gameObject.transform.position.y;
		
            woodSprite = InitSprite(ResourceTypes.Wood, true, transform);
            _stoneSprite = InitSprite(ResourceTypes.Stone, true, transform);
            _oreSprite = InitSprite(ResourceTypes.Ore, true, transform);

            _foodSprite = InitSprite(ResourceTypes.Food, true, transform);
            _rubySprite = InitSprite(ResourceTypes.Ruby, true, transform);
            _sheepSprite = InitSprite(ResourceTypes.Sheep, true, transform);
            _donkeySprite = InitSprite(ResourceTypes.Donkeys, true, transform);

            //if (_type == ActionSpaceTypes.OreDelivery)
            //{
            //    _stoneSprite.transform.position = new Vector3(_stoneSprite.transform.position.x, _stoneSprite.transform.position.y-0.5f, 0f);
            //}

            //GameObject instance2 = (GameObject)Instantiate(Resources.Load("wood"));
            //woodSprite.transform.position = new Vector3 (x, y, 0f);		
            //Instantiate(wood, new Vector3 (x, y, 0f), Quaternion.identity);
            _spritesInit = true;

            if (_wood > 0)
                SetWood (_wood);

            if (_stone > 0)
                SetStone (_stone);

            if (_ore > 0)
                SetOre (_ore);

            if (_food > 0)
                SetFood (_food);

            if (_rubies > 0)
                SetRubies(_rubies);

            if (_sheep > 0)
                SetSheep(_sheep);

            if (_donkeys > 0)
                SetDonkeys(_donkeys);

            _dwarfToken = (GameObject)Instantiate(Resources.Load("DwarfIconUI"));
            //_dwarfToken.transform.parent = transform;
            //_dwarfToken.transform.position = new Vector2(x, y);
            _dwarfToken.transform.SetParent(transform, true);
            _dwarfToken.transform.localScale = transform.localScale;
            _dwarfToken.transform.position = transform.position;
            var cardWidth = transform.GetComponent<RectTransform>().rect.width;
            var cardHeight = transform.GetComponent<RectTransform>().rect.height;
            var spriteSize = new Vector2(cardWidth / 2.5f, cardHeight / 2.5f);
            _dwarfToken.GetComponent<RectTransform>().sizeDelta = spriteSize;
            _dwarfToken.GetComponent<RectTransform>().anchoredPosition = new Vector2(_dwarfToken.GetComponent<RectTransform>().anchoredPosition.x + spriteSize.x*1.3f, _dwarfToken.GetComponent<RectTransform>().anchoredPosition.y - 1.5f* spriteSize.y);
            _dwarfToken.SetActive(false);
            //_dwarfToken.GetComponent<DwarfGameObject>().SetActive(false);
        }

        private static GameObject InitSprite(string resourceType, bool isTopOfCard, Transform transform)
        {
            var x2 = transform.GetComponent<RectTransform>().anchoredPosition.x;
            var y2 = transform.GetComponent<RectTransform>().anchoredPosition.y;

            var cardWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
            var cardHeight = transform.GetComponent<RectTransform>().sizeDelta.y;
            GameObject sprite = (GameObject)Instantiate(Resources.Load("ResourceIconUI"));
            sprite.GetComponent<ResourceIcon>().SetType(resourceType);
            sprite.transform.SetParent(transform.parent, true);
            sprite.transform.localScale = transform.localScale;
            //sprite.transform.position = new Vector3(transform.position.x + 0.0f*cardWidth, transform.position.y- 0.0f*cardHeight);
            var spriteSize = new Vector2(cardWidth/4, cardHeight/4);
            sprite.GetComponent<RectTransform>().sizeDelta = spriteSize;
            //sprite.GetComponent<RectTransform>().anchoredPosition = new Vector2(sprite.GetComponent<RectTransform>().anchoredPosition.x + 3.5f * spriteSize.x, sprite.GetComponent<RectTransform>().anchoredPosition.y - CavernaGame.Instance.GetComponent<CavernaGame>().cardScale * 60);
            sprite.GetComponent<RectTransform>().anchoredPosition = new Vector2(x2 + 0.6f*cardWidth, y2-0.15f*cardHeight);

            //var xNew = sprite.transform.position.x;
            //var x2New = sprite.GetComponent<RectTransform>().anchoredPosition.x;
            //var yNew = sprite.transform.position.y;
            //var y2New = sprite.GetComponent<RectTransform>().anchoredPosition.y;
            return sprite;
        }

        private void SetWood(int wood)
        {	
            _wood = wood;
            if (_spritesInit)
                woodSprite.GetComponent<ResourceIcon> ().SetValue(wood);
        }

        private void SetStone(int stone)
        {	
            _stone = stone;
            if (_spritesInit)
                _stoneSprite.GetComponent<ResourceIcon> ().SetValue(stone);
        }

        private void SetOre(int ore)
        {	
            _ore = ore;
            if (_spritesInit)
                _oreSprite.GetComponent<ResourceIcon> ().SetValue(ore);
        }

        private void SetFood(int food)
        {	
            _food = food;
            if (_spritesInit)
                _foodSprite.GetComponent<ResourceIcon> ().SetValue(food);
        }

        private void SetRubies(int rubies)
        {
            _rubies = rubies;
            if (_spritesInit)
                _rubySprite.GetComponent<ResourceIcon>().SetValue(rubies);
        }

        private void SetSheep(int sheep)
        {
            _sheep = sheep;
            if (_spritesInit)
                _sheepSprite.GetComponent<ResourceIcon>().SetValue(sheep);
        }

        private void SetDonkeys(int donkeys)
        {
            _donkeys = donkeys;
            if (_spritesInit)
                _donkeySprite.GetComponent<ResourceIcon>().SetValue(donkeys);
        }

        public void SetActive()
        {
            StartCoroutine(SetSpaceActive());
        }

        public IEnumerator SetSpaceActive()
        {
            yield return new WaitForSeconds(0.1f);
            _isActive = true;
        }

        public void OnMouseDown() {
            Debug.Log("click detected on " + _type);
            
            //do action if possible
            if (_isActive) {
                Debug.Log("triggering " + _type);
                _isActive = false;
                CavernaGame.Instance.GetComponent<CavernaGame>().SetChosenActionSpace("Player", ActionID);
            }
        }

        public void OnGUI()
        {
            if (_isMouseOver && _isActive)
                GetComponent<Image>().canvasRenderer.SetColor(Color.green);
            else
            {
                GetComponent<Image>().canvasRenderer.SetColor(Color.white);
            }
        }

        public void OnMouseEnter()
        {
            _isMouseOver = true;
        }

        public void OnMouseExit()
        {
            _isMouseOver = false;
        }

        public void ShowActionSpaceDwarf()
        {
            //_dwarfToken.enabled = true;
            _dwarfToken.SetActive(true);
        }

        public void HideActionSpaceDwarf()
        {
            //_dwarfToken.enabled = false;
            if (_dwarfToken != null)
                _dwarfToken.SetActive(false);
        }

        public void SetInactive()
        {
            _isActive = false;
        }

        public void SetResources(string resType, int count)
        {
            if (resType == ResourceTypes.Wood)
                SetWood(count);

            if (resType == ResourceTypes.Stone)
                SetStone(count);

            if (resType == ResourceTypes.Ore)
                SetOre(count);

            if (resType == ResourceTypes.Food)
                SetFood(count);

            if (resType == ResourceTypes.Ruby)
                SetRubies(count);

            if (resType == ResourceTypes.Sheep)
                SetSheep(count);

            if (resType == ResourceTypes.Donkeys)
                SetDonkeys(count);
        }
    }
}
