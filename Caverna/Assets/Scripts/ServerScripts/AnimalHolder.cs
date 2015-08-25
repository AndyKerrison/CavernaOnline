using System;
using UnityEngine;

namespace Assets.ServerScripts
{
    public class AnimalHolder
    {
        public Vector2 position;
        public bool isCave;
        public int genericCapacity;
        public int donkeyCapacity;
        public int pigCapacity;
        public int sheepCapacity;
        public int cowCapacity;

        private int dogs;
        private int sheep;
        private int donkeys;
        private int pigs;
        private int cows;

        public int RemainingCowCapacity
        {
            get
            {
                if (sheep > 0 || pigs > 0 || donkeys > 0)
                    return 0;
                return Math.Max(genericCapacity, cowCapacity) - cows;
            }
        }

        public int RemainingPigCapacity
        {
            get
            {
                if (sheep > 0 || cows > 0 || donkeys > 0)
                    return 0;
                return Math.Max(genericCapacity, pigCapacity) - pigs;
            }
        }

        public int RemainingSheepCapacity
        {
            get
            {
                if (pigs > 0 || cows > 0 || donkeys > 0)
                    return 0;
                return Math.Max(genericCapacity, sheepCapacity) - sheep;
            }
        }

        public bool IsUnfilledCowHolder
        {
            get { return cowCapacity > cows; }
        }

        public bool IsUnfilledPigHolder
        {
            get { return pigCapacity > pigs; }
        }

        public bool IsUnfilledDonkeyHolder
        {
            get { return donkeyCapacity > donkeys; }
        }

        public bool IsUnfilledSheepHolder
        {
            get { return sheepCapacity > sheep; }
        }

        public int RemainingDonkeyCapacity
        {
            get
            {
                if (sheep > 0 || cows > 0 || pigs > 0)
                    return 0;
                return Math.Max(genericCapacity, donkeyCapacity) - donkeys;
            }
        }

        public string TileType { get; set; }

        public void FillAnimals(string animalType, ref int animalsToAllocate)
        {
            int remainingCapacity = 0;
            if (animalType == ResourceTypes.Cows)
                remainingCapacity = RemainingCowCapacity;
            if (animalType == ResourceTypes.Pigs)
                remainingCapacity = RemainingPigCapacity;
            if (animalType == ResourceTypes.Donkeys)
                remainingCapacity = RemainingDonkeyCapacity;
            if (animalType == ResourceTypes.Sheep)
                remainingCapacity = RemainingSheepCapacity;
            if (animalType == ResourceTypes.Dogs)
                remainingCapacity = 1000;

            int animalsToAdd = Math.Min(remainingCapacity, animalsToAllocate);

            if (animalType == ResourceTypes.Cows)
                cows += animalsToAdd;
            if (animalType == ResourceTypes.Pigs)
                pigs += animalsToAdd;
            if (animalType == ResourceTypes.Donkeys)
                donkeys += animalsToAdd;
            if (animalType == ResourceTypes.Sheep)
                sheep += animalsToAdd;
            if (animalType == ResourceTypes.Dogs)
                dogs += animalsToAdd;

            if (!isCave && dogs > 0)
                sheepCapacity = dogs + 1;
            else
                sheepCapacity = 0;
            
            animalsToAllocate -= animalsToAdd;
        }

        public void AddDog(ref int dogsToAllocate)
        {
            if (dogsToAllocate <= 0)
                return;
            dogs++;
            dogsToAllocate--;
            if (!isCave)
                sheepCapacity = dogs + 1;
        }

        public int GetDogs()
        {
            return dogs;
        }

        public int GetSheep()
        {
            return sheep;
        }

        public int GetDonkeys()
        {
            return donkeys;
        }

        public int GetPigs()
        {
            return pigs;
        }

        public int GetCows()
        {
            return cows;
        }
    }
}
