using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace adventOfCode2023
{
    enum ConnectionDirs
    {
        NorthSouth,
        EastWest,
        NorthEast,
        NorthWest,
        SouthWest,
        SouthEast,
        NoConnection
    };

    class TilePos
    {
        public int row;
        public int col;

        public TilePos()
        {
            row = -1;
            col = -1;
        }

        override public bool Equals(object inPos)
        {
            return ((TilePos)inPos).row == row && ((TilePos)inPos).col == col;
        }
        override public int GetHashCode()
        {
            return (row << 16) | col;
        }
    }

    class MapTile
    {
        static readonly Dictionary<char, ConnectionDirs> ConnectionDirMap =
            new Dictionary<char,ConnectionDirs> {
            { '|', ConnectionDirs.NorthSouth },
            { '-', ConnectionDirs.EastWest },
            { 'L', ConnectionDirs.NorthEast },
            { 'J', ConnectionDirs.NorthWest },
            { '7', ConnectionDirs.SouthWest },
            { 'F', ConnectionDirs.SouthEast },
            { '.', ConnectionDirs.NoConnection },
            { 'S', ConnectionDirs.NoConnection }
        };

        public TilePos pos;
        public char letter;
        public ConnectionDirs connectionDir;
        public TilePos connectionA;
        public TilePos connectionB;
        public bool isLoopTile;

        public MapTile(int _row, int _col, char _letter)
        {
            pos = new TilePos();
            pos.row = _row;
            pos.col = _col;
            letter = _letter;
            isLoopTile = false;
            connectionA = new TilePos();
            connectionB = new TilePos();
            connectionDir = ConnectionDirMap[letter];
            switch (connectionDir)
            {
                case ConnectionDirs.NorthSouth:
                    connectionA.row = _row - 1; // north
                    connectionA.col = _col;
                    connectionB.row = _row + 1; // south
                    connectionB.col = _col;
                    break;
                case ConnectionDirs.EastWest:
                    connectionA.row = _row; // east
                    connectionA.col = _col + 1;
                    connectionB.row = _row; // west
                    connectionB.col = _col - 1;
                    break;
                case ConnectionDirs.NorthEast:
                    connectionA.row = _row - 1; // north
                    connectionA.col = _col;
                    connectionB.row = _row; // east
                    connectionB.col = _col + 1;
                    break;
                case ConnectionDirs.NorthWest:
                    connectionA.row = _row - 1; // north
                    connectionA.col = _col;
                    connectionB.row = _row; // west
                    connectionB.col = _col - 1;
                    break;
                case ConnectionDirs.SouthWest:
                    connectionA.row = _row + 1; // south
                    connectionA.col = _col;
                    connectionB.row = _row; // west
                    connectionB.col = _col - 1;
                    break;
                case ConnectionDirs.SouthEast:
                    connectionA.row = _row + 1; // south
                    connectionA.col = _col;
                    connectionB.row = _row; // east
                    connectionB.col = _col + 1;
                    break;
                default:
                    // no connection
                    break;
            }
        }

        public bool ConnectsTo(MapTile tile)
        {
            if (connectionA.row != -1 && connectionA.col != -1)
            {
                if (tile.pos.row == connectionA.row &&
                    tile.pos.col == connectionA.col)
                {
                    return true;
                }
                if (tile.pos.row == connectionA.row &&
                    tile.pos.col == connectionA.col)
                {
                    return true;
                }
            }
            if (connectionB.row != -1 && connectionB.col != -1)
            {
                if (tile.pos.row == connectionB.row &&
                    tile.pos.col == connectionB.col)
                {
                    return true;
                }
                if (tile.pos.row == connectionB.row &&
                    tile.pos.col == connectionB.col)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ConnectsTo(TilePos pos)
        {
            if (connectionA.row != -1 && connectionA.col != -1)
            {
                if (pos.row == connectionA.row &&
                    pos.col == connectionA.col)
                {
                    return true;
                }
            }
            if (connectionB.row != -1 && connectionB.col != -1)
            {
                if (pos.row == connectionB.row &&
                    pos.col == connectionB.col)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCorner()
        {
            return letter == 'L' ||
                letter == 'J' ||
                letter == '7' ||
                letter == 'F';
        }

        public void SetStartLetter(bool connectsNorth, bool connectsSouth, bool connectsEast, bool connectsWest)
        {
            /*
            { '|', ConnectionDirs.NorthSouth },
            { '-', ConnectionDirs.EastWest },
            { 'L', ConnectionDirs.NorthEast },
            { 'J', ConnectionDirs.NorthWest },
            { '7', ConnectionDirs.SouthWest },
            { 'F', ConnectionDirs.SouthEast },
            { '.', ConnectionDirs.NoConnection },
            { 'S', ConnectionDirs.NoConnection }
            */
            if (connectsNorth && connectsSouth)
            {
                letter = '|';
            }
            if (connectsEast && connectsWest)
            {
                letter = '-';
            }
            if (connectsNorth && connectsEast)
            {
                letter = 'L';
            }
            if (connectsNorth && connectsWest)
            {
                letter = 'J';
            }
            if (connectsSouth && connectsWest)
            {
                letter = '7';
            }
            if (connectsSouth && connectsEast)
            {
                letter = 'F';
            }
        }
    }

    class PipeMap
    {
        List<List<MapTile>> tiles;
        int sRow, sCol;
        public long longestPathCount;
        public long enclosedTileCount;

        public PipeMap(String[] input)
        {
            tiles = new List<List<MapTile>>();
            sRow = -1;
            sCol = -1;
            enclosedTileCount = 0;
            TilePos curPos1 = new TilePos();
            TilePos curPos2 = new TilePos();
            TilePos prevPos1;
            TilePos prevPos2;
            HashSet<TilePos> tileHistory = new HashSet<TilePos>();

            for (int row = 0; row < input.Length; ++row)
            {
                List<MapTile> mapRow = new List<MapTile>();
                for (int col = 0; col < input[row].Length; ++col)
                {
                    mapRow.Add(new MapTile(row, col, input[row][col]));
                    if (input[row][col] == 'S')
                    {
                        sRow = row;
                        sCol = col;
                    }
                }
                tiles.Add(mapRow);
            }
            tiles[sRow][sCol].isLoopTile = true;

            bool startConnectsNorth = false;
            bool startConnectsSouth = false;
            bool startConnectsEast = false;
            bool startConnectsWest = false;
            for (int r = sRow - 1; r <= sRow + 1; r++)
            {
                for (int c = sCol - 1; c <= sCol + 1; c++)
                {
                    if (r == sRow && c == sCol)
                    {
                        continue;
                    }
                    if (r < 0 || r >= tiles.Count ||
                        c < 0 || c >= tiles[0].Count)
                    {
                        continue;
                    }

                    if (tiles[r][c].ConnectsTo(tiles[sRow][sCol]))
                    {
                        tiles[r][c].isLoopTile = true;
                        if (curPos1.row == -1 && curPos1.col == -1)
                        {
                            curPos1 = tiles[r][c].pos;
                        }
                        else
                        {
                            curPos2 = tiles[r][c].pos;
                        }
                        
                        if (r == sRow - 1)
                        {
                            startConnectsNorth = true;
                        }
                        else if (r == sRow + 1)
                        {
                            startConnectsSouth = true;
                        }

                        if (c == sCol - 1)
                        {
                            startConnectsWest = true;
                        }
                        else if (c == sCol + 1)
                        {
                            startConnectsEast = true;
                        }
                    }
                }
            }
            tiles[sRow][sCol].SetStartLetter(startConnectsNorth, startConnectsSouth, startConnectsEast, startConnectsWest);

            long path1Count = 0;
            long path2Count = 0;
            prevPos1 = tiles[sRow][sCol].pos;
            prevPos2 = tiles[sRow][sCol].pos;
            while (!tileHistory.Contains(curPos1) && !tileHistory.Contains(curPos2))
            {
                tileHistory.Add(curPos1);
                tileHistory.Add(curPos2);
                
                if (tiles[curPos1.row][curPos1.col].connectionA.Equals(prevPos1))
                {
                    prevPos1 = curPos1;
                    curPos1 = tiles[curPos1.row][curPos1.col].connectionB;
                }
                else
                {
                    prevPos1 = curPos1;
                    curPos1 = tiles[curPos1.row][curPos1.col].connectionA;
                }
                if (tiles[curPos2.row][curPos2.col].connectionA.Equals(prevPos2))
                {
                    prevPos2 = curPos2;
                    curPos2 = tiles[curPos2.row][curPos2.col].connectionB;
                }
                else
                {
                    prevPos2 = curPos2;
                    curPos2 = tiles[curPos2.row][curPos2.col].connectionA;
                }
                tiles[curPos1.row][curPos1.col].isLoopTile = true;
                tiles[curPos2.row][curPos2.col].isLoopTile = true;
                ++path1Count;
                ++path2Count;
            }
            longestPathCount = path1Count;

            for (int row = 0; row < tiles.Count; ++row)
            {
                bool isInLoop = false;
                int col = 0;
                while (col < tiles[row].Count)
                {
                    MapTile tile = tiles[row][col];
                    if (!tile.isLoopTile)
                    {
                        if (isInLoop)
                        {
                            ++enclosedTileCount;
                        }
                        ++col;
                    }
                    else
                    {
                        if (tile.IsCorner())
                        {
                            do
                            {
                                ++col;
                            } while (col < tiles[row].Count && !tiles[row][col].IsCorner());
                            MapTile secondTile = tiles[row][col];
                            bool cornersOppositeNorthSouth = false;
                            if ((tile.letter == 'L' && secondTile.letter == '7') ||
                                (tile.letter == 'F' && secondTile.letter == 'J'))
                            {
                                cornersOppositeNorthSouth = true;
                            }
                            if (cornersOppositeNorthSouth)
                            {
                                    isInLoop = !isInLoop;
                            }
                            ++col;
                        }
                        else
                        {
                            isInLoop = !isInLoop;
                            ++col;
                        }
                    }
                }
            }
        }

        public void PrintMap()
        {
            foreach (List<MapTile> row in tiles)
            {
                bool insideLoop = false;
                bool prevWasLoopTile = false;
                foreach (MapTile tile in row)
                {
                    if (tile.isLoopTile)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        if (insideLoop)
                        {
                            Console.Write("I");
                        }
                        else
                        {
                            Console.Write(tile.letter);
                        }
                    }
                    prevWasLoopTile = tile.isLoopTile;
                }
                Console.WriteLine();
            }
        }
    }

    class Day10
    {
        static String[] testInput =
        {
            "..F7.",
            ".FJ|.",
            "SJ.L7",
            "|F--J",
            "LJ..."
        };

        static String[] testInput2 =
        {
            "FF7FSF7F7F7F7F7F---7",
            "L|LJ||||||||||||F--J",
            "FL-7LJLJ||||||LJL-77",
            "F--JF--7||LJLJ7F7FJ-",
            "L---JF-JLJ.||-FJLJJ7",
            "|F|F-JF---7F7-L7L|7|",
            "|FFJF7L7F-JF7|JL---7",
            "7-L-JL7||F7|L7F-7F7|",
            "L.L7LFJ|||||FJL7||LJ",
            "L7JLJL-JLJLJL--JLJ.L"
        };

        static String[] actualInput =
        {
            "F.F77.F7F7FFJJ..77FF.--L777.-7777--F7F7FJFFF7FF|.F77FF|-7-|-F7-7-FF|FJ-FFJ7-F--F|-L--7L|-7FFFF---7-FJ.7J.F-7J.F|7-LF7L7|.F-L|7...LF--LL7F-77",
            "|77FL-F7F-FJJF7FJ-FJF.L||FLJF7JFJ7FJ77LJ-7-||L77-F-7||L7|LL.JJ.L-FFJ-|.FJJ|F|FJJ|..|-7.77LLJF7JJ-|7L77J|.|L|-L7|L-|LL-L7-|7.L7F7-FLJ|FFLJ-LJ",
            "JFFJJFF-|J.|L7.7FL|7|7.LL-J.-JF||J|-L|7|FL7L7FJ|.--7|F---.L-J.|-7JL-F7-7--|7L7.7.F.-JL7L|J.F-JJJ7LFJJLL7F|-7.LF-7.F7LF|L-LFLJL-7FJL|-|7|7.FJ",
            "LLJ7LJL7||FF-|-J7J||--7J7.FJL7-.F-J.|..F7L|FF|J77LFFLFJL|F7|FLJLFFJ-J7.LJ|L|FJ-F7J.LF|--JF--7|L||7.|JJ|LFJF7J-7.|.|J-|-JFFJ|J7|L|--F.--F77.7",
            "LLLJL|-7-7-|-F7-77.JJ.|-F-7J||L-F7.|.7-L|7JFLJLJ|.7--|7JL.FJJJ-F.L.7F-7JF7J|JL|LJ-..F7|7.F-LL-FF-|7J|FLJ|.J.FFF7F7JJLF.|F7LL.F|7L7-|7JJJLF.L",
            "F|F|F||L.|L7|||FJ|7L--JFL7L-|-7.7--JJ|LLJ7LFJ.LL|.LJFLL7.F.|7F-LJJ-JLLJ.|7J|LLF7J.|7|L-77|-JLF7J7LJF-77F|.||7.F7||.|.L-|-|.FFFJJ7JFLF7-F|J.|",
            "-77.|J7|-7.L-J7L-F77L|J77LFLL||F|7FF7|..|J.L-L-.L7L|L7FL77.-7L7J||.F-LJ-JJ.|7||L-.FFL-7L7J.JJLL7F77F-L77|--L-F|LJ|-77|7|J|-7-LL.|LL7||F-JFL-",
            "L7--|F-F7..FLL|7FJJL-7-F7L--L|F|LF|.|F-F7-7J.|F|.|-7F7-||J|7||L7L|F-7.LJ|.F77|--L7F7F7|FJ.FL7.||..FJ-7--F--|.LL7FJ.L|-JJ|.FL7J.LLF-|--L-7--L",
            "7.|L-7.|7|777..|FJF7-|LLF7|.|LF7F7L7.L-LJFJLF|-F-7L|J...L-L|J-J-|||L7J|FL---J|FFLF|||||L-77-L|-JFF|7FLJFL7FF7-FJ|FL-J.|77|-|--JJ-|7.7-L-|FLL",
            "|F|7-|.L||L-J-J7L7L7F|7-7-FF7.||-F7|F7F|F7J7--7L7|-L.7FF.|L|7FJ-77.L|.7-7|FJJ--7|F|||||F-JL7L|LF7FL-7JF7LF-JL7|FJ7.77-J7F77LF|.|.|JJ|7|--7FL",
            "7-J7-J--J|-|||.FF7F-7F7F77||L-JL--7-JF7FJ|J|FJ7.|L7F--7F-|-77|7..|7F77|LL||||-|.FFJLJLJ|J|L|.FFJ|-J|77LJ-L--7LJL-7.7J7||-7JF-L7--JLLLJJ.||F7",
            "-7JF7|.J7|F7F7.F|||FJ|||L7-L7F--7FJ.FJ|L7|F7J-FF|FJ-|-L|F--F7|FLJ-7LJ-|-J|FF|FLF-JF---7|F7F7-FJFJJ.L-|||J.F-JF7F-J7LL-JJ.|FL7J.7-77FFJ...L|J",
            "F7.LJ|FLLFFJFL|-|||L-JLJFJF7||-FJL-7L7|FJ||||.LFJL-7.|J|-L7||77J.L|JFF7J---|L7.L--J.F-JLJLJL-JFJJLF7FFF7.FL-7|||LFFF7J7.F|J-J.LF-77JLJF77L-J",
            "FJ|J|-7|L-JFFF--J||F7F--J7||LJ-L-7FJ-||L7||L-7.L7F-J-F-7JLFJ|JL-.LLF7|L7-LFJ.|FF7|F7L--7F-----JF7L|-F7|L7F7|||LJ-F7F7JF-FL--F-|LFL|FLF-F7-|7",
            "|F|.J-J77JF|.L--7|LJ|L-7F7||F77LFJ||FJL7|||F-J7FJ|F7FJFJF7|FJF7LFF7||L7|F-7L-F-JL-JL7F7|L7F7F77||F7.|||FJ||FJL-7-|LJ|FJJ.|LL|7|FJ|JF7|FL||.F",
            "F--7J|LL77|.FF--JL-7|F-J|LJ|||F7L7|FJF-J|LJL7F7L7LJLJFJFJLJL-JL--J||L-JLJFJ-|L-----7|||L7||LJL7|||L7||||FJ|L--7L-JF-JJL-F77.|LJJ-J.J7|-JLFF7",
            "JJJ|F7FFJ-L7FL7F--7LJ|F7L7FJ||||FJ||FJF-JF--J||FJF---J|L---7F-----JL-7F--JLFF7-LF--JLJL7|||F--J|||FJ|||||FJF77|F7FJF7LF||F777.|.F|F|||J7-FJJ",
            "L77.JJ--J|.FJ.LJF7L-7LJL-J|FJ|||L7|||LL-7|F7L||L7|F-7-F-7F7|L-7F7F---JL--7-FJL7-L---7F7LJ|||FF-JLJL7|LJ||L-JL-J|LJ7|L7F7FJL77-7-77-F-J-F---J",
            ".L|7--7|-FFF--7FJL-7L--7F-J|FJ||FJLJ|F-7|LJL7|L-J|L7L7L7||||F-J||L7F7F---JJ|F-J-F7F7||L-7LJL-JF7F--JL-7|L--7F-7|F7F|FJ|LJF-J--F7LJJ.-J.J.FL.",
            "7J||F7L---JL-7|L7F7L7F-JL7FJL7||L7F-J|FJ|F--JL7F7|7|FJFJLJLJL7FJL7LJ||F--7-||F7L|LJ|LJF-JF----JLJF7F7FJL7F7LJFJLJL-J|FL-7L7..FJF77.|.F|JF|..",
            "-77F-FJ-JJ7FL||-LJL7|L-7FJL-7|||-||F7|L7|L-7F7||LJFJL7L---7F-JL-7L7FJLJF-JFJ||L7L-7L77L-7L-7F7F7FJLJ|L-7LJ|F7|F-----JF7FJFJF7JFF-77L--.|-F|J",
            "|LL77||L||L-7|L----JL--JL---JLJL7|||LJFJ|F-J|LJL-7L-7L--7L|L-7F7|FJL7F7L7-L7LJFJ-FJFJF7FJF-J||||L--7|F7L7FJ||||F7F---J|L7L7||F7|FJ--7-F7JJLL",
            "|7-L7|J.L|.LFL-7F-7F7F----7F--7FJ|||F7L-J|F7L---7L7FJF--JFJF-J|LJ|F7LJ|FJF7L-7|F7L7L7||L7|F7|LJ|F7FJ||L-J|-|LJLJLJF7F-JFJFJ|||||||.F7.-J.-J|",
            "LJJL-7-|J||L7|JLJ|LJLJF7F7LJF-JL7|||||F--J|L7.F7|FJL7L-7JL7L-7L-7||L7L|L7|L7FJ||L7|FJ|L-JLJLJF-J|||FJL--7L7L-7F---J|L7-L7L-J||LJL-7-|7|.|JL|",
            "|JL77L-J--JJ|F7F-7F---JLJ|F-JF-7|||||LJF7.|FJFJLJ||FJF-JF7L7FJF7|||FJFJFJ|FJL7|L7LJ|-L--7F7F-JF-J||L---7L7|F7||F7F7L-JF7|F--J|F---J|L7JF|-7|",
            "|.-J7|7|FLJF7|||FJ|F--7F-JL--JJ|||LJL7FJL-JL7L--7L7L7L7FJL-J|FJLJ|||FL7|7|||J||.L-7L7F7FJ|LJF7L-7||F---JFJLJLJLJ||L7F7||||F7FJL7J7LF-|.L||FF",
            "LJJ.FF7-7JLF7|LJL7||F-JL7F-----JLJF--J|F---7|F-7L7|FJFJ|F--7||F--J|L7FJL-JL7FJ|F-7|FJ||L7L7.||JFJ||L7LF7L---7F--JL7LJ||LJLJLJF7L-77L-J-.-|FJ",
            "JJ|.77JL7--||L-7FJLJL--7|L------7FJ-F7|L7F7LJL7|FJ||FJLLJF7|LJL--7|FJL---7FJL7|L7|||FJ|JL7L-JL7L7||FJFJ|F-7L||F7F7|F-J|F7F-7FJL-7L7JL|FF||F7",
            "LFF7LFJJ..||L--JL--7-F7|L7F----7|L7FJLJFJ|L7F-J||FJ|L77F-JLJF----J|L--7|FJL-7|L7||||L7|F7|F---JFJ||L-JFJL7|FJLJLJLJL--J|LJ.|L--7L-JJJ.F7LF.F",
            "LL-7FL-7F--|F7F-7F7L7|||FJL---7||FJL-7FJFJFJL-7||L7L7L7L---7L7F7F7L--7|FJF--J|FJ|||||||||||7F77L7LJF--JF7||L-7F7F------JJF7L--7L-77J.F-JF..|",
            "JJ|7.|-F|JJLJLJFLJL7LJLJL-7-F7||||F7FJL7|FJF7.||L7L7L7L7LF7L7||LJ|F7FJ||FJ7F-JL7||||FJLJLJ|FJL7-L-7L7F7|LJ|F7||LJF-7F7F7FJL--7|F-J--7|.77-FJ",
            "F7L---7L-JF|F--7-F7L---7F-JFJLJLJLJ||F7LJL7||FJ|-L7|FJFJFJ|FJ||F-J||L7|||F7L--7LJ|||L-7F--JL7FJF7FJFJ||L-7||||L--JFJ|||||F---J|L-7.LL-7||LJ.",
            "F77JF-J|JFLFJF7L7||F7F7||7FJF-7F7F7LJ|L---J||L7|F7|||FJ7L7|L7|||LFJL-JLJ|||-F7L7FJ||F7|L7F7FJL7||L7L7||-FJ||||F7F-JFJLJ||L---7L-7|FLL7.|-7F-",
            "LJJJ|FL7F|.L7|L7LJLJLJLJL-JFJFJ||||F7|F7F7FJL7|LJ||LJL7F7||FJ|||FJF-----J||FJ|FJ|FJ||||FJ|LJF7|||FJFJ||FJFJ||||LJF7|F--J|F---JF7LJJ.J||LL--L",
            "|..|.-J||LJ.LJJL-7F7F-7F-7FJFJFJ||LJ||||||L7FJL-7||F--J||||L7||||FJ7F7F7F|||FJL7|L7|||||.L7FJLJ||L7|J||L7L7|||L7FJLJ|F7FJL---7||J|.|FF|7|LF|",
            "|.FFJJ77JFLF-77F7LJLJ|||FJL7L-J-LJ-FJ||||L7|L7F7|||L7F7|LJ|FJ||LJL7FJ||L7|||L7FJL7|||||L7FJL-7FJL7||FJL7|FJ||L7||F--J|LJF----J||7JJ.F|.LJ.J|",
            "7-F|7F.L7L.L7L-JL-----J|L--J7F-----JFJ|||J||FJ|||||FJ||L7FJL7|L7F-J|J|L7LJ||L|L7FJ|||||FJL-7FJ|F-J|||F-J||F||7|LJL7F7L7FJF-7F7|L7JJ.||FJ.-JF",
            "|FJ|F--7LJ7.L------7F-7|F7F--JF7F7F7L7|||FJ|L7|LJ|||FJ|FJL7FJ|FJL-7L7|FJF-JL7|FJL7|||||L7F-J|FJL-7||||F-JL-JL7|F--J||FJL7|FJ|||FJLLFJF|...77",
            "L7-FLJFJLF-F------7LJFJ|||L---JLJLJL7LJ|||FJFJL-7|||L7||F-J|FJL7F-JFJ|L7L7F7|LJF7||||||FJ|LFJL--7||LJLJF7F7F7LJL7F7|LJF7LJ|FJLJL-7FF|FLFFL.|",
            "||7.L-J|F-7L--7F-7L7LL-J||F--7F7F7F-JF-J|LJFL7F-J||L7||||F7|L7FJ|F7L7|FJ||||L--J|||||||L7L7L-7F-J||F-7FJLJLJL7F7LJLJF7|L-7LJF----J777||JL|7F",
            "|-|77.|L7LF---J|FL7|F7F7||L-7LJLJ|L-7L-7L7F--JL7FJ|FJ|LJ||||FJL7LJL7|||F-J||F--7||||||L7L7|F-JL-7||L7|L-----7LJ|F7F7||L-7|F7L----7F7||7-F|L7",
            "LF|-FF7L7JL----JF-JLJLJLJ|F-JF--7|JFJF7L-JL-7F7|L-JL7|F-J|||L-7L-7J||LJ|F7||L-7|||||||FJFJ|L-7F7|LJFJL-7F--7L-7||LJ|||F-JLJ|F-7F7|||-|L-LJ--",
            "|FJF-J|L|-|-F-7FL-------7|L--JJFJ|FJFJL---77LJ|L--7FJ|L-7|||F-JF7|FJL7FJ|||L7FJLJ||||LJFL7L7|LJ||F-JF7FJL-7|F7||L-7|||L---7LJJLJ|LJ|.L|J.|.|",
            "-L7LLF7.77.FL7|F7F-7F7-FJL-7-F7L7|L-JF7F-7|F--JF7FJL7L7FJ|||L7FJ|||F7|L7|||FJL7F7LJLJF7F-JFJF7-LJL-7||L---JLJLJL--J|||F7F7L----7|F7|7F7-|7-7",
            "|FL7L-|-J|.LFJLJ|L7LJL7L7F7L7||FJL---J|L7|||F7FJ||F7L7|L-J||FJL7||LJ||FJ|LJ|F7LJL7F--JLJF7L-JL7F--7LJL-7F7F7F--7F-7LJ||LJL--7F7|LJ|L7JL7.|||",
            "L7.77F|-||7-L--7L7L--7||LJL7LJLJF----7|FJLJ||||FJLJ|FJL7F-J|L7FJLJ|FJ|L7L7FJ||F-7|L7F7F7||F7F7||F7L----J|LJLJF7LJFJF7|L---7FJ||L7FJFJ-7J|FLJ",
            "|FJJ.||||JF-7F7|FJF--JL----JF7F7|F---JLJFF7||LJL-7FJL-7|L-7|FJL---7|FJL|FJ|FJ|L7||FJ|LJ||||||LJLJL-7F---JF-7FJ|F7L-J||F7F-JL-J|FJ|FJ-FLFJ7|7",
            "-JJFF7-F7FL7||LJL-JF7F7F--7FJ||LJ|F-7F7F7|||L---7|L-7FLJF-J||F7F7FJ||F-J|FJ|FJFJLJL7|F-J|||||F----7LJF--7L7|L7LJL7F7|LJ|L----7LJ|LJJ-FLJJF.|",
            "|-J7|.L|L7FJ|L-7F-7|LJLJF7LJ.LJ7FJ|FJ|LJLJ||F7F-J|F7L7F-JF7||||||L7LJL-7|L-JL7L--7-LJ|F7|LJLJL--7FJF7L7FJFJL-JLF7LJ||F7|F----JF7F7|7F7J7LJ-J",
            "|7F|LF-JFJL7|F7LJ|LJFF--JL7F7LF7L-JL-JF---J|||L-7||L-JL-7|||||LJ|FJF---JL-7F7|F7FJF--J||L---7F--JL-JL-JL-JF7F--JL-7|LJ||L7F7F7|LJL-777F77-|7",
            "L-FL-L-7|F-JLJ|F7F--7L-7F7LJL7||F-7F--JF-7|LJL--J|L--7F-J|||||F-JL7L---7F7LJ|LJLJ-L--7||F7F-JL-7F7F7F-----JLJF7F7FJ|F-JL7LJLJLJF--7|JJL7LF-L",
            "|L7|LF7||L7F-7LJ|L7FJF7LJL--7|||L7|L---JFJF-77F7|L7F7||F7||||||F7FJF-7FJ||F7L--7FFF--J|LJ||F7F7LJLJLJF-------JLJLJ-LJF7LL----7FJ|-||J7L|JL..",
            "|--J.|LJL7LJFJF7L7|L-J|LF7F-JLJL-JL-7F7FJ||FJFJL-7||LJ||LJ||||LJ||JL7LJFJ|||F-7L7FJF-7L7FJ||||||F--7FL7F--------77F7FJL-7F--7LJ-|-LJFJ-L|..-",
            "L7JLLL-7FJF7L-JL7|L7F7L7|||F----7F-7LJLJF7||FJF7FJLJF7LJF-J|LJF-JL-7L-7L7||||JL-JL7L7L-JL-J|LJL7|F7L--J|F--7F7F7L-JLJF--J|F-JF--77FF-JF--7F7",
            "7J7.||FJL-JL-7.FJL-J|L7LJ||L7|F-J|7L--7FJLJLJFJ||F-7||F7L-7L-7|F7F7|F-JFJ|||L---7LL-JF-----JF-7LJ|L---7|L-7|||||F-7F7L---JL--JF-JJ-7J-L|-F-J",
            "LF|-LLL-7F7F7|FJF7F7||L-7|L-JFJF7|F---J|F-7F-J.LJ|FJ||||F7|F-JLJ||||L7FJ-||L7F--JF---JF--7F7|FJF-J|F7JLJLFJLJLJLJFJ|L--------7L-7J|JLF7|7.FJ",
            "|FJL|JLLLJLJ|LJFJ||||F--JL---JFJLJL----JL7|L--7.FJL-JLJLJ|LJF7FFJ|LJFJL-7||FJL-7FJF7F7L77LJLJL-JFF-JL----J|F-7F77L7L----7F--7|F-JLJ7FFJ|LFJ|",
            "F|7|F7F|LF7LL--J7LJLJL--7F7F7FJF----7F7F7|L7F-JFJF7F7F---JF7|L7L7|F7L-7FJLJ|F7FJL7||||FJF-------7|F7F-----7|FJ|L77L-7F--J|F-J||7LFFF7|F-LL-|",
            "FL--J--F-J|F-------7F---J||||L7L-7F7LJ||LJ-LJF7|FJLS||F7F7|LJFJL|LJ|F7|L--7||||F-J||||L7L----7F7|||LJF----J|L7|FJF7||L---JL-7|L77-JF|7|LL|7|",
            "|7L|.|LL-7LJF-7F--7LJF7F7|LJL-JF7LJL-7|L-7F7FJLJL-7FJ||||LJF7|F7L--J|LJF7FJLJ|||F7|LJ|FJF--7|||||LJF-JF----JFJ|L-JL7L---7F7FJL7L-7FFLJ.F.|-J",
            "LL--77LLFJF7|-LJF-JF-JLJLJ-F7FFJL-7F-J|F-J|||F----JL-J|LJF-JLJ||.F7|L7FJLJLLFJ|LJLJF7||FJF7L-J||L7FL--JF---7L-JF7F-JF7F7LJLJF7L--JJ||-|-7J7|",
            "F|LLJJF.L7|||F-7L--JF7F7F7FJL7L--7|L-7|L-7|||L-7F7|F7FJF7L7F7FJ|FJL-7||-L7..|FJ|F7-|LJ||FJ|F--J|FJF7|F7L--7L-7FJ|L--JLJL----JL---7.||LF-L7J7",
            "-|F|7-7FL||||L7L----JLJ|||L-7|FF7|L--J|F-J|||F-J|L7|||FJL-J|||FJL7F-JLJ|JF--LJF-J|.L7FJLJFJ|F7FJL-JL-JL---JF7LJFJF-7F---7F----7F-JF7-7J7||.J",
            "F77.JJLJLLJLJ-L-------7||L--JL-JLJF--7LJF7||||F7|FJ|||L-7F7||||F7|L-77FF-7.FLLL-7|F7LJF77L-J||L-7F7F7F----7|L7LL7|FJL--7|L---7|L7.FJF7||-|7J",
            "FLJ7LF-||.F7LF--------JLJF-7F---7FJF-JF-JLJLJLJLJL-J||F-J|||LJLJLJF7L--JFJ7F|.F-JLJ|7FJL7F7FJL-7||LJ||F---J|FJF7LJL-7F-J|F7F-J|FJ.|L7-J||LJ.",
            "LJFJ|.FF-FJL-JF-7F7F7F-7FJFJ|F--J|LL7FJF--7F--7F---7LJL--JLJF7F7F-JL7F--JF77.FL-7F7L7L7FJ||L-7FJLJF7LJL--7FJL-JL---7|L-7LJ|L-7||F--FJ|FF7-|7",
            "JFF.L-7J7L----JFJ|||LJFJL7L-JL7F7|F-J|FJF7|L-7|L--7L7F---7F7|||LJ||FJL---JL77F7-LJL7L-J|FJL--J|F7FJ|F----JL7F------J|F7L-7L--JLJ-7.J---.-7L7",
            "J7J7.|J7FF-----JFJLJF7L7FJ.F7|LJLJL--JL7||L--JL---JL|L--7|||||L-7F7|F-7F---J-JF.JLLL--7||F---7LJ|L7|L--7F--JL-----7LLJL--JF--7LL7F--7-|7|||L",
            "||L7-LJ|7L------JF--JL7LJF-J|F--------7LJL7|F7F--7F7L---JLJ|||F7LJ|LJJLJ.JLL|-JJ7JF7F7||LJLF-JF7L-JL7F7LJF---7F---JF---7F-JF7||||JLFFJ|LFF-J",
            "FFLJF|FF--------7L---7|F7L-7|L-----7F7L7F-JFJLJF-J||LF---7.LJLJL-7|LL7|.7J|FF-F|7.|||||||F7L-7|L7F-7LJ|F7L--7LJF---JF-7LJF7|LJ7F|-FJJFL7LL.L",
            "|J|FL7FL-----7F7L7F--JLJL--JL------J|L7|L--JF-7L--JL-JF-7L------7LJ|7F-7L.LFJF----JLJLJL7||F7LJJLJJL-7LJL---JF7|F---J|L7FJLJJLJF|-|L||7J.|FL",
            "||F|JL7J|F-7FLJL7|L-7F--------------JFJ|F-7FJLL7F-7F--JLL-----7FJJFL-FJ77F-L-|F--------7||||L7F7F----JF------JLJL--7F-7|L---7|||L7|--|--7-L.",
            "F-J|7JJ.FL7L7F--J|F7||F--------------J|LJJ|L--7LJ-|L---7F----7||7F|FL|J-L77|FLJF-------JLJ||FJ||L---7FJF-----------J|FJL---7L--7L-J7FJ|J|..J",
            "|.|-J7...LL7LJF-7LJ|LJL---7F----------7F-7|F-7L--7L7F-7LJF7F-J|L7F-7F|-7-7LL-F-JF7F7F-7F-7LJL-JL--7LLJFJF7F7F----7FFJL----7L---J|..77--777-L",
            "FF-.|7FJ7JLL7FJ-|F7L7FF---J|F7F7F-7F--J|FJLJ-L-7FJ|||FJF7||L-7L7LJFJ-F7L7|-.|L-7|LJLJJLJ.L-7F-7F-7|F7-L-JLJLJF7F7L-JF-----JF7F7F|F--J-JL-F7.",
            "F7.F7FJ|7.FF||F-J|L7L-JF--7||LJ|L7|L---J|F7F7F7LJF7LJL7|||L--J.|F-J7|J|FLJJ7F--J|J|FF------J|FJ|FJLJ|F7LF-7F7|LJL---JF7F7F7|||L-77.|-|L7.|L7",
            "FF7.L|.LLF--J|L7FJLL-7FJF-J||F7L-JL----7LJLJLJL-7|L--7|||L---7-||J-LJJF|JJLFL---J.F7L7F--7F7|L-JL--7LJL7L7LJ||F7F7F7FJLJLJLJLJF7L77JF|||.|FJ",
            "F..F-|-|LL-7FJLLJFF--JL7|F7|LJL7F7F---7|F----7F-J|F-7||||F---JLLJ||.FFF7F7.F-7LF7FJL-J|F7LJLJF7LF--JF-7L-JF7|LJLJLJLJF--------JL-JL|-J-F-F||",
            "|-F7F|-JFF7LJF7F-7L----JLJLJLF7LJLJF--J|L---7|L--J|-LJLJ||F-7J.|.FJFF7|LJL7L7|.|||F-7FJ|L--7FJ|FJF-7|FJF--J||F------7L--7F7LF7JL|-F7-|-7.LJJ",
            "|..|7|7F7|L7|||L7|F-------7F-JL----JF7FL----J|F7F7L7F7F7LJ|FJ|.L7|LFJ|L--7|FJL7||||FJL7L--7|L7|L-JFLJL7|F--JLJF7F--7L--7LJL-JL7FF77|.L.L.|..",
            "J7----F-7L7|FJL-JLJF-----7LJF7F-----JL-7F-7F7LJLJL-J|LJL--JL--7.LFFL7L7F7|LJF7|||LJL-7|F--JL7||F7|F7F7LJL7F77FJ||F-JF-7L7F7F--JFJL-7-|7L----",
            ".F-|7JL7L-JLJF7F7F-JF----JF7||L7F------JL7LJL-7F7.F-JF--------JFF.LLL7LJ||F-JLJ|L7LF7LJ|F7F7LJLJL7|LJ|F-7||L-JFJ|L-7L7L7LJ|L-7LL7F-JF-7FLJ-L",
            "JLFJ||.|F7F7FJLJ|L-7L7F7F7|||L-JL7F7F----JF---J|L7|F-JF------7F7F77-FJF7LJ|F7F7|FJL|L-7LJLJ|F7F-7|L7FJL7|LJF-7L-JF-JFJFJF7L--JF-JL--JFJ-.77.",
            "FJJFF7FLJLJLJ.F7L--J7LJLJ||LJF7F7LJLJF----JF7F7L7LJL--JF--7F7||LJL7FJFJ|F-J|||||L-7L7FJF-7FJ|||FJL-JL--JL--JLL7F7|F-JFJFJL---7L7F----J.|.L|7",
            "F|F|L|7J|-LF--J|LF7F7F-7FLJF7|LJL----JF---7|LJ|FJF-7F7FJF7LJ|||F--JL7|FJ|F7|LJLJF-JFJL7L7|L-JLJL------7F----7FJ||LJF-JFJF----JFJL----7F--FF-",
            ".77L-F--J7LL--7L-JLJ||FJF77||L------7FJF7FJ|F-J|FJ.|||L7|L7|LJ||F7||LJL7LJ||F--7L-7L-7L-J|F------7F7F7LJF---JL7||F7L-7L7L-----JF-----JJJ-L.|",
            ".L7JJ|.FJF|F7FJF---7LJL-JL-JL-------J|FJ|L-JL7J||F7LJL7|L7L7LFJLJ|F7.F7|F-J||F7L--JF7L--7||F----7||LJL77L----7|||||F7|FJF-----7L-----7J|7.F|",
            "FL-7-7-J7F7|||FJF7FJF----7F--7F------J|LL7F-7L-JLJL--7||LL7L7L7F-J||FJLJL7||LJL-7F7||F--J|LJF---JLJF-7L------J||LJLJ|LJFJF7F7.L7F-7F7|.L---J",
            "L7LJ.|.|F|LJLJ|FJLJFJF---J|F-J|F7F7F-7L-7LJFJF7F7F---JLJF7L7L-JL-7||L---7L7L7F--J||||L7F7L--JF----7L7L7F-----7|L--7FJF7L-JLJL7|LJFLJ|L7JFLF|",
            "|JF-FFL|7L----JL-7FJFJF7F-JL--J|||LJ|L--JF7L-JLJ||F---7FJ|FJF----J||F--7|FJL|L-7FJ|||JLJL---7L7F--JFJFJ|F----JL---JL-JL7F7F-7L-7F7F7L-J77F||",
            "F7L-J|.FF7F7LF---J||L-J||F-----JLJF--7F--JL----7|||F--JL7|L7L---7F||L-7LJL7FJF-JL7LJ|F7F----JFJL7F-JLL-JL--7F7F--7F---7||LJ7|F7LJLJL-7-7||-|",
            "FL--L|-||LJL-JF---JF7F-J|L7F------JF7LJF7F7F---JLJ||F-7FJ|FJF--7|FJL7FJF-7||FJ-F7L-7LJ|L7F-7FJF7||F-7|F----J||L7FJ|F--J|L7F7LJ|F-7F7FJJJ7|F7",
            "|7J|L7.FL-----J.F7FJLJF7L7LJF------JL--JLJLJF----7|LJFJL7|L7|F7LJL7FJL-JFJLJ|F7||F7|F7|FJ|FJ|FJLJ|L7L-JF-7F7||FJL-J|F-7L7LJL-7|L7LJ||JJFFF7J",
            "-77LF---.LF7F---JLJF7FJL-JF7L---------------JF7F-J|F-JF7||L||||F7FJL7F-7L--7||||LJLJ|LJL-JL-JL-7FJ|L7F7|FJ|||LJF--7LJFJL|F7F-JL7L7FJ|J-LJL||",
            ".LJ7|.LLJJ|LJF---7FJLJF---JL--------7F7F7F7F-JLJF-J|F7|||L-JLJ||||F-JL7|.F7|LJLJF7F-JF-7F-7F---J|F-7LJLJL-JLJF7L-7L7FJF7LJLJF-7L-JL-J.L|--J7",
            ".|-F-7F-J-L-7|F--J|F7.L--7F----7F7F7LJLJ|||L---7L-7|||||L--7F7LJLJL7F7||FJLJF---JLJF7|FJL7|L---7|L7L-7-F7F---JL-7L7|L-JL---7L7L-777-J--|J.F|",
            "FLF||JLJJFFFJ|L---J||F7F7LJF--7LJLJL---7LJL7F7FJF7|||LJ|F-7LJ|F----J|||||F-7|7F7F7L||||F7||F-7FLJFJF-JFJLJF7F7F7L-JL---7F--JJ|F-JF7FJ7||7FF|",
            "J|LLJ.7JF7FL-JFF--7||||||F7L-7|F7F7F--7L--7LJLJ|||||L-7||FJF-JL---7J||||LJFJL7|LJ|FJ||LJ|||L7|F7FJFJF7L---JLJLJL----7F7|L--7FJL7FJL7.|7LJ--J",
            "L7-LJ.J.-|-LF-7L-7LJLJLJLJL--JLJLJ|L-7|F--JF-7-FJ||L7||LJL7L-7F---JFJLJ|LFJF7LJF-J|FJ|F-J||FJ||LJFJ-||.F7F7F7F7F----J||L---JL-7LJF7|-JJ.LJJ.",
            "FJL|-|.FL|7LL7|F7L--------7F7F7F-7L--JLJ|F7L7|FJFJ|FJFJF--JF7|L---7L--7L7L-J|F-JF7|L-JL-7||L7|L-7|F7||FJLJLJ||LJF7F--JL---7F7-|F-JLJ7F.F7|LF",
            "JJ-7-JF-7L77FJLJ|F-------7LJLJLJ7L-7F7LF7|L7|||FJFJL7L7L7F-J||F---JF7LL7L7LFJL--J|L-7F--J||FJL7L||||||L---7FJL--J|L7F-----J||FJL7F7|F77-F-.|",
            "|F-|.LL7|-J7L--7LJF7F---7L---7F---7LJ|FJLJFJ|LJL7L-7|F|FJ|F-J|L-7F7||F7|FJFJF----JF7|L7FFJ||F-JFJLJ|||F7F-JL7F7F7L-JL-7|F7FJLJF7LJ|FJ|J-7.F7",
            "F|-|-.F--JLFF--JF7|LJ.F7L---7|L-7FJF-JL-7FJ7L--7|F7||FJL7|L7FJF-J|||||LJL7|FJF7FF7||L7L7L7|||F7L7F-J||||L7F7LJLJL-----JFJLJF7FJ|F7LJFJJ7L--7",
            "F|7|.-F7L7LLL---JLJJF7||F---JL--JL-JF7F7||F-7F7|LJ||||F-J|FJL7L7FJ|||L--7||L7||FJ|||.L7L7||||||FJ|F7|||L7LJL----7F7F7F-JF--JLJJ|||F7L7L77|.L",
            "-JL-JL|FF7.-F-------JLJLJF----7F---7|||||||FJ||L-7|||||F7||F7|FJL7|||F7F||L7||||FJ||F7|FJ||||||L7||||||FJF-7F--7LJLJLJF7L---7F7LJLJL-J.F---|",
            "|7L|.FJJL7FFL----7F-7F-7FJF---J|F--J|LJ||||L7|L7FJ||LJ||||||||L7FJ||||L7|L7|||LJ|FJ||||L7|||||L7|LJ||||L7L7|L-7|F-7F-7||F7F7LJL7F-7JL|.L-JJJ",
            "F|F-J-|LF7-F-7F--J|FJ|FJ||L---7||F-7L-7LJ|L7||FJ|FJL-7LJ||||||FJL7LJ|||LJFJ|LJF-JL7LJ|L7||||LJFJL-7||||FJFJL--J|L7||FJ|||LJ|F-7LJFJJFL-7.|-F",
            "|LJJJ|F-JL-JFJL-7FJL7|L7|F7F--J|LJFJF7L7FJFJ||L-JL7F7L7FJ|||||L-7|F7|L7F-JFJF7L7F7L7F|FJ|||L7FJF-7|||||L7L----7L-JLJL7||L-7||FJF-JJFJJL-7|F7",
            "L7||.-L----7L--7LJF-J|-LJ|LJF-7|F-JFJL7|L7|FJL-7F-J|L7|L7|LJLJF-JLJ|L7||F7L7|L-J|L7L7|L7||L7|L7L7||||||FJF-7F7L--7F7FJ||F-JLJL7|J|FLJ7-LJLLJ",
            "LJF-7-J.FJ.L--7L-7L-7|F--JF7|FJ|L-7|F7LJFJ|L7F-J|F7L7LJ7LJLF--JF7F7L7||LJL-JL--7|-L7||FJ|L7LJL|FJ||LJLJL7L7|||F7FJ||L-J|L--7FL|||F7JFL7F.7F-",
            "FLJ-J7JL7LJ.F-JF7|F7LJL---JLJL7|F7|LJL-7L7|J|L7FJ|L7L--7-F-JF7FJLJ|FJLJF7F-----JL-7LJ||J|FJF7FJL7LJF----JFJ||LJ|L-JL--7L7F7L7LLJ-JFF7.FJL|J7",
            "-7.FJL7.J-JFL--J|LJL--7F--7F7|LJ|||F--7L7|L7L7|L7L7L7F7|FJF7||L7F-JL--7|LJF-7F7F7FJF-J|FJ|FJLJF7L-7L7F--7L7|L7.L----7FJFLJL-JL|JLL7JFL77-LJ|",
            "F-J|F7L7.J.LL-F7L----7|L7FJ||F--JLJL-7|FJ|FJFJ|FJFJJ||||L-J|||FJL7F7F7||F7|FJ|||||JL-7|L7|L7F7||F7|FJ|F-JFJL-JF7JF-7|L------7.|.-J.FJ|J.F|-J",
            "|7.LLJ7||7F7-FJL-----JL-JL-JLJF---7F-J|||||FL7|L7L7FJ||L7F-J||L7FJ|||LJLJLJL7|LJ|L--7LJFJL7||||LJ||L7|L-7|F---JL-JFJ|F7F7F--J7FJ--||J|7.F-J7",
            "L-L..LJ|L7JL-L---7F7F-7F-7F7F7|F--JL-7|L7|L-7|L7|FJL7||FJL-7||FJL-JLJF7JF7F-JL7FJF-7L-7|F7||||L77|||LJF7|LJF-7F7F-J.LJLJ||J..|7J.|F-.77-||-J",
            "F|L7-LFJ--7FF---7LJ||FLJFJ|||||L-7F7FJ|FJ|F7|L7||L7J||LJF--J|LJF7FF7FJL-JLJF7FJ|FJLL7FJLJ||||L7L7||F--JLJF-J-LJ|L------7|L77-L|.F.JL|||FJ.|7",
            "77.|F7LL-||LL-7FJF7LJF7FJFJ|||L7JLJ||LLJ7LJ||FJ|L7|FJL7FJF-7L--J|FJLJF7F-7FJ||FJL-7FJL--7||||FJFJLJL7F7F7L---7FJF7F----JL7L-7-J-|.LJLJ7-.F-7",
            "||.|J7.FF77|F-JL-JL--J|L7L7|||FJF--JL-----7LJ|FJFJ||F-J|FJ|L-7F-JL7F-J||FJL7|||F7FJL-7F-JLJ||L7|F7F7||||L7F7FJ|FJ|L----7LL--J7JFL-|LJ.LJ7|-|",
            "L-7L-|.7.L--L-7F7F7F-7L-JFJ|||L7L--7F--7F7|F-J|LL7|LJF-J|F77FJL--7LJ.FJ|L7FJLJLJ|L-7J|L--7FJ|FJLJLJ|||LJ.||LJ-LJJ|F---7|JL|J-7-FJFJ7L|.FL--|",
            "|L|J|J..7.|7F-J|LJ||7L7F7L7|||FJ|F-JL7FJ|LJ|F7L-7|L-7L-7LJL7L7F-7L7F-JFJFJL7F7F-JF-JFJF7FJL7||F7F7FJ||F--JL7F---7|L--7LJL||.7LF|J|JL.FL7JJF7",
            "|-|.--.F-J7FJF-JF-J|F-J|L7|||LJF-JF7FJL7L-7LJL7FJ|F-J.FJF-7L7LJ.L7||F7|FJF7||LJF7|F7L-J||F7||LJ||||FJ||F7F7LJF--JL---J|F77J|7LL7.F7|7JL|-JL|",
            "JFL7.JFJJL-L7L7FJF7|L--JFJ|||F-JF7||L-7|F7L--7|L7LJF--JFJFJFJ-F--J|LJ||L7|LJL-7||LJL--7|LJ|||F-J|||L-JLJLJ|F7L7F7F--7F-J|77.|7.J-|-7F7J.|-J|",
            "L7LL|L-J-7LFJFJ|FJLJF---JFJ||L-7||||F-J||L7F7|L7L-7L--7L7L7|F7|F7FJF7||F|L7F--J||F7F7FJL7FJLJ|F7|||F-----7||L7LJLJF7LJF-JLLF7L7.||7L7..L||.|",
            ".|.F7F|LF|FL-J7||77-L7F7FJ7LJLFJ||||L7L||-||LJ|L7FJF7FJFJF|LJ|||||||LJL7|FJ|F7FJ|||||||J|L--7||||LJL----7|||FJF7F-J|F-JJL7FLL.FFFJJLLL7.-7FJ",
            "FF77LF--F7-L|JF||F7F|LJ||FF7F-JFJ||L7L7||FJL-7F-J|FJ|L7L-7L--J||||FJF7FJ||FJ|||FLJ|||L7FJF-7|||LJF7F----JLJ|L7||L-7|L7J7LFF|L7J|L|7.-JJF|--7",
            "LLJJ-J|LFJ7F7|LLJ||F7F-JL-J|L-7L7|L7L7|||L7F-JL7FJL7L-JF7L--7-||||L7||L7LJL7|LJF7.|||FJ|FJJLJLJF-JLJF-7F7F7L7||L7FJL7|.L-7.J...JF||7LF--.||.",
            "F|7|JF7.LFF-L|-F-JLJ|L7F7F7|F-JFJ|FJF||LJFJL7F7||F7|F7FJL7F7|FJ|LJF||L-JF--JL--J|FJ|LJF|L-7F--7|F7F-JFJ|LJ|FJ|L7LJLFJL-7J|-|-.FLF-7-7L7JF-.F",
            "FL|L-7L77F|7.LJL---7L-J|||LJL7FJ-||-FJ|F-JF7LJ|LJ||LJ|L7FJ|||L7||F7|L--7L--7F--7|L7|JF7|F-JL-7LJ||L7-|FJF-J|7L7|F7.L-7FJ-L.|JFJ-|FJ.L7L.F.FJ",
            "F7L7L7.L-L.F|JJF|LFJF7FJ|L7LFJL-7||FJFJL-7|L7FJF-J|F-JFJL7|LJFJL-J|L7F-JF--JL-7||L|L7|||L7F7FJF7|L7L7||FJF7L-7|LJL7J-LJ7|F-L.F|FJ7-L7F|7.F|F",
            "LL.|JL7|L|7LJJ||LLL7||L7|FJ.L7F-J||L7|-F-JL7||JL-7LJF7L-7|L-7|F7F7|FJL-7L-7F7FJ|L7L7LJ||FJ|LJFJLJFJFJ|||FJ|F7|L-7FJ--|.7|F-J-JFJLF---LFJ7FJJ",
            "FJ7|FLF7|FL7LFFJ.F.LJ|FJLJJFFJL-7LJ|LJ-L-7FJLJF--JF7|L7FJ|F7||||||||F--JF-J||L7|FJFJF7|||FJF7L-7LL-JFJ||L7LJ|L-7|L7J.77J|7|.LF7J.L.LFLJ.FJL7",
            "|JLF7FFJJ--7F-|-..FLFJ|J...FJF--J|L7.LF-FJ|.LFJF--J||FJ|FJ|LJ||LJLJ|L-7FJF-J|FJ|L7|FJ|||||FJL-7|.F||L7|L-J|FJF7|L7L--77-|-FJ-FL7|J7.7|..|F7F",
            "|L|.|L7JJF|LF7J-J7|LL-JFJ-.|FJFJLL7L7-LFL7|.FL7|F--J||FJL7|-L|||J-LL7FJL7|F-JL7L7||L7||||LJJ.LLJ-FJ|LLJ7F--JFJ|L7L7F-JJ||L|L-|F|-.77F--.7.|7",
            "L7LFF-FJF-F7L|7LLJJ7LJJ|JFFLJJ|-LFLJJJ7L|LJJLFLJL7F7|LJJFLJ||LJ7.FLFJ|FLLJ|F7FJ-|||FJ||LJ|LL7|L7|L|J.F--JF-7|FJFJ||L-7F|.FL7.F-.J.LJJF|-J.||",
            "FF7.JJL-J7|7L-J7J7F||.F|.-F.|FJF--|77-LL7J|.FFF--J|||F7|F|7LJ-||F-7L-JFF--J||L-7|||L7LJ.|||.|7|-LFL.FJF--J.||L7L7FJF-J7J.|JLF--7.|L|.F|.FL|7",
            "----L7J7LFL77.L7-JJ|77FFJ-F7|FLJ7LF-L7|JL7L--F|F7FJ|LJL---7JLFLJJLL7F--JF7FJL7FJLJL-J7|7LF7F|JL-7|FFL-JJ7LL||L|FJ|FJJ|L7.7.F|7JJ-F--LJ.FFJLL",
            "L7L-7J-7F-7FL--JJ.LF.FFL-|L-|-JF7LLLJ|L--JJFFJLJLJ-|F-7F7FJ.L-7J|LLLL---J||7L||LL7F|J|LLJL|----L|||J7J-LJ.L||.|L7|||FFJFJ-|-7-----77L77|...|",
            "|F7JLJJF|--|||.J-7-J--7JFJ7.-7LJ77||-LFJ-7--||L|J7.|L7LJ||--J|F.|..|L|JF|LJ7L||-7LLJF7-L77L-J-77F|||7JFFFJ7LJ-L-JLJF-|F|F||..|-..F-7-||L----",
            "FJ||LLF-77.-|LF|-L.||LL.7JFL.F-L-7-L-.L.F-FJ.FFJJF-L-JJFJL7JFFL-|-L-7|.F7FJJ-LJ7L77.L-F-LL7JF--||LJJ--F-7L|JLLL-FJJF-|-F|-.-LL-77|.7.J-|..||",
            "FL--7.LLJ-JL7-FFJLF-J-|-F-7J-F.L|.|LJ---L-L7.-FJJL7JL..L--J-|L|.7--L|L-FJ|JJ-L.L-L-F-L|-LL7-JJ.LLFJ-|LLJLJ.JJLJ77J--.-.LL--JLJL|-LLJ7JL-J.-7"
        };

        public static void Part1()
        {
            PipeMap testMap = new PipeMap(testInput);
            if (testMap.longestPathCount != 8)
            {
                Console.WriteLine("Part1 test failed: " + testMap.longestPathCount);
                return;
            }
            Console.WriteLine("Part1 test passed");

            PipeMap actualMap = new PipeMap(actualInput);
            Console.WriteLine("Part1 max path length: " + actualMap.longestPathCount);
        }

        public static void Part2()
        {
            PipeMap testMap = new PipeMap(testInput2);
            if (testMap.enclosedTileCount != 10)
            {
                Console.WriteLine("Part2 test failed: " + testMap.enclosedTileCount);
                //testMap.PrintMap();
                return;
            }
            Console.WriteLine("Part2 test passed");

            // 670 is too high
            // 583 is too high
            PipeMap actualMap = new PipeMap(actualInput);
            Console.WriteLine("Part2 enclosed count: " + actualMap.enclosedTileCount);
        }
    }
}
