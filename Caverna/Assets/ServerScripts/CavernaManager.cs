using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Assets.ServerScripts
{
    public class CavernaManager
    {
        public static CavernaManager Instance;

        private readonly IServerSocket _serverSocket;

        private readonly List<string> _soloActions = new List<string>
        {
            ActionSpaceTypes.DriftMining,
            ActionSpaceTypes.Excavation,
            ActionSpaceTypes.StartingPlayer,
            ActionSpaceTypes.Logging,
            ActionSpaceTypes.Supplies,
            ActionSpaceTypes.OreMining,
            ActionSpaceTypes.WoodGathering,
            ActionSpaceTypes.Clearing,
            ActionSpaceTypes.Sustenance
        };

        private readonly List<string> _standardActions = new List<string>
        {
            ActionSpaceTypes.RubyMining,
            ActionSpaceTypes.Housework,
            ActionSpaceTypes.SlashAndBurn
        };

        private readonly List<string> _stage1Actions = new List<string>
        {
            ActionSpaceTypes.Blacksmithing,
            ActionSpaceTypes.OreMineConstruction,
            ActionSpaceTypes.SheepFarming
        };

        private readonly List<string> _stage2Actions = new List<string>
        {
            ActionSpaceTypes.WishForChildren,
            ActionSpaceTypes.DonkeyFarming,
            ActionSpaceTypes.RubyMine,
        };

        private readonly List<string> _stage3Actions = new List<string>
        {
            ActionSpaceTypes.Exploration,
            ActionSpaceTypes.OreDelivery,
            ActionSpaceTypes.FamilyLife
        };

        private readonly List<string> _stage4Actions = new List<string>
        {
            ActionSpaceTypes.Adventure,
            ActionSpaceTypes.OreTrading,
            ActionSpaceTypes.RubyDelivery
        };

        private readonly List<string> _roundHarvests = new List<string>
        {
            HarvestTypes.Harvest,
            HarvestTypes.Harvest,
            HarvestTypes.Harvest,
            HarvestTypes.Harvest,
            HarvestTypes.Special,
            HarvestTypes.Special,
            HarvestTypes.Special
        };

        private readonly List<string> _specialHarvests = new List<string>
        {
            string.Empty,
            HarvestTypes.SingleFoodPerDwarf,
            HarvestTypes.SkipFieldsOrAnimals
        };

        private List<CavernaPlayer> _players;
        private List<string> _remainingHarvests;       
        private List<CavernaActionSpace> _actionSpaces;
        private int _gameRound;
        private int _startPlayerIndex;
        private int _activePlayerIndex;
        private int _activeActionSpaceID;
        private bool _expeditionInProgress;
        private bool _isHarvest;

        public List<BuildingTile> BuildingTiles;

        public CavernaManager(IServerSocket serverSocket, int numPlayers)
        {
            _serverSocket = serverSocket;
            StartGame(numPlayers);
        }

        private void StartGame(int numPlayers)
        {
            //create players
            _players = new List<CavernaPlayer>();
            for (int i = 0; i < numPlayers; i++)
            {
                int food;
                if (IsSoloGame)
                    food = 2;
                else
                    food = Mathf.Min(3, Mathf.Max(1, i)); //1, 1, 2, 3, 3, 3, 3, etc

                _serverSocket.ClearPlayerTiles("playerID");
                _players.Add(new CavernaPlayer(food, _serverSocket));
            }
            _startPlayerIndex = 0;
            _gameRound = 0;

            //create action board. Depends on the number of players.
            _serverSocket.ClearActionSpaces();
            _actionSpaces = new List<CavernaActionSpace>();

            //solo game
            foreach (string actionName in _soloActions)
            {
                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, actionName));
            }

            //standard actions
            foreach (string actionName in _standardActions)
            {
                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, actionName));
            }

            foreach (CavernaActionSpace actionSpace in _actionSpaces)
            {
                _serverSocket.AddActionSpace(actionSpace.ID, actionSpace.Type);
            }

            //create building tiles
            //todo - clear existing buildings panel if any
            BuildingTiles = new List<BuildingTile>();
            BuildingTiles.Add(new BuildingTile(BuildingTypes.Dwelling));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.MixedDwelling));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Carpenter));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Miner));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.SimpleDwelling1));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.CoupleDwelling));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.StoneCarver));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Builder));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.SimpleDwelling2));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.AdditionalDwelling));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Blacksmith));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Trader));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.CuddleRoom));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.WorkRoom));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.WoodSupplier));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.DogSchool));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.BreakfastRoom));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.GuestRoom));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.StoneSupplier));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Quarry));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.StubbleRoom));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.OfficeRoom));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.RubySupplier));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.Seam));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.SlaughteringCave));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.MiningCave));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.StoneStorage));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.MainStorage));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.CookingCave));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.BreedingCave));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.OreStorage));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.WeaponStorage));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.WorkingCave));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.PeacefulCave));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.SparePartStorage));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.SuppliesStorage));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.WeavingParlour));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.HuntingParlour));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.BroomChamber));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.PrayerChamber));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.MilkingParlour));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.BeerParlour));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.TreasureChamber));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.WritingChamber));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.StateParlour));
            //BuildingTiles.Add(new BuildingTile(BuildingTypes.BlacksmithingParlour));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.FoodChamber));
            BuildingTiles.Add(new BuildingTile(BuildingTypes.FodderChamber));
            foreach (BuildingTile buildingTile in BuildingTiles)
            {
                _serverSocket.SetBuildingAvailable(buildingTile.BuildingType);
            }
            
            //randomize harvest order
            _remainingHarvests = new List<string>();
            _remainingHarvests.Add(string.Empty);
            _remainingHarvests.Add(string.Empty);
            _remainingHarvests.Add(HarvestTypes.Harvest);
            _remainingHarvests.Add(HarvestTypes.SingleFoodPerDwarf);
            _remainingHarvests.Add(HarvestTypes.Harvest);
            List<string> otherHarvests = new List<string>(_roundHarvests);
            int specialCount = 0;
            while (otherHarvests.Count > 0)
            {
                int index = Random.Range(0, otherHarvests.Count);
                string harvestType = otherHarvests[index];
                otherHarvests.RemoveAt(index);
                if (harvestType == HarvestTypes.Special && !IsSoloGame)
                {
                    _remainingHarvests.Add(_specialHarvests[specialCount]);
                    specialCount++;
                }
                else
                {
                    _remainingHarvests.Add(HarvestTypes.Harvest);
                }
            }

            NextRound();
        }

        public bool IsSoloGame
        {
            get { return _players.Count == 1; }
        }

        private void NextRound()
        {
            _gameRound++;

            if (IsSoloGame && _gameRound == 9)
            {
                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, ActionSpaceTypes.SkipRound));
                _serverSocket.AddActionSpace(_actionSpaces[_actionSpaces.Count - 1].ID, ActionSpaceTypes.SkipRound);
                NextRound();
                return;
            }

            //reset all players dwarves & harvest status
            _players.ForEach(x => x.HarvestComplete = false);
            _players.ForEach(x => x.HarvestFieldsComplete = false);
            _players.ForEach(x => x.HarvestFeedingComplete = false);
            _players.ForEach(x => x.HarvestAnimalsComplete = false);
            _players.ForEach(x => x.ResetDwarves());

            _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());

            /*
           4 Wish for children
           7 Ore delivery
           10 Ore trading
            5 Donkey farming
                8 Family life
                    (Wish for children
                        Urgent wish for children)
                            11 Adventure
            6 Ruby mine construction
            9 - 
                12 Ruby delivery
            */

            //add a new action card to the game
            if (_gameRound >= 1 && _gameRound <= 3)
            {
                int index;
                do
                {
                    if (IsSoloGame)
                    {
                        if (_gameRound == 1)
                            index = _stage1Actions.IndexOf(ActionSpaceTypes.Blacksmithing);
                        else if (_gameRound == 2)
                            index = _stage1Actions.IndexOf(ActionSpaceTypes.SheepFarming);
                        else //if (_gameRound == 3)
                            index = _stage1Actions.IndexOf(ActionSpaceTypes.OreMineConstruction);
                    }
                    else
                    {
                        index = Random.Range(0, _stage1Actions.Count);
                    }
                } while (_actionSpaces.Find(x => x.Type == _stage1Actions[index]) != null);

                string actionName = _stage1Actions[index];

                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, actionName));
                _serverSocket.AddActionSpace(_actionSpaces[_actionSpaces.Count-1].ID, actionName);
            }

            if (_gameRound >= 4 && _gameRound <= 6)
            {
                int index = 0;
                do
                {
                    if (_gameRound == 4) //always wish for children in round 4
                    {
                        index = _stage2Actions.IndexOf(ActionSpaceTypes.WishForChildren);
                    }
                    else
                    {
                        if (IsSoloGame)
                        {
                            if (_gameRound == 5)
                                index = _stage2Actions.IndexOf(ActionSpaceTypes.DonkeyFarming);
                            if (_gameRound == 6)
                                index = _stage2Actions.IndexOf(ActionSpaceTypes.RubyMine);
                        }
                        else
                        {
                            index = Random.Range(0, _stage2Actions.Count);
                        }
                    }
                } while (_actionSpaces.Find(x => x.Type == _stage2Actions[index]) != null);

                string actionName = _stage2Actions[index];

                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, actionName));
                _serverSocket.AddActionSpace(_actionSpaces[_actionSpaces.Count - 1].ID, actionName);
            }

            if (_gameRound >= 7 && _gameRound <= 9)
            {
                int index;
                do
                {
                    index = Random.Range(0, _stage3Actions.Count);

                    if (IsSoloGame)
                    {
                        if (_gameRound == 7)
                            index = _stage3Actions.IndexOf(ActionSpaceTypes.OreDelivery);
                        if (_gameRound == 8)
                            index = _stage3Actions.IndexOf(ActionSpaceTypes.FamilyLife);
                        if (_gameRound == 9)
                            throw new Exception("Invalid solo round");
                    }
                } while (_actionSpaces.Find(x => x.Type == _stage3Actions[index]) != null);

                string actionName = _stage3Actions[index];
                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, actionName));
                _serverSocket.AddActionSpace(_actionSpaces[_actionSpaces.Count - 1].ID, actionName);

                //if this action was family life, replace wish for children with urgent wish
                if (actionName == ActionSpaceTypes.FamilyLife)
                {
                    for (int i = 0; i < _actionSpaces.Count; i++)
                    {
                        if (_actionSpaces[i].Type == ActionSpaceTypes.WishForChildren)
                            index = i;
                    }
                    _actionSpaces[index] = new CavernaActionSpace(_actionSpaces[index].ID, ActionSpaceTypes.UrgentWish);
                    _serverSocket.ReplaceActionSpace(_actionSpaces[index].ID, ActionSpaceTypes.UrgentWish);
                }

            }

            if (_gameRound >= 10 && _gameRound <= 12)
            {
                int index;
                do
                {
                    index = Random.Range(0, _stage4Actions.Count);

                    if (IsSoloGame)
                    {
                        if (_gameRound == 10)
                            index = _stage4Actions.IndexOf(ActionSpaceTypes.OreTrading);
                        if (_gameRound == 11)
                            index = _stage4Actions.IndexOf(ActionSpaceTypes.Adventure);
                        if (_gameRound == 12)
                            index = _stage4Actions.IndexOf(ActionSpaceTypes.RubyDelivery);
                    }
                } while (_actionSpaces.Find(x => x.Type == _stage4Actions[index]) != null);

                string actionName = _stage4Actions[index];
                
                _actionSpaces.Add(new CavernaActionSpace(_actionSpaces.Count + 1, actionName));
                _serverSocket.AddActionSpace(_actionSpaces[_actionSpaces.Count - 1].ID, actionName);
            }

            if (_gameRound >5)
            {
                //GAME OVER!
                _serverSocket.GetPlayerChoice("playerID", "Game Over!", "Your score was " + _players[0].PlayerScore, new List<string>() {"Play Again" } );
                return;
            }

            //send harvest token status
            _serverSocket.SetHarvestTokenStatus(GetHarvestTokenStatus());

            //replenish and re-enable all action cards
            foreach (CavernaActionSpace space in _actionSpaces)
            {
                space.CleanupAndAddResources(_serverSocket, IsSoloGame);
                _serverSocket.HideActionSpaceDwarf(space.ID);
            }

            _serverSocket.StartGameRound(_gameRound);
            _activePlayerIndex = _startPlayerIndex;
            _activeActionSpaceID = 0;

            NextPlayerAction();
        }

        private List<string> GetHarvestTokenStatus()
        {
            //return harvest token status for rounds 5-12 inc
            List<string> harvests = new List<string>();
            for (int i = 5; i < 12; i++)
            {
                if (_gameRound <= i)
                    harvests.Add(HarvestTypes.Unknown);
                else if (_remainingHarvests[i] == HarvestTypes.Harvest)
                {
                    harvests.Add(_remainingHarvests[i]);
                }
                else
                {
                    harvests.Add(HarvestTypes.Special);
                }
            }
            return harvests;
        }

        private void NextPlayerAction()
        {
            //do any players have actions left?
            if (_players.Find(x => x.HasActions()) == null)
            {
                EndOfRound();
                return;
            }

            AdvanceActivePlayerIndex(true);

            //set active player
            //TODO send by playerID
            foreach (CavernaActionSpace space in _actionSpaces)
            {
                if (!space.IsUsed)
                    _serverSocket.SetActionSpaceActive(space.ID);
            }
        }

        private void AdvanceActivePlayerIndex(bool hasActionsOnly)
        {
            CavernaPlayer nextPlayer = null;
            while (nextPlayer == null || (hasActionsOnly && !nextPlayer.HasActions()))
            {
                if (_activePlayerIndex >= _players.Count)
                {
                    _activePlayerIndex = 0;
                }
                nextPlayer = _players[_activePlayerIndex];
                _activePlayerIndex++;
            }
        }

        private void EndOfRound()
        {
            _activePlayerIndex = _startPlayerIndex;

            RunHarvest();
        }

        private void RunHarvest()
        {
            _isHarvest = true;
            
            //gonna need to maintain state in some way here.... as we will be waiting for player response.
            while (_players.Exists(x => !x.HarvestComplete))
            {
                CavernaPlayer player = _players[_activePlayerIndex];
                if (player.HarvestComplete)
                {
                    AdvanceActivePlayerIndex(false);
                    continue;
                }

                switch (_remainingHarvests[_gameRound - 1])
                {
                    case HarvestTypes.None:
                    {
                        player.HarvestComplete = true;
                        AdvanceActivePlayerIndex(false);
                        break;
                    }
                    case HarvestTypes.Harvest:
                    {
                        //do field phase
                        if (!player.HarvestFieldsComplete)
                        {
                            _players[0].FieldPhase();
                        }

                        //do food
                        if (!player.HarvestFeedingComplete)
                        {
                            player.IsOneFoodPerDwarf = false;
                            _serverSocket.GetPlayerChoice("playerID", "Harvest Feeding", string.Empty, player.GetFoodOptions());
                            return;
                        }

                        //breed animals
                        if (!player.HarvestAnimalsComplete)
                        {
                            player.IsBreeding = true;
                            player.BreedAnimals();
                            if (player.RearrangeAnimalsCheck())
                                return;
                        }

                        player.IsBreeding = false;
                        player.HarvestComplete = true;
                        break;
                    }
                    case HarvestTypes.SingleFoodPerDwarf:
                    {
                        if (!player.HarvestFeedingComplete)
                        {
                            player.IsOneFoodPerDwarf = true;
                            _serverSocket.GetPlayerChoice("playerID", "Feeding - 1 food per dwarf", string.Empty, player.GetFoodOptions());
                            return;
                        }

                        player.HarvestComplete = true;
                        break;
                    }
                    case HarvestTypes.SkipFieldsOrAnimals:
                    {
                        //works like a regular harvest, but before it starts we get player to pick which
                        //phase to skip
                        if (!(player.HarvestFieldsComplete || player.HarvestAnimalsComplete))
                        {
                            _serverSocket.GetPlayerChoice("playerID", "Skip Field or Breeding phase?", string.Empty,
                                new List<string> {HarvestOptions.SkipFieldPhase, HarvestOptions.SkipBreedingPhase});
                            return;
                        }

                        //do field phase
                        if (!player.HarvestFieldsComplete)
                        {
                            _players[0].FieldPhase();
                        }

                        //do food
                        if (!player.HarvestFeedingComplete)
                        {
                            player.IsOneFoodPerDwarf = false;
                            _serverSocket.GetPlayerChoice("playerID", "Harvest Feeding", string.Empty, player.GetFoodOptions());
                            return;
                        }

                        //breed animals
                        if (!player.HarvestAnimalsComplete)
                        {
                            player.IsBreeding = true;
                            player.BreedAnimals();
                            if (player.RearrangeAnimalsCheck())
                                return;
                        }

                        player.IsBreeding = false;
                        player.HarvestComplete = true;
                        break;
                    }
                    default:
                    {
                        throw new Exception("Should not happen");
                    }
                }
                
            }

            _isHarvest = false;

            //after any harvests but before the next round, we must check if the player wants to prevent any action spaces from
            //clearing in the solo game
            if (IsSoloGame && _players[0].Rubies > 0)
            {
                List<string> actionSpacesWhichWillReset = new List<string>();
                foreach (CavernaActionSpace actionSpace in _actionSpaces)
                {
                    if (actionSpace.AccumulatingResourcesTotal >= 6 && !actionSpace.IsClearingPrevented)
                    {
                        actionSpacesWhichWillReset.Add(actionSpace.Type);
                    }
                }

                if (actionSpacesWhichWillReset.Count > 0)
                {
                    actionSpacesWhichWillReset.Add("Let the spaces reset");
                    _serverSocket.GetPlayerChoice("playerID", "Prevent Resource Clearing", "1 ruby each to prevent clearing", actionSpacesWhichWillReset);
                    
                    return;
                }
                
            }
            
            NextRound();
        }

        public void CollectResources(CavernaPlayer player, int actionSpaceID)
        {
            CavernaActionSpace actionSpace = _actionSpaces.Find(x => x.ID == actionSpaceID);
            actionSpace.CollectResources(_players[0], _serverSocket);
        }

        private void GetActions(string playerID, int actionID)
        {
            //can this player use the action space chosen?
            //if not his turn, return nothing?

            if (_expeditionInProgress)
            {
                List<string> actions = _players[0].GetExpeditionOptions();
                if (actions.Count > 0)
                {
                    _serverSocket.GetPlayerChoice(playerID, _players[0].GetExpeditionText(), string.Empty, actions);
                }
                else
                {
                    _expeditionInProgress = false;
                    _players[0].ApplyExpeditionWeaponBonus();
                    _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());
                    GetActions(playerID, actionID);
                }
            }
            else
            {
                CavernaActionSpace actionSpace = _actionSpaces.Find(x => x.ID == actionID);
                _activeActionSpaceID = actionID;
                _serverSocket.SendActions(actionID, actionSpace.GetActions(_players[0]));
            }
        }

        private void SetPlayerPlaceTile(CavernaPlayer player, string tile)
        {
            SetPlayerPlaceTiles(player, new List<string> {tile});
        }

        private void SetPlayerPlaceTiles(CavernaPlayer player, List<string> tileList)
        {
            bool isCave = tileList[0] == TileTypes.Cavern ||
                tileList[0] == TileTypes.DeepTunnel ||
                tileList[0] == TileTypes.OreMine ||
                tileList[0] == TileTypes.RubyMine ||
                tileList[0] == TileTypes.Tunnel;
            
            List<Vector2> validSpots = player.SetTilesToPlace(tileList);
            _serverSocket.SetPlaceTile(player.ID, tileList[0], validSpots, isCave);
        }

        public void SetChosenActionSpace(string playerID, int actionID)
        {
            //if (_players[_activePlayerIndex] == "player")
            //    return;
            if (_activeActionSpaceID != 0)
                return;

            _activeActionSpaceID = actionID;
            GetActions(playerID, actionID);
        }

        public void SetTileClicked(string playerID, Vector2 position, string type, bool isCave, bool isBuilding)
        {
            //TODO, check it was this player's turn and they were supposed to click a tile
            _serverSocket.SetTilesUnclickable("player");

            //TODO was this a furnish action?
            if (isBuilding & !isCave) //must be a click on the buildings board
            {
                List<Vector2> validSpots = _players[0].GetValidBuildingSpots();
                _serverSocket.SetPlaceBuildingTile("playerID", type, validSpots);
                return;
            }
            else if (isBuilding) //a building confirmation click on the player board
            {
                _players[0].SetBuildingTileAt(position, type);
                if (!BuildingTiles.Find(x => x.BuildingType == type).IsUnlimited)
                {
                    BuildingTiles.RemoveAll(x => x.BuildingType == type);
                    _serverSocket.SetBuildingTaken(type);
                }
                
                //if this is the dwelling from urgent wish, do FG also
                if (_actionSpaces.Find(x => x.ID == _activeActionSpaceID).Type == ActionSpaceTypes.UrgentWish)
                {
                    _players[0].FamilyGrowth();
                    _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());
                }

                GetActions("player", _activeActionSpaceID);
                return;
            }
            else //a tile placement confirmation somewhere on the player board
            {
                _players[0].SetTileAt(position, !isCave);
                _serverSocket.SetDoubleFencedPastures("playerID", _players[0].GetDoubleFencedPastures());
            }
            
            //was this the end of an action or was it a double tile?
            if (_players[0].HasTilesToPlace())
            {
                //if there is only one option left, click that one next
                List<Vector2> validSpots = _players[0].GetValidTileSpots();
                if (validSpots.Count == 1)
                    SetTileClicked(playerID, validSpots[0], _players[0].GetNextTile(), isCave, false);
                else
                    _serverSocket.SetPlaceTile("playerID", _players[0].GetNextTile(), validSpots, isCave);
            }
            else
            {
                ReturnControlToPlayer(_players[0]);
            }
        }

        public void SetSelectedAction(int actionID, string actionName)
        {
            //if (_players[_activePlayerIndex] == "player")
            //    return;
            if (_activeActionSpaceID != actionID)
                return;
            //TODO get playerID also

            if (actionName == CavernaActions.Cancel)
            {
                _activeActionSpaceID = 0;
                _serverSocket.SetActionFinished(actionID);
                if (actionID > 0)
                {
                    _serverSocket.SetActionSpaceActive(actionID);
                }

                //TODO send by playerID
                foreach (CavernaActionSpace space in _actionSpaces)
                {
                    if (!space.IsUsed)
                        _serverSocket.SetActionSpaceActive(space.ID);
                }
                return;
            }

            if (actionName == CavernaActions.Finish)
            {
                _activeActionSpaceID = 0;
                _actionSpaces.Find(x => x.ID == actionID).SetFinished();
                _serverSocket.SetActionFinished(actionID);
                _players[0].SetActionFinished();

                //action ended, disable all spaces and get next player
                foreach (CavernaActionSpace space in _actionSpaces)
                {
                    _serverSocket.SetActionSpaceInactive(space.ID);
                }
                _activePlayerIndex++;
                NextPlayerAction();
                return;
            }

            //any other NEW (i.e, not incomplete, could have been cancelled) action, mark space (and player's dwarf) as used
            if (_actionSpaces.Find(x => x.ID == actionID).GetActions(_players[0]).Contains("Cancel"))
            {
                _serverSocket.ShowActionSpaceDwarf(actionID);
                _players[0].SetDwarfUsed();
                _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());
            }

            _actionSpaces.Find(x => x.ID == actionID).SetActionUsed(_players[0], actionName);

            if (actionName == CavernaActions.CollectResources)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we check animals etc
                CollectResources(_players[0], actionID);
                ReturnControlToPlayer(_players[0]);
                return;
            }

            //this action is NOT complete until both tiles are placed, so don't run GetActions()
            if (actionName == CavernaActions.AddTunnelCaveDualTile)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tunnel/cave

                SetPlayerPlaceTiles(_players[0], TileLists.TunnelCaveDualTile);
                return;
            }

            if (actionName == CavernaActions.AddCaveCaveDualTile)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tunnel/cave

                SetPlayerPlaceTiles(_players[0], TileLists.CaveCaveDualTile);
                return;
            }

            if (actionName == CavernaActions.AddFieldClearingTile)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tunnel/cave

                SetPlayerPlaceTiles(_players[0], TileLists.FieldClearingDualTile);
                return;
            }

            if (actionName == CavernaActions.AddOreMineDeepTunnelDualTile)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tunnel/cave

                _players[0].Ore += 3;
                SetPlayerPlaceTiles(_players[0], TileLists.OreMineDeepTunnelDualTile);
                return;
            }

            if (actionName == CavernaActions.AddRubyMineTile)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tile

                SetPlayerPlaceTile(_players[0], TileTypes.RubyMine);
                return;
            }

            if (actionName == CavernaActions.BuyStable)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tile

                SetPlayerPlaceTile(_players[0], TileTypes.Stable);
                return;
            }

            if (actionName == CavernaActions.SmallFence)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tile

                SetPlayerPlaceTile(_players[0], TileTypes.SmallFence);
                return;
            }

            if (actionName == CavernaActions.BigFence)
            {
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the tile

                SetPlayerPlaceTiles(_players[0], TileLists.BigFence);
                return;
            }

            if (actionName == CavernaActions.Trade2Ore)
            {
                _players[0].Ore -= 2;
                _players[0].Food += 1;
                _players[0].Gold += 2;
                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.FamilyGrowth)
            {
                _players[0].FamilyGrowth();
                _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.SowBake)
            {
                List<string> sowActions = _actionSpaces.Find(x => x.ID == actionID).GetSowActions(_players[0]);
                _serverSocket.SendActions(actionID, sowActions);
                return;
            }

            if (actionName == CavernaActions.Sow1Grain)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowGrain();
                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow2Grain)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowGrain();
                _players[0].SowGrain();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow1Veg)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowVeg();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow2Veg)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowVeg();
                _players[0].SowVeg();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow1Grain1Veg)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowGrain();
                _players[0].SowVeg();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow1Grain2Veg)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowGrain();
                _players[0].SowVeg();
                _players[0].SowVeg();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow2Grain1Veg)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowGrain();
                _players[0].SowGrain();
                _players[0].SowVeg();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Sow2Grain2Veg)
            {
                if (_expeditionInProgress)
                    _serverSocket.SetActionFinished(actionID);

                _players[0].SowGrain();
                _players[0].SowGrain();
                _players[0].SowVeg();
                _players[0].SowVeg();

                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.FurnishCavern)
            {
                //TODO handle other building types
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the dwelling

                FurnishCavern();
                return;
            }

            if (actionName == CavernaActions.FurnishDwellingThenGrow)
            {
                //TODO handle other building types
                _serverSocket.SetActionFinished(actionID); //hide the modal till we've placed the dwelling

                FurnishDwelling();
                return;
            }

            if (actionName == CavernaActions.Blacksmithing)
            {
                List<string> blacksmithActions = _players[0].GetBlacksmithActions();
                _serverSocket.SendActions(actionID, blacksmithActions);
                return;
            }

            if (actionName == CavernaActions.Level8Weapon ||
                actionName == CavernaActions.Level7Weapon ||
                actionName == CavernaActions.Level6Weapon ||
                actionName == CavernaActions.Level5Weapon ||
                actionName == CavernaActions.Level4Weapon ||
                actionName == CavernaActions.Level3Weapon ||
                actionName == CavernaActions.Level2Weapon ||
                actionName == CavernaActions.Level1Weapon)
            {
                if (actionName == CavernaActions.Level8Weapon)
                    _players[0].Ore -= 8;
                if (actionName == CavernaActions.Level7Weapon)
                    _players[0].Ore -= 7;
                if (actionName == CavernaActions.Level6Weapon)
                    _players[0].Ore -= 6;
                if (actionName == CavernaActions.Level5Weapon)
                    _players[0].Ore -= 5;
                if (actionName == CavernaActions.Level4Weapon)
                    _players[0].Ore -= 4;
                if (actionName == CavernaActions.Level3Weapon)
                    _players[0].Ore -= 3;
                if (actionName == CavernaActions.Level2Weapon)
                    _players[0].Ore -= 2;
                if (actionName == CavernaActions.Level1Weapon)
                    _players[0].Ore -= 1;

                _players[0].ArmActiveDwarf(actionName);
                _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());
                GetActions("playerID", actionID);
                return;
            }

            if (actionName == CavernaActions.Level1Expedition ||
                actionName == CavernaActions.Level2Expedition ||
                actionName == CavernaActions.Level3Expedition)
            {
                _expeditionInProgress = true;
                _serverSocket.SetActionFinished(actionID); //hide modal

                _players[0].NewExpedition(actionName);
                _serverSocket.GetPlayerChoice("playerID", actionName, string.Empty, _players[0].GetExpeditionOptions());
                return;
            }
        }

        public void SetPlayerChoice(string action)
        {
            if (action == "Play Again")
            {
                _serverSocket.HidePlayerChoice("playerID");
                StartGame(1);
                return;
            }

            if (IsActionSpaceAction(action))
            {
                SetSelectedAction(_activeActionSpaceID, action);
                return;
            }

            //means it was a ruby payment to prevent clearing
            if (IsActionCardName(action))
            {
                _serverSocket.HidePlayerChoice("playerID");
                _actionSpaces.Find(x => x.Type == action).IsClearingPrevented = true;
                _players[0].Rubies--;

                //return control to player? run harvest?
                RunHarvest();

                return;
            }
            else if (action == "Let the spaces reset")
            {
                _serverSocket.HidePlayerChoice("playerID");
                NextRound();
            }

            //TODO needs playerID also
            CavernaPlayer player = _players[0];
            
            if (action == FoodActions.FeedAllDwarves || action == FoodActions.FeedAndTakeBeggingCards)
            {
                player.FeedDwarves();
                _serverSocket.HidePlayerChoice("playerID");

                NextPlayerAction();
            }
            else if (IsDiscardAction(action))
            {
                _serverSocket.HidePlayerChoice("playerID");
                player.Discard(action);
                ReturnControlToPlayer(player);
            }
            else if (IsFoodConversionAction(action))
            {
                //TODO food conversion could trigger more animal arranging...
                //but here it could trigger two modals at once with the choice/getactions afterwards
                _serverSocket.HidePlayerChoice("playerID");
                player.ConvertToFood(action);
                ReturnControlToPlayer(player);
            }
            else if (action == HarvestOptions.SkipBreedingPhase)
            {
                player.HarvestAnimalsComplete = true;
                NextPlayerAction();
            }
            else if (action == HarvestOptions.SkipFieldPhase)
            {
                player.HarvestFieldsComplete = true;
                NextPlayerAction();
            }
            else if (Array.Find(typeof(RubyTrades).GetFields(), x => x.GetValue(null).ToString() == action) != null)
            {
                _serverSocket.HidePlayerChoice("playerID");

                if (action == RubyTrades.Cancel)
                    return;
                
                player.Rubies--;

                if (action == RubyTrades.Wood)
                    player.Wood++;
                else if (action == RubyTrades.Stone)
                    player.Stone++;
                else if (action == RubyTrades.Ore)
                    player.Ore++;
                else if (action == RubyTrades.Grain)
                    player.Grain++;
                else if (action == RubyTrades.Veg)
                    player.Veg++;
                else if (action == RubyTrades.Sheep)
                    player.Sheep++;
                else if (action == RubyTrades.Donkey)
                    player.Donkeys++;
                else if (action == RubyTrades.Pig)
                    player.Pigs++;
                else if (action == RubyTrades.Cow)
                {
                    player.Cows++;
                    player.Food--;
                }
                else if (action == RubyTrades.Field)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Field);
                    return;
                }
                else if (action == RubyTrades.Clearing)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Clearing);
                    return;
                }
                else if (action == RubyTrades.Tunnel)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Tunnel);
                    return;
                }
                else if (action == RubyTrades.Cavern)
                {
                    player.Rubies--;
                    SetPlayerPlaceTile(_players[0], TileTypes.Cavern);
                    return;
                }
                else if (action == RubyTrades.ReorderDwarf)
                {
                    _serverSocket.GetPlayerChoice("playerID", "Move which Dwarf to the front?", string.Empty,
                        _players[0].GetReorderDwarfOptions());
                    return;
                }

                ReturnControlToPlayer(_players[0]);
            }
            else if (action.Contains(DwarfText.Unarmed) || action.Contains(DwarfText.WeaponLevel))
            {
                _serverSocket.HidePlayerChoice("playerID");
                _players[0].ReorderDwarf(action);
                _serverSocket.SetPlayerDwarves(_players[0].GetDwarfStatus());
            }
            else if (Array.Find(typeof (Expeditions).GetFields(), x => x.GetValue(null).ToString() == action) != null)
            {
                _serverSocket.HidePlayerChoice("playerID");
                player.SetExpeditionRewardTaken(action);

                //LEVEL 1
                //Expeditions.AllWeaponsPlusOne
                //resolved after expedition

                if (action == Expeditions.Wood)
                    player.Wood++;

                if (action == Expeditions.Dog)
                    player.Dogs++;


                //LEVEL 2
                if (action == Expeditions.Grain)
                    player.Grain++;

                if (action == Expeditions.Sheep)
                    player.Sheep++;


                //LEVEL 3
                if (action == Expeditions.Stone)
                    player.Stone++;
                
                if (action == Expeditions.Donkey)
                    player.Donkeys++;


                //LEVEL 4
                if (action == Expeditions.Veg)
                    player.Veg++;

                if (action == Expeditions.Ore)
                    player.Ore+=2;
                

                //LEVEL 5
                if (action == Expeditions.Pig)
                    player.Pigs++;


                //LEVEL 6
                if (action == Expeditions.Gold)
                    player.Gold+=2;
                
                
                //LEVEL 7
                if (action == Expeditions.FurnishCavern)
                {
                    FurnishCavern();
                    return;
                }


                //LEVEL 8
                if (action == Expeditions.Stable)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Stable);
                    return;
                }


                //LEVEL 9
                if (action == Expeditions.Tunnel)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Tunnel);
                    return;                    
                }

                if (action == Expeditions.SmallFence)
                {
                    player.Wood--;
                    SetPlayerPlaceTile(_players[0], TileTypes.SmallFence);
                    return;
                }


                //LEVEL 10
                if (action == Expeditions.Cow)
                    _players[0].Cows++;


                if (action == Expeditions.BigFence)
                {
                    player.Wood-=2;
                    SetPlayerPlaceTiles(_players[0], TileLists.BigFence);
                    return;
                }


                //LEVEL 11
                if (action == Expeditions.Clearing)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Clearing);
                    return;
                }

                if (action == Expeditions.Dwelling)
                {
                    player.Wood -= 2;
                    player.Stone -= 2;

                    List<Vector2> validSpots = _players[0].SetBuildingToPlace(BuildingTypes.Dwelling);
                    _serverSocket.SetPlaceBuildingTile("playerID", BuildingTypes.Dwelling, validSpots);
                    return;
                }


                //LEVEL 12
                if (action == Expeditions.Field)
                {
                    SetPlayerPlaceTile(_players[0], TileTypes.Field);
                    return;
                }

                if (action == Expeditions.Sow)
                {
                    List<string> sowActions = _actionSpaces.Find(x => x.ID == _activeActionSpaceID).GetSowActions(_players[0]);
                    _serverSocket.SendActions(_activeActionSpaceID, sowActions);
                    return;
                }


                //LEVEL 14
                if (action == Expeditions.BreedAnimals)
                {
                    List<string> breedActions = player.GetBreedActions();
                    _serverSocket.GetPlayerChoice("playerID", "Breed which animals?", string.Empty, breedActions);
                    return;
                }

                ReturnControlToPlayer(_players[0]);
            }
            else if (Array.Find(typeof (BreedActions).GetFields(), x => x.GetValue(null).ToString() == action) != null)
            {
                if (action == BreedActions.SheepOnly || 
                    action == BreedActions.SheepAndDonkeys ||
                    action == BreedActions.SheepAndPigs ||
                    action == BreedActions.SheepAndCows)
                {
                    player.BreedSheep();
                }

                if (action == BreedActions.DonkeysOnly ||
                    action == BreedActions.SheepAndDonkeys ||
                    action == BreedActions.DonkeysAndPigs ||
                    action == BreedActions.DonkeysAndCows)
                {
                    player.BreedDonkeys();
                }

                if (action == BreedActions.PigsOnly ||
                    action == BreedActions.SheepAndPigs ||
                    action == BreedActions.DonkeysAndPigs ||
                    action == BreedActions.PigsAndCows)
                {
                    player.BreedPigs();
                }

                if (action == BreedActions.CowsOnly ||
                    action == BreedActions.SheepAndCows ||
                    action == BreedActions.DonkeysAndCows ||
                    action == BreedActions.PigsAndCows)
                {
                    player.BreedCows();
                }

                List<string> actions = player.GetExpeditionOptions();
                if (actions.Count > 0)
                {
                    _serverSocket.GetPlayerChoice("playerID", player.GetExpeditionText(), string.Empty, actions);
                }
                else
                {
                    //do the dwarf weapon bonus
                    _expeditionInProgress = false;
                    player.ApplyExpeditionWeaponBonus();
                    _serverSocket.SetPlayerDwarves(player.GetDwarfStatus());

                    GetActions("playerID", _activeActionSpaceID);
                }
            }
            else
            {
                throw new NotImplementedException("Action " + action + " is not supported");
            }
        }

        private void FurnishCavern()
        {
            List<string> validBuildings = new List<string>();
            foreach (BuildingTile tile in BuildingTiles)
            {
                if (_players[0].CanAffordAndPlaceBuilding(tile))
                    validBuildings.Add(tile.BuildingType);
            }
            _serverSocket.ChooseBuildingTile("playerID", validBuildings);
        }

        private void FurnishDwelling()
        {
            List<string> validBuildings = new List<string>();
            foreach (BuildingTile tile in BuildingTiles)
            {
                if (_players[0].CanAffordAndPlaceBuilding(tile) && tile.BuildingGroup == BuildingTile.BuildingGroups.Dwelling)
                    validBuildings.Add(tile.BuildingType);
            }
            _serverSocket.ChooseBuildingTile("playerID", validBuildings);
        }

        private void ReturnControlToPlayer(CavernaPlayer player)
        {
            if (player.RearrangeAnimalsCheck())
                return;
            else if (_isHarvest)
                RunHarvest();
            else if (_activeActionSpaceID > 0)
                GetActions("playerID", _activeActionSpaceID);
            else
            {
                foreach (CavernaActionSpace space in _actionSpaces)
                {
                    if (!space.IsUsed)
                        _serverSocket.SetActionSpaceActive(space.ID);
                }
            }
        }

        private bool IsActionCardName(string action)
        {
            return Array.Find(typeof(ActionSpaceTypes).GetFields(), x => x.GetValue(null).ToString() == action) != null;
        }

        private bool IsActionSpaceAction(string action)
        {
            return Array.Find(typeof (CavernaActions).GetFields(), x => x.GetValue(null).ToString() == action) != null;
        }

        private bool IsDiscardAction(string action)
        {
            return Array.Find(typeof(DiscardActions).GetFields(), x => x.GetValue(null).ToString() == action) != null;
        }

        private bool IsFoodConversionAction(string action)
        {
            switch (action)
            {
                case FoodActions.Convert2Gold:
                case FoodActions.Convert3Gold:
                case FoodActions.Convert4Gold:
                case FoodActions.ConvertCow:
                case FoodActions.ConvertDonkey:
                case FoodActions.ConvertDonkeyPair:
                case FoodActions.ConvertGrain:
                case FoodActions.ConvertPig:
                case FoodActions.ConvertRuby:
                case FoodActions.ConvertSheep:
                case FoodActions.ConvertVeg:
                {
                    return true;
                }
            }
            return false;
        }

        public void GetRubyActions(string playerID)
        {
            _serverSocket.GetPlayerChoice("playerID", "Ruby Conversion", string.Empty, _players[0].GetRubyTradeOptions());
        }
    }
}