using System;
using UnityEngine;

namespace Assets.UIScripts
{
    public class SideBarPanelScript : MonoBehaviour
    {
        private GameObject _woodCount;
        private GameObject _oreCount;
        private GameObject _stoneCount;
        private GameObject _foodCount;
        private GameObject _goldCount;
        private GameObject _grainCount;
        private GameObject _vegCount;
        private GameObject _rubyCount;
        private GameObject _sheepCount;
        private GameObject _donkeyCount;
        private GameObject _pigCount;
        private GameObject _cowCount;
        private GameObject _beggingCount;
        private GameObject _scoreCount;
    
        // Use this for initialization
        void Start ()
        {
            int numIcons = 14;
            var parentWidth = transform.GetComponent<RectTransform>().rect.width;
            var parentHeight = transform.GetComponent<RectTransform>().rect.height;
            var spriteHeight = Math.Min(0.8f*parentWidth, parentHeight/(numIcons+1)); //+1 gives a bit of spacing
            var spriteSize = new Vector2(spriteHeight, spriteHeight);
            var verticalIncrement = parentHeight/numIcons;
            var verticalOffset = 0;
            var horizontalOffset = 0.5f*parentWidth - 0.5f*spriteHeight;

            _woodCount = InitIcon(ResourceTypes.Wood, spriteSize);
            _oreCount = InitIcon(ResourceTypes.Ore, spriteSize);
            _stoneCount = InitIcon(ResourceTypes.Stone, spriteSize);
            _foodCount = InitIcon(ResourceTypes.Food, spriteSize);
            _goldCount = InitIcon(ResourceTypes.Gold, spriteSize);
            _grainCount = InitIcon(ResourceTypes.Grain, spriteSize);
            _vegCount = InitIcon(ResourceTypes.Veg, spriteSize);
            _rubyCount = InitIcon(ResourceTypes.Ruby, spriteSize);
            _rubyCount.GetComponent<ResourceIcon>().SetClickable();

            _sheepCount = InitIcon(ResourceTypes.Sheep, spriteSize);
            _donkeyCount = InitIcon(ResourceTypes.Donkeys, spriteSize);
            _pigCount = InitIcon(ResourceTypes.Pigs, spriteSize);
            _cowCount = InitIcon(ResourceTypes.Cows, spriteSize);

            _beggingCount = InitIcon(ResourceTypes.Begging, spriteSize);
            _scoreCount = InitIcon(ResourceTypes.ScoreMarker, spriteSize);

            _woodCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset);
            _oreCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - verticalIncrement);
            _stoneCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 2 * verticalIncrement);
            _foodCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 3 * verticalIncrement);
            _goldCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 4 * verticalIncrement);
            _grainCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 5 * verticalIncrement);
            _vegCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 6 * verticalIncrement);
            _rubyCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 7 * verticalIncrement);
            _sheepCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 8 * verticalIncrement);
            _donkeyCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 9 * verticalIncrement);
            _pigCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 10 * verticalIncrement);
            _cowCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 11 * verticalIncrement);
            _beggingCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 12 * verticalIncrement);
            _scoreCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 13 * verticalIncrement);
        }

        private GameObject InitIcon(string resourceType, Vector2 spriteSize)
        {
            GameObject icon = ResourceIcon.Create(resourceType, true);
            icon.transform.SetParent(transform, true);
            icon.transform.localScale = transform.localScale;
            //icon.transform.position = transform.position;
            spriteSize.y = (spriteSize.x/icon.GetComponent<RectTransform>().sizeDelta.x)*
                           icon.GetComponent<RectTransform>().sizeDelta.y;
            icon.GetComponent<RectTransform>().sizeDelta = spriteSize;
            return icon;
        }

        public void SetResources(string resourceName, int count)
        {
            if (resourceName == ResourceTypes.Wood)
                _woodCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Ore)
                _oreCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Stone)
                _stoneCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Food)
                _foodCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Gold)
                _goldCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Grain)
                _grainCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Veg)
                _vegCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Ruby)
                _rubyCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Sheep)
                _sheepCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Donkeys)
                _donkeyCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Pigs)
                _pigCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Cows)
                _cowCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.Begging)
                _beggingCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.ScoreMarker)
                _scoreCount.GetComponent<ResourceIcon>().SetValue(count);
        }
    }
}
