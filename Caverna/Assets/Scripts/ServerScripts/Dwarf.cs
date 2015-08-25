namespace Assets.ServerScripts
{
    public class Dwarf
    {
        public int ID { get; set; }

        public Dwarf(bool isChild, int ID)
        {
            this.ID = ID;
            IsChild = isChild;
            IsUsed = isChild;
        }

        public int GetFoodRequirement()
        {
            return IsChild ? 1 : 2;
        }

        public bool IsChild { get; set; }

        public bool IsUsed { get; set; }
        public int WeaponLevel { get; private set; }

        public void AddWeapon(string actionName)
        {
            if (actionName == CavernaActions.Level8Weapon)
                WeaponLevel = 8;
            if (actionName == CavernaActions.Level7Weapon)
                WeaponLevel = 7;
            if (actionName == CavernaActions.Level6Weapon)
                WeaponLevel = 6;
            if (actionName == CavernaActions.Level5Weapon)
                WeaponLevel = 5;
            if (actionName == CavernaActions.Level4Weapon)
                WeaponLevel = 4;
            if (actionName == CavernaActions.Level3Weapon)
                WeaponLevel = 3;
            if (actionName == CavernaActions.Level2Weapon)
                WeaponLevel = 2;
            if (actionName == CavernaActions.Level1Weapon)
                WeaponLevel = 1;
        }

        public void IncreaseWeaponLevel()
        {
            //only works if you have a weapon. Max level is 14
            if (WeaponLevel > 0 && WeaponLevel < 14)
                WeaponLevel++;
        }
    }
}
