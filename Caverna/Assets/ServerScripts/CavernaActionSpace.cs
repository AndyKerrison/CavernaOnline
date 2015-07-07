using System.Collections.Generic;

namespace Assets.ServerScripts
{
    public class ActionGroup
    {
        private bool _isUsed;

        public enum ActionGroupTypes
        {
            Exclusive,
            Optional,
            OrderedOptional
        }

        public ActionGroup(string actionName, bool isSingleAction, int order): this(actionName, isSingleAction)
        {
            Order = order;
        }

        public ActionGroup(string actionName, bool isSingleAction)
        {
            ActionName = actionName;
            IsSingleAction = isSingleAction;
            Actions = new List<ActionGroup>();
        }

        public string ActionName { get; set; }
        public bool IsSingleAction { get; set; }
        public List<ActionGroup> Actions;
        public ActionGroupTypes Type { get; set; }

        public bool IsUsed
        {
            get
            {
                if (IsSingleAction)
                    return _isUsed;
                
                bool isUsed = false;

                foreach (ActionGroup actionGroup in Actions)
                {
                    if (actionGroup.IsUsed)
                        isUsed = true;
                }
                return isUsed;
            }
            set
            {
                if (IsSingleAction)
                    _isUsed = value;
                else
                {
                    foreach (ActionGroup actionGroup in Actions)
                    {
                        actionGroup.IsUsed = value;
                    }
                }
            }
        }

        public int Order { get; set; }

        public void SetUsed(string actionName)
        {
            if (actionName == ActionName)
                IsUsed = true;
            else
            {
                foreach (ActionGroup actionGroup in Actions)
                {
                    //only flag the first matching action of this name.
                    if (actionGroup.IsSingleAction && !actionGroup.IsUsed)
                    {
                        actionGroup.SetUsed(actionName);
                        if (actionGroup.IsUsed)
                            return;                       
                    }
                    else
                    {
                        actionGroup.SetUsed(actionName);
                    }

                }
            }
        }
    }

    public class CavernaActionSpace
    {

        public bool IsUsed
        {
            get { return _actionGroup.IsUsed || Type == ActionSpaceTypes.SkipRound; }
        }

        public int ID { get; set; }
        public string Type { get; set; }

        private int _woodInitial;
        private int _woodAccumulating;
        private int _woodCurrent;
        private int _woodStatic;

        private int _stoneInitial;
        private int _stoneAccumulating;
        private int _stoneCurrent;
        private int _stoneStatic;

        private int _oreInitial;
        private int _oreAccumulating;
        private int _oreCurrent;
        private int _oreStatic;
        private int _oreMineBonus;

        private int _foodInitial;
        private int _foodAccumulating;
        private int _foodCurrent;
        private int _foodStatic;

        private int _rubyInitial;
        private int _rubyAccumulating;
        private int _rubyCurrent;
        private int _rubyMineBonusThreshold;

        private int _sheepInitial;
        private int _sheepAccumulating;
        private int _sheepCurrent;

        private int _donkeysInitial;
        private int _donkeysAccumulating;
        private int _donkeysCurrent;

        private int _goldStatic;
        private int _grainStatic;
        private int _dogsStatic;

        private ActionGroup _actionGroup;

        public CavernaActionSpace(int id, string type)
        {
            ID = id;
            Type = type;
            SetType();
        }

        private void SetType()
        {
            switch (Type)
            {
                case "driftMining":
                {
                    _actionGroup = new ActionGroup("test1", false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.AddTunnelCaveDualTile, true));

                    _stoneInitial = 1;
                    _stoneAccumulating = 1;
                    break;
                }
                case "excavation":
                {
                    ActionGroup subGroup = new ActionGroup("test", false);
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.AddCaveCaveDualTile, true));
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.AddTunnelCaveDualTile, true));
                    subGroup.Type = ActionGroup.ActionGroupTypes.Exclusive;

                    _actionGroup = new ActionGroup("test2", false);
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true));
                    _actionGroup.Actions.Add(subGroup);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;

                    _stoneInitial = 1;
                    _stoneAccumulating = 1;
                    break;
                }
                case "startingPlayer":
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);
                    _oreStatic = 2;
                    _foodInitial = 1;
                    _foodAccumulating = 1;
                    break;
                }
                case "logging":
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true, 1));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Level1Expedition, true, 2));
                    _woodInitial = 3;
                    _woodAccumulating = 1;
                    break;
                }
                case "supplies":
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);
                    _woodStatic = 1;
                    _stoneStatic = 1;
                    _oreStatic = 1;
                    _foodStatic = 1;
                    _goldStatic = 2;
                    break;
                }
                case "oreMining":
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);
                    _oreMineBonus = 2;
                    _oreInitial = 2;
                    _oreAccumulating = 1;
                    break;
                }
                case "woodGathering":
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);
                    _woodInitial = 1;
                    _woodAccumulating = 1;
                    break;
                }
                case "clearing":
                {
                    _actionGroup = new ActionGroup("test1", false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.AddFieldClearingTile, true));
                    
                    _woodInitial = 1;
                    _woodAccumulating = 1;
                    break;
                }
                case "sustenance":
                {
                    _actionGroup = new ActionGroup("test1", false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.AddFieldClearingTile, true));
                    
                    _grainStatic = 1;
                    _foodInitial = 1;
                    _foodAccumulating = 1;
                    break;
                }
                case "rubyMining":
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);
                    _rubyMineBonusThreshold = 1;
                    _rubyAccumulating = 1;
                    _rubyInitial = 1;
                    break;
                }
                case ActionSpaceTypes.SlashAndBurn:
                {
                    _actionGroup = new ActionGroup("test1", false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.AddFieldClearingTile, true, 1));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.SowBake, true, 2));
                    break;
                }
                case ActionSpaceTypes.Housework:
                {
                    _actionGroup = new ActionGroup("test1", false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.FurnishCavern, true));
                    _dogsStatic = 1;
                    break;
                }
                case ActionSpaceTypes.Blacksmithing:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Blacksmithing, true, 1));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Level3Expedition, true, 2));
                    break;
                }
                case ActionSpaceTypes.OreMineConstruction:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.AddOreMineDeepTunnelDualTile, true, 1));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Level2Expedition, true, 2));
                    break;
                }
                case ActionSpaceTypes.SheepFarming:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;

                    ActionGroup subGroup = new ActionGroup(string.Empty, false, 1);
                    subGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.SmallFence, true));
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.BigFence, true));
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.BuyStable, true));
                    _actionGroup.Actions.Add(subGroup);

                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true, 2));
                    _sheepAccumulating = 1;
                    _sheepInitial = 1;
                    break;
                }
                case ActionSpaceTypes.DonkeyFarming:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;

                    ActionGroup subGroup = new ActionGroup(string.Empty, false, 1);
                    subGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.SmallFence, true));
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.BigFence, true));
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.BuyStable, true));
                    _actionGroup.Actions.Add(subGroup);

                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true, 2));
                    _donkeysAccumulating = 1;
                    _donkeysInitial = 1;
                    break;
                }
                case ActionSpaceTypes.RubyMine:
                {
                    _actionGroup = new ActionGroup(CavernaActions.AddRubyMineTile, true);
                    break;
                }
                case ActionSpaceTypes.OreDelivery:
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);

                    _oreMineBonus = 2;
                    _stoneInitial = 1;
                    _stoneAccumulating = 1;
                    _oreInitial = 1;
                    _oreAccumulating = 1;
                    break;
                }
                case ActionSpaceTypes.RubyDelivery:
                {
                    _actionGroup = new ActionGroup(CavernaActions.CollectResources, true);

                    _rubyMineBonusThreshold = 2;
                    _rubyInitial = 2;
                    _rubyAccumulating = 1;
                    break;
                }
                case ActionSpaceTypes.Adventure:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.OrderedOptional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Blacksmithing, true, 1));

                    ActionGroup subGroup = new ActionGroup(string.Empty, false, 2);
                    subGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.Level1Expedition, true));
                    subGroup.Actions.Add(new ActionGroup(CavernaActions.Level1Expedition, true));

                    _actionGroup.Actions.Add(subGroup);
                    break;
                }
                case ActionSpaceTypes.OreTrading:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Trade2Ore, true, 1));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Trade2Ore, true, 1));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.Trade2Ore, true, 1));
                    break;
                }
                case ActionSpaceTypes.FamilyLife:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Optional;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.FamilyGrowth, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.SowBake, true));
                    break;
                }
                case ActionSpaceTypes.WishForChildren:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Exclusive;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.FamilyGrowth, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.FurnishCavern, true));
                    break;
                }
                case ActionSpaceTypes.UrgentWish:
                {
                    _actionGroup = new ActionGroup(string.Empty, false);
                    _actionGroup.Type = ActionGroup.ActionGroupTypes.Exclusive;
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.FurnishDwellingThenGrow, true));
                    _actionGroup.Actions.Add(new ActionGroup(CavernaActions.CollectResources, true));
                    _goldStatic = 3;
                    break;
                }
                default:
                {
                    _actionGroup = new ActionGroup(string.Empty, true);
                    break;
                }
            }
        }

        public void CleanupAndAddResources(IServerSocket serverSocket, bool isSoloGame)
        {
            if (Type == ActionSpaceTypes.SkipRound)
                return;

            _actionGroup.IsUsed = false;
            
            //accumulating
            if (AccumulatingResourcesTotal == 0 || (AccumulatingResourcesTotal >=6 && isSoloGame))
            {
                _woodCurrent = _woodInitial;
                _stoneCurrent = _stoneInitial;
                _oreCurrent = _oreInitial;
                _foodCurrent = _foodInitial;
                _rubyCurrent = _rubyInitial;
                _sheepCurrent = _sheepInitial;
                _donkeysCurrent = _donkeysInitial;
            }
            else
            {
                _woodCurrent += _woodAccumulating;
                _stoneCurrent += _stoneAccumulating;
                _oreCurrent += _oreAccumulating;
                _foodCurrent += _foodAccumulating;
                _rubyCurrent += _rubyAccumulating;
                _sheepCurrent += _sheepAccumulating;
                _donkeysCurrent += _donkeysAccumulating;
            }

            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Wood, _woodCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Stone, _stoneCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Ore, _oreCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Food, _foodCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Ruby, _rubyCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Sheep, _sheepCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Donkeys, _donkeysCurrent);
        }

        public void CollectResources(CavernaPlayer player, IServerSocket serverSocket)
        {
            _actionGroup.SetUsed(CavernaActions.CollectResources);
            
            if (_woodCurrent > 0)
            {
                player.Wood += _woodCurrent;
                _woodCurrent = 0;
            }
            if (_stoneCurrent > 0)
            {
                player.Stone += _stoneCurrent;
                _stoneCurrent = 0;
            }
            if (_oreCurrent > 0)
            {
                player.Ore += _oreCurrent;
                _oreCurrent = 0;
            }
            if (_foodCurrent > 0)
            {
                player.Food += _foodCurrent;
                _foodCurrent = 0;
            }
            if (_rubyCurrent > 0)
            {
                player.Rubies += _rubyCurrent;
                _rubyCurrent = 0;
            }
            if (_sheepCurrent > 0)
            {
                player.Sheep +=_sheepCurrent;
                _sheepCurrent = 0;
            }
            if (_donkeysCurrent > 0)
            {
                player.Donkeys += _donkeysCurrent;
                _donkeysCurrent = 0;
            }

            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Wood, _woodCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Stone, _stoneCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Ore, _oreCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Food, _foodCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Ruby, _rubyCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Sheep, _sheepCurrent);
            serverSocket.SetActionSpaceResources(ID, ResourceTypes.Donkeys, _donkeysCurrent);

            player.Wood += _woodStatic;
            player.Stone += _stoneStatic;
            player.Ore += _oreStatic;
            player.Food += _foodStatic;
            player.Gold += _goldStatic;
            player.Grain+= _grainStatic;
            player.Dogs += _dogsStatic;

            player.Ore += _oreMineBonus*player.GetTileCount(TileTypes.OreMine);
            if (_rubyMineBonusThreshold > 0 && player.GetTileCount(TileTypes.RubyMine) >= _rubyMineBonusThreshold)
                player.Rubies += 1;
        }

        public void SetFinished()
        {
            _actionGroup.IsUsed = true;
        }

        public List<string> GetActionGroupActions(ActionGroup group, CavernaPlayer player)
        {
            List<string> actions = new List<string>();
            if (group.IsSingleAction)
            {
                if (group.IsUsed)
                    return actions;

                //also need to check if you can actually perform this action (e.g have grain to sow, space to build, etc)
                if (group.ActionName == CavernaActions.AddTunnelCaveDualTile) //cave cave & cave/tunnel are either/or
                {
                    if (!player.CanBuildTiles(TileLists.TunnelCaveDualTile))
                        return actions;
                }
                if (group.ActionName == CavernaActions.AddCaveCaveDualTile)
                {
                    if (!player.CanBuildTiles(TileLists.CaveCaveDualTile))
                        return actions;
                }
                if (group.ActionName == CavernaActions.AddFieldClearingTile)
                {
                    if (!player.CanBuildTiles(TileLists.FieldClearingDualTile))
                        return actions;
                }
                if (group.ActionName == CavernaActions.AddOreMineDeepTunnelDualTile)
                {
                    if (!player.CanBuildTiles(TileLists.OreMineDeepTunnelDualTile))
                        return actions;
                }
                if (group.ActionName == CavernaActions.AddRubyMineTile)
                {
                    if (!player.CanBuildTiles(TileTypes.RubyMine))
                        return actions;
                }
                if (group.ActionName == CavernaActions.BuyStable)
                {
                    if (player.Stone == 0)
                        return actions;
                    if (!player.CanBuildTiles(TileTypes.Stable))
                        return actions;
                }

                if (group.ActionName == CavernaActions.BigFence)
                {
                    if (player.Wood < 4 || !player.CanBuildDoubleFence())
                        return actions;
                }

                if (group.ActionName == CavernaActions.SmallFence)
                {
                    if (player.Wood < 2 || player.GetTileCount(TileTypes.Clearing) < 1)
                        return actions;
                }

                if (group.ActionName == CavernaActions.SowBake)
                {
                    if (player.GetFieldCount() == 0 || (player.Grain + player.Veg) == 0)
                        return actions;
                }
                if (group.ActionName == CavernaActions.Trade2Ore)
                {
                    if (player.Ore < 2)
                        return actions;
                }
                if (group.ActionName == CavernaActions.FamilyGrowth)
                {
                    if (player.GetDwarfCapacity() <= player.GetDwarfStatus().Count)
                        return actions;
                }
                if (group.ActionName == CavernaActions.FurnishCavern)
                {
                    //are there any tiles this player can build?
                    if (!CavernaManager.Instance.BuildingTiles.Exists(player.CanBuildTile))
                        return actions;
                }
                if (group.ActionName == CavernaActions.FurnishDwellingThenGrow)
                {
                    //return false IFieldInfo we can't BuildingTypes a dwelling, 
                    //or if we can't grow afterwards
                    if (!CavernaManager.Instance.BuildingTiles.Exists(x => player.CanBuildTile(x) && x.IsDwelling))
                        return actions;
                    if (player.GetDwarfCapacity() > 4)
                        return actions;
                }
                if (group.ActionName == CavernaActions.Blacksmithing)
                {
                    if (player.Ore == 0 || player.GetActiveDwarfWeaponLevel() > 0)
                        return actions;
                }
                if (group.ActionName == CavernaActions.Level1Expedition ||
                    group.ActionName == CavernaActions.Level2Expedition ||
                    group.ActionName == CavernaActions.Level3Expedition)
                {
                    if (! (player.GetActiveDwarfWeaponLevel() > 0))
                        return actions;
                }

                actions.Add(group.ActionName);
            }
            else
            {
                //if this is an exclusive subgroup, one being used means all are used
                if (group.Type == ActionGroup.ActionGroupTypes.Exclusive && group.Actions.Exists(x => x.IsUsed))
                    return new List<string>();

                foreach (ActionGroup subGroup in group.Actions)
                {
                    //if this is an ordered subgroup, you can only add if no later one has been used
                    if (group.Type == ActionGroup.ActionGroupTypes.OrderedOptional && group.Actions.Exists(x => x.IsUsed && x.Order > subGroup.Order))
                    {
                        continue;
                    }

                    actions.AddRange(GetActionGroupActions(subGroup, player));
                }
            }
            return actions;
        }

        public List<string> GetActions(CavernaPlayer cavernaPlayer)
        {
            //iterate through the _actionGroup contents
            List<string> actions1 = GetActionGroupActions(_actionGroup, cavernaPlayer);
            if (_actionGroup.IsUsed)
                actions1.Add(CavernaActions.Finish);
            else
                actions1.Add(CavernaActions.Cancel);
            return actions1;

            //can this player use this space?
        }

        public List<string> GetSowActions(CavernaPlayer player)
        {
            List<string> actions = new List<string>();

            int fields = player.GetFieldCount();
            
            if (fields >=4 && player.Grain >= 2 && player.Veg >= 2)
            {
                actions.Add(CavernaActions.Sow2Grain2Veg);
            }
            if (fields >= 3 && player.Grain >= 1 && player.Veg >= 2)
            {
                actions.Add(CavernaActions.Sow1Grain2Veg);
            }
            if (fields >= 3 && player.Grain >= 2 && player.Veg >= 1)
            {
                actions.Add(CavernaActions.Sow2Grain1Veg);
            }
            if (fields >= 2 && player.Grain >= 1 && player.Veg >= 1)
            {
                actions.Add(CavernaActions.Sow1Grain1Veg);
            }
            if (fields >= 2 && player.Grain >= 2)
            {
                actions.Add(CavernaActions.Sow2Grain);
            }
            if (fields >= 2 && player.Veg >= 2)
            {
                actions.Add(CavernaActions.Sow2Veg);
            }
            if (fields >= 1 && player.Grain >= 1)
            {
                actions.Add(CavernaActions.Sow1Grain);
            }
            if (fields >= 1 && player.Veg >= 1)
            {
                actions.Add(CavernaActions.Sow1Veg);
            }
            return actions;
        }

        private int AccumulatingResourcesTotal
        {
            get
            {
                int totalResources = _woodCurrent;
                totalResources += _stoneCurrent;
                totalResources += _oreCurrent;
                totalResources += _foodCurrent;
                totalResources += _rubyCurrent;
                totalResources += _sheepCurrent;
                totalResources += _donkeysCurrent;

                return totalResources;
            }
        }

        public void SetActionUsed(CavernaPlayer player, string actionName)
        {
            _actionGroup.SetUsed(actionName);
        }
    }
}