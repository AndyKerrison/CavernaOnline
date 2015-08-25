using UnityEngine;

namespace Assets.UIScripts
{
    public class ClickableScript : MonoBehaviour {

        public void OnMouseDown()
        {
            Debug.Log("clickable clicked");
            CavernaGame.Instance.GetComponent<CavernaGame>().GetFoodActions();
        }
    }
}
