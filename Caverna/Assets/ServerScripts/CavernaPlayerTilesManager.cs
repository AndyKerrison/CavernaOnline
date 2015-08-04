using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using UnityEngine;

namespace Assets.ServerScripts
{
    class CavernaPlayerTilesManager
    {
        private CavernaPlayer _player;

        private const int TileAreaWidth = 3;
        private const int TileAreaHeight = 4;

        private readonly string[,] CaveSpaces = new string[TileAreaWidth, TileAreaHeight];
        private readonly string[,] ForestSpaces = new string[TileAreaWidth, TileAreaHeight];
        private readonly string[,] StableSpaces = new string[TileAreaWidth, TileAreaHeight];

        public bool ArrangeAnimalsTriggered { get; private set; }
        private readonly List<Vector2> _doubleFencedPastures;
        private List<string> TilesToPlace { get; set; }
        private Vector2 _firstPartOfDouble = new Vector2(-1, -1);
        private bool _isPlacingSecondPartOfTile;

        public CavernaPlayerTilesManager(CavernaPlayer player)
        {
            _player = player;
            _doubleFencedPastures = new List<Vector2>();
            TilesToPlace = new List<string>();

            CaveSpaces[0, 0] = TileTypes.StartingTile;
            CaveSpaces[0, 1] = TileTypes.Cavern;
        }

        public List<Vector2> GetDoubleFencedPastures()
        {
            return _doubleFencedPastures;
        }

        public bool HasSpaceForTile(string tileType)
        {
            return HasSpaceForTiles(new List<string> { tileType});
        }

        public bool HasSpaceForTiles(List<string> tileTypes)
        {
            List<string> temp = TilesToPlace;

            TilesToPlace = new List<string>(tileTypes);

            bool canPlace = GetValidTileSpots().Count > 0;

            TilesToPlace = new List<string>(temp);

            return canPlace;
        }

        public List<Vector2> SetTilesToPlace(List<string> inputTiles)
        {
            TilesToPlace = new List<string>(inputTiles);
            return GetValidTileSpots();
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

        public bool CanBuildDoubleFence()
        {
            //return true if any of the clearings is next to any of the others.
            List<Vector2> clearings = GetMatchingForestTileSpaces(TileTypes.Clearing);
            return clearings.Exists(x => clearings.Exists(y => IsAdjacent(x, y)));
        }

        public void SetBuildingTileAt(IServerSocket serverSocket, Vector2 position, string buildingType)
        {
            CaveSpaces[(int)position.x, (int)position.y] = buildingType;
            serverSocket.SetPlayerTileType(position, GetTileType(position, false), true);
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

        private bool IsAdjacent(Vector2 pos1, Vector2 pos2)
        {
            if ((int)pos1.x == (int)pos2.x && Math.Abs((int)pos1.y - (int)pos2.y) == 1)
                return true;
            if ((int)pos1.y == (int)pos2.y && Math.Abs((int)pos1.x - (int)pos2.x) == 1)
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

            int x = (int)position.x;
            int y = (int)position.y;
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
                    if (tileArea[x, y] != null && ((int)_firstPartOfDouble.x != x || (int)_firstPartOfDouble.y != y))
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

            int x = (int)targetSpace.x;
            int y = (int)targetSpace.y;
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
                        stableVector => StableSpaces[(int)stableVector.x, (int)stableVector.y] != TileTypes.Stable);
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
                    resultSpaces.RemoveAll(space => StableSpaces[(int)space.x, (int)space.y] == TileTypes.Stable);
                }

                return resultSpaces.Distinct().ToList();
            }
            if (_isPlacingSecondPartOfTile)
            {
                //if a forest, and we chose the first start tile
                //any place next to the starting space is also valid
                if (isForest && (int)_firstPartOfDouble.x == 2 && (int)_firstPartOfDouble.y == 0)
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
                validSpaces.RemoveAll(space => StableSpaces[(int)space.x, (int)space.y] == TileTypes.Stable);
            }
            return validSpaces;
        }

        public int GetDwarfCapacity()
        {
            int capacity = 2;
            capacity += GetTileCount(BuildingTypes.Dwelling);
            capacity += GetTileCount(BuildingTypes.SimpleDwelling1);
            capacity += GetTileCount(BuildingTypes.SimpleDwelling2);
            capacity += 2*GetTileCount(BuildingTypes.CoupleDwelling);
            if (capacity > 5)
                capacity = 5;
            if (capacity == 5 && HasTile(BuildingTypes.AdditionalDwelling))
                capacity = 6;
            return capacity;
        }

        private List<string> GetTileType(Vector2 position, bool isForest)
        {
            List<string> tileType = new List<string>();

            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            tileType.Add(tileArea[(int)position.x, (int)position.y]);
            if (isForest && StableSpaces[(int)position.x, (int)position.y] == TileTypes.Stable)
            {
                tileType.Add(TileTypes.Stable);
            }

            return tileType;
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

        public void SetTileAt(IServerSocket serverSocket, Vector2 position, bool isForest, bool isExpedition, out int foodGain, out int pigsGain, out int rubyGain)
        {
            foodGain = 0;
            pigsGain = 0;
            rubyGain = 0;

            if (TilesToPlace.First() == TileTypes.SmallFence)
            {
                if (isExpedition)
                {
                    //already paid for
                }
                else
                {
                    int woodCost = 2;
                    if (HasTile(BuildingTypes.Carpenter))
                    {
                        woodCost --;
                    }
                    _player.Wood -= woodCost;
                }
            }
            if (TilesToPlace.First() == TileTypes.BigFence1)
            {
                if (isExpedition)
                {
                    //already paid for
                }
                else
                {
                    int woodCost = 4;
                    if (HasTile(BuildingTypes.Carpenter))
                    {
                        woodCost--;
                    }
                    _player.Wood -= woodCost;
                }
            }
            if (TilesToPlace.First() == TileTypes.Stable)
            {
                //pay for the stable
                if (!HasTile(BuildingTypes.StoneCarver) && !isExpedition)
                {
                    _player.Stone--;
                }

                int x = (int)position.x;
                int y = (int)position.y;
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
                serverSocket.SetPlayerTileType(position, GetTileType(position, isForest), !isForest);
                return;
            }
            string[,] tileArea;
            if (isForest)
                tileArea = ForestSpaces;
            else
                tileArea = CaveSpaces;

            string oldTile = tileArea[(int)position.x, (int)position.y];
            string newTile = TilesToPlace.First();

            tileArea[(int)position.x, (int)position.y] = newTile;
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
                if ((int)position.x == 1 && (int)position.y == 0)
                    foodGain++;
                if ((int)position.x == 0 && (int)position.y == 1)
                    pigsGain++;
                if ((int)position.x == 2 && (int)position.y == 3)
                    pigsGain++;
            }
            else
            {
                if (oldTile == TileTypes.DeepTunnel && newTile == TileTypes.RubyMine)
                    rubyGain++;
                if ((int)position.x == 1 && (int)position.y == 0)
                    foodGain++;
                if ((int)position.x == 2 && (int)position.y == 3)
                    foodGain += 2;
            }

            serverSocket.SetPlayerTileType(position, GetTileType(position, isForest), !isForest);
        }

        public void SowVeg(IServerSocket serverSocket)
        {
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field)
                    {
                        ForestSpaces[x, y] = TileTypes.Field2Veg;
                        serverSocket.SetPlayerTileType(new Vector2(x, y), GetTileType(new Vector2(x, y), true), false);
                        return;
                    }
                }
            }
        }

        public void SowGrain(IServerSocket serverSocket)
        {
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field)
                    {
                        ForestSpaces[x, y] = TileTypes.Field3Grain;
                        serverSocket.SetPlayerTileType(new Vector2(x, y), GetTileType(new Vector2(x, y), true), false);
                        return;
                    }
                }
            }
        }

        public void FieldPhase(IServerSocket _serverSocket, out int harvestedGrain, out int harvestedVeg)
        {
            harvestedGrain = 0;
            harvestedVeg = 0;
            List<Vector2> updatedFields = new List<Vector2>();

            //replace each 1 corn or 1 veg with zero, 2 with 1, 3 with 2, etc
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == TileTypes.Field1Grain)
                    {
                        ForestSpaces[x, y] = TileTypes.Field;
                        harvestedGrain++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field2Grain)
                    {
                        ForestSpaces[x, y] = TileTypes.Field1Grain;
                        harvestedGrain++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field3Grain)
                    {
                        ForestSpaces[x, y] = TileTypes.Field2Grain;
                        harvestedGrain++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field1Veg)
                    {
                        ForestSpaces[x, y] = TileTypes.Field;
                        harvestedVeg++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                    if (ForestSpaces[x, y] == TileTypes.Field2Veg)
                    {
                        ForestSpaces[x, y] = TileTypes.Field1Veg;
                        harvestedVeg++;
                        updatedFields.Add(new Vector2(x, y));
                    }
                }
            }

            foreach (Vector2 position in updatedFields)
            {
                _serverSocket.SetPlayerTileType(position, GetTileType(position, true), false);
            }
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

        public bool HasCavern()
        {
            return GetTileCount(TileTypes.Cavern) > 0;
        }

        public bool HasTile(string tileType)
        {
            return GetTileCount(tileType) > 0;
        }

        public int EmptySpaceCount()
        {
            int emptySpaces = 0;
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (ForestSpaces[x, y] == null && StableSpaces[x, y] == null)
                        emptySpaces++;
                    if (CaveSpaces[x, y] == null)
                        emptySpaces++;
                }
            }
            return emptySpaces;
        }

        public int GetBuildingPoints()
        {
            int buildingPoints = 0;
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    //normal building points
                    if (Array.Find(typeof(BuildingTypes).GetFields(), f => f.GetValue(null).ToString() == CaveSpaces[x, y]) != null)
                        buildingPoints += new BuildingTile(CaveSpaces[x, y]).VP;
                }
            }
            return buildingPoints;
        }

        public int GetScoringTileCount()
        {
            int scoringTiles = 0;
            for (int x = 0; x < TileAreaWidth; x++)
            {
                for (int y = 0; y < TileAreaHeight; y++)
                {
                    if (new BuildingTile(CaveSpaces[x, y]).BuildingGroup == BuildingTile.BuildingGroups.Scoring)
                        scoringTiles++;
                }
            }
            return scoringTiles;
        }

        public void AutoArrangeAnimals(IServerSocket serverSocket, AnimalManager animalManager, bool triggerChoice, bool discardExcess, bool isBreeding)
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

            //add special buildings
            List<Vector2> mixedDwellingPositions = GetMatchingCaveTileSpaces(BuildingTypes.MixedDwelling);
            foreach (Vector2 position in mixedDwellingPositions)
            {
                AnimalHolder holder = new AnimalHolder();
                holder.TileType = BuildingTypes.MixedDwelling;
                holder.isCave = true;
                holder.genericCapacity = 2;
                holder.position = position;
                animalsHolders.Add(holder);
            }
            List<Vector2> breakfastRoomPositions = GetMatchingCaveTileSpaces(BuildingTypes.BreakfastRoom);
            foreach (Vector2 position in breakfastRoomPositions)
            {
                AnimalHolder holder = new AnimalHolder();
                holder.TileType = BuildingTypes.BreakfastRoom;
                holder.isCave = true;
                holder.cowCapacity = 3;
                holder.position = position;
                animalsHolders.Add(holder);
            }
            List<Vector2> cuddleRoomPositions = GetMatchingCaveTileSpaces(BuildingTypes.CuddleRoom);
            foreach (Vector2 position in cuddleRoomPositions)
            {
                AnimalHolder holder = new AnimalHolder();
                holder.TileType = BuildingTypes.CuddleRoom;
                holder.isCave = true;
                holder.sheepCapacity = _player.GetDwarfStatus().Count;
                holder.position = position;
                animalsHolders.Add(holder);
            }

            //add whatever the dogs are doing

            int cowsToAllocate = animalManager.Cows;
            int pigsToAllocate = animalManager.Pigs;
            int donkeysToAllocate = animalManager.Donkeys;
            int sheepToAllocate = animalManager.Sheep;
            int dogsToAllocate = animalManager.Dogs;

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

            while (cowsToAllocate > 0)
            {
                //first find spaces that only hold cows
                List<AnimalHolder> cowHolders = animalsHolders.FindAll(x => x.IsUnfilledCowHolder);
                if (cowHolders.Count > 0)
                {
                    cowHolders[0].FillAnimals(ResourceTypes.Cows, ref cowsToAllocate);
                    continue;
                }

                //nowhere to put them!
                break;
            }

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
                    }
                }
            }

       
            //now draw it to the screen
            foreach (AnimalHolder holder in animalsHolders)
            {
                serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Dogs, 0);
                serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Sheep, 0);
                serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Donkeys, 0);
                serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Pigs, 0);
                serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Cows, 0);

                if (holder.GetDogs() > 0)
                    serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Dogs, holder.GetDogs());
                if (holder.GetSheep() > 0)
                    serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Sheep, holder.GetSheep());
                if (holder.GetDonkeys() > 0)
                    serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Donkeys, holder.GetDonkeys());
                if (holder.GetPigs() > 0)
                    serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Pigs, holder.GetPigs());
                if (holder.GetCows() > 0)
                    serverSocket.SetTileAnimals(holder.position, holder.isCave, ResourceTypes.Cows, holder.GetCows());
            }

            //did we have leftovers? if so, need to warn the player
            ArrangeAnimalsTriggered = false;
            if (cowsToAllocate + pigsToAllocate + donkeysToAllocate + sheepToAllocate == 0)
                return;

            ArrangeAnimalsTriggered = true;

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
            if (isBreeding)
            {
                if (animalManager.Cows > 0)
                    options.Add(DiscardActions.DiscardCow);
                if (animalManager.Pigs > 0)
                    options.Add(DiscardActions.DiscardPig);
                if (animalManager.Donkeys > 0)
                    options.Add(DiscardActions.DiscardDonkey);
                if (animalManager.Sheep > 0)
                    options.Add(DiscardActions.DiscardSheep);
            }
            else
            {
                if (HasTile(BuildingTypes.SlaughteringCave))
                {
                    if (animalManager.Cows > 0)
                        options.Add(FoodActions.SlaughteringCaveConvertCow);
                    if (animalManager.Pigs > 0)
                        options.Add(FoodActions.SlaughteringCaveConvertPig);
                    if (animalManager.Donkeys > 1)
                        options.Add(FoodActions.SlaughteringCaveConvertDonkeyPair);
                    if (animalManager.Donkeys > 0)
                        options.Add(FoodActions.SlaughteringCaveConvertDonkey);
                    if (animalManager.Sheep > 0)
                        options.Add(FoodActions.SlaughteringCaveConvertSheep);
                }
                else
                {
                    if (animalManager.Cows > 0)
                        options.Add(FoodActions.ConvertCow);
                    if (animalManager.Pigs > 0)
                        options.Add(FoodActions.ConvertPig);
                    if (animalManager.Donkeys > 1)
                        options.Add(FoodActions.ConvertDonkeyPair);
                    if (animalManager.Donkeys > 0)
                        options.Add(FoodActions.ConvertDonkey);
                    if (animalManager.Sheep > 0)
                        options.Add(FoodActions.ConvertSheep);
                }
            }

            serverSocket.GetPlayerChoice("playerID", "Allocate Animals", result, options);
        }

        public List<string> GetTileActions(Vector2 position, string tileType)
        {
            List<string> options = new List<string>();

            if (CaveSpaces[(int)position.x, (int)position.y] != tileType)
                return new List<string>() { BuildingTileActions.Invalid };

            if (tileType == BuildingTypes.BlacksmithingParlour && _player.Ore > 0 && _player.Rubies > 0)
                options.Add(BuildingTileActions.Trade1RubyAnd1OreFor2GoldAnd1Food);

            if (tileType == BuildingTypes.BeerParlour && _player.Grain >= 2)
            {
                options.Add(BuildingTileActions.Trade2GrainFor3Gold);
                options.Add(BuildingTileActions.Trade2GrainFor4Food);
            }

            if (tileType == BuildingTypes.HuntingParlour && _player.Pigs >= 2)
            {
                options.Add(BuildingTileActions.Trade2PigsFor2GoldAnd2Food);
            }

            if (tileType == BuildingTypes.SparePartStorage && _player.Wood >= 1 && _player.Stone >=1 && _player.Ore >=1)
            {
                options.Add(BuildingTileActions.Trade1Stone1Wood1OreFor2Gold);
            }

            if (tileType == BuildingTypes.CookingCave && _player.Grain >= 1 && _player.Veg >=1)
            {
                options.Add(BuildingTileActions.Trade1Veg1GrainFor5Food);
            }

            if (tileType == BuildingTypes.Trader && _player.Gold >= 2)
            {
                options.Add(BuildingTileActions.Trade2GoldFor1Wood1Stone1Ore);
            }

            options.Add(BuildingTileActions.Cancel);

            return options;
        }
    }
}
