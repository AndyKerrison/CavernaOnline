using UnityEngine;

namespace Assets.UIScripts
{
    public class HarvestTokenScript : MonoBehaviour
    {

        public Sprite HarvestSprite;
        public Sprite UnknownHarvestSprite;
        public Sprite SpecialHarvestSprite;

        private Sprite _currentSprite;
        private SpriteRenderer _renderer;

        // Use this for initialization
        void Start () {
            _renderer = gameObject.GetComponent<SpriteRenderer>();
        }
	
        // Update is called once per frame
        void Update () {
            _renderer.sprite = _currentSprite;
        }

        public void SetType(string harvestTokenStatus)
        {
            if (harvestTokenStatus == HarvestTypes.Harvest)
            {
                _currentSprite = HarvestSprite;
            }
            if (harvestTokenStatus == HarvestTypes.Unknown)
            {
                _currentSprite = UnknownHarvestSprite;
            }
            if (harvestTokenStatus == HarvestTypes.Special)
            {
                _currentSprite = SpecialHarvestSprite;
            }
        }
    }
}
