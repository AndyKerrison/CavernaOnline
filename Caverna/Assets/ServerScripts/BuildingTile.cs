namespace Assets.ServerScripts
{
    public class BuildingTile {
        public int WoodCost { get; private set; }
        public int StoneCost { get; private set; }
        public bool IsDwelling { get; set; }
        public int VP { get; private set; }

        private bool _isUnlimited;

        public BuildingTile(string buildingType)
        {
            if (buildingType == BuildingTypes.Dwelling)
            {
                VP = 3;
                WoodCost = 4;
                StoneCost = 3;
                _isUnlimited = true;
                IsDwelling = true;
            }
        }
    }
}
