using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventOfCode2023
{
    class ToyBoat
    {
        public long velocity; // millimeters per millisecond
        public long acceleration; // millimeters per millisecond

        public ToyBoat()
        {
            velocity = 0;
            acceleration = 1;
        }

        public long CalcDistanceFromHoldButtonAndRaceTime(long holdMilliseconds, long runMilliseconds)
        {
            long distance = 0;
            long vel = holdMilliseconds * acceleration;
            distance = vel * runMilliseconds;
            return distance;
        }

        public long CalcNumWaysToWinRace(long raceTotalMilliseconds, long raceMaxDistance)
        {
            long numWaysToWinRace = 0;
            for (long i = 0; i < raceTotalMilliseconds; ++i)
            {
                if (CalcDistanceFromHoldButtonAndRaceTime(
                    i,
                    raceTotalMilliseconds-i) > raceMaxDistance)
                {
                    ++numWaysToWinRace;
                }
            }
            return numWaysToWinRace;
        }
    }

    class Day6
    {
        static long[] testTimes = { 7, 15, 30 };
        static long[] testDistances = { 9, 40, 200 };
               
        static long[] actualTimes = { 40, 81, 77, 72 };
        static long[] actualDistances = { 219, 1012, 1365, 1089 };
               
        static long testTimePart2 = 71530;
        static long testDistancePart2 = 940200;
               
        static long actualTimePart2 = 40817772;
        static long actualDistancePart2 = 219101213651089;

        public static long CalcNumWinRaceWaysProduct(long[] inTimes, long[] inDistances)
        {
            long product = 1;
            ToyBoat boat = new ToyBoat();
            for (int i=0; i<inTimes.Length; ++i)
            {
                product *= boat.CalcNumWaysToWinRace(inTimes[i], inDistances[i]);
            }
            return product;
        }

        public static void Part1()
        {
            if (CalcNumWinRaceWaysProduct(testTimes, testDistances) != 288)
            {
                Console.WriteLine("Part1 test failed");
                return;
            }
            Console.WriteLine("Part1 test passed");

            Console.WriteLine("Part1 num ways product: " + 
                CalcNumWinRaceWaysProduct(actualTimes, actualDistances));
        }

        public static void Part2()
        {
            ToyBoat boat = new ToyBoat();
            if (boat.CalcNumWaysToWinRace(testTimePart2, testDistancePart2) != 71503)
            {
                Console.WriteLine("Part2 test failed");
                return;
            }
            Console.WriteLine("Part2 test passed");

            Console.WriteLine("Part2 num ways: " +
                boat.CalcNumWaysToWinRace(actualTimePart2, actualDistancePart2));
        }
    }
}
