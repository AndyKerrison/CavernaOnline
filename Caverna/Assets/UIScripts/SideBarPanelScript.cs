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
        private GameObject _beggingCount;
        private GameObject _scoreCount;
    
        // Use this for initialization
        void Start ()
        {
            var parentWidth = transform.GetComponent<RectTransform>().rect.width;
            var parentHeight = transform.GetComponent<RectTransform>().rect.height;
            var spriteSize = new Vector2(parentWidth / 1.2f, parentHeight / 8f);
            var verticalIncrement = parentHeight/11f;
            var verticalOffset = transform.GetComponent<RectTransform>().anchoredPosition.y + parentHeight / 20f;
            var horizontalOffset = transform.GetComponent<RectTransform>().anchoredPosition.x - parentWidth/10f;

            _woodCount = InitIcon(ResourceTypes.Wood, spriteSize);
            _oreCount = InitIcon(ResourceTypes.Ore, spriteSize);
            _stoneCount = InitIcon(ResourceTypes.Stone, spriteSize);
            _foodCount = InitIcon(ResourceTypes.Food, spriteSize);
            _goldCount = InitIcon(ResourceTypes.Gold, spriteSize);
            _grainCount = InitIcon(ResourceTypes.Grain, spriteSize);
            _vegCount = InitIcon(ResourceTypes.Veg, spriteSize);
            _rubyCount = InitIcon(ResourceTypes.Ruby, spriteSize);
            _rubyCount.GetComponent<ResourceIcon>().SetClickable();
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
            _beggingCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 8 * verticalIncrement);
            _scoreCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalOffset, verticalOffset - 9 * verticalIncrement);
        }

        private GameObject InitIcon(string resourceType, Vector2 spriteSize)
        {
            GameObject icon = ResourceIcon.Create(resourceType, true);
            icon.transform.SetParent(transform, true);
            icon.transform.localScale = transform.localScale;
            icon.transform.position = transform.position;
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

            if (resourceName == ResourceTypes.Begging)
                _beggingCount.GetComponent<ResourceIcon>().SetValue(count);

            if (resourceName == ResourceTypes.ScoreMarker)
                _scoreCount.GetComponent<ResourceIcon>().SetValue(count);
        }
    }
}
