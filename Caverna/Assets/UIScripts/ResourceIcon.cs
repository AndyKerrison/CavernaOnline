using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIScripts
{
    public class ResourceIcon : MonoBehaviour {

        public GameObject resourceText;
        public Sprite stoneSprite;
        public Sprite oreSprite;
        public Sprite foodSprite;
        public Sprite goldSprite;
        public Sprite grainSprite;
        public Sprite VegSprite;
        public Sprite RubySprite;
        public Sprite DogSprite;
        public Sprite SheepSprite;
        public Sprite DonkeySprite;
        public Sprite PigSprite;
        public Sprite CowSprite;
        public Sprite BeggingSprite;
        public Sprite ScoreSprite;

        private bool _init;
        private int _value;
        private string _type;

        private BoxCollider2D _box;

        public static GameObject Create(string type, bool showOnZeroValue)
        {
            GameObject instance = (GameObject)Instantiate(Resources.Load("ResourceIconUI"));
            instance.GetComponent<ResourceIcon>().SetType(type);
            instance.GetComponent<ResourceIcon>().ShowOnZeroValue = showOnZeroValue;
            
            return instance;
        }

        public void SetType(string type)
        {
            _type = type;
            if (type == ResourceTypes.Stone) {
                SetSprite(stoneSprite);
            }

            if (type == ResourceTypes.Ore) {
                SetSprite(oreSprite);
            }

            if (type == ResourceTypes.Food) {
                SetSprite(foodSprite);
            }

            if (type == ResourceTypes.Gold)
            {
                SetSprite(goldSprite);
            }

            if (type == ResourceTypes.Grain)
            {
                SetSprite(grainSprite);
            }

            if (type == ResourceTypes.Veg)
            {
                SetSprite(VegSprite);
            }

            if (type == ResourceTypes.Ruby)
            {
                SetSprite(RubySprite);
            }

            if (type == ResourceTypes.Dogs)
            {
                SetSprite(DogSprite);
            }

            if (type == ResourceTypes.Sheep)
            {
                SetSprite(SheepSprite);
            }

            if (type == ResourceTypes.Donkeys)
            {
                SetSprite(DonkeySprite);
            }

            if (type == ResourceTypes.Pigs)
            {
                SetSprite(PigSprite);
            }

            if (type == ResourceTypes.Cows)
            {
                SetSprite(CowSprite);
            }

            if (type == ResourceTypes.Begging)
            {
                SetSprite(BeggingSprite);
            }

            if (type == ResourceTypes.ScoreMarker)
            {
                SetSprite(ScoreSprite);
            }
        }

        public void SetValue(int newValue)
        {
            _value = newValue;
            if (resourceText != null)
            {
                resourceText.GetComponent<Text>().text = _value.ToString(CultureInfo.InvariantCulture);
                //resourceText.SetActive(newValue != 0);
            }
            gameObject.SetActive(newValue != 0 || ShowOnZeroValue);
        }

        public bool ShowOnZeroValue { get; set; }

        // Update is called once per frame
// ReSharper disable once UnusedMember.Local
        void Update () {
            if (!_init) {
                float x = gameObject.transform.position.x;
                float y = gameObject.transform.position.y;
                //resourceText.transform.SetParent(transform.parent);
                //resourceText.transform.localScale = transform.localScale;// new Vector3(transform.localScale.x * 0.2f, transform.localScale.y * 0.2f, transform.localScale.z * 0.2f);
                //resourceText.transform.position = transform.position;// new Vector3(x-0.1f, y+0.25f, 0f);
                SetValue(_value);
                _init = true;
            }
        }

        public void SetClickable()
        {
            _box = new BoxCollider2D();
            gameObject.AddComponent<BoxCollider2D>();
        }

        void OnMouseDown()
        {
            //do action if possible
            if (_type == ResourceTypes.Ruby)
            {
                CavernaGame.ClientSocket.GetRubyActions("Player");
            }
        }

        private void SetSprite(Sprite sprite)
        {
            if (GetComponent<Image>() != null)
            GetComponent<Image>().sprite = sprite;
        }
    }
}
