using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIScripts
{
    public class CavernaGame : MonoBehaviour {

        public static GameObject Instance;
        
        private readonly List<GameObject> _actionSpaces = new List<GameObject>();

        public Canvas LoaderCanvas;

        public static IClientSocket ClientSocket;

        private bool _showModal;
        private List<string> _actions;
        private string _actionName;
        private string _helpText;

        public GameObject MainActionsDisplayBoard;
        public Canvas GameBoard;
        public Canvas ActionBoard;
        public GameObject ActionsPanel;
        public GameObject ActionsPanelView;
        public Scrollbar ActionPanelScrollbar;
        public Canvas MainCanvas;
        public Image Image;

        // Use this for initialization
// ReSharper disable once UnusedMember.Local
        void Start () {	
            Instance = gameObject;
            //_loaderImage = GameObject.Find("LoaderImage");
            //_levelText = GameObject.Find("LevelText").GetComponent<Text>();
            ClientSocket = UIScripts.ClientSocket.Instance;
            ClientSocket.StartGame(1);
            GameBoard.enabled = false;
            ActionBoard.enabled = true;
            ActionPanelScrollbar.value = 0;
        }

        public void ShowPlayerBoard()
        {
            ActionBoard.enabled = false;
            GameBoard.enabled = true;
            //foreach (GameObject actionSpace in _actionSpaces)
            //{
            //    actionSpace.GetComponent<Renderer>().enabled = false;
            //}
        }

        public void ShowActionBoard()
        {
            GameBoard.enabled = false;
            ActionBoard.enabled = true;
        }

        public void StartGameRound(int gameRound)
        {
            //can't delay this at the mo unless we also delay card updates etc
            //_levelText.text = "Round " + gameRound;
            Invoke("ShowLevelImage", 0);
        }

// ReSharper disable once UnusedMember.Local
        private void ShowLevelImage()
        {
            LoaderCanvas.enabled = true;
            Invoke("HideLevelImage", 2);
        }

// ReSharper disable once UnusedMember.Local
        private void HideLevelImage()
        {
            LoaderCanvas.enabled = false;
        }

        #region action spaces

        private ActionSpace GetActionSpaceByID(int actionID)
        {
            foreach (GameObject actionSpace in _actionSpaces)
            {
                ActionSpace space = actionSpace.GetComponent<ActionSpace>();
                if (space.ActionID == actionID)
                    return space;
            }
            return null;
        }

        private void ResizeActionCards()
        {
            int hPadding = 5;
            int vPadding = 12;
            //int xMaxCards = ((_actionSpaces.Count-1) / 3) + 1;
            int yMaxCards = 3;

            RectTransform actionCanvasRect = ActionsPanel.GetComponent<RectTransform>();
            //var availableX = actionCanvasRect.rect.width - (padding * (xMaxCards + 1));
            var availableY = ActionsPanelView.GetComponent<RectTransform>().rect.height -(vPadding * (yMaxCards + 1));
            //var xSizeMax = availableX / xMaxCards;
            var ySizeMax = availableY / yMaxCards;

/*RectTransform actionCanvasRect = ActionBoard.GetComponent<RectTransform>();
var cardX = _actionSpaces[_actionSpaces.Count - 1].GetComponent<RectTransform>().rect.width;
var cardY = _actionSpaces[_actionSpaces.Count - 1].GetComponent<RectTransform>().rect.height;
var availableX = actionCanvasRect.rect.width - (padding*(xMaxCards + 1));
var availableY = actionCanvasRect.rect.height - (padding*(yMaxCards + 1));
var xScale = availableX / (cardX * xMaxCards);
var yScale = availableY / (cardY * yMaxCards);
cardScale = Math.Min(xScale, yScale);
*/
            //return;

            float cardOffsetX = 0f;
            float cardX = 0;
            //float xScale =0;
            float yScale = 0;

            var maxCards = 24; //for solo game
            var maxWidth = 0f;
            Vector2 cardSize = new Vector2();

            for(int i=0; i<_actionSpaces.Count; i++)
            {
                GameObject space = _actionSpaces[i];
                cardX = space.GetComponent<RectTransform>().rect.width;
                var cardY = space.GetComponent<RectTransform>().rect.height;
                //xScale = xSizeMax / cardX;
                yScale = ySizeMax / cardY;
                cardSize = new Vector2(cardX*yScale, cardY*yScale);
                
                space.GetComponent<RectTransform>().sizeDelta = cardSize;
                cardOffsetX = (float)Math.Floor(i / 3f) * (cardSize.x + hPadding) + hPadding;
                float cardOffsetY = -(i % 3) * (cardSize.y + vPadding) - vPadding;
                space.GetComponent<RectTransform>().anchoredPosition = new Vector2(cardOffsetX, cardOffsetY);

                maxWidth = (float)Math.Floor(maxCards / 3f) * (cardSize.x + hPadding) + hPadding;
            }

            actionCanvasRect.sizeDelta = new Vector2(maxWidth, ActionsPanelView.GetComponent<RectTransform>().rect.height);
            //MainActionsDisplayBoard.GetComponent<RectTransform>().anchoredPosition = new Vector2(actionCanvasRect.anchoredPosition.x + 2*cardSize.x, MainActionsDisplayBoard.GetComponent<RectTransform>().anchoredPosition.y);
            MainActionsDisplayBoard.GetComponent<RectTransform>().anchoredPosition = new Vector2(maxWidth - actionCanvasRect.sizeDelta.x + 2*cardSize.x, MainActionsDisplayBoard.GetComponent<RectTransform>().anchoredPosition.y);
        }

        public void AddActionSpace(int actionID, string actionName)
        {
            GameObject card2 = ActionSpace.Create(actionID, actionName);
            card2.transform.SetParent(ActionsPanel.transform);
            card2.transform.localScale = ActionsPanel.transform.localScale;
            card2.transform.position = ActionsPanel.transform.position;
            _actionSpaces.Add(card2);

            ResizeActionCards();

            /*GameObject instance = ActionSpace.Create(actionID, actionName, ClientSocket);

            //what if I put it in the actionboard canvas instead?
            //instance.transform.parent = ActionBoard.transform;
            instance.transform.position = _nextActionPos;
            _actionSpaces.Add(instance);
            if (_actionSpaces.Count % 3 == 0)
            {
                _nextActionPos.x += 2;
                _nextActionPos.y += 6;
            }
            else
            {
                _nextActionPos.y -= 3;
            }*/
        }

        public void SetActionSpaceResources(int actionID, string resType, int count)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            space.SetResources(resType, count);
        }

        public void SetActionSpaceActive(int actionID)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            space.SetActive();
        }

        public void SetActions(int actionID, List<string> actions)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            DisableActionSpaces();
            GetPlayerChoice("playerID", space._type, string.Empty, actions);
            //space.SetActions(actions);
        }

        public void SetActionCancelled(int actionID)
        {
            HidePlayerChoice("playerID");
            //ActionSpace space = GetActionSpaceByID(actionID);
            //space.SetActions(new List<string>());
            //space.SetActive();
        }

        public void SetActionFinished(int actionID)
        {
            HidePlayerChoice("playerID");
            //ActionSpace space = GetActionSpaceByID(actionID);
            //space.SetActions(new List<string>());
            //space.SetInactive();
        }

        public void SetActionSpaceInactive(int actionID)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            space.SetInactive();
        }

        public void ShowActionSpaceDwarf(int actionID)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            space.ShowActionSpaceDwarf();
        }

        public void HideActionSpaceDwarf(int actionID)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            space.HideActionSpaceDwarf();
        }

        #endregion

        public void SetPlaceTile(string playerid, string tileType, List<Vector2> validSpots, bool isCave)
        {
            Debug.Log("SetPlaceTile " + tileType + " " + validSpots[0] + "...");
            //on the player board, make it highlight green and enable click on cave vectors listed
            foreach (Vector2 validSpot in validSpots)
            {
                GameObject.Find("GameBoard").GetComponent<PlayerBoardScript>().SetClickableTile(validSpot, tileType, isCave);
            }

            ShowPlayerBoard();
            
            //also draw a tunnel icon on the mouse pointer which follows it
            //_mouseCursorTile = (GameObject)Instantiate(Resources.Load("Tile"));
            //_mouseCursorTile.GetComponent<TileGameObject>().SetFollowMouse();
        }

        public void SetPlayerTileType(Vector2 position, List<string> tileType, bool isCave)
        {
            Debug.Log("SetPlayerTileType " + tileType + " " + position.x + "," + position.y);
            GameObject.Find("GameBoard").GetComponent<PlayerBoardScript>().SetTileType(position, tileType, isCave);
        }

        public void SetTilesUnclickable(string playerID)
        {
            GameObject.Find("GameBoard").GetComponent<PlayerBoardScript>().SetTilesUnclickable();
        }

        public void SetPlayerResources(string playerID, string resourceName, int count)
        {
            GameObject sideBar = GameObject.Find("SideBarPanel");
            sideBar.GetComponent<SideBarPanelScript>().SetResources(resourceName, count);

            GameObject p = GameObject.Find("GameBoard");
            p.GetComponent<PlayerBoardScript>().SetResources(resourceName, count);
        }

        public void SetPlaceBuildingTile(string playerid, string buildingType, List<Vector2> validSpots)
        {
            foreach (Vector2 validSpot in validSpots)
            {
                GameObject.Find("GameBoard").GetComponent<PlayerBoardScript>().SetClickableBuildingTile(validSpot, buildingType);
            }

            ShowPlayerBoard();
        }

        public void SetPlayerDwarves(List<string> dwarfStatus)
        {
            GameObject p = GameObject.Find("GameBoard");
            p.GetComponent<PlayerBoardScript>().SetDwarves(dwarfStatus);
        }

// ReSharper disable once UnusedMember.Local
        void OnGUI()
        {
            if (_showModal)
            {
                int width = 320;
                int height = _actions.Count*30 + 25;
                if (_helpText.Length > 0)
                    height += 30;
                GUI.ModalWindow(1, new Rect(Screen.width/2 - width/2, 0.6f*Screen.height/2f, width, height), DoMyWindow, _actionName);
            }
        }

        void DoMyWindow(int windowID)
        {
            int y = 25;
            if (_helpText.Length > 0)
            {
                GUI.Label(new Rect(10, y, 300, 20), _helpText);
                y += 30;
            }
            foreach (var action in _actions)
            {
                if (GUI.Button(new Rect(10, y, 300, 20), action))
                {
                    ClientSocket.SendPlayerChoice(action);
                }
                y += 30;
            }
        }

        public void HidePlayerChoice(string playerid)
        {
            _showModal = false;
            _actions = new List<string>();
        }

        public void GetPlayerChoice(string playerid, string actionName, string text, List<string> options)
        {
            DisableActionSpaces();
            if (!string.IsNullOrEmpty(actionName))
                _actionName = actionName;
            _showModal = true;
            _actions = options;
            _helpText = text;
        }

        public void SetHarvestTokenStatus(List<string> harvestTokenStatus)
        {
            //GameObject p = GameObject.Find("ActionBoard");
            //p.GetComponent<ActionBoardScript>().SetHarvestTokens(harvestTokenStatus);            
        }

        public void ReplaceActionSpace(int actionID, string newActionName)
        {
            ActionSpace space = GetActionSpaceByID(actionID);
            space.ShowActionSpaceDwarf();

            GameObject instance = ActionSpace.Create(actionID, newActionName);
            instance.transform.position = space.transform.position;

            for (int i=0; i< _actionSpaces.Count; i++)
            {
                ActionSpace oldSpace = _actionSpaces[i].GetComponent<ActionSpace>();
                if (oldSpace.ActionID == actionID)
                {
                    Destroy(oldSpace.gameObject);
                    _actionSpaces[i] = instance;
                }
            }
        }

        public void SetDoubleFencedPastures(string playerid, List<Vector2> doubleFencedPastures)
        {
            GameObject.Find("GameBoard").GetComponent<PlayerBoardScript>().SetDoubleFencedPastures(doubleFencedPastures);
        }

        public void SetTileAnimals(Vector2 position, bool isCave, string animalType, int count)
        {
            GameObject p = GameObject.Find("GameBoard");
            p.GetComponent<PlayerBoardScript>().SetTileAnimals(position, isCave, animalType, count);
        }

        public void SetChosenActionSpace(string player, int actionID)
        {
            ClientSocket.SetChosenActionSpace(player, actionID);
            DisableActionSpaces();
        }

        private void DisableActionSpaces()
        {
            foreach (GameObject actionSpace in _actionSpaces)
            {
                actionSpace.GetComponent<ActionSpace>().SetInactive();
            }
        }

        public static void SendAction(int actionID, string action)
        {
            ClientSocket.SendAction(actionID, action);
        }
    }
}
