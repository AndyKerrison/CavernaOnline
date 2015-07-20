using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Assets.ServerScripts
{
    public class CavernaPlayer
    {
        //general player board data
        private readonly IServerSocket _serverSocket;
        private List<Dwarf> _dwarves = new List<Dwarf>();
        private readonly AnimalManager animalManager;
        private readonly CavernaPlayerTilesManager tilesManager = new CavernaPlayerTilesManager();
        private readonly PlayerResources _playerResources;
        
        //temporary variables to hold action status
        private int _activeDwarfIndex;
      
        private List<string> _usedExpeditionActions;
        private int _expeditionLevel;

        public string ID { get; set; }
        public int PlayerScore { get; private set; }

        //harvest variables
        private int _harvestFoodReq;
        public bool IsBreeding { get; set; }
        public bool HarvestFieldsComplete { get; set; }
        public bool HarvestFeedingComplete { get; set; }
        public bool HarvestAnimalsComplete { get; set; }
        public bool HarvestComplete { get; set; }
        public bool IsOneFoodPerDwarf { get; set; }

        public int Food
        {
            get { return _playerResources.Food; }
            set
            {
                _playerResources.Food = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Food, Food);
                CalculatePlayerScore();
            }
        }

        public int Wood
        {
            get { return _playerResources.Wood; }
            set
            {
                _playerResources.Wood = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Wood, Wood);
                CalculatePlayerScore();
            }
        }

        public int Stone
        {
            get { return _playerResources.Stone; }
            set
            {
                _playerResources.Stone = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Stone, Stone);
                CalculatePlayerScore();
            }
        }

        public int Ore
        {
            get { return _playerResources.Ore; }
            set
            {
                _playerResources.Ore = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Ore, Ore);
                CalculatePlayerScore();
            }
        }

        public int Rubies
        {
            get { return _playerResources.Rubies; }
            set
            {
                _playerResources.Rubies = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Ruby, Rubies);
                CalculatePlayerScore();
            }
        }

        public int Grain
        {
            get { return _playerResources.Grain; }
            set
            {
                _playerResources.Grain = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Grain, Grain);
                CalculatePlayerScore();
            }
        }

        public int Veg
        {
            get { return _playerResources.Veg; }
            set
            {
                _playerResources.Veg = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Veg, Veg);
                CalculatePlayerScore();
            }
        }

        public int Gold
        {
            get { return _playerResources.Gold; }
            set
            {
                _playerResources.Gold = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Gold, Gold);
                CalculatePlayerScore();
            }
        }

        public int BeggingCards
        {
            get { return _playerResources.BeggingCards; }
            set
            {
                _playerResources.BeggingCards = value;
                _serverSocket.SetPlayerResources(ID, ResourceTypes.Begging, BeggingCards);
                CalculatePlayerScore();
            }
        }

        public int Dogs
        {
            get { return animalManager.Dogs; }
            set
            {
                animalManager.Dogs = value;
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
            }
        }

        public int Sheep
        {
            get { return animalManager.Sheep; }
            set
            {
                animalManager.Sheep = value;
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
            }
        }

        public int Donkeys
        {
            get { return animalManager.Donkeys; }
            set
            {
                animalManager.Donkeys = value;
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
            }
        }

        public int Pigs
        {
            get { return animalManager.Pigs; }
            set
            {
                animalManager.Pigs = value;
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
            }
        }

        public int Cows
        {
            get { return animalManager.Cows; }
            set
            {
                animalManager.Cows = value;
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
            }
        }

        public CavernaPlayer(int food, IServerSocket serverSocket)
        {
            _serverSocket = serverSocket;
            animalManager = new AnimalManager();
            _playerResources = new PlayerResources();
            //these will trigger a ui update, in case you started a new game
            Wood = 0;
            Ore = 0;
            Stone = 0;
            Veg = 0;
            Grain = 0;
            Food = food;
            Rubies = 0;
            Gold = 0;
            BeggingCards = 0;
            _dwarves.Add(new Dwarf(false, _dwarves.Count+1));
            _dwarves.Add(new Dwarf(false, _dwarves.Count+1));
            CalculatePlayerScore();
        }

        public int GetActiveDwarfWeaponLevel()
        {
            return _dwarves[_activeDwarfIndex].WeaponLevel;
        }

        public bool HasActions()
        {
            return _dwarves.Any(x => !x.IsUsed);
        }

        public void ResetDwarves()
        {
            _activeDwarfIndex = 0;
            _dwarves.ForEach(x => x.IsUsed = false);
            _dwarves.ForEach(x => x.IsChild = false);
            _dwarves = _dwarves.OrderBy(x => x.WeaponLevel).ToList();

            //reassign the IDs, makes it easier to see what you are doing when reordering them.
            for (int i = 0; i < _dwarves.Count; i++)
            {
                _dwarves[i].ID = i + 1;
            }
        }

        public void SetDwarfUsed()
        {
            _dwarves.Find(x => !x.IsUsed).IsUsed = true;
        }

        public void SetActionFinished()
        {
            _activeDwarfIndex++;
        }

        public void SowGrain()
        {
            //find first empty field. Replace with field2Grain
            tilesManager.SowGrain(_serverSocket);
            Grain--;
        }

        public void SowVeg()
        {
            //find first empty field. Replace with field2Veg
            tilesManager.SowVeg(_serverSocket);
            Veg--;
        }

        private void AutoArrangeAnimals(bool triggerChoice, bool discardExcess)
        {
            tilesManager.AutoArrangeAnimals(_serverSocket, animalManager, triggerChoice, discardExcess, IsBreeding);
        }

        public bool CanAffordAndPlaceBuilding(BuildingTile buildingTile)
        {
            return (Wood >= buildingTile.WoodCost &&
                    Stone >= buildingTile.StoneCost &&
                    Veg >= buildingTile.VegCost &&
                    Grain >= buildingTile.GrainCost &&
                    Ore >= buildingTile.OreCost &&
                    Food >= buildingTile.FoodCost &&
                    tilesManager.HasCavern());
        }

        public List<Vector2> GetValidBuildingSpots()
        {
            return tilesManager.GetValidBuildingSpots();
        }

        public List<string> GetBlacksmithActions()
        {
            List<string> actions = new List<string>();

            if (Ore >= 8)
                actions.Add(CavernaActions.Level8Weapon);
            if (Ore >= 7)
                actions.Add(CavernaActions.Level7Weapon);
            if (Ore >= 6)
                actions.Add(CavernaActions.Level6Weapon);
            if (Ore >= 5)
                actions.Add(CavernaActions.Level5Weapon);
            if (Ore >= 4)
                actions.Add(CavernaActions.Level4Weapon);
            if (Ore >= 3)
                actions.Add(CavernaActions.Level3Weapon);
            if (Ore >= 2)
                actions.Add(CavernaActions.Level2Weapon);
            if (Ore >= 1)
                actions.Add(CavernaActions.Level1Weapon);
            return actions;
        }

        public void ArmActiveDwarf(string actionName)
        {
            _dwarves[_activeDwarfIndex].AddWeapon(actionName);
            CalculatePlayerScore();
        }

        public List<string> GetDwarfStatus()
        {
            List<string> dwarfStatus = new List<string>();
            foreach (Dwarf d in _dwarves)
            {
                dwarfStatus.Add(d.WeaponLevel + "_" + d.IsUsed + "_" + d.IsChild);
            }
            return dwarfStatus;
        }

        public void FamilyGrowth()
        {
            Dwarf dwarf = new Dwarf(true, _dwarves.Count + 1);
            _dwarves.Add(dwarf);
            CalculatePlayerScore();
        }

        public void FieldPhase()
        {
            int harvestedGrain;
            int harvestedVeg;
            tilesManager.FieldPhase(_serverSocket, out harvestedGrain, out harvestedVeg);
            Grain += harvestedGrain;
            Veg += harvestedVeg;

            HarvestFieldsComplete = true;
            CalculatePlayerScore();
        }

        public void BreedAnimals()
        {
            BreedSheep();
            BreedDonkeys();
            BreedPigs();
            BreedCows();

            HarvestAnimalsComplete = true;
            CalculatePlayerScore();
        }

        public void BreedSheep()
        {
            if (Sheep >= 2)
                Sheep++;
            CalculatePlayerScore();
        }

        public void BreedDonkeys()
        {
            if (Donkeys >= 2)
                Donkeys++;
            CalculatePlayerScore();
        }

        public void BreedPigs()
        {
            if (Pigs >= 2)
                Pigs++;
            CalculatePlayerScore();
        }

        public void BreedCows()
        {
            if (Cows >= 2)
                Cows++;
            CalculatePlayerScore();
        }

        public List<string> GetFoodOptions()
        {
            //return a list of all the feeding options. 'Feed' or 'begging cards' first
            List<string> foodActions = new List<string>();

            if (IsOneFoodPerDwarf)
                _harvestFoodReq = _dwarves.Count;
            else
            {
                _harvestFoodReq = _dwarves.Count*2 - _dwarves.Count(x => x.IsChild);
            }
            if (Food >= _harvestFoodReq)
            {
                foodActions.Add(FoodActions.FeedAllDwarves);
            }
            else
            {
                foodActions.Add(FoodActions.FeedAndTakeBeggingCards);
            }

            if (Sheep > 0)
                foodActions.Add(FoodActions.ConvertSheep);
            if (Pigs > 0)
                foodActions.Add(FoodActions.ConvertPig);
            if (Donkeys > 0)
                foodActions.Add(FoodActions.ConvertDonkey);
            if (Donkeys > 1)
                foodActions.Add(FoodActions.ConvertDonkeyPair);
            if (Cows > 0)
                foodActions.Add(FoodActions.ConvertCow);
            if (Grain > 0)
                foodActions.Add(FoodActions.ConvertGrain);
            if (Veg > 0)
                foodActions.Add(FoodActions.ConvertVeg);
            if (Rubies > 0)
                foodActions.Add(FoodActions.ConvertRuby);
            if (Gold > 1)
                foodActions.Add(FoodActions.Convert2Gold);
            if (Gold > 2)
                foodActions.Add(FoodActions.Convert3Gold);
            if (Gold > 3)
                foodActions.Add(FoodActions.Convert4Gold);
            return foodActions;
        }

        //returns true if triggers action
        public void ConvertToFood(string action)
        {
            switch (action)
            {
                case (FoodActions.Convert2Gold):
                {
                    Food++;
                    Gold -= 2;
                    break;
                }
                case (FoodActions.Convert3Gold):
                {
                    Food += 2;
                    Gold -= 3;
                    break;
                }
                case (FoodActions.Convert4Gold):
                {
                    Food += 3;
                    Gold -= 4;
                    break;
                }
                case (FoodActions.ConvertCow):
                {
                    Food += 3;
                    Cows -= 1;
                    break;
                }
                case (FoodActions.ConvertDonkey):
                {
                    Food += 1;
                    Donkeys -= 1;
                    break;
                }
                case (FoodActions.ConvertDonkeyPair):
                {
                    Food += 3;
                    Donkeys -= 2;
                    break;
                }
                case (FoodActions.ConvertGrain):
                {
                    Food += 1;
                    Grain -= 1;
                    break;
                }
                case (FoodActions.ConvertPig):
                {
                    Food += 2;
                    Pigs -= 1;
                    break;
                }
                case (FoodActions.ConvertRuby):
                {
                    Food += 2;
                    Rubies -= 1;
                    break;
                }
                case (FoodActions.ConvertSheep):
                {
                    Food += 1;
                    Sheep -= 1;
                    break;
                }
                case (FoodActions.ConvertVeg):
                {
                    Food += 2;
                    Veg -= 1;
                    break;
                }
                default:
                {
                    throw new NotImplementedException("Unimplemented food action: " + action);
                }
            }
            CalculatePlayerScore();
        }

        public void FeedDwarves()
        {
            Food -= _harvestFoodReq;
            while (Food < 0)
            {
                Food++;
                BeggingCards++;
            }
            HarvestFeedingComplete = true;
            CalculatePlayerScore();
        }

        public List<string> GetRubyTradeOptions()
        {
            List<string> actions = new List<string>();
            actions.Add(RubyTrades.Cancel);

            if (Rubies == 0)
                return actions;

            actions.Add(RubyTrades.Wood);
            actions.Add(RubyTrades.Stone);
            actions.Add(RubyTrades.Ore);
            actions.Add(RubyTrades.Grain);
            actions.Add(RubyTrades.Veg);
            actions.Add(RubyTrades.Sheep);
            actions.Add(RubyTrades.Donkey);
            actions.Add(RubyTrades.Pig);

            if (Food > 0)
                actions.Add(RubyTrades.Cow);

            //field
            if (tilesManager.HasSpaceForTile(TileTypes.Field))
                actions.Add(RubyTrades.Field);

            //clearing
            if (tilesManager.HasSpaceForTile(TileTypes.Clearing))
                actions.Add(RubyTrades.Clearing);

            //tunnel
            if (tilesManager.HasSpaceForTile(TileTypes.Tunnel))
                actions.Add(RubyTrades.Tunnel);

            //cavern
            if (Rubies > 1 && tilesManager.HasSpaceForTile(TileTypes.Cavern))
                actions.Add(RubyTrades.Cavern);
            
            if (_dwarves.Count(x => !x.IsChild && !x.IsUsed) > 1)
                actions.Add(RubyTrades.ReorderDwarf);

            return actions;
        }

        public List<string> GetReorderDwarfOptions()
        {
            List<string> results = new List<string>();

            foreach (Dwarf dwarf in _dwarves)
            {
                if (!dwarf.IsChild && !dwarf.IsUsed)
                {
                    if (dwarf.WeaponLevel == 0)
                        results.Add("Dwarf " + dwarf.ID + ", " + DwarfText.Unarmed);
                    else
                        results.Add("Dwarf " + dwarf.ID + ", " + DwarfText.WeaponLevel + " " + dwarf.WeaponLevel);
                }
            }
            return results;
        }

        public void ReorderDwarf(string action)
        {
            string dwarfID = action.Split(new[] {','})[0].Split(new[] {' '})[1];

            Dwarf dwarfToMove = _dwarves.Find(x => x.ID.ToString(CultureInfo.InvariantCulture) == dwarfID);

            _dwarves.Remove(dwarfToMove);

            _dwarves.Insert(_dwarves.FindIndex(x => !x.IsUsed), dwarfToMove);
        }

        public void NewExpedition(string actionName)
        {
            _expeditionLevel = 0;
            _usedExpeditionActions = new List<string>();
            if (actionName == CavernaActions.Level1Expedition)
                _expeditionLevel = 1;
            _usedExpeditionActions = new List<string>();
            if (actionName == CavernaActions.Level2Expedition)
                _expeditionLevel = 2;
            _usedExpeditionActions = new List<string>();
            if (actionName == CavernaActions.Level3Expedition)
                _expeditionLevel = 3;
        }

        public List<string> GetExpeditionOptions()
        {
            List<string> actions = new List<string>();
            if (_usedExpeditionActions.Count() >= _expeditionLevel)
                return actions;

            int weaponLevel = GetActiveDwarfWeaponLevel();

            actions.Add(Expeditions.Nothing);

            if (weaponLevel >= 1)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.AllWeaponsPlusOne))
                    actions.Add(Expeditions.AllWeaponsPlusOne);
                if (!_usedExpeditionActions.Contains(Expeditions.Wood))
                    actions.Add(Expeditions.Wood);
                if (!_usedExpeditionActions.Contains(Expeditions.Dog))
                    actions.Add(Expeditions.Dog);
            }

            if (weaponLevel >= 2)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Grain))
                    actions.Add(Expeditions.Grain);
                if (!_usedExpeditionActions.Contains(Expeditions.Sheep))
                    actions.Add(Expeditions.Sheep);
            }

            if (weaponLevel >= 3)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Stone))
                    actions.Add(Expeditions.Stone);
                if (!_usedExpeditionActions.Contains(Expeditions.Donkey))
                    actions.Add(Expeditions.Donkey);
            }

            if (weaponLevel >= 4)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Veg))
                    actions.Add(Expeditions.Veg);
                if (!_usedExpeditionActions.Contains(Expeditions.Ore))
                    actions.Add(Expeditions.Ore);
            }

            if (weaponLevel >= 5)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Pig))
                    actions.Add(Expeditions.Pig);
            }

            if (weaponLevel >= 6)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Gold))
                    actions.Add(Expeditions.Gold);
            }

            if (weaponLevel >= 7)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.FurnishCavern) &&
                    (CavernaManager.Instance.BuildingTiles.Exists(CanAffordAndPlaceBuilding)))
                    actions.Add(Expeditions.FurnishCavern);
            }

            if (weaponLevel >= 8)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Stable) &&
                    tilesManager.HasSpaceForTile(TileTypes.Stable))
                    actions.Add(Expeditions.Stable);                
            }

            if (weaponLevel >= 9)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Stable) &&
                    tilesManager.HasSpaceForTile(TileTypes.Tunnel))
                    actions.Add(Expeditions.Tunnel);

                if (!_usedExpeditionActions.Contains(Expeditions.SmallFence) &&
                    tilesManager.HasSpaceForTile(TileTypes.SmallFence) &&
                    Wood >=1)
                    actions.Add(Expeditions.SmallFence);
            }

            if (weaponLevel >= 10)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Cow))
                    actions.Add(Expeditions.Cow);

                if (!_usedExpeditionActions.Contains(Expeditions.BigFence) &&
                    tilesManager.HasSpaceForTiles(TileLists.BigFence) &&
                    Wood >=2)
                    actions.Add(Expeditions.BigFence);
            }

            if (weaponLevel >= 11)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Clearing) &&
                    tilesManager.HasSpaceForTile(TileTypes.Clearing))
                    actions.Add(Expeditions.Clearing);

                if (!_usedExpeditionActions.Contains(Expeditions.Dwelling) &&
                    tilesManager.HasCavern() &&
                    Wood >= 2 &&
                    Stone >=2)
                    actions.Add(Expeditions.Dwelling);
            }

            if (weaponLevel >= 12)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Field) &&
                    tilesManager.HasSpaceForTile(TileTypes.Field))
                    actions.Add(Expeditions.Field);

                if (!_usedExpeditionActions.Contains(Expeditions.Sow) &&
                    tilesManager.HasTile(TileTypes.Field) &&
                    (Veg > 0 || Grain > 0))
                    actions.Add(Expeditions.Sow);
            }

            if (weaponLevel >= 14)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.BreedAnimals) &&
                    (Sheep >=2 || Donkeys >=2 || Pigs >=2 || Cows >=2))
                    actions.Add(Expeditions.BreedAnimals);
            }

            return actions;
        }

        public void SetExpeditionRewardTaken(string action)
        {
            _usedExpeditionActions.Add(action);
        }

        public string GetExpeditionText()
        {
            return (_expeditionLevel - _usedExpeditionActions.Count()) + " rewards remainig";
        }

        public void ApplyExpeditionWeaponBonus()
        {
            if (_usedExpeditionActions.Contains(Expeditions.AllWeaponsPlusOne))
            {
                _dwarves.ForEach(x => x.IncreaseWeaponLevel());
            }
            _dwarves[_activeDwarfIndex].IncreaseWeaponLevel();
            CalculatePlayerScore();
        }

        public List<string> GetBreedActions()
        {
            List<string> breedActions = new List<string>();
            
            if (Sheep >=2)
                breedActions.Add(BreedActions.SheepOnly);
            if (Donkeys >= 2)
                breedActions.Add(BreedActions.DonkeysOnly);
            if (Pigs >= 2)
                breedActions.Add(BreedActions.PigsOnly);
            if (Cows >= 2)
                breedActions.Add(BreedActions.CowsOnly);
            if (Sheep >= 2)
            {
                if (Donkeys >= 2)
                    breedActions.Add(BreedActions.SheepAndDonkeys);
                if (Pigs >= 2)
                    breedActions.Add(BreedActions.SheepAndPigs);
                if (Cows >= 2)
                    breedActions.Add(BreedActions.SheepAndCows);                
            }
            if (Donkeys >= 2)
            {
                if (Pigs >= 2)
                    breedActions.Add(BreedActions.DonkeysAndPigs);
                if (Cows >= 2)
                    breedActions.Add(BreedActions.DonkeysAndCows);
            }
            if (Pigs >= 2 && Cows >= 2)
                breedActions.Add(BreedActions.PigsAndCows);

            return breedActions;
        }

        public void CalculatePlayerScore()
        {
            int negativePoints = 0;
            int score = 0;

            //Gold coins and Begging markers: Add up the values on your Gold coins and subtract 3 Gold points from that for
            //each Begging marker you have.
            score += Gold;
            negativePoints += 3 * BeggingCards;
            
            //1 Gold per Farm animal and Dog: Each animal is worth 1 Gold point at the end of the
            //game, even Dogs. (Dogs are not considered Farm animals.)

            score += Dogs;
            score += Sheep;
            score += Donkeys;
            score += Pigs;
            score += Cows;

            //-2 Gold per missing type of Farm animal: At the end of the game, you should have
            //at least 1 Sheep, 1 Donkey, 1 Wild boar and 1 Cattle. You lose 2 Gold points for each
            //of these types that you do not have on your Home board. (You do not need to have any
            //Dogs.)

            if (Sheep == 0)
                negativePoints += 2;
            if (Donkeys == 0)
                negativePoints += 2;
            if (Pigs == 0)
                negativePoints += 2;
            if (Cows == 0)
                negativePoints += 2;

            //½ Gold per Grain (rounded up): Count all of your Grain tokens – both those in your
            //supply and those still left on Fields. Divide this number by 2 and round it up. This is the
            //number of Gold points you get for Grain.

            int grainCount = Grain;
            grainCount += tilesManager.GetTileCount(TileTypes.Field1Grain);
            grainCount += 2*tilesManager.GetTileCount(TileTypes.Field2Grain);
            grainCount += 3*tilesManager.GetTileCount(TileTypes.Field3Grain);

            score += (int)Math.Ceiling(grainCount/2f);

            //1 Gold per Vegetable: Count all of your Vegetable tokens – both those in your supply and those still left on Fields.
            //You get this number of Gold points for Vegetables.
            score += Veg;
            score += tilesManager.GetTileCount(TileTypes.Field1Veg);
            score += 2*tilesManager.GetTileCount(TileTypes.Field2Veg);

            //1 Gold per Ruby: At the end of the game, each Ruby is worth 1 Gold point.
            score += Rubies;

            //1 Gold per Dwarf: At the end of the game, each of your Dwarfs is worth 1 Gold point.
            score += _dwarves.Count;

            //-1 Gold per unused space: Count the number of spaces on your Home board that have no tile or Stable on them.
            //You lose 1 Gold point for each such space. The two pre-printed Caverns of your cave system are considered used.
            //(Unfurnished Caverns are also considered used.)

            negativePoints += tilesManager.EmptySpaceCount();

            
            //Gold for Furnishing tiles, Pastures and Mines: Add up the Gold point values on all of your tiles. Small pastures
            //are worth 2 Gold points, Large pastures are worth 4 Gold points (regardless of the type and number of animals on
            //those tiles). Ore mines are worth 3 Gold points, Ruby mines are worth 4 Gold points (regardless of whether they hold
            //a Donkey or not). The value of a Furnishing tile is printed on the right of the tile, right under the name of the tile.

            score += 2*tilesManager.GetTileCount(TileTypes.SmallFence);
            score += 4*tilesManager.GetTileCount(TileTypes.BigFence1);
            score += 3*tilesManager.GetTileCount(TileTypes.OreMine);
            score += 4*tilesManager.GetTileCount(TileTypes.RubyMine);

            score += tilesManager.GetBuildingPoints();

            //Bonus points for Parlors, Storages and Chambers: Most of the Furnishing tiles called Parlors, Storages and
            //Chambers (indicated by the yellow name tag) may be worth Bonus points depending on the condition they impose.
            //The scoring pad has multiple lines for Bonus points. You can use one line per Furnishing tile that you get Bonus
            //points for. (Details on the Furnishing tiles can be found on page A3 of the appendix.)
            if (tilesManager.HasTile(BuildingTypes.TreasureChamber))
                score += Rubies;
            if (tilesManager.HasTile(BuildingTypes.FoodChamber))
                score += 2*Math.Min(Veg, Grain);
            if (tilesManager.HasTile(BuildingTypes.PrayerChamber) && !_dwarves.Exists(x => x.WeaponLevel > 0))
                score += 8;
            if (tilesManager.HasTile(BuildingTypes.BroomChamber))
            {
                if (_dwarves.Count == 5)
                    score += 5;
                if (_dwarves.Count >=6)
                    score += 10;
            }
            if (tilesManager.HasTile(BuildingTypes.FodderChamber))
            {
                int totalAnimals = Sheep + Cows + Donkeys + Pigs;
                score += (int)Math.Floor(totalAnimals/3f);
            }
            if (tilesManager.HasTile(BuildingTypes.WritingChamber))
            {
                negativePoints -= 7;
                if (negativePoints < 0)
                    negativePoints = 0;
            }
            if (tilesManager.HasTile(BuildingTypes.StoneStorage))
            {
                score += Stone;
            }
            if (tilesManager.HasTile(BuildingTypes.OreStorage))
            {
                score += (int)Math.Floor(Ore/2f);
            }
            if (tilesManager.HasTile(BuildingTypes.WeaponStorage))
            {
                score += 3*_dwarves.FindAll(x => x.WeaponLevel > 0).Count;
            }
            if (tilesManager.HasTile(BuildingTypes.SuppliesStorage))
            {
                if (!_dwarves.Exists(x => x.WeaponLevel == 0)) //no unarmed dwarves
                    score += 8;
            }
            if (tilesManager.HasTile(BuildingTypes.MainStorage))
            {
                //2 points per scoring tile, including this one
                score += 2*tilesManager.GetScoringTileCount();
            }
            score -= negativePoints;

            PlayerScore = score;
            _serverSocket.SetPlayerResources(ID, ResourceTypes.ScoreMarker, score);
        }

        public bool RearrangeAnimalsCheck()
        {
            if (tilesManager.ArrangeAnimalsTriggered)
                AutoArrangeAnimals(true, false);
            return tilesManager.ArrangeAnimalsTriggered;
        }

        public void Discard(string action)
        {
            if (action == DiscardActions.DiscardAllUnassignedAnimals)
            {
                AutoArrangeAnimals(false, true);
            }
            if (action == DiscardActions.DiscardCow)
            {
                Cows--;
            }
            if (action == DiscardActions.DiscardPig)
            {
                Pigs--;
            }
            if (action == DiscardActions.DiscardDonkey)
            {
                Donkeys--;
            }
            if (action == DiscardActions.DiscardSheep)
            {
                Sheep--;
            }
        }

        public List<Vector2> SetTilesToPlace(List<string> tileList)
        {
            return tilesManager.SetTilesToPlace(tileList);
        }

        //TODO check this one.... should I set a tiles list somewhere??
        public List<Vector2> SetBuildingToPlace(string buildingType)
        {
            return tilesManager.SetBuildingToPlace(buildingType);
        }

        public string GetNextTile()
        {
            return tilesManager.GetNextTile();
        }

        public List<Vector2> GetValidTileSpots()
        {
            return tilesManager.GetValidTileSpots();
        }

        public bool HasTilesToPlace()
        {
            return tilesManager.HasTilesToPlace();
        }

        public List<Vector2> GetDoubleFencedPastures()
        {
            return tilesManager.GetDoubleFencedPastures();
        }

        public void SetBuildingTileAt(Vector2 position, string buildingType)
        {
            Wood -= new BuildingTile(buildingType).WoodCost;
            Stone -= new BuildingTile(buildingType).StoneCost;
            Veg -= new BuildingTile(buildingType).VegCost;
            Grain -= new BuildingTile(buildingType).GrainCost;
            Ore -= new BuildingTile(buildingType).OreCost;
            Food -= new BuildingTile(buildingType).FoodCost;

            tilesManager.SetBuildingTileAt(_serverSocket, position, buildingType);

            CalculatePlayerScore();
        }

        public void SetTileAt(Vector2 position, bool isForest)
        {
            int foodGain;
            int rubyGain;
            int pigsGain;
            tilesManager.SetTileAt(_serverSocket, position, isForest, out foodGain, out pigsGain, out rubyGain);

            Food += foodGain;
            Pigs += pigsGain;
            Rubies += rubyGain;

            CalculatePlayerScore();
            AutoArrangeAnimals(false, false);
        }

        public int GetDwarfCapacity()
        {
            return tilesManager.GetDwarfCapacity();
        }

        public bool HasSpaceForTiles(List<string> tileList)
        {
            return tilesManager.HasSpaceForTiles(tileList);
        }

        public bool HasSpaceForTile(string tileType)
        {
            return tilesManager.HasSpaceForTile(tileType);
        }

        public int GetTileCount(string tileType)
        {
            return tilesManager.GetTileCount(tileType);
        }

        public bool CanBuildDoubleFence()
        {
            return tilesManager.CanBuildDoubleFence();
        }
    }
}