using System.Collections.Generic;
using UnityEngine;

namespace Assets.UIScripts
{
    public class ActionBoardScript : MonoBehaviour
    {

        private List<GameObject> _harvestTokens;
        private SpriteRenderer _renderer;
        private bool _setTokensFlag;
        private List<string> _harvestTokenStatus;

        // Use this for initialization
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _renderer = gameObject.GetComponent<SpriteRenderer>();

            Vector2 parentPosition = gameObject.transform.position;
            float boardHeight = _renderer.bounds.size.y;
            float boardWidth = _renderer.bounds.size.x;
            
            float x = parentPosition.x - boardWidth /9.5f;
            float y = parentPosition.y - boardHeight / 2.2f;

            _harvestTokens = new List<GameObject>();
            for (int i = 0; i < 7; i++)
            {
                int xShift = (i+2)/3; //0, 1, 1, 1, 2, 2, 2
                int yShift = 2-((i+2)%3); //0, 2, 1, 0, 2, 1, 0
                GameObject harvestToken = (GameObject)Instantiate(Resources.Load("HarvestToken"));
                harvestToken.transform.parent = transform.parent;
                harvestToken.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                harvestToken.transform.position = new Vector2(x + xShift * boardWidth / 4, y + yShift*boardHeight/3.2f);

                _harvestTokens.Add(harvestToken);
            }
        }

        // Update is called once per frame
        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if (_setTokensFlag)
                SetHarvestTokens(_harvestTokenStatus);
        }

        public void SetHarvestTokens(List<string> harvestTokenStatus)
        {
            _setTokensFlag = true;
            _harvestTokenStatus = harvestTokenStatus;

            if (_harvestTokens == null || _harvestTokens.Count == 0)
            {
                return;
            }
            for (int i = 0; i < harvestTokenStatus.Count; i++)
            {
                _harvestTokens[i].GetComponent<HarvestTokenScript>().SetType(harvestTokenStatus[i]);
            }
            _setTokensFlag = false;
        }
    }
}
