using System;

namespace Enigma.Test.TestDb
{
    public static class Scenario
    {


        public static TestDbContext SomeCars()
        {
            var context = new TestDbContext();
            context.Cars.Add(RandomCars.AK9777);
            context.Cars.Add(RandomCars.NDN100);
            context.Cars.Add(RandomCars.MDS800);
            context.SaveChanges();
            context.WaitForBackgroundQueue();
            return context;
        }

        public static TestDbContext ManyCars()
        {
            var context = new TestDbContext();
            context.Cars.Add(RandomCars.AK9777);
            context.Cars.Add(RandomCars.NDN100);
            context.Cars.Add(RandomCars.MDS800);
            context.Cars.Add(TheACars.AAA001);
            context.Cars.Add(TheACars.AAA002);
            context.Cars.Add(TheACars.AAA003);
            context.Cars.Add(TheACars.AAA004);
            context.Cars.Add(TheACars.AAA005);
            context.Cars.Add(TheACars.AAA006);
            context.Cars.Add(TheACars.AAA007);
            context.Cars.Add(TheACars.AAA008);
            context.Cars.Add(TheACars.AAA009);
            context.Cars.Add(TheACars.AAA010);
            context.SaveChanges();
            context.WaitForBackgroundQueue();
            return context;
        }
    }
}
