using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingPanelInfo
{
    //coords count from top left
    public static string[,] BuildingPanelLocations = 
    {
        {BuildingTypes.Dwelling, BuildingTypes.MixedDwelling, BuildingTypes.Carpenter, BuildingTypes.Miner},
        {BuildingTypes.SimpleDwelling1, BuildingTypes.CoupleDwelling, BuildingTypes.StoneCarver, BuildingTypes.Builder},
        {BuildingTypes.SimpleDwelling2, BuildingTypes.AdditionalDwelling, BuildingTypes.Blacksmith, BuildingTypes.Trader},
        {BuildingTypes.CuddleRoom, BuildingTypes.WorkRoom, BuildingTypes.WoodSupplier, BuildingTypes.DogSchool},
        {BuildingTypes.BreakfastRoom, BuildingTypes.GuestRoom, BuildingTypes.StoneSupplier, BuildingTypes.Quarry},
        {BuildingTypes.StubbleRoom, BuildingTypes.OfficeRoom, BuildingTypes.RubySupplier, BuildingTypes.Seam},
        {BuildingTypes.SlaughteringCave, BuildingTypes.MiningCave, BuildingTypes.StoneStorage, BuildingTypes.MainStorage},
        {BuildingTypes.CookingCave, BuildingTypes.BreedingCave, BuildingTypes.OreStorage, BuildingTypes.WeaponStorage},
        {BuildingTypes.WorkingCave, BuildingTypes.PeacefulCave, BuildingTypes.SparePartStorage, BuildingTypes.SuppliesStorage},
        {BuildingTypes.WeavingParlour, BuildingTypes.HuntingParlour, BuildingTypes.BroomChamber, BuildingTypes.PrayerChamber},
        {BuildingTypes.MilkingParlour, BuildingTypes.BeerParlour, BuildingTypes.TreasureChamber, BuildingTypes.WritingChamber},
        {BuildingTypes.StateParlour, BuildingTypes.BlacksmithingParlour, BuildingTypes.FoodChamber, BuildingTypes.FodderChamber},
    };

    public static Vector2 GetBuildingTileLocation(string buildingType)
    {
        for (int x = 0; x <= BuildingPanelLocations.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= BuildingPanelLocations.GetUpperBound(1); y++)
            {
                if (BuildingPanelLocations[x, y] == buildingType)
                    return new Vector2(x, y);
            }
        }
        throw new Exception("Invalid building type : " + buildingType);
    }
}

public static class BreedActions
{
    public static string SheepOnly = "Sheep only";
    public static string DonkeysOnly = "Donkeys only";
    public static string PigsOnly = "Pigs only";
    public static string CowsOnly = "Cows only";
    public static string SheepAndDonkeys = "Sheep and Donnkeys";
    public static string SheepAndPigs = "Sheep and Pigs";
    public static string SheepAndCows = "Sheep and Cows";
    public static string DonkeysAndPigs = "Donkeys and Pigs";
    public static string DonkeysAndCows = "Donkeys and Cows";
    public static string PigsAndCows = "Pigs and Cows";
}
public static class Expeditions
{
    public const string AllWeaponsPlusOne = "All Weapons +1";
    public const string Wood = "1 Wood";
    public const string Dog = "1 Dog";
    public const string Grain= "1 Grain";
    public const string Sheep = "1 Sheep";
    public static string Nothing = "Take Nothing";
    public static string Stone = "1 Stone";
    public static string Donkey = "1 Donkey";
    public static string Veg = "1 Vegetable";
    public static string Ore = "2 Ore";
    public static string Pig = "1 Pig";
    public static string Gold = "2 Gold";
    public static string FurnishCavern = "Furnish 1 Cavern";
    public static string Stable = "1 Stable (free)";
    public static string Tunnel = "1 Tunnel";
    public static string SmallFence = "Fence a single pasture for 1 wood";
    public static string SmallFenceDiscount = "Fence a single pasture for 0 wood";
    public static string Cow = "1 Cow";
    public static string BigFence = "Fence a double pasture for 2 wood";
    public static string BigFenceDiscount = "Fence a double pasture for 1 wood";
    public static string Clearing = "1 Pasture";
    public static string Dwelling = "Build a Dwelling for 2 wood & 2 stone";
    public static string Field = "1 Field";
    public static string Sow = "Sow up to 2 grain and up to 2 veg";
    public static string BreedAnimals = "Breed up 2 types of farm animals";
}
public static class DwarfText
{
    public const string Unarmed = "Unarmed Dwarf";
    public const string WeaponLevel = "Dwarf with Weapon level";
}
public static class HarvestOptions
{
    public const string SkipFieldPhase = "Skip Field Phase";
    public const string SkipBreedingPhase = "Skip Breeding Phase";
}

public static class RubyTrades
{
    public static string Cancel = "Cancel";
    public static string Wood = "Trade 1 Ruby for 1 Wood";
    public static string Stone = "Trade 1 Ruby for 1 Stone";
    public static string Ore = "Trade 1 Ruby for 1 Ore";
    public static string Grain = "Trade 1 Ruby for 1 Grain";
    public static string Veg = "Trade 1 Ruby for 1 Veg";
    public static string Sheep = "Trade 1 Ruby for 1 Sheep";
    public static string Donkey = "Trade 1 Ruby for 1 Donkey";
    public static string Pig = "Trade 1 Ruby for 1 Pig";
    public static string Cow = "Trade 1 Ruby and 1 Food for 1 Cow";
    public static string Field = "Trade 1 Ruby for 1 Field";
    public static string Clearing = "Trade 1 Ruby for 1 Pasture";
    public static string Tunnel = "Trade 1 Ruby for 1 Tunnel";
    public static string Cavern = "Trade 2 Rubies for 1 Cavern";
    public static string ReorderDwarf = "Pay 1 Ruby to re-order a dwarf";
}

public static class DiscardActions
{
    public const string DiscardAllUnassignedAnimals = "Discard all unassigned animals";
    public static string DiscardCow = "Discard 1 cow";
    public static string DiscardPig = "Discard 1 pig";
    public static string DiscardDonkey = "Discard 1 donkey";
    public static string DiscardSheep = "Discard 1 sheep";
}

public static class FoodActions
{
    public const string FeedAllDwarves = "Feed All Dwarves";
    public const string FeedAndTakeBeggingCards = "Partially feed dwarves and take begging cards";
    public const string ConvertSheep = "Convert 1 sheep to 1 food";
    public const string ConvertPig = "Convert 1 pig to 2 food";
    public const string ConvertDonkey = "Convert 1 donkey to 1 food";
    public const string ConvertDonkeyPair = "Convert 2 donkeys to 3 food";
    public const string ConvertCow = "Convert 1 cow to 3 food";
    public const string ConvertGrain = "Convert 1 grain to 1 food";
    public const string ConvertVeg = "Convert 1 veg to 2 food";
    public const string ConvertRuby = "Convert 1 ruby to 2 food";
    public const string Convert2Gold = "Convert 2 gold to 1 food";
    public const string Convert3Gold = "Convert 3 gold to 2 food";
    public const string Convert4Gold = "Convert 4 gold to 3 food";
}

public static class HarvestTypes
{
    public const string None = "";
    public const string Harvest = "Harvest";
    public const string SingleFoodPerDwarf = "SingleFoodPerDwarf";
    public const string Special = "Special";
    public const string SkipFieldsOrAnimals = "SkipFieldsOrAnimals";
    public static string Unknown = "Unknown";
}

public static class BuildingTypes
{
    public const string Dwelling = "dwelling";
    public static string Unavailable = "unavailable";
    public static string MixedDwelling = "mixedDwelling";
    public static string Carpenter = "carpenter";
    public static string Miner = "miner";
    public static string SimpleDwelling1 = "simpleDwelling1";
    public static string CoupleDwelling = "coupleDwelling";
    public static string StoneCarver = "stoneCarver";
    public static string Builder = "builder";
    public static string AdditionalDwelling = "additionalDwelling";
    public static string Blacksmith = "blacksmith";
    public static string Trader = "trader";
    public static string SimpleDwelling2 = "simpleDwelling2";
    public static string CuddleRoom = "cuddleRoom";
    public static string WorkRoom = "workRoom";
    public static string WoodSupplier = "woodSupplier";
    public static string DogSchool = "dogSchool";
    public static string BreakfastRoom = "breakfastRoom";
    public static string GuestRoom = "guestRoom";
    public static string StoneSupplier = "stoneSupplier";
    public static string Quarry = "quarry";
    public static string StubbleRoom = "stubbleRoom";
    public static string OfficeRoom = "officeRoom";
    public static string RubySupplier = "rubySupplier";
    public static string Seam = "seam";
    public static string SlaughteringCave = "slaughteringCave";
    public static string MiningCave = "miningCave";
    public static string StoneStorage = "stoneStorage";
    public static string MainStorage = "mainStorage";
    public static string CookingCave = "cookingCave";
    public static string BreedingCave = "breedingCave";
    public static string OreStorage = "oreStorage";
    public static string WeaponStorage = "weaponStorage";
    public static string WorkingCave = "workingCave";
    public static string PeacefulCave = "peacefulCave";
    public static string SparePartStorage = "sparePartStorage";
    public static string SuppliesStorage = "suppliesStorage";
    public static string WeavingParlour = "weavingParlour";
    public static string HuntingParlour = "huntingParlour";
    public static string BroomChamber = "broomChamber";
    public static string PrayerChamber = "prayerChamber";
    public static string MilkingParlour = "milkingParlour";
    public static string BeerParlour = "beerParlour";
    public static string TreasureChamber = "treasureChamber";
    public static string WritingChamber = "writingChamber";
    public static string StateParlour = "stateParlour";
    public static string BlacksmithingParlour = "blacksmithingParlour";
    public static string FoodChamber = "foodChamber";
    public static string FodderChamber = "fodderChamber";
}

public static class ActionSpaceTypes
{
    public const string SlashAndBurn = "slashAndBurn";
    public const string Blacksmithing = "blacksmithing";
    public const string OreMineConstruction = "oreMineConstruction";
    public const string SheepFarming = "sheepFarming";
    public const string Housework = "housework";
    public const string DonkeyFarming = "donkeyFarming";
    public const string RubyMine = "rubyMine";
    public const string WishForChildren = "wishForChildren";
    public const string UrgentWish = "urgentWish";
    public const string Exploration = "exploration";
    public const string FamilyLife = "familyLife";
    public const string OreDelivery = "oreDelivery";
    public const string Adventure = "adventure";
    public const string OreTrading = "oreTrading";
    public const string RubyDelivery = "rubyDeliver";
    public static string DriftMining = "driftMining";
    public static string Excavation = "excavation";
    public static string StartingPlayer = "startingPlayer";
    public static string Logging = "logging";
    public static string Supplies = "supplies";
    public static string OreMining = "oreMining";
    public static string WoodGathering = "woodGathering";
    public static string Clearing = "clearing";
    public static string Sustenance = "sustenance";
    public static string RubyMining = "rubyMining";
    public const string SkipRound = "skipRound";
}

public static class TileTypes
{
    public static string StartingTile = "StartingTile";
    public static string Cavern = "Cavern";
    public static string Field = "Field";
    public static string Clearing = "Clearing";
    public static string Tunnel = "Tunnel";
    public static string Field3Grain = "Field3Grain";
    public static string Field2Grain = "Field2Grain";
    public static string Field1Grain = "Field1Grain";
    public static string Field2Veg = "Field2Veg";
    public static string Field1Veg = "Field1Veg";
    public static string OreMine = "OreMine";
    public static string DeepTunnel = "DeepTunnel";
    public static string RubyMine = "RubyMine";
    public static string Stable = "Stable";
    public static string SmallFence = "SmallFence";
    public static string BigFence1 = "BigFence1";
    public static string BigFence2 = "BigFence2";
}

public class TileLists
{
    public static readonly List<string> TunnelCaveDualTile = new List<string>
    {
        TileTypes.Tunnel,
        TileTypes.Cavern
    };

    public static readonly List<string> CaveCaveDualTile = new List<string>
    {
        TileTypes.Cavern,
        TileTypes.Cavern
    };

    public static List<string> FieldClearingDualTile = new List<string>
    {
        TileTypes.Field,
        TileTypes.Clearing
    };

    public static List<string> OreMineDeepTunnelDualTile = new List<string>
    {
        TileTypes.OreMine,
        TileTypes.DeepTunnel
    };

    public static List<string> BigFence = new List<string>
    {
        TileTypes.BigFence1,
        TileTypes.BigFence2
    };
}

public static class ResourceTypes
{
    public static string Food = "food";
    public static string Wood = "wood";
    public static string Stone = "stone";
    public static string Ore = "ore";
    public static string Gold = "gold";
    public static string Grain = "grain";
    public static string Ruby = "ruby";
    public static string Sheep = "sheep";
    public static string Donkeys = "donkeys";
    public static string Veg = "veg";
    public static string Pigs = "pigs";
    public static string Cows = "cows";
    public static string Begging = "begging";
    public static string Dogs = "dogs";
    public static string ScoreMarker = "score";
}

public static class CavernaActions
{
    public static readonly string Cancel = "Cancel";
    public static readonly string Finish = "Finish";
    public static readonly string CollectResources = "Collect Resources";
    public static readonly string AddTunnelCaveDualTile = "Add Tunnel/Cave dual tile";
    public static readonly string AddCaveCaveDualTile = "Add Cave/Cave dual tile";
    public static readonly string AddFieldClearingTile = "Add Field/Clearing dual tile";

    public static string SowBake = "Sowing Fields";
    public static string Sow2Grain2Veg = "Sow 2 grain and 2 veg";
    public static string Sow1Grain2Veg = "Sow 1 grain and 2 veg";
    public static string Sow2Grain1Veg = "Sow 2 grain and 1 veg";
    public static string Sow1Grain1Veg = "Sow 1 grain and 1 veg";
    public static string Sow2Grain = "Sow 2 grain";
    public static string Sow2Veg = "Sow 2 veg";
    public static string Sow1Grain = "Sow 1 grain";
    public static string Sow1Veg = "Sow 1 veg";
    public static string FurnishCavern = "Furnish a Cavern";
    public static string Blacksmithing = "Blacksmithing";
    public static string Level8Weapon = "Level 8 Weapon";
    public static string Level7Weapon = "Level 7 Weapon";
    public static string Level6Weapon = "Level 6 Weapon";
    public static string Level5Weapon = "Level 5 Weapon";
    public static string Level4Weapon = "Level 4 Weapon";
    public static string Level3Weapon = "Level 3 Weapon";
    public static string Level2Weapon = "Level 2 Weapon";
    public static string Level1Weapon = "Level 1 Weapon";
    public static string Level1Expedition = "Level 1 Expedition";
    public static string Level3Expedition = "Level 3 Expedition";
    public static string Level2Expedition = "Level 2 Expedition";
    public static string AddOreMineDeepTunnelDualTile = "Add Ore Mine/Deep Tunnel dual tile";
    public static string SmallFence = "Fence 1 field";
    public static string BigFence = "Fence 2 adjacent fields";
    public static string BuyStable = "Buy a stable";
    public static string AddRubyMineTile = "Build a Ruby Mine";
    public static string Trade2Ore = "Trade 2 Ore for 2 Gold and 1 Food";
    public static string FamilyGrowth = "Family Growth";
    public static string FurnishDwellingThenGrow = "Furnish a dwelling, then Family Growth";
    public static string FurnishDwelling = "Furnish a Dwelling";
}
