using System.Collections.Generic;
using Assets.ServerScripts;
using UnityEngine;

namespace Assets.UIScripts
{
    public interface IClientSocket
    {
        void StartGame(int numPlayers);
        void SendAction(int actionID, string action);
        void SetChosenActionSpace(string player, int actionID);
        void SetTileClicked(string player, Vector2 position, bool isCave, bool isBuilding);
        void SendPlayerChoice(string action);
        void GetRubyActions(string playerID);
    }

    public class ClientSocket : IClientSocket
    {

        private static readonly ClientSocket Socket = new ClientSocket();
        private static CavernaGame _gameRef;

        public static IClientSocket Instance
        {
            get { return Socket; }
        }

        private readonly ServerSocket _serverSocket;

        public ClientSocket()
        {
            _serverSocket = new ServerSocket(this);
            _gameRef = CavernaGame.Instance.GetComponent<CavernaGame>();
        }

        public void StartGame(int numPlayers)
        {
            _serverSocket.StartGame(numPlayers);
        }

        public void SetChosenActionSpace(string playerID, int actionID)
        {
            _serverSocket.SetChosenActionSpace(playerID, actionID);
        }

        public void SendAction(int actionID, string action)
        {
            _serverSocket.SetSelectedAction(actionID, action);
        }

        public void SetActionSpaceActive(int actionID)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().SetActionSpaceActive(actionID);
        }

        public void SetPlayerResources(string playerID, string resourceName, int count)
        {
            _gameRef.SetPlayerResources(playerID, resourceName, count);
        }

        public void AddActionSpace(int id, string type)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().AddActionSpace(id, type);
        }

        public void SetActionSpaceResource(int actionSpaceID, string resource, int count)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().SetActionSpaceResources(actionSpaceID, resource, count);
        }

        public void ShowActionSpaceDwarf(int actionSpaceID)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().ShowActionSpaceDwarf(actionSpaceID);
        }

        public void HideActionSpaceDwarf(int actionSpaceID)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().HideActionSpaceDwarf(actionSpaceID);
        }

        public void StartGameRound(int gameRound)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().StartGameRound(gameRound);
        }

        public void SendActions(int actionID, List<string> actions)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().SetActions(actionID, actions);
        }

        public void SetActionCancelled(int actionID)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().SetActionCancelled(actionID);
        }

        public void SetActionFinished(int actionID)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().SetActionFinished(actionID);
        }

        public void SetActionSpaceInactive(int actionID)
        {
            CavernaGame.Instance.GetComponent<CavernaGame>().SetActionSpaceInactive(actionID);
        }

        public void SetPlaceTile(string playerid, string tileType, List<Vector2> validSpots, bool isCave)
        {
            _gameRef.SetPlaceTile(playerid, tileType, validSpots, isCave);
        }

        public void SetTileClicked(string playerID, Vector2 position, bool isCave, bool isBuilding)
        {
            _serverSocket.SetTileClicked(playerID, position, isCave, isBuilding);
        }

        void IClientSocket.SendPlayerChoice(string action)
        {
            _serverSocket.SendPlayerChoice(action);
        }

        public void GetRubyActions(string playerID)
        {
            _serverSocket.GetRubyActions(playerID);
        }

        public void SetPlayerTileType(Vector2 position, List<string> tileType, bool isCave)
        {
            _gameRef.SetPlayerTileType(position, tileType, isCave);
        }

        public void SetTilesUnclickable(string playerID)
        {
            _gameRef.SetTilesUnclickable(playerID);
        }

        public void SetPlaceBuildingTile(string playerid, string buildingType, List<Vector2> validSpots)
        {
            _gameRef.SetPlaceBuildingTile(playerid, buildingType, validSpots);
        }

        public void SetPlayerDwarves(List<string> dwarfStatus)
        {
            _gameRef.SetPlayerDwarves(dwarfStatus);
        }

        public void HidePlayerChoice(string playerid)
        {
            _gameRef.HidePlayerChoice(playerid);
        }

        public void GetPlayerChoice(string playerid, string actionName, string text, List<string> options)
        {
            _gameRef.GetPlayerChoice(playerid, actionName, text, options);
        }

        public void SetHarvestTokenStatus(List<string> harvestTokenStatus)
        {
            _gameRef.SetHarvestTokenStatus(harvestTokenStatus);
        }

        public void ReplaceActionSpace(int actionID, string newActionName)
        {
            _gameRef.ReplaceActionSpace(actionID, newActionName);
        }

        public void SetDoubleFencedPastures(string playerid, List<Vector2> doubleFencedPastures)
        {
            _gameRef.SetDoubleFencedPastures(playerid, doubleFencedPastures);
        }

        public void SetTileAnimals(Vector2 position, bool isCave, string animalType, int count)
        {
            _gameRef.SetTileAnimals(position, isCave, animalType, count);
        }

        public void ClearPlayerTiles(string playerID)
        {
            _gameRef.ClearPlayerTiles(playerID);
        }

        public void ClearActionSpaces()
        {
            _gameRef.ClearActionSpaces();
        }
    }
}
