using System.Collections.Generic;
using Assets.UIScripts;
using UnityEngine;

namespace Assets.ServerScripts
{
    public interface IServerSocket
    {
        void SetActionSpaceActive(int id);
        void SetPlayerResources(string playerID, string resourceName, int count);
        void AddActionSpace(int id, string type);
        void SetActionSpaceResources(int actionSpaceID, string resource, int count);
        void ShowActionSpaceDwarf(int actionSpaceID);
        void HideActionSpaceDwarf(int actionSpaceID);
        void StartGameRound(int gameRound);
        void SendActions(int actionID, List<string> list);
        void SetActionCancelled(int actionID);
        void SetActionFinished(int actionID);
        void SetActionSpaceInactive(int actionID);
        void SetPlaceTile(string playerid, string tileType, List<Vector2> validSpots, bool isCave);
        void SetPlayerTileType(Vector2 position, List<string> tileType, bool isCave);
        void SetTilesUnclickable(string playerID);
        void SetPlaceBuildingTile(string playerid, string buildingType, List<Vector2> validSpots);
        void SetPlayerDwarves(List<string> dwarfStatus);
        void HidePlayerChoice(string playerid);

        void GetPlayerChoice(string playerid, string actionName, string text, List<string> options);
        void SetHarvestTokenStatus(List<string> harvestTokenStatus);
        void ReplaceActionSpace(int actionID, string newActionName);
        void SetDoubleFencedPastures(string playerid, List<Vector2> doubleFencedPastures);
        void SetTileAnimals(Vector2 position, bool isCave, string animalType, int count);
        void ClearPlayerTiles(string playerID);
        void ClearActionSpaces();
        void SetBuildingAvailable(string buildingType);
        void ChooseBuildingTile(string playerid, List<string> validBuildings);
        void SetBuildingTaken(string type);
        void ResetBuildingTiles(string playerid);
        void SetPlayerTileActive(string playerID, Vector2 position);
    }

    public class ServerSocket : IServerSocket
    {
        private readonly ClientSocket _clientSocket;

        public ServerSocket(ClientSocket clientSocket)
        {
            _clientSocket = clientSocket;
        }

        public void SetPlayerResources(string playerID, string resourceName, int count)
        {
            _clientSocket.SetPlayerResources(playerID, resourceName, count);
        }

        public void AddActionSpace(int id, string type)
        {
            _clientSocket.AddActionSpace(id, type);
        }

        public void SetActionSpaceResources(int actionSpaceID, string resource, int count)
        {
            _clientSocket.SetActionSpaceResource(actionSpaceID, resource, count);
        }

        public void ShowActionSpaceDwarf(int actionSpaceID)
        {
            _clientSocket.ShowActionSpaceDwarf(actionSpaceID);
        }

        public void HideActionSpaceDwarf(int actionSpaceID)
        {
            _clientSocket.HideActionSpaceDwarf(actionSpaceID);
        }

        public void StartGameRound(int gameRound)
        {
            _clientSocket.StartGameRound(gameRound);
        }

        public void SendActions(int actionID, List<string> actions)
        {
            _clientSocket.SendActions(actionID, actions);
        }

        public void SetActionCancelled(int actionID)
        {
            _clientSocket.SetActionCancelled(actionID);
        }

        public void SetActionFinished(int actionID)
        {
            _clientSocket.SetActionFinished(actionID);
        }

        public void SetActionSpaceInactive(int actionID)
        {
            _clientSocket.SetActionSpaceInactive(actionID);
        }

        public void SetPlaceTile(string playerid, string tileType, List<Vector2> validSpots, bool isCave)
        {
            _clientSocket.SetPlaceTile(playerid, tileType, validSpots, isCave);
        }

        public void SetPlayerTileType(Vector2 position, List<string> tileType, bool isCave)
        {
            _clientSocket.SetPlayerTileType(position, tileType, isCave);
        }

        public void SetTilesUnclickable(string playerID)
        {
            _clientSocket.SetTilesUnclickable(playerID);
        }

        public void SetPlaceBuildingTile(string playerid, string buildingType, List<Vector2> validSpots)
        {
            _clientSocket.SetPlaceBuildingTile(playerid, buildingType, validSpots);
        }

        public void SetPlayerDwarves(List<string> dwarfStatus)
        {
            _clientSocket.SetPlayerDwarves(dwarfStatus);
        }

        public void GetPlayerChoice(string playerid, string actionName, string text, List<string> options)
        {
            _clientSocket.GetPlayerChoice(playerid, actionName, text, options);
        }

        public void SetHarvestTokenStatus(List<string> harvestTokenStatus)
        {
            _clientSocket.SetHarvestTokenStatus(harvestTokenStatus);
        }

        public void ReplaceActionSpace(int actionID, string newActionName)
        {
            _clientSocket.ReplaceActionSpace(actionID, newActionName);
        }

        public void SetDoubleFencedPastures(string playerid, List<Vector2> doubleFencedPastures)
        {
            _clientSocket.SetDoubleFencedPastures(playerid, doubleFencedPastures);
        }

        public void SetTileAnimals(Vector2 position, bool isCave, string animalType, int count)
        {
            _clientSocket.SetTileAnimals(position, isCave, animalType, count);
        }

        public void ClearPlayerTiles(string playerID)
        {
            _clientSocket.ClearPlayerTiles(playerID);
        }

        public void ClearActionSpaces()
        {
            _clientSocket.ClearActionSpaces();
        }

        public void SetBuildingAvailable(string buildingType)
        {
            _clientSocket.SetBuildingAvailable(buildingType);
        }

        public void ChooseBuildingTile(string playerid, List<string> validBuildings)
        {
            _clientSocket.ChooseBuildingTile(playerid, validBuildings);
        }

        public void SetBuildingTaken(string type)
        {
            _clientSocket.SetBuildingTaken(type);
        }

        public void ResetBuildingTiles(string playerid)
        {
            _clientSocket.ResetBuildingTiles(playerid);
        }

        public void SetPlayerTileActive(string playerID, Vector2 position)
        {
            _clientSocket.SetPlayerBuildingActive(playerID, position);
        }

        public void HidePlayerChoice(string playerid)
        {
            _clientSocket.HidePlayerChoice(playerid);
        }

        public void SetActionSpaceActive(int actionID)
        {
            _clientSocket.SetActionSpaceActive(actionID);
        }

        public void StartGame(int numPlayers)
        {
            CavernaManager.Instance = new CavernaManager(this, numPlayers);
        }

        public void SetSelectedAction(int actionID, string actionName)
        {
            CavernaManager.Instance.SetSelectedAction(actionID, actionName);
        }

        public void SetChosenActionSpace(string playerID, int actionID)
        {
            CavernaManager.Instance.SetChosenActionSpace(playerID, actionID);
        }

        public void SetTileClicked(string playerID, Vector2 position, string type, bool isCave, bool isBuilding)
        {
            CavernaManager.Instance.SetTileClicked(playerID, position, type, isCave, isBuilding);
        }

        public void SendPlayerChoice(string action)
        {
            CavernaManager.Instance.SetPlayerChoice(action);
        }

        public void GetRubyActions(string playerID)
        {
            CavernaManager.Instance.GetRubyActions(playerID);
        }

        public void GetTileAction(string playerID, Vector2 position, string tileType)
        {
            CavernaManager.Instance.GetTileAction(playerID, position, tileType);
        }
    }
}