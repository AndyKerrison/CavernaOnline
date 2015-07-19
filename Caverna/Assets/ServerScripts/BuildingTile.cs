using System;

namespace Assets.ServerScripts
{
    public class BuildingTile {

        public enum BuildingGroups
        {
            Dwelling,
            Working,
            Scoring
        }

        public int WoodCost { get; private set; }
        public int StoneCost { get; private set; }
        public int GrainCost { get; private set; }
        public int VegCost { get; private set; }
        public int VP { get; private set; }
        public string BuildingType { get; private set; }
        public BuildingGroups BuildingGroup { get; private set; }
        public bool IsUnlimited { get; private set; }
        public int OreCost { get; set; }
        public int FoodCost { get; set; }

        public BuildingTile(string buildingType)
        {
            BuildingType = buildingType;

            if (buildingType == BuildingTypes.Dwelling)
            {
                VP = 3;
                WoodCost = 4;
                StoneCost = 3;
                IsUnlimited = true;
                BuildingGroup = BuildingGroups.Dwelling;
            }
            if (buildingType == BuildingTypes.SimpleDwelling1)
            {
                VP = 0;
                WoodCost = 4;
                StoneCost = 2;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Dwelling;
            }
            if (buildingType == BuildingTypes.SimpleDwelling2)
            {
                VP = 0;
                WoodCost = 3;
                StoneCost = 3;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Dwelling;
            }
            if (buildingType == BuildingTypes.TreasureChamber)
            {
                VP = 0;
                WoodCost = 1;
                StoneCost = 1;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.FoodChamber)
            {
                VP = 0;
                WoodCost = 2;
                VegCost = 2;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.PrayerChamber)
            {
                VP = 0;
                WoodCost = 2;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.BroomChamber)
            {
                VP = 0;
                WoodCost = 1;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.FodderChamber)
            {
                VP = 0;
                GrainCost = 2;
                StoneCost = 1;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.WritingChamber)
            {
                VP = 0;
                StoneCost = 2;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.StoneStorage)
            {
                VP = 0;
                WoodCost = 3;
                OreCost = 1;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.OreStorage)
            {
                VP = 0;
                WoodCost = 1;
                StoneCost = 2;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.WeaponStorage)
            {
                VP = 0;
                WoodCost = 3;
                StoneCost = 2;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.SuppliesStorage)
            {
                VP = 0;
                FoodCost = 3;
                WoodCost = 1;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
            if (buildingType == BuildingTypes.MainStorage)
            {
                VP = 0;
                WoodCost = 2;
                StoneCost = 1;
                IsUnlimited = false;
                BuildingGroup = BuildingGroups.Scoring;
            }
        }
    }
}
