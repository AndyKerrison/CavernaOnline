namespace Assets.ServerScripts
{
    public class BuildingTile {
        public int WoodCost { get; private set; }
        public int StoneCost { get; private set; }
        public int VegCost { get; private set; }
        public bool IsDwelling { get; set; }
        public int VP { get; private set; }
        public string BuildingType { get; private set; }
        public bool IsUnlimited { get; private set; }

        public BuildingTile(string buildingType)
        {
            BuildingType = buildingType;

            if (buildingType == BuildingTypes.Dwelling)
            {
                VP = 3;
                WoodCost = 4;
                StoneCost = 3;
                IsUnlimited = true;
                IsDwelling = true;
            }
            if (buildingType == BuildingTypes.SimpleDwelling1)
            {
                VP = 0;
                WoodCost = 4;
                StoneCost = 2;
                IsUnlimited = false;
                IsDwelling = true;
            }
            if (buildingType == BuildingTypes.SimpleDwelling2)
            {
                VP = 0;
                WoodCost = 3;
                StoneCost = 3;
                IsUnlimited = false;
                IsDwelling = true;
            }
            if (buildingType == BuildingTypes.TreasureChamber)
            {
                VP = 0;
                WoodCost = 1;
                StoneCost = 1;
                IsUnlimited = false;
                IsDwelling = false;
            }
            if (buildingType == BuildingTypes.FoodChamber)
            {
                VP = 0;
                WoodCost = 2;
                VegCost = 2;
                IsUnlimited = false;
                IsDwelling = false;
            }
            if (buildingType == BuildingTypes.PrayerChamber)
            {
                VP = 0;
                WoodCost = 2;
                IsUnlimited = false;
                IsDwelling = false;
            }
            if (buildingType == BuildingTypes.BroomChamber)
            {
                VP = 0;
                WoodCost = 1;
                IsUnlimited = false;
                IsDwelling = false;
            }
        }
    }
}
