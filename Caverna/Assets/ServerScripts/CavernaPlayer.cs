using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.ServerScripts
{
    public class CavernaPlayer
    {
        public string ID { get; set; }
        private IServerSocket _serverSocket;
        private List<Dwarf> _dwarves = new List<Dwarf>();
        private int _activeDwarfIndex;

        private const int TileAreaWidth = 3;
        private const int TileAreaHeight = 4;

        public string[,] CaveSpaces = new string[TileAreaWidth, TileAreaHeight];
        public string[,] ForestSpaces = new string[TileAreaWidth, TileAreaHeight];
        public string[,] StableSpaces = new string[TileAreaWidth, TileAreaHeight];
        private Vector2 _firstPartOfDouble = new Vector2(-1, -1);
        private bool _isPlacingSecondPartOfTile;
        private int _harvestFoodReq;
        public bool IsBreeding { get; set; }
        private List<string> TilesToPlace { get; set; }

        private AnimalManager animalManager;
        private PlayerResources _playerResources;

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
                //_serverSocket.SetPlayerResources(ID, ResourceTypes.Dogs, Dogs);
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
                //_serverSocket.SetPlayerResources(ID, ResourceTypes.Sheep, Sheep);
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
                //_serverSocket.SetPlayerResources(ID, ResourceTypes.Donkeys, Donkeys);
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
                //_serverSocket.SetPlayerResources(ID, ResourceTypes.Pigs, Pigs);
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
                //_serverSocket.SetPlayerResources(ID, ResourceTypes.Cows, Cows);
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
            }
        }

        private List<Vector2> _doubleFencedPastures;

        private List<string> _usedExpeditionActions;
        private int _expeditionLevel;
        private bool _arrangeAnimalsTriggered;

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
            CaveSpaces[0, 0] = TileTypes.StartingTile;
            CaveSpaces[0, 1] = TileTypes.Cavern;
            _doubleFencedPastures = new List<Vector2>();
            TilesToPlace = new List<string>();
            CalculatePlayerScore();
        }

        public int GetActiveDwarfWeaponLevel()
        {
            return _dwarves[_activeDwarfIndex].WeaponLevel;
        }

        public int GetDwarfCapacity()
        {
            int capacity = 2;
            capacity += GetTileCount(BuildingTypes.Dwelling);
            capacity += GetTileCount(BuildingTypes.SimpleDwelling1);
            if (capacity > 5)
                capacity = 5;
            return capacity;
        }

        public bool HasActions()
        {
            return _dwarves.Where(x => !x.IsUsed).Count() > 0;
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

        private List<Vector2> GetMatchingCaveTileSpaces(string tileType)
        {
            List<Vector2> results = new List<Vector2>();
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (CaveSpaces[x, y] == tileType)
                    {
                        results.Add(new Vector2(x, y));
                    }
                }
            }
            return results;
        }

        private List<Vector2> GetMatchingForestTileSpaces(string tileType)
        {
            List<Vector2> results = new List<Vector2>();
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == tileType)
                    {
                        results.Add(new Vector2(x, y));
                    }
                }
            }
            return results;
        }

        public List<Vector2> GetValidTileSpots()
        {
            //big fences go on clearings with adjacent clearings.
            if (TilesToPlace[0] == TileTypes.BigFence1)
            {
                List<Vector2> clearings = GetMatchingForestTileSpaces(TileTypes.Clearing);
                return clearings.FindAll(x => clearings.Exists(y => IsAdjacent(x, y)));
            }

            //the second part must go next to the first
            if (TilesToPlace[0] == TileTypes.BigFence2)
            {
                List<Vector2> clearings = GetMatchingForestTileSpaces(TileTypes.Clearing);
                return clearings.FindAll(x => IsAdjacent(x, _firstPartOfDouble));
            }

            //small fences go on clearings.
            if (TilesToPlace[0] == TileTypes.SmallFence)
            {
                List<Vector2> fenceList = GetMatchingForestTileSpaces(TileTypes.Clearing);
                return fenceList;
            }

            //if this is a stable, return Forest, Meadow or Pasture
            //so long as there's no stable there already
            if (TilesToPlace[0] == TileTypes.Stable)
            {
                if (GetTileCount(TileTypes.Stable) == 3)
                    return new List<Vector2>();

                List<Vector2> stableSpots = GetMatchingForestTileSpaces(null);
                stableSpots.AddRange(GetMatchingForestTileSpaces(TileTypes.Clearing));
                stableSpots.AddRange(GetMatchingForestTileSpaces(TileTypes.SmallFence));
                stableSpots.AddRange(GetMatchingForestTileSpaces(TileTypes.BigFence1));
                stableSpots.AddRange(GetMatchingForestTileSpaces(TileTypes.BigFence2));

                return
                    stableSpots.FindAll(
                        stableVector => StableSpaces[(int) stableVector.x, (int) stableVector.y] != TileTypes.Stable);
            }

            //if this is a RubyMine, return tunnels and deep tunnels
            if (TilesToPlace[0] == TileTypes.RubyMine)
            {
                List<Vector2> tunnels = GetMatchingCaveTileSpaces(TileTypes.Tunnel);
                tunnels.AddRange(GetMatchingCaveTileSpaces(TileTypes.DeepTunnel));
                return tunnels;
            }

            bool isForest = TilesToPlace[0] == TileTypes.Field || TilesToPlace[0] == TileTypes.Clearing;
            bool deepTile = TilesToPlace[0] == TileTypes.OreMine || TilesToPlace[0] == TileTypes.DeepTunnel;

            //if this is a deep tile, only tunnel spots are valid
            //if this is the first of a double deep tile, only tunnel spots adjacent to tunnel spots are valid
            if (deepTile)
            {
                List<Vector2> tunnels = GetMatchingCaveTileSpaces(TileTypes.Tunnel);
                if (TilesToPlace.Count == 1)
                    return tunnels;

                List<Vector2> results = new List<Vector2>();
                for (int i = 0; i < tunnels.Count; i++)
                {
                    for (int j = 0; j < tunnels.Count; j++)
                    {
                        if (IsAdjacent(tunnels[i], tunnels[j]))
                            results.Add(tunnels[i]);
                    }
                }
                return results.Distinct().ToList();
            }

            List<Vector2> validSpaces = GetValidSingleEmptyTileSpaces(isForest);

            //if this is the first part of a double, valid spot is an empty space next to an empty space next to an existing tile
            //if this is the second part of a double, valid spot is an empty space next to the first part, next to an existing tile
            //if this is a single piece, valid spot is an empty space next to an existing tile

            if (TilesToPlace.Count > 1) //first part of double tile
            {
                List<Vector2> resultSpaces = new List<Vector2>();
                foreach (Vector2 validSpace in validSpaces)
                {
                    //it can be an empty space adjacent to a valid single tile space
                    List<Vector2> emptySpaces = GetAdjacentEmptySpaces(validSpace, isForest);
                    if (emptySpaces.Count > 0)
                    {
                        resultSpaces.AddRange(emptySpaces);

                        //or a valid single tile space with an adjacent empty space
                        resultSpaces.Add(validSpace);
                    }
                }

                //if this is a field, remove all stable spaces
                if (TilesToPlace[0] == TileTypes.Field)
                {
                    resultSpaces.RemoveAll(space => StableSpaces[(int) space.x, (int) space.y] == TileTypes.Stable);
                }

                return resultSpaces.Distinct().ToList();
            }
            if (_isPlacingSecondPartOfTile)
            {
                //if a forest, and we chose the first start tile
                //any place next to the starting space is also valid
                if (isForest && (int) _firstPartOfDouble.x == 2 && (int) _firstPartOfDouble.y == 0)
                    return GetAdjacentEmptySpaces(_firstPartOfDouble, true);

                //either this space was a normal space and any empty connected one will do
                if (HasAdjacentTile(_firstPartOfDouble, isForest))
                    return GetAdjacentEmptySpaces(_firstPartOfDouble, isForest);

                //or this one wasn't, so you must pick a connector
                return validSpaces.Where(x => IsAdjacent(x, _firstPartOfDouble)).ToList();
            }

            //normal single tiles
            //if this is a field, remove all stable spaces
            if (TilesToPlace[0] == TileTypes.Field)
            {
                validSpaces.RemoveAll(space => StableSpaces[(int) space.x, (int) space.y] == TileTypes.Stable);
            }
            return validSpaces;
        }

        private bool IsAdjacent(Vector2 pos1, Vector2 pos2)
        {
            if ((int) pos1.x == (int) pos2.x && Math.Abs((int) pos1.y - (int) pos2.y) == 1)
                return true;
            if ((int) pos1.y == (int) pos2.y && Math.Abs((int) pos1.x - (int) pos2.x) == 1)
                return true;
            return false;
        }

        private bool HasAdjacentTile(Vector2 position, bool isForest)
        {
            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            int x = (int) position.x;
            int y = (int) position.y;
            if (position.x > 0 && tileArea[x - 1, y] != null)
                return true;
            if (position.x < TileAreaWidth - 1 && tileArea[x + 1, y] != null)
                return true;
            if (position.y > 0 && tileArea[x, y - 1] != null)
                return true;
            if (position.y < TileAreaHeight - 1 && tileArea[x, y + 1] != null)
                return true;
            return false;
        }

        private List<Vector2> GetValidSingleEmptyTileSpaces(bool isForest)
        {
            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            //returns any empty spaces next to a non-null space.
            List<Vector2> results = new List<Vector2>();
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    //adjacent to non null space which is NOT the first of a two psrt tile
                    if (tileArea[x, y] != null && ((int) _firstPartOfDouble.x != x || (int) _firstPartOfDouble.y != y))
                    {
                        results.AddRange(GetAdjacentEmptySpaces(new Vector2(x, y), isForest));
                    }

                    //in the forest, (2,0) is always valid if empty
                    if (isForest && x == 2 && y == 0 && tileArea[x, y] == null)
                        results.Add(new Vector2(x, y));
                }
            }
            return results.Distinct().ToList();
        }

        private List<Vector2> GetAdjacentEmptySpaces(Vector2 targetSpace, bool isForest)
        {
            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            int x = (int) targetSpace.x;
            int y = (int) targetSpace.y;
            List<Vector2> results = new List<Vector2>();
            if (x > 0 && tileArea[x - 1, y] == null)
            {
                results.Add(new Vector2(x - 1, y));
            }
            if (x < TileAreaWidth - 1 && tileArea[x + 1, y] == null)
            {
                results.Add(new Vector2(x + 1, y));
            }
            if (y < TileAreaHeight - 1 && tileArea[x, y + 1] == null)
            {
                results.Add(new Vector2(x, y + 1));
            }
            if (y > 0 && tileArea[x, y - 1] == null)
            {
                results.Add(new Vector2(x, y - 1));
            }
            return results;
        }

        public void SetTileAt(Vector2 position, bool isForest)
        {
            if (TilesToPlace.First() == TileTypes.Stable)
            {
                int x = (int) position.x;
                int y = (int) position.y;
                StableSpaces[x, y] = TileTypes.Stable;
                TilesToPlace.RemoveAt(0);
                _isPlacingSecondPartOfTile = TilesToPlace.Count > 0;
                if (_isPlacingSecondPartOfTile)
                {
                    _firstPartOfDouble = position;
                }
                else
                {
                    _firstPartOfDouble = new Vector2(-1, -1);
                }
                _serverSocket.SetPlayerTileType(position, GetTileType(position, isForest), !isForest);
                AutoArrangeAnimals(false, false);
                CalculatePlayerScore();
                return;
            }
            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            string oldTile = tileArea[(int) position.x, (int) position.y];
            string newTile = TilesToPlace.First();

            tileArea[(int) position.x, (int) position.y] = newTile;
            TilesToPlace.RemoveAt(0);

            //if this was a double fenced pasture, record it
            if (_isPlacingSecondPartOfTile && newTile == TileTypes.BigFence2)
            {
                _doubleFencedPastures.Add(_firstPartOfDouble);
                _doubleFencedPastures.Add(position);
            }


            _isPlacingSecondPartOfTile = TilesToPlace.Count > 0;
            if (_isPlacingSecondPartOfTile)
            {
                _firstPartOfDouble = position;
            }
            else
            {
                _firstPartOfDouble = new Vector2(-1, -1);
            }

            //apply any bonuses from this tile
            if (isForest)
            {
                if ((int) position.x == 1 && (int) position.y == 0)
                    Food++;
                if ((int) position.x == 0 && (int) position.y == 1)
                    Pigs++;
                if ((int) position.x == 2 && (int) position.y == 3)
                    Pigs++;
            }
            else
            {
                if (oldTile == TileTypes.DeepTunnel && newTile == TileTypes.RubyMine)
                    Rubies++;
                if ((int) position.x == 1 && (int) position.y == 0)
                    Food++;
                if ((int) position.x == 2 && (int) position.y == 3)
                    Food += 2;
            }

            _serverSocket.SetPlayerTileType(position, GetTileType(position, isForest), !isForest);
            CalculatePlayerScore();
            AutoArrangeAnimals(false, false);
        }

        public List<string> GetTileType(Vector2 position, bool isForest)
        {
            List<string> tileType = new List<string>();

            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            tileType.Add(tileArea[(int) position.x, (int) position.y]);
            if (isForest && StableSpaces[(int) position.x, (int) position.y] == TileTypes.Stable)
            {
                tileType.Add(TileTypes.Stable);
            }
            //if (String.IsNullOrEmpty(tileType[0]))
            //    tileType.RemoveAt(0);
            return tileType;
        }

        public int GetFieldCount()
        {
            int fieldCount = 0;
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field)
                        fieldCount++;
                }
            }

            return fieldCount;
        }

        public void SowGrain()
        {
            //find first empty field. Replace with field2Grain
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field)
                    {
                        ForestSpaces[x, y] = TileTypes.Field3Grain;
                        Grain--;
                        _serverSocket.SetPlayerTileType(new Vector2(x, y), GetTileType(new Vector2(x, y), true), false);
                        CalculatePlayerScore();
                        return;
                    }
                }
            }
        }

        public void SowVeg()
        {
            //find first empty field. Replace with field2Grain
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field)
                    {
                        ForestSpaces[x, y] = TileTypes.Field2Veg;
                        Veg--;
                        _serverSocket.SetPlayerTileType(new Vector2(x, y), GetTileType(new Vector2(x, y), true), false);
                        CalculatePlayerScore();
                        return;
                    }
                }
            }
        }

        private void AutoArrangeAnimals(bool triggerChoice, bool discardExcess)
        {
            //Entry room holds 2 of same type
            //Pasture holds 2 of same type
            //Large pasture holds 4 of same time
            //Stable on pasture doubles capacity
            //Stable on meadow holds 1 animal

            //Ore/Ruby mine -> 1 donkey each
            //Stable on forest holds 1 pig
            //Dogs go anywhere... but preferably on meadow/pasture
            //dog(s) on meadow holds dogs+1 sheep OR meadow capacity
            //dog(s) on pasture holder dogs+1 sheep OR pasture capacity

            //other areas exist as 'blocks'
            //dogs can cause these to be variable... there will be P*D combinations, where
            //P = number of meadows/pastures, D = number of dogs 
            //maybe just do these if we have excess sheep... which will be allocated last anyway.

            List<AnimalHolder> animalsHolders = new List<AnimalHolder>();

            //add starting tile
            AnimalHolder startingTile = new AnimalHolder();
            startingTile.TileType = TileTypes.StartingTile;
            startingTile.genericCapacity = 2;
            startingTile.position = new Vector2(0f, 0f);
            startingTile.isCave = true;
            animalsHolders.Add(startingTile);

            //add single fenced areas
            List<Vector2> smallFencedAreas = GetMatchingForestTileSpaces(TileTypes.SmallFence);
            foreach (Vector2 smallPasture in smallFencedAreas)
            {
                AnimalHolder pasture = new AnimalHolder();
                pasture.TileType = TileTypes.SmallFence;
                pasture.isCave = false;
                pasture.genericCapacity = 2;
                pasture.position = smallPasture;
                if (StableSpaces[(int)smallPasture.x, (int)smallPasture.y] == TileTypes.Stable)
                    pasture.genericCapacity = pasture.genericCapacity*2;
                animalsHolders.Add(pasture);
            }

            //add double fenced areas
            for(int i=0; i< _doubleFencedPastures.Count; i+=2)
            {
                AnimalHolder pasture = new AnimalHolder();
                pasture.TileType = TileTypes.BigFence1;
                pasture.isCave = false;
                pasture.genericCapacity = 4;
                pasture.position = _doubleFencedPastures[i];
                if (StableSpaces[(int)_doubleFencedPastures[i].x, (int)_doubleFencedPastures[i].y] == TileTypes.Stable)
                    pasture.genericCapacity = pasture.genericCapacity * 2;
                if (StableSpaces[(int)_doubleFencedPastures[i+1].x, (int)_doubleFencedPastures[i+1].y] == TileTypes.Stable)
                    pasture.genericCapacity = pasture.genericCapacity * 2;
                animalsHolders.Add(pasture);
            }

            //add meadows with stables
            List<Vector2> meadows = GetMatchingForestTileSpaces(TileTypes.Clearing);
            foreach (Vector2 meadow in meadows)
            {
                AnimalHolder clearing = new AnimalHolder();
                clearing.TileType = TileTypes.Clearing;
                clearing.isCave = false;
                clearing.genericCapacity = 0;
                clearing.position = meadow;
                if (StableSpaces[(int)meadow.x, (int)meadow.y] == TileTypes.Stable)
                    clearing.genericCapacity = 1;
                animalsHolders.Add(clearing);
            }

            //add forests with stables
            List<Vector2> forestPositions = GetMatchingForestTileSpaces(null);
            foreach (Vector2 position in forestPositions)
            {
                AnimalHolder holder = new AnimalHolder();
                holder.TileType = null;
                holder.isCave = false;
                holder.genericCapacity = 0;
                holder.position = position;
                if (StableSpaces[(int)position.x, (int)position.y] == TileTypes.Stable)
                    holder.pigCapacity = 1;
                animalsHolders.Add(holder);
            }

            //add ore mines
            List<Vector2> oreMinePositions = GetMatchingCaveTileSpaces(TileTypes.OreMine);
            foreach (Vector2 position in oreMinePositions)
            {
                AnimalHolder holder = new AnimalHolder();
                holder.TileType = TileTypes.OreMine;
                holder.isCave = true;
                holder.donkeyCapacity = 1;
                holder.position = position;
                animalsHolders.Add(holder);
            }

            //add ruby mines
            List<Vector2> rubyMinePositions = GetMatchingCaveTileSpaces(TileTypes.RubyMine);
            foreach (Vector2 position in rubyMinePositions)
            {
                AnimalHolder holder = new AnimalHolder();
                holder.TileType = TileTypes.RubyMine;
                holder.isCave = true;
                holder.donkeyCapacity = 1;
                holder.position = position;
                animalsHolders.Add(holder);
            }

            //add whatever the dogs are doing

            int cowsToAllocate = Cows;
            int pigsToAllocate = Pigs;
            int donkeysToAllocate = Donkeys;
            int sheepToAllocate = Sheep;
            int dogsToAllocate = Dogs;

            //Dogs spread between meadows as evenly as possible
            //sheepWatched = Math.Min(dogs, meadows)*2
            //if dogs > meadows
            //sheepWatched += dogs-meadows
            //e.g, 5 dogs 2 meadows --> 7 sheep
            //---> possible exception for stabled meadow... assign dogs to this last (generic capacity 1)
            //
            //If no meadows, dogs cluster in lowest capacity pasture to maximise extra sheep capacity
            //
            //If no pastures, dogs can live in the home dwelling or near the house or something
            //
            //When filling areas, do not assign sheep/generic areas until after all sheep only areas.
            //Then assign sheep only if there are MORE sheep than generic capacity (ie, extra space will get used)
            //Otherwise, treat as normal

            //ASSIGN DOGS
            if (dogsToAllocate > 0)
            {
                if (animalsHolders.Count(x => x.TileType == TileTypes.Clearing) > 0)
                {
                    List<AnimalHolder> clearings = animalsHolders.Where(x => x.TileType == TileTypes.Clearing).OrderBy(x => x.genericCapacity).ToList();
                    while (dogsToAllocate > 0)
                    {
                        foreach (AnimalHolder clearing in clearings)
                        {
                            clearing.AddDog(ref dogsToAllocate);
                        } 
                    }
                }
                else if (animalsHolders.Count(x => x.TileType == TileTypes.SmallFence || x.TileType == TileTypes.BigFence1) > 0)
                {
                    AnimalHolder smallestCapacity = animalsHolders.Where(x => x.TileType == TileTypes.SmallFence || x.TileType == TileTypes.BigFence1).OrderBy(x => x.genericCapacity).First();
                    smallestCapacity.FillAnimals(ResourceTypes.Dogs, ref dogsToAllocate);
                }
                else
                {
                    //put them in the house
                    AnimalHolder tile = animalsHolders.First(x => x.TileType == TileTypes.StartingTile);
                    tile.FillAnimals(ResourceTypes.Dogs, ref dogsToAllocate);
                }
            }

            //FIRST PASS - areas only for specific animals

            while (pigsToAllocate > 0)
            {
                //first find spaces that only hold pigs
                List<AnimalHolder> pigHolders = animalsHolders.FindAll(x => x.IsUnfilledPigHolder);
                if (pigHolders.Count > 0)
                {
                    pigHolders[0].FillAnimals(ResourceTypes.Pigs, ref pigsToAllocate);
                    continue;
                }

                //nowhere to put them!
                break;
            }

            while (donkeysToAllocate > 0)
            {
                //first find spaces that only hold donkeys
                List<AnimalHolder> donkeyHolders = animalsHolders.FindAll(x => x.IsUnfilledDonkeyHolder);
                if (donkeyHolders.Count > 0)
                {
                    donkeyHolders[0].FillAnimals(ResourceTypes.Donkeys, ref donkeysToAllocate);
                    continue;
                }

                //nowhere to put them!
                break;
            }

            //When filling areas, do not assign sheep & generic areas until after all sheep only areas.
            //Then assign sheep only if there are MORE sheep than generic capacity (ie, extra space will get used)
            while (sheepToAllocate > 0)
            {
                //first find spaces that ONLY hold sheep
                List<AnimalHolder> sheepHolders = animalsHolders.FindAll(x => x.IsUnfilledSheepHolder && x.genericCapacity ==0);
                if (sheepHolders.Count > 0)
                {
                    sheepHolders[0].FillAnimals(ResourceTypes.Sheep, ref sheepToAllocate);
                    continue;
                }

                //then places that will get more sheep assigned than generic capacity
                sheepHolders = animalsHolders.FindAll(x => x.IsUnfilledSheepHolder && sheepToAllocate > x.genericCapacity);
                if (sheepHolders.Count > 0)
                {
                    sheepHolders[0].FillAnimals(ResourceTypes.Sheep, ref sheepToAllocate);
                    continue;
                }

                //other places get assigned as normal
                break;
            }


            //SECOND PASS - take group with most animals, put in biggest pen. Repeat.
            bool fillMade = true;
            while (fillMade)
            {
                int maxAnimals = new[] {cowsToAllocate, pigsToAllocate, donkeysToAllocate, sheepToAllocate}.Max();
                fillMade = false;

                if (cowsToAllocate > 0 && cowsToAllocate == maxAnimals)
                {
                    //find the space that holds most cows, and put them there
                    List<AnimalHolder> primaryHolders =
                        animalsHolders.FindAll(x => x.RemainingCowCapacity > 0).OrderByDescending(x => x.RemainingCowCapacity)
                        .ToList();
                    if (primaryHolders.Count > 0)
                    {
                        primaryHolders[0].FillAnimals(ResourceTypes.Cows, ref cowsToAllocate);
                        fillMade = true;
                        continue;
                    }
                }
                else if (pigsToAllocate > 0 && pigsToAllocate == maxAnimals)
                {
                    //find the space that holds most cows, and put them there
                    List<AnimalHolder> primaryHolders =
                        animalsHolders.FindAll(x => x.RemainingPigCapacity > 0).OrderByDescending(x => x.RemainingPigCapacity)
                        .ToList();
                    if (primaryHolders.Count > 0)
                    {
                        primaryHolders[0].FillAnimals(ResourceTypes.Pigs, ref pigsToAllocate);
                        fillMade = true;
                        continue;
                    }
                }
                else if (donkeysToAllocate > 0 && donkeysToAllocate == maxAnimals)
                {
                    //find the space that holds most cows, and put them there
                    List<AnimalHolder> primaryHolders =
                        animalsHolders.FindAll(x => x.RemainingDonkeyCapacity > 0).OrderByDescending(x => x.RemainingDonkeyCapacity)
                        .ToList();
                    if (primaryHolders.Count > 0)
                    {
                        primaryHolders[0].FillAnimals(ResourceTypes.Donkeys, ref donkeysToAllocate);
                        fillMade = true;
                        continue;
                    }
                }
                else if (sheepToAllocate > 0 && sheepToAllocate == maxAnimals)
                {
                    //find the space that holds most cows, and put them there
                    List<AnimalHolder> primaryHolders =
                        animalsHolders.FindAll(x => x.RemainingSheepCapacity > 0).OrderByDescending(x => x.RemainingSheepCapacity)
                        .ToList();
                    if (primaryHolders.Count > 0)
                    {
                        primaryHolders[0].FillAnimals(ResourceTypes.Sheep, ref sheepToAllocate);
                        fillMade = true;
                        continue;
                    }
                }
            }

       
            //now draw it to the screen
            foreach (AnimalHolder holder in animalsHolders)
            {
                _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Dogs, 0);
                _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Sheep, 0);
                _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Donkeys, 0);
                _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Pigs, 0);
                _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Cows, 0);

                if (holder.GetDogs() > 0)
                    _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Dogs, holder.GetDogs());
                if (holder.GetSheep() > 0)
                    _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Sheep, holder.GetSheep());
                if (holder.GetDonkeys() > 0)
                    _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Donkeys, holder.GetDonkeys());
                if (holder.GetPigs() > 0)
                    _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Pigs, holder.GetPigs());
                if (holder.GetCows() > 0)
                    _serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Cows, holder.GetCows());
            }

            //did we have leftovers? if so, need to warn the player
            _arrangeAnimalsTriggered = false;
            if (cowsToAllocate + pigsToAllocate + donkeysToAllocate + sheepToAllocate == 0)
                return;

            _arrangeAnimalsTriggered = true;

            if (discardExcess)
            {
                //do not trigger the normal checks with this, use aninal manager to avoid
                animalManager.Cows -= cowsToAllocate;
                animalManager.Pigs -= pigsToAllocate;
                animalManager.Donkeys -= donkeysToAllocate;
                animalManager.Sheep -= sheepToAllocate;
            }

            if (!triggerChoice)
                return;

            string result = "Unassigned animals: ";
            if (cowsToAllocate > 0)
                result += cowsToAllocate + " cows, ";
            if (pigsToAllocate > 0)
                result += pigsToAllocate + " pigs, ";
            if (donkeysToAllocate > 0)
                result += donkeysToAllocate + " donkeys, ";
            if (sheepToAllocate > 0)
                result += sheepToAllocate + " sheep, ";

            List<string> options = new List<string>();
            options.Add(DiscardActions.DiscardAllUnassignedAnimals);

            //are we in the breeding phase?
            if (IsBreeding)
            {
                if (Cows > 0)
                    options.Add(DiscardActions.DiscardCow);
                if (Pigs > 0)
                    options.Add(DiscardActions.DiscardPig);
                if (Donkeys > 0)
                    options.Add(DiscardActions.DiscardDonkey);
                if (Sheep > 0)
                    options.Add(DiscardActions.DiscardSheep);
            }
            else
            {
                if (Cows > 0)
                    options.Add(FoodActions.ConvertCow);
                if (Pigs > 0)
                    options.Add(FoodActions.ConvertPig);
                if (Donkeys > 1)
                    options.Add(FoodActions.ConvertDonkeyPair);
                if (Donkeys > 0)
                    options.Add(FoodActions.ConvertDonkey);
                if (Sheep > 0)
                    options.Add(FoodActions.ConvertSheep);
            }

            _serverSocket.GetPlayerChoice("playerID", "Allocate Animals", result, options);
        }

        public bool HasCavern()
        {
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (CaveSpaces[x, y] == TileTypes.Cavern)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanBuildTile(BuildingTile buildingTile)
        {
            return (Wood >= buildingTile.WoodCost && Stone >= buildingTile.StoneCost &&
                    GetTileCount(TileTypes.Cavern) > 0);
        }

        public List<Vector2> GetValidBuildingSpots()
        {
            List<Vector2> validSpots = new List<Vector2>();
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (CaveSpaces[x, y] == TileTypes.Cavern)
                    {
                        validSpots.Add(new Vector2(x, y));
                    }
                }
            }
            return validSpots;
        }

        public void SetBuildingTileAt(Vector2 position, string buildingType)
        {
            Wood -= new BuildingTile(buildingType).WoodCost;
            Stone -= new BuildingTile(buildingType).StoneCost;

            CaveSpaces[(int) position.x, (int) position.y] = buildingType;
            _serverSocket.SetPlayerTileType(position, GetTileType(position, false), true);
            CalculatePlayerScore();
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
            for (int i = 0; i < _dwarves.Count; i++)
            {
                dwarfStatus.Add(_dwarves[i].WeaponLevel + "_" + _dwarves[i].IsUsed + "_" + _dwarves[i].IsChild);
            }
            return dwarfStatus;
        }

        public int GetTileCount(string tileType)
        {
            int tileCount = 0;
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (CaveSpaces[x, y] == tileType)
                        tileCount++;
                    if (ForestSpaces[x, y] == tileType)
                        tileCount++;
                    if (StableSpaces[x, y] == tileType)
                        tileCount++;
                }
            }
            return tileCount;
        }

        public void FamilyGrowth()
        {
            Dwarf dwarf = new Dwarf(true, _dwarves.Count + 1);
            _dwarves.Add(dwarf);
            CalculatePlayerScore();
        }

        public void FieldPhase()
        {
            List<Vector2> updatedFields = new List<Vector2>();
            //replace each 1 corn or 1 veg with zero, 2 with 1, 3 with 2, etc
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field1Grain)
                    {
                        ForestSpaces[x, y] = TileTypes.Field;
                        Grain++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field2Grain)
                    {
                        ForestSpaces[x, y] = TileTypes.Field1Grain;
                        Grain++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field3Grain)
                    {
                        ForestSpaces[x, y] = TileTypes.Field2Grain;
                        Grain++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field1Veg)
                    {
                        ForestSpaces[x, y] = TileTypes.Field;
                        Veg++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field2Veg)
                    {
                        ForestSpaces[x, y] = TileTypes.Field1Veg;
                        Veg++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                }
            }

            HarvestFieldsComplete = true;

            foreach (Vector2 position in updatedFields)
            {
                _serverSocket.SetPlayerTileType(position, GetTileType(position, true), false);
            }
            CalculatePlayerScore();
        }

        public bool HarvestFieldsComplete { get; set; }
        public bool HarvestComplete { get; set; }
        public bool HarvestFeedingComplete { get; set; }

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

        public bool HarvestAnimalsComplete { get; set; }

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

        public bool IsOneFoodPerDwarf { get; set; }

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

        public bool CanBuildDoubleFence()
        {
            //return true if any of the clearings is next to any of the others.
            List<Vector2> clearings = GetMatchingForestTileSpaces(TileTypes.Clearing);
            return clearings.Exists(x => clearings.Exists(y => IsAdjacent(x, y)));
        }

        public List<Vector2> GetDoubleFencedPastures()
        {
            return _doubleFencedPastures;
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
            TilesToPlace = new List<string> {TileTypes.Field};

            if (GetValidTileSpots().Count > 0)
                actions.Add(RubyTrades.Field);

            TilesToPlace = new List<string>();

            //clearing
            TilesToPlace = new List<string> {TileTypes.Clearing};

            if (GetValidTileSpots().Count > 0)
                actions.Add(RubyTrades.Clearing);

            TilesToPlace = new List<string>();

            //tunnel
            TilesToPlace = new List<string> {TileTypes.Tunnel};

            if (GetValidTileSpots().Count > 0)
                actions.Add(RubyTrades.Tunnel);

            TilesToPlace = new List<string>();

            //cavern
            if (Rubies > 1)
            {
                TilesToPlace = new List<string> {TileTypes.Cavern};

                if (GetValidTileSpots().Count > 0)
                    actions.Add(RubyTrades.Cavern);

                TilesToPlace = new List<string>();
            }

            if (_dwarves.Count(x => !x.IsChild && !x.IsUsed) > 1)
                actions.Add(RubyTrades.ReorderDwarf);

            return actions;
        }

        public List<string> GetReorderDwarfs()
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

            Dwarf dwarfToMove = _dwarves.Find(x => x.ID.ToString() == dwarfID);

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
                    (CavernaManager.Instance.BuildingTiles.Exists(CanBuildTile)))
                    actions.Add(Expeditions.FurnishCavern);
            }

            if (weaponLevel >= 8)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Stable) &&
                    CanBuildTiles(TileTypes.Stable))
                    actions.Add(Expeditions.Stable);                
            }

            if (weaponLevel >= 9)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Stable) &&
                    CanBuildTiles(TileTypes.Tunnel))
                    actions.Add(Expeditions.Tunnel);

                if (!_usedExpeditionActions.Contains(Expeditions.SmallFence) &&
                    CanBuildTiles(TileTypes.SmallFence) &&
                    Wood >=1)
                    actions.Add(Expeditions.SmallFence);
            }

            if (weaponLevel >= 10)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Cow))
                    actions.Add(Expeditions.Cow);

                if (!_usedExpeditionActions.Contains(Expeditions.BigFence) &&
                    CanBuildTiles(TileLists.BigFence) &&
                    Wood >=2)
                    actions.Add(Expeditions.BigFence);
            }

            if (weaponLevel >= 11)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Clearing) &&
                    CanBuildTiles(TileTypes.Clearing))
                    actions.Add(Expeditions.Clearing);

                if (!_usedExpeditionActions.Contains(Expeditions.Dwelling) &&
                    GetTileCount(TileTypes.Cavern) > 0 &&
                    Wood >= 2 &&
                    Stone >=2)
                    actions.Add(Expeditions.Dwelling);
            }

            if (weaponLevel >= 12)
            {
                if (!_usedExpeditionActions.Contains(Expeditions.Field) &&
                    CanBuildTiles(TileTypes.Field))
                    actions.Add(Expeditions.Field);

                if (!_usedExpeditionActions.Contains(Expeditions.Sow) &&
                    GetTileCount(TileTypes.Field) > 0 &&
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
            int score = 0;
            
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
                score -=2;
            if (Donkeys == 0)
                score -=2;
            if (Pigs == 0)
                score -=2;
            if (Cows == 0)
                score -=2;

            //½ Gold per Grain (rounded up): Count all of your Grain tokens – both those in your
            //supply and those still left on Fields. Divide this number by 2 and round it up. This is the
            //number of Gold points you get for Grain.

            int grainCount = Grain;
            grainCount += GetTileCount(TileTypes.Field1Grain);
            grainCount += 2*GetTileCount(TileTypes.Field2Grain);
            grainCount += 3*GetTileCount(TileTypes.Field3Grain);

            score += (int)Math.Ceiling(grainCount/2f);

            //1 Gold per Vegetable: Count all of your Vegetable tokens – both those in your supply and those still left on Fields.
            //You get this number of Gold points for Vegetables.
            score += Veg;
            score += GetTileCount(TileTypes.Field1Veg);
            score += 2*GetTileCount(TileTypes.Field2Veg);

            //1 Gold per Ruby: At the end of the game, each Ruby is worth 1 Gold point.
            score += Rubies;

            //1 Gold per Dwarf: At the end of the game, each of your Dwarfs is worth 1 Gold point.
            score += _dwarves.Count;

            //-1 Gold per unused space: Count the number of spaces on your Home board that have no tile or Stable on them.
            //You lose 1 Gold point for each such space. The two pre-printed Caverns of your cave system are considered used.
            //(Unfurnished Caverns are also considered used.)

            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == null && StableSpaces[x, y] == null)
                        score--;
                    if (CaveSpaces[x, y] == null)
                        score--;
                }
            }
            
            //Gold for Furnishing tiles, Pastures and Mines: Add up the Gold point values on all of your tiles. Small pastures
            //are worth 2 Gold points, Large pastures are worth 4 Gold points (regardless of the type and number of animals on
            //those tiles). Ore mines are worth 3 Gold points, Ruby mines are worth 4 Gold points (regardless of whether they hold
            //a Donkey or not). The value of a Furnishing tile is printed on the right of the tile, right under the name of the tile.

            score += 2*GetTileCount(TileTypes.SmallFence);
            score += 4*GetTileCount(TileTypes.BigFence1);
            score += 3*GetTileCount(TileTypes.OreMine);
            score += 4*GetTileCount(TileTypes.RubyMine);

            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    //normal building points
                    if (Array.Find(typeof (BuildingTypes).GetFields(), f => f.GetValue(null).ToString() == CaveSpaces[x,y]) != null)
                        score += new BuildingTile(CaveSpaces[x,y]).VP;
                }
            }

            //Bonus points for Parlors, Storages and Chambers: Most of the Furnishing tiles called Parlors, Storages and
            //Chambers (indicated by the yellow name tag) may be worth Bonus points depending on the condition they impose.
            //The scoring pad has multiple lines for Bonus points. You can use one line per Furnishing tile that you get Bonus
            //points for. (Details on the Furnishing tiles can be found on page A3 of the appendix.)
            //TODO

            //Gold coins and Begging markers: Add up the values on your Gold coins and subtract 3 Gold points from that for
            //each Begging marker you have.
            score += Gold;
            score -= 3*BeggingCards;

            PlayerScore = score;
            _serverSocket.SetPlayerResources(ID, ResourceTypes.ScoreMarker, score);
        }

        public int PlayerScore { get; private set; }

        public List<Vector2> SetTilesToPlace(List<string> inputTiles)
        {
            TilesToPlace = new List<string>(inputTiles);
            return GetValidTileSpots();
        }

        public List<Vector2> SetTilesToPlace(string inputTile)
        {
            return SetTilesToPlace(new List<string> {inputTile});
        }

        public bool CanBuildTiles(List<string> tiles)
        {
            List<string> temp = new List<string>(TilesToPlace);
            TilesToPlace = new List<string>(tiles);

            bool canPlace = GetValidTileSpots().Count > 0;

            TilesToPlace = new List<string>(temp);

            return canPlace;
        }

        public bool CanBuildTiles(string tile)
        {
            return CanBuildTiles(new List<string> {tile});
        }

        public bool HasTilesToPlace()
        {
            return TilesToPlace.Count > 0;
        }

        public string GetNextTile()
        {
            return TilesToPlace[0];
        }

        public List<Vector2> SetBuildingToPlace(string buildingType)
        {
            return GetValidBuildingSpots();
        }

        public bool RearrangeAnimalsCheck()
        {
            if (_arrangeAnimalsTriggered)
                AutoArrangeAnimals(true, false);
            return _arrangeAnimalsTriggered;
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
    }
}