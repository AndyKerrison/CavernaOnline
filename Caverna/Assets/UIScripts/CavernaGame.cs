using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        private int _currentActionPage;
        private int _firstActionIndex;
        private int _lastActionIndex;
        private int _maxPages;
        private List<string> _actions;
        private string _actionName;
        private string _helpText;
        private GameObject[,] _buildingTiles;

        public GameObject MainActionsDisplayBoard;
        public Canvas GameBoard;
        public Canvas ActionBoard;
        public Canvas BuildingsBoard;
        public GameObject ActionsPanel;
        public GameObject ActionsPanelView;
        public GameObject BuildingsPanel;
        public GameObject BuildingsPanelView;
        public Scrollbar ActionPanelScrollbar;
        public Scrollbar BuildingPanelScrollbar;
        public Canvas MainCanvas;
        public Image Image;

        // Use this for initialization
// ReSharper disable once UnusedMember.Local
        void Start () {	
            Instance = gameObject;
            //_loaderImage = GameObject.Find("LoaderImage");
            //_levelText = GameObject.Find("LevelText").GetComponent<Text>();
            GameBoard.enabled = false;
            ActionBoard.enabled = true;
            ActionPanelScrollbar.value = 0;
            BuildingPanelScrollbar.value = 0;
            ResizeBuildingPanel();
            InitBuildingTiles();

            //after everything is set up, THEN start game
            ClientSocket = UIScripts.ClientSocket.Instance;
            ClientSocket.StartGame(1);
        }

        public void ShowPlayerBoard()
        {
            ActionBoard.enabled = false;
            BuildingsBoard.enabled = false;
            GameBoard.enabled = true;
        }

        public void ShowActionBoard()
        {
            GameBoard.enabled = false;
            BuildingsBoard.enabled = false;
            ActionBoard.enabled = true;
        }

        public void ShowBuildingsBoard()
        {
            GameBoard.enabled = false;
            BuildingsBoard.enabled = true;
            ActionBoard.enabled = false;
        }

        public void StartGameRound(int gameRound)
        {
            //can't delay this at the mo unless we also delay card updates etc
            //_levelText.text = "Round " + gameRound;
            GameObject.Find("RoundMarker").GetComponent<RoundMarkerScript>().SetRound(gameRound);
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

        private void InitBuildingTiles()
        {
            var panelWidth = BuildingsPanel.GetComponent<RectTransform>().rect.width;
            var panelHeight = BuildingsPanel.GetComponent<RectTransform>().rect.height;
            var tileWidth = 0.075f*panelWidth;

            _buildingTiles = new GameObject[12, 4];
            
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    var xPos = tileWidth / 2 - panelWidth / 2 + x*tileWidth;
                    var yPos = panelHeight / 2 - tileWidth / 2 - y*tileWidth;

                    if (x >= 3)
                        xPos += 0.047f*panelWidth;
                    if (x >= 9)
                        xPos += 0.047f * panelWidth;

                    GameObject tile = CreateTile(BuildingTypes.Unavailable, tileWidth, xPos, yPos);
                    _buildingTiles[x, y] = tile;
                }
            }
        }

        private GameObject CreateTile(string type, float tileWidth, float xPos, float yPos)
        {
            GameObject tile = (GameObject)Instantiate(Resources.Load("TileUI"));
            tile.transform.SetParent(BuildingsPanel.transform);
            tile.transform.localScale = BuildingsPanel.transform.localScale;
            tile.transform.position = BuildingsPanel.transform.position;
            tile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileWidth, tileWidth);
            tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            tile.GetComponent<TileUIScript>().SetType(new List<string>() { type });
            return tile;
        }

        private void ResizeBuildingPanel()
        {
            var maxHeight = BuildingsPanelView.GetComponent<RectTransform>().rect.height;

            RectTransform buildingCanvasRect = BuildingsPanel.GetComponent<RectTransform>();
            
            //TODO contents are not sizing correctly...
            GameObject roomBoard1 = GameObject.Find("RoomBoard1");
            GameObject roomBoard2 = GameObject.Find("RoomBoard2");
            var roomX = roomBoard1.GetComponent<RectTransform>().rect.width;
            var roomY = roomBoard1.GetComponent<RectTransform>().rect.height;
            
            var yScale = maxHeight / roomY;
            Vector2 roomBoardSize = new Vector2(roomX * yScale, roomY * yScale);

            buildingCanvasRect.sizeDelta = new Vector2(roomBoardSize.x * 2, maxHeight);
            roomBoard1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            roomBoard2.GetComponent<RectTransform>().anchoredPosition = new Vector2(roomBoardSize.x, 0);
            roomBoard1.GetComponent<RectTransform>().sizeDelta = roomBoardSize;
            roomBoard2.GetComponent<RectTransform>().sizeDelta = roomBoardSize;

            //*/
            //MainBuildingsDisplayBoard.GetComponent<RectTransform>().anchoredPosition = new Vector2(maxWidth - buildingCanvasRect.sizeDelta.x + 2 * cardSize.x, MainActionsDisplayBoard.GetComponent<RectTransform>().anchoredPosition.y);
        }

        private void ResizeActionCards()
        {
            int hPadding = 5;
            int vPadding = 5;
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

        public void ReplaceActionSpace(int actionID, string newActionName)
        {
            GameObject instance = ActionSpace.Create(actionID, newActionName);
            instance.transform.SetParent(ActionsPanel.transform);
            instance.transform.localScale = ActionsPanel.transform.localScale;
            instance.transform.position = ActionsPanel.transform.position;
            //_actionSpaces.Add(card2);

            for (int i = 0; i < _actionSpaces.Count; i++)
            {
                ActionSpace oldSpace = _actionSpaces[i].GetComponent<ActionSpace>();
                if (oldSpace.ActionID == actionID)
                {
                    Destroy(oldSpace.gameObject);
                    _actionSpaces[i] = instance;
                }
            }

            ResizeActionCards();
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
            foreach (GameObject buildingTile in _buildingTiles)
            {
                buildingTile.GetComponent<TileUIScript>().SetUnclickable();
            }
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

        private void NextPage()
        {
            _currentActionPage++;
            SetModalVars();
        }

        private void PrevPage()
        {
            _currentActionPage--;
            SetModalVars();
        }

        private void SetModalVars()
        {
            float modalY = 0.4f * Screen.height / 2f;
            float screenHeight = Screen.height;
            int maxOptionsOnScreen = (int)(screenHeight - modalY) / 30 - 1;
            maxOptionsOnScreen = Math.Min(maxOptionsOnScreen, _actions.Count);

            _maxPages = 0;
            int remainingActions = _actions.Count;
            while (remainingActions > 0)
            {
                if (remainingActions <= maxOptionsOnScreen && _maxPages == 0) //all fit on first page
                {
                    _maxPages++;
                    remainingActions -= maxOptionsOnScreen;
                }
                else if (_maxPages == 0) //first page, but doesn't all fit
                {
                    _maxPages++;
                    remainingActions -= (maxOptionsOnScreen - 1); //allow space for 'more' button
                }
                else //2nd or later page
                {
                    _maxPages++;
                    remainingActions -= (maxOptionsOnScreen - 2); //allow for 'more' and 'back' buttons
                    if (remainingActions == 1) //unless there's only one more action, in which case replace 'more' with the action
                        remainingActions--;
                }
            }
            
            _firstActionIndex = _currentActionPage * (maxOptionsOnScreen - 2);
            if (_firstActionIndex > 0)
                _firstActionIndex++; //because there is no 'Back' option on the first page

            _lastActionIndex = _firstActionIndex + (maxOptionsOnScreen-1);
            if (_currentActionPage > 0)
                _lastActionIndex--; //leave room for 'Back' option
            if (_currentActionPage < (_maxPages - 1) )
                _lastActionIndex--; //leave room for 'More' option
            if (_lastActionIndex > _actions.Count-1)
                _lastActionIndex = _actions.Count-1;
        }

        void OnGUI()
        {
            if (_showModal)
            {
                float modalY = 0.4f*Screen.height/2f;
                float screenHeight = Screen.height;
                int maxOptionsOnScreen = (int)(screenHeight - modalY)/30 - 1;
                int numActionsToShow = _lastActionIndex - _firstActionIndex + 1;
                if (_currentActionPage > 0)
                    numActionsToShow++;
                if (_currentActionPage < _maxPages - 1)
                    numActionsToShow++;
                maxOptionsOnScreen = Math.Min(maxOptionsOnScreen, numActionsToShow);
                
                int width = 320;
                int height = maxOptionsOnScreen*30 + 25;
                if (_helpText.Length > 0)
                    height += 30;
                GUI.ModalWindow(1, new Rect(Screen.width/2 - width/2, modalY, width, height), DoMyWindow, _actionName);
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

            if (_currentActionPage > 0)
            {
                if (GUI.Button(new Rect(10, y, 300, 20), "Back..."))
                {
                    PrevPage();
                }
                y += 30;
            }

            for (int i=_firstActionIndex; i <= _lastActionIndex; i++)
            {
                if (GUI.Button(new Rect(10, y, 300, 20), _actions[i]))
                {
                    ClientSocket.SendPlayerChoice(_actions[i]);
                }
                y += 30;
            }

            if (_currentActionPage < _maxPages-1)
            {
                if (GUI.Button(new Rect(10, y, 300, 20), "More..."))
                {
                    NextPage();
                }
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
            _currentActionPage = 0;
            _actions = options;
            _helpText = text;
            SetModalVars();
        }

        public void SetHarvestTokenStatus(List<string> harvestTokenStatus)
        {
            //GameObject p = GameObject.Find("ActionBoard");
            //p.GetComponent<ActionBoardScript>().SetHarvestTokens(harvestTokenStatus);            
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

        public void ClearPlayerTiles(string playerID)
        {
            GameObject p = GameObject.Find("GameBoard");
            p.GetComponent<PlayerBoardScript>().ClearTiles();
        }

        public void ClearActionSpaces()
        {
            while (_actionSpaces.Count > 0)
            {
                Destroy(_actionSpaces[0]);
                _actionSpaces.RemoveAt(0);
            }
        }

        public void SetBuildingAvailable(string buildingType)
        {
            Vector2 buildingLocation = BuildingPanelInfo.GetBuildingTileLocation(buildingType);
            _buildingTiles[(int)buildingLocation.x, (int)buildingLocation.y].GetComponent<TileUIScript>().SetType(new List<string>() {buildingType});
        }

        public void ChooseBuildingTile(string playerid, List<string> validBuildings)
        {
            //find the valid buildings, set them as clickable
            for (int x = 0; x <= _buildingTiles.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= _buildingTiles.GetUpperBound(1); y++)
                {
                    string tileType = _buildingTiles[x, y].GetComponent<TileUIScript>().GetTileType();
                    if (validBuildings.Contains(tileType))
                        _buildingTiles[x,y].GetComponent<TileUIScript>().SetClickable(tileType, true, true, true);
                }
            }

            ShowBuildingsBoard();
        }

        public void SetBuildingTaken(string type)
        {
            for (int x = 0; x <= _buildingTiles.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= _buildingTiles.GetUpperBound(1); y++)
                {
                    if (type == _buildingTiles[x, y].GetComponent<TileUIScript>().GetTileType())
                    {
                        _buildingTiles[x, y].GetComponent<TileUIScript>().SetTaken();
                    }
                }
            }
        }

        public void ResetBuildingTiles()
        {
                        
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Destroy(_buildingTiles[x, y]);
                }
            }
            InitBuildingTiles();
        }

        public void SetPlayerBuildingActive(string playerID, Vector2 position)
        {
            GameObject.Find("GameBoard").GetComponent<PlayerBoardScript>().SetTileActionActive(position);
        }
    }
}
