using UnityEngine;

namespace Assets.UIScripts
{
    public class ClickableScript : MonoBehaviour {

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void OnMouseDown()
        {
            Debug.Log("clickable clicked");
            CavernaGame.Instance.GetComponent<CavernaGame>().GetFoodActions();
        }
    }
}
