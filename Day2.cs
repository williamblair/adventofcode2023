using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventOfCode2023
{
    class GameSet
    {
        public int red;
        public int green;
        public int blue;

        public GameSet(int _red, int _green, int _blue)
        {
            red = _red;
            green = _green;
            blue = _blue;
        }
    }

    class GamesList
    {
        public int id;
        public List<GameSet> gameSet;

        public GamesList(int id, List<GameSet> gameSet)
        {
            this.id = id;
            this.gameSet = gameSet;
        }

        public int GetMaxRed()
        {
            int maxRed = 0;
            foreach (GameSet gameSet in gameSet)
            {
                if (gameSet.red > maxRed)
                {
                    maxRed = gameSet.red;
                }
            }
            return maxRed;
        }
        public int GetMaxGreen()
        {
            int maxGreen = 0;
            foreach (GameSet gameSet in gameSet)
            {
                if (gameSet.green > maxGreen)
                {
                    maxGreen = gameSet.green;
                }
            }
            return maxGreen;
        }
        public int GetMaxBlue()
        {
            int maxBlue = 0;
            foreach(GameSet gameSet in gameSet)
            {
                if (gameSet.blue > maxBlue)
                {
                    maxBlue = gameSet.blue;
                }
            }
            return maxBlue;
        }

        public int GetPower()
        {
            int maxRed = GetMaxRed();
            int maxGreen = GetMaxGreen();
            int maxBlue = GetMaxBlue();
            return maxRed * maxGreen * maxBlue;
        }
    }

    class Day2
    {
        static String[] testLines =
        {
            "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
            "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
            "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
            "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
            "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
        };

        static String[] inputLines =
        {
            "Game 1: 4 blue, 16 green, 2 red; 5 red, 11 blue, 16 green; 9 green, 11 blue; 10 blue, 6 green, 4 red",
            "Game 2: 15 green, 20 red, 8 blue; 12 green, 7 red; 10 green, 2 blue, 15 red; 13 blue, 15 red",
            "Game 3: 8 red, 2 blue; 3 green, 10 blue, 10 red; 7 green, 4 blue, 7 red; 8 red, 6 green, 13 blue; 4 green, 3 blue, 10 red; 7 blue, 7 green, 5 red",
            "Game 4: 13 green, 14 blue, 9 red; 6 green, 14 red, 18 blue; 9 red, 11 green, 3 blue; 11 green, 10 red, 14 blue; 17 blue, 3 red, 4 green; 17 blue, 1 red, 9 green",
            "Game 5: 2 green, 1 red; 8 blue, 2 green, 6 red; 5 blue, 9 red, 2 green; 3 green, 8 red, 6 blue; 6 blue, 5 red",
            "Game 6: 3 green, 7 blue, 5 red; 3 green, 6 red; 11 blue, 6 red, 1 green",
            "Game 7: 8 red, 4 green, 11 blue; 12 blue, 1 green, 5 red; 6 red, 1 green, 5 blue; 12 blue, 2 green, 2 red; 4 blue, 4 green, 3 red; 9 blue, 4 green, 8 red",
            "Game 8: 1 red, 4 green; 6 red, 1 green; 10 red; 1 blue, 2 green; 4 green, 3 red; 1 blue, 8 red",
            "Game 9: 9 blue, 13 green, 1 red; 10 green, 4 blue, 4 red; 3 red, 4 blue, 14 green; 13 blue, 1 red, 12 green",
            "Game 10: 2 blue, 16 red, 2 green; 1 green, 16 red, 6 blue; 9 red, 3 green; 1 green, 2 blue, 8 red; 8 red, 6 blue, 3 green",
            "Game 11: 7 green, 11 red, 12 blue; 3 blue, 6 green, 6 red; 10 blue, 13 green; 1 red, 13 green, 9 blue; 2 blue, 2 red, 13 green; 2 red, 3 blue, 15 green",
            "Game 12: 3 green, 2 red, 2 blue; 7 green, 5 blue; 1 blue, 1 red, 3 green",
            "Game 13: 2 green, 2 red, 3 blue; 3 blue, 3 red, 3 green; 3 green, 2 red; 2 blue, 3 red, 3 green; 2 green, 3 red, 1 blue",
            "Game 14: 4 green, 9 red; 11 green, 10 red, 12 blue; 6 red, 3 green, 12 blue; 5 green, 4 red, 4 blue; 18 blue, 7 red, 11 green; 16 blue, 4 red, 10 green",
            "Game 15: 5 green, 2 red, 9 blue; 18 green, 6 red, 20 blue; 11 blue, 12 green, 11 red; 9 red, 17 blue, 16 green; 7 green, 1 red, 9 blue",
            "Game 16: 9 blue, 11 green; 8 green, 2 blue; 1 red, 6 green, 4 blue",
            "Game 17: 2 red, 2 green, 2 blue; 7 blue, 4 green, 3 red; 2 red, 8 blue, 1 green; 2 red, 6 blue, 2 green; 4 blue, 3 red; 4 green, 5 red, 6 blue",
            "Game 18: 6 green, 7 red; 3 blue, 6 green, 1 red; 6 red, 3 blue, 5 green",
            "Game 19: 6 red, 4 green, 5 blue; 2 red, 4 blue, 13 green; 1 green, 1 blue, 2 red; 4 green",
            "Game 20: 7 red, 17 blue, 6 green; 3 blue, 6 green, 8 red; 7 blue, 6 red, 1 green; 3 green; 8 red, 7 green, 14 blue",
            "Game 21: 5 red, 3 blue, 7 green; 1 blue, 2 red, 5 green; 2 blue, 8 green, 3 red; 3 blue, 8 red, 4 green; 5 red, 1 blue, 3 green",
            "Game 22: 2 red, 6 green, 1 blue; 3 red, 3 green, 1 blue; 2 green, 7 red, 2 blue; 5 green, 1 red",
            "Game 23: 2 red, 16 green, 1 blue; 1 red, 12 green, 3 blue; 12 green, 1 blue, 3 red",
            "Game 24: 7 red, 1 blue, 12 green; 2 red, 19 green, 3 blue; 19 green, 1 blue, 12 red; 6 green, 16 red, 5 blue; 11 red, 4 blue, 12 green",
            "Game 25: 2 blue, 3 red, 8 green; 4 blue, 2 red, 9 green; 2 red, 7 blue",
            "Game 26: 17 red, 8 blue, 3 green; 3 green, 13 red, 4 blue; 20 red, 1 green, 6 blue; 7 blue, 2 red, 2 green; 20 red, 8 blue; 2 green, 16 red, 8 blue",
            "Game 27: 3 blue, 17 green, 19 red; 16 green, 5 red, 6 blue; 17 green, 16 red, 4 blue",
            "Game 28: 1 green, 7 red, 1 blue; 8 green, 12 red, 1 blue; 1 blue, 9 red, 1 green",
            "Game 29: 3 green, 3 blue, 2 red; 3 green, 2 red, 1 blue; 3 green, 2 red, 3 blue; 3 blue, 3 red, 4 green",
            "Game 30: 3 red, 8 blue, 3 green; 1 green, 1 red; 17 green, 17 blue; 19 green, 15 blue, 1 red; 1 green, 2 red, 16 blue",
            "Game 31: 11 green, 11 blue, 14 red; 6 blue, 15 green, 2 red; 11 blue, 19 green, 2 red",
            "Game 32: 9 red, 2 green; 7 green, 4 blue, 2 red; 6 red, 5 green, 1 blue; 4 red, 4 blue, 1 green; 8 red, 6 green",
            "Game 33: 6 blue, 16 red, 9 green; 5 red, 7 blue, 13 green; 1 green, 9 blue, 1 red; 4 green, 9 blue, 17 red; 2 green, 10 red, 13 blue; 9 red, 1 blue, 14 green",
            "Game 34: 2 red, 2 green, 4 blue; 3 blue, 2 green; 1 green, 1 red, 2 blue; 1 red, 3 blue, 3 green; 2 green, 8 blue, 2 red; 3 blue, 1 red",
            "Game 35: 4 red, 14 blue, 2 green; 1 green, 15 blue, 1 red; 1 blue, 2 red, 1 green",
            "Game 36: 4 blue, 1 red, 2 green; 2 green, 15 blue, 8 red; 7 blue, 1 red; 7 red, 1 green, 1 blue",
            "Game 37: 2 blue, 1 green, 5 red; 2 blue, 2 green, 4 red; 2 blue, 5 red, 8 green; 3 green, 2 blue, 1 red; 1 red, 1 blue, 5 green; 2 blue, 1 red, 8 green",
            "Game 38: 2 blue, 4 green, 11 red; 7 green, 6 red, 2 blue; 1 green, 3 red, 1 blue; 4 blue, 4 green, 4 red; 2 red, 5 blue, 2 green",
            "Game 39: 7 green, 7 blue, 2 red; 11 blue, 4 green, 8 red; 10 red, 4 green, 1 blue; 8 green, 9 blue; 9 green, 4 red; 1 green, 8 blue",
            "Game 40: 1 green, 13 blue; 6 blue, 7 red; 8 red; 1 green, 13 blue, 3 red; 1 green, 16 red, 13 blue; 14 blue, 14 red, 1 green",
            "Game 41: 5 green, 2 blue, 10 red; 4 green, 2 blue, 5 red; 6 green, 9 red, 1 blue; 4 red, 1 blue; 1 red, 3 green, 2 blue; 3 red",
            "Game 42: 17 green, 11 blue, 11 red; 5 blue, 11 green, 9 red; 10 blue, 13 red, 4 green; 8 green, 4 blue, 15 red",
            "Game 43: 1 red, 3 blue; 1 green, 3 blue, 1 red; 2 blue, 1 green; 2 green, 1 blue; 1 red, 3 blue",
            "Game 44: 7 green, 5 red, 1 blue; 6 green, 1 blue, 5 red; 2 blue, 6 green; 3 green, 2 red; 4 green; 6 red",
            "Game 45: 16 red, 14 blue, 19 green; 1 red, 5 green, 6 blue; 16 blue, 2 green, 1 red; 15 green, 6 red, 16 blue",
            "Game 46: 8 blue, 2 green; 4 red, 3 green, 6 blue; 1 green, 8 blue, 3 red; 3 green, 12 blue, 1 red",
            "Game 47: 9 green, 3 blue; 1 green, 1 blue; 4 blue, 9 green, 6 red; 8 green, 4 blue, 6 red; 6 red, 12 green, 1 blue; 4 blue, 7 green",
            "Game 48: 11 green, 4 blue, 1 red; 11 blue, 8 red, 9 green; 4 blue, 3 red, 7 green; 10 blue, 2 green, 9 red; 8 green, 2 blue, 2 red",
            "Game 49: 8 green, 1 blue, 5 red; 1 green, 1 blue; 3 green, 4 red, 2 blue; 1 blue, 7 green, 1 red; 1 blue, 7 green, 3 red; 5 red, 5 green",
            "Game 50: 2 green, 2 red, 4 blue; 8 blue, 2 green, 7 red; 4 blue, 5 red; 9 red, 4 blue; 5 blue, 9 red; 2 green, 8 red, 6 blue",
            "Game 51: 6 green, 1 red, 2 blue; 2 red, 4 blue, 6 green; 9 blue, 4 green",
            "Game 52: 7 green, 3 red, 12 blue; 8 blue, 9 red, 5 green; 2 blue, 10 green, 8 red; 12 red, 5 green, 3 blue; 8 red, 8 green, 12 blue; 2 green",
            "Game 53: 2 green, 9 blue, 5 red; 6 red, 3 green; 5 red, 2 green",
            "Game 54: 9 red, 13 blue; 1 green, 9 red, 16 blue; 12 red, 1 blue, 4 green",
            "Game 55: 1 red, 2 blue, 3 green; 1 blue; 1 red, 5 blue, 3 green; 1 blue, 3 green; 5 blue",
            "Game 56: 1 green, 4 red, 1 blue; 1 blue, 2 red, 13 green; 5 blue, 4 red; 13 green, 3 red, 3 blue",
            "Game 57: 13 blue, 2 red, 7 green; 3 green, 4 red, 14 blue; 3 red, 3 green, 3 blue; 7 blue, 5 green, 1 red",
            "Game 58: 6 red; 1 blue, 4 red, 2 green; 3 green, 1 blue; 7 green, 1 red; 6 red, 13 green, 1 blue; 3 red, 13 green, 1 blue",
            "Game 59: 5 green, 10 red, 8 blue; 7 red, 3 green, 2 blue; 6 green, 3 red, 6 blue",
            "Game 60: 2 green, 5 red, 15 blue; 2 green, 9 blue; 9 blue, 8 green, 3 red; 2 green, 6 red, 2 blue",
            "Game 61: 8 blue, 3 green, 4 red; 1 red, 10 blue, 1 green; 4 red, 5 green, 3 blue; 3 red, 8 blue, 5 green",
            "Game 62: 19 blue, 3 red, 14 green; 1 green, 7 blue, 1 red; 15 red, 20 blue, 6 green; 8 red, 4 green, 14 blue",
            "Game 63: 13 red, 1 blue; 18 red, 4 green; 6 green, 9 red, 1 blue; 7 green, 1 blue, 9 red; 5 red, 1 blue, 4 green; 5 green, 1 blue, 17 red",
            "Game 64: 2 green, 1 blue, 5 red; 2 red, 5 green; 6 red, 4 green",
            "Game 65: 1 blue, 7 green, 1 red; 7 red, 1 green; 1 blue, 3 green, 3 red; 7 red, 3 green; 3 green, 7 red; 1 blue, 4 green",
            "Game 66: 7 green, 6 blue, 8 red; 4 green, 9 red, 3 blue; 6 green, 4 blue; 5 blue, 2 green; 6 red, 4 green, 2 blue",
            "Game 67: 10 blue, 17 green, 17 red; 11 red, 9 blue, 9 green; 9 blue, 19 red, 5 green; 5 red, 3 blue, 20 green; 11 red, 1 blue, 7 green",
            "Game 68: 9 green, 4 red, 5 blue; 11 blue, 9 green, 2 red; 11 blue, 2 red, 6 green; 2 green, 6 red, 3 blue; 1 blue, 6 green, 4 red",
            "Game 69: 3 red, 15 blue, 1 green; 4 red, 14 blue, 2 green; 4 red, 18 blue, 4 green",
            "Game 70: 3 red, 8 green; 2 red, 6 green; 4 red, 2 blue, 2 green; 8 red, 1 green, 2 blue; 6 red, 3 blue, 4 green; 13 green, 8 red",
            "Game 71: 3 green, 17 red; 2 red, 3 green; 2 green, 8 red, 1 blue; 11 red, 4 blue; 3 green, 11 red, 3 blue",
            "Game 72: 1 red, 17 blue, 8 green; 2 red, 11 blue, 16 green; 3 red, 16 blue, 1 green; 2 red, 3 green, 10 blue",
            "Game 73: 1 blue, 10 green, 8 red; 19 green, 10 red, 5 blue; 3 green, 13 red, 8 blue; 12 green, 4 blue; 2 green, 10 blue, 12 red",
            "Game 74: 17 blue, 7 red, 10 green; 16 blue, 5 red; 9 blue, 7 green, 2 red; 10 red, 4 green, 14 blue",
            "Game 75: 10 green, 5 blue, 4 red; 7 red, 10 blue, 7 green; 7 blue, 9 green, 2 red",
            "Game 76: 13 green, 16 red, 20 blue; 4 red, 14 blue, 5 green; 12 red, 1 blue, 8 green",
            "Game 77: 4 red, 2 green; 8 blue, 3 green, 2 red; 5 blue, 7 green, 3 red",
            "Game 78: 12 green, 8 red, 8 blue; 10 green, 9 red, 10 blue; 16 blue, 1 red, 17 green; 4 red, 15 green, 13 blue",
            "Game 79: 4 green, 2 red; 15 red, 3 blue; 15 red, 5 green",
            "Game 80: 4 blue, 1 green, 13 red; 13 red, 1 blue, 5 green; 5 blue, 9 red; 3 blue, 3 green; 1 red; 3 red, 7 green, 6 blue",
            "Game 81: 10 red, 3 green, 4 blue; 2 red, 5 green, 16 blue; 3 green, 1 blue; 9 blue, 2 green, 12 red",
            "Game 82: 1 green, 9 blue, 1 red; 10 blue, 1 red, 1 green; 1 green, 7 blue; 8 blue",
            "Game 83: 1 blue, 5 red; 2 blue, 3 red; 1 green, 2 blue, 1 red; 2 red, 1 blue, 1 green; 1 green, 1 blue; 2 red, 1 green",
            "Game 84: 5 red, 14 blue, 2 green; 6 blue, 5 red, 8 green; 12 green, 3 blue, 5 red; 2 red, 10 green; 9 green, 14 blue",
            "Game 85: 2 blue, 2 red; 14 red, 6 green, 5 blue; 5 green, 4 blue, 6 red; 8 red, 5 blue, 6 green",
            "Game 86: 1 blue, 10 red; 4 red; 9 blue, 18 red, 3 green; 1 green, 1 blue, 7 red; 3 green, 8 red, 9 blue; 14 red, 2 green, 4 blue",
            "Game 87: 1 green, 11 red, 8 blue; 1 green, 11 red, 2 blue; 7 red, 4 blue; 6 blue, 1 red, 2 green; 13 blue, 2 green; 6 blue, 12 red, 3 green",
            "Game 88: 2 blue, 4 red, 8 green; 4 blue, 7 red; 3 red, 10 green, 4 blue; 9 green, 3 blue, 5 red; 4 red, 6 blue, 3 green",
            "Game 89: 6 red, 10 green; 15 green, 15 red, 10 blue; 15 red, 1 green, 4 blue; 13 red, 6 blue, 4 green",
            "Game 90: 17 green, 2 red, 1 blue; 6 green; 1 blue, 1 green; 1 blue, 16 green, 3 red; 14 green, 1 red",
            "Game 91: 3 blue, 8 green; 3 green, 7 red, 9 blue; 12 blue; 9 red, 7 blue, 4 green; 1 green, 7 red, 1 blue",
            "Game 92: 11 blue, 9 red, 12 green; 1 blue, 14 red, 6 green; 9 green, 6 red, 6 blue",
            "Game 93: 1 red, 2 blue; 3 blue, 6 green; 1 red, 4 green, 3 blue",
            "Game 94: 3 green, 3 blue; 1 red, 3 blue, 9 green; 3 blue, 10 green, 3 red; 10 green, 6 blue, 2 red; 9 blue, 14 green, 2 red; 1 red, 4 blue, 1 green",
            "Game 95: 7 blue, 10 green; 3 blue, 5 green, 2 red; 4 blue, 10 green, 12 red; 6 green, 2 red, 6 blue",
            "Game 96: 2 blue, 18 green, 8 red; 13 green, 3 blue, 3 red; 3 blue, 15 red, 8 green; 13 green, 10 red, 2 blue",
            "Game 97: 14 blue, 2 red; 15 blue, 1 green, 2 red; 3 red, 6 blue, 1 green; 1 green, 14 blue, 4 red",
            "Game 98: 4 blue, 9 red; 10 red, 1 green, 11 blue; 7 blue, 1 red; 1 red, 6 blue, 1 green",
            "Game 99: 7 red, 6 green, 2 blue; 8 red; 16 green, 7 red, 4 blue",
            "Game 100: 1 red, 1 green, 9 blue; 6 blue, 4 green, 3 red; 4 red, 2 green; 3 green, 2 red, 11 blue; 6 green, 5 blue, 1 red"
        };

        public static GamesList ParseLine(String line)
        {
            List<GameSet> games = new List<GameSet>();

            // Get id
            String[] words = line.Split();
            String idStr = words[1];
            int id = Int32.Parse(idStr.TrimEnd(':'));

            // Get games
            int i = 2;
            while (i < words.Length)
            {
                int red = 0;
                int green = 0;
                int blue = 0;

                while (i < words.Length)
                {
                    int val = Int32.Parse(words[i]);
                    String color = words[i + 1];
                    if (color.EndsWith(",") || color.EndsWith(';'))
                    {
                        color = color.Substring(0, color.Length - 1);
                    }
                    switch (color)
                    {
                        case "red":
                            red = val;
                            break;
                        case "green":
                            green = val;
                            break;
                        case "blue":
                            blue = val;
                            break;
                        default:
                            throw new Exception("Invalid color: " + color);
                    }
                    if (words[i + 1].EndsWith(';') || (i + 2 >= words.Length))
                    {
                        games.Add(new GameSet(red, green, blue));
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                    i += 2;
                }
                
            }

            return new GamesList(id, games);
        }

        public static bool TestGame(GamesList gameList, int maxRed, int maxGreen, int maxBlue)
        {
            foreach (GameSet gameSet in gameList.gameSet)
            {
                if (gameSet.red > maxRed || gameSet.green > maxGreen || gameSet.blue > maxBlue)
                {
                    return false;
                }
            }
            return true;
        }

        public static int SumValidGameIds(List<GamesList> games)
        {
            int sum = 0;
            foreach (GamesList gamesList in games)
            {
                int id = gamesList.id;
                int maxRed = 12;
                int maxGreen = 13;
                int maxBlue = 14;
                if (TestGame(gamesList, maxRed, maxGreen, maxBlue))
                {
                    sum += id;
                }
            }
            return sum;
        }

        public static void Part1And2()
        {
            List<GamesList> testInList = new List<GamesList>();
            foreach (String str in testLines)
            {
                testInList.Add(ParseLine(str));
            }
            if (SumValidGameIds(testInList) == 8)
            {
                Console.WriteLine("Part1 test list sum passed");
            }
            else
            {
                Console.WriteLine("Part1 test lis sum failed");
                return;
            }

            if (testInList[0].GetPower() != 48 ||
                testInList[1].GetPower() != 12 ||
                testInList[2].GetPower() != 1560 ||
                testInList[3].GetPower() != 630 ||
                testInList[4].GetPower() != 36)
            {
                Console.WriteLine("Part2 get powers failed");
                return;
            }
            Console.WriteLine("Part2 get powers passed");

            List<GamesList> inputList = new List<GamesList>();
            foreach (String str in inputLines)
            {
                inputList.Add(ParseLine(str));
            }
            int validIdSum = SumValidGameIds(inputList);

            Console.WriteLine("Part1 validIdSum: " + validIdSum);


            int powerSum = 0;
            foreach (GamesList gamesList in inputList)
            {
                powerSum += gamesList.GetPower();
            }
            Console.WriteLine("Part2 inputList power sum: " +  powerSum);
        }
    }
}
