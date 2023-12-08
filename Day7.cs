﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventOfCode2023
{
    enum HandType
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        FullHouse = 4,
        FourOfAKind = 5,
        FiveOfAKind = 6
    }

    class Hand
    {
        public char[] cards;
        public char[] cardsOrdered;

        public Hand()
        {
            cards = new char[5];
            cardsOrdered = new char[5];
        }
        public Hand(String cards)
        {
            this.cards = new char[5];
            this.cards[0] = cards[0];
            this.cards[1] = cards[1];
            this.cards[2] = cards[2];
            this.cards[3] = cards[3];
            this.cards[4] = cards[4];

            this.cardsOrdered = new char[5];
            this.cardsOrdered[0] = cards[0];
            this.cardsOrdered[1] = cards[1];
            this.cardsOrdered[2] = cards[2];
            this.cardsOrdered[3] = cards[3];
            this.cardsOrdered[4] = cards[4];
            Array.Sort(this.cardsOrdered);
        }

        bool IsFiveOfAKind()
        {
            return cards[0] == cards[1] &&
                cards[0] == cards[2] &&
                cards[0] == cards[3] &&
                cards[0] == cards[4];
        }
        bool IsFourOfAKind()
        {
            int maxSameCount = 0;
            for (int i = 0; i < cardsOrdered.Length; i++)
            {
                char whichCard = cardsOrdered[i];
                int sameCount = 1;
                for (int j = 0; j < cardsOrdered.Length; ++j)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (cardsOrdered[j] == whichCard)
                    {
                        sameCount++;
                    }
                }
                if (sameCount > maxSameCount)
                {
                    maxSameCount = sameCount;
                    if (maxSameCount >= 4)
                    {
                        break;
                    }
                }
            }
            return maxSameCount >= 4;
        }

        bool IsFullHouse()
        {
            bool has3OfAKind = false;
            int threeIdx0 = 0, threeIdx1 = 0, threeIdx2 = 0;
            for (int i = 0; i <= 2; ++i)
            {
                if (cardsOrdered[i] == cardsOrdered[i+1] &&
                    cardsOrdered[i] == cardsOrdered[i+2])
                {
                    threeIdx0 = i;
                    threeIdx1 = i+1;
                    threeIdx2 = i+2;
                    has3OfAKind = true;
                    break;
                }
            }
            if (!has3OfAKind)
            {
                return false;
            }

            bool has2OfAKind = false;
            for (int i = 0; i <= 3; ++i)
            {
                if (i == threeIdx0 || i == threeIdx1 || i == threeIdx2)
                {
                    continue;
                }
                if (cardsOrdered[i] == cardsOrdered[i+1])
                {
                    has2OfAKind = true;
                    break;
                }
            }
            return has2OfAKind;
        }

        bool IsThreeOfAKind()
        {
            bool has3OfAKind = false;
            int threeIdx0 = 0, threeIdx1 = 0, threeIdx2 = 0;
            for (int i = 0; i <= 2; ++i)
            {
                if (cardsOrdered[i] == cardsOrdered[i + 1] &&
                    cardsOrdered[i] == cardsOrdered[i + 2])
                {
                    threeIdx0 = i;
                    threeIdx0 = i+1;
                    threeIdx0 = i+2;
                    has3OfAKind = true;
                    break;
                }
            }
            if (!has3OfAKind)
            {
                return false;
            }
            return true;
        }

        bool IsTwoPair()
        {
            bool hasFirstPair = false;
            int pair1Idx0 = 0, pair1Idx1 = 0;
            char firstPairCard = ' ';
            char secondPairCard = ' ';
            for (int i=0; i<=3; ++i)
            {
                if (cardsOrdered[i] == cardsOrdered[i+1])
                {
                    hasFirstPair = true;
                    pair1Idx0 = i;
                    pair1Idx1 = i + 1;
                    firstPairCard = cardsOrdered[i];
                    break;
                }
            }
            if (!hasFirstPair)
            {
                return false;
            }
            bool hasSecondPair = false;
            int pair2Idx0 = 0, pair2Idx1 = 0;
            for (int i = 0; i <= 3; ++i)
            {
                if (i == pair1Idx0 || i == pair1Idx1)
                {
                    continue;
                }
                if (cardsOrdered[i] == cardsOrdered[i + 1])
                {
                    hasSecondPair = true;
                    pair2Idx0 = i;
                    pair2Idx1 = i + 1;
                    secondPairCard = cards[i];
                    break;
                }
            }
            if (!hasSecondPair)
            {
                return false;
            }
            return true;
        }

        bool IsOnePair()
        {
            bool hasPair = false;
            for (int i = 0; i <= 3; ++i)
            {
                if (cardsOrdered[i] == cardsOrdered[i + 1])
                {
                    hasPair = true;
                    break;
                }
            }
            if (!hasPair)
            {
                return false;
            }
            return true;
        }

        bool IsHighCard()
        {
            return true;
        }

        public HandType GetHandType()
        {
            if (IsFiveOfAKind()) return HandType.FiveOfAKind;
            if (IsFourOfAKind()) return HandType.FourOfAKind;
            if (IsFullHouse()) return HandType.FullHouse;
            if (IsThreeOfAKind()) return HandType.ThreeOfAKind;
            if (IsTwoPair()) return HandType.TwoPair;
            if (IsOnePair()) return HandType.OnePair;
            return HandType.HighCard;
        }

        public HandType GetHandTypePart2()
        {
            HandType maxType = HandType.HighCard;
            char[] valsToTest = {
                'A',
                'K',
                'Q',
                'J',
                'T',
                '9',
                '8',
                '7',
                '6',
                '5',
                '4',
                '3',
                '2'
            }; // DO test joker

            // Find indexes to test
            List<int> jIdxs = new List<int>();
            for (int i = 0; i < 5; ++i)
            {
                if (cards[i] == 'J')
                {
                    jIdxs.Add(i);
                }
            }

            // test each possible combination
            if (jIdxs.Count == 5)
            {
                return HandType.FiveOfAKind;
            }
            else if (jIdxs.Count == 4)
            {
                return HandType.FiveOfAKind;
            }
            else if (jIdxs.Count == 3)
            {
                foreach (char val1 in valsToTest)
                {
                    foreach (char val2 in valsToTest)
                    {
                        foreach (char val3 in valsToTest)
                        {
                            Hand testHand = new Hand();
                            testHand.cards[0] = cards[0];
                            testHand.cards[1] = cards[1];
                            testHand.cards[2] = cards[2];
                            testHand.cards[3] = cards[3];
                            testHand.cards[4] = cards[4];

                            testHand.cards[jIdxs[0]] = val1;
                            testHand.cards[jIdxs[1]] = val2;
                            testHand.cards[jIdxs[2]] = val3;

                            testHand.cardsOrdered[0] = testHand.cards[0];
                            testHand.cardsOrdered[1] = testHand.cards[1];
                            testHand.cardsOrdered[2] = testHand.cards[2];
                            testHand.cardsOrdered[3] = testHand.cards[3];
                            testHand.cardsOrdered[4] = testHand.cards[4];

                            Array.Sort(testHand.cardsOrdered);

                            HandType testType = testHand.GetHandType();
                            if (testType > maxType)
                            {
                                maxType = testType;
                            }
                        }
                    }
                }
                return maxType;
            }
            else if (jIdxs.Count == 2)
            {
                foreach (char val1 in valsToTest)
                {
                    foreach (char val2 in valsToTest)
                    {
                        Hand testHand = new Hand();
                        testHand.cards[0] = cards[0];
                        testHand.cards[1] = cards[1];
                        testHand.cards[2] = cards[2];
                        testHand.cards[3] = cards[3];
                        testHand.cards[4] = cards[4];

                        testHand.cards[jIdxs[0]] = val1;
                        testHand.cards[jIdxs[1]] = val2;

                        testHand.cardsOrdered[0] = testHand.cards[0];
                        testHand.cardsOrdered[1] = testHand.cards[1];
                        testHand.cardsOrdered[2] = testHand.cards[2];
                        testHand.cardsOrdered[3] = testHand.cards[3];
                        testHand.cardsOrdered[4] = testHand.cards[4];

                        Array.Sort(testHand.cardsOrdered);

                        HandType testType = testHand.GetHandType();
                        if (testType > maxType)
                        {
                            maxType = testType;
                        }
                    }
                }
                return maxType;
            }
            else if (jIdxs.Count == 1)
            {
                foreach (char val1 in valsToTest)
                {
                    Hand testHand = new Hand();
                    testHand.cards[0] = cards[0];
                    testHand.cards[1] = cards[1];
                    testHand.cards[2] = cards[2];
                    testHand.cards[3] = cards[3];
                    testHand.cards[4] = cards[4];

                    testHand.cards[jIdxs[0]] = val1;

                    testHand.cardsOrdered[0] = testHand.cards[0];
                    testHand.cardsOrdered[1] = testHand.cards[1];
                    testHand.cardsOrdered[2] = testHand.cards[2];
                    testHand.cardsOrdered[3] = testHand.cards[3];
                    testHand.cardsOrdered[4] = testHand.cards[4];

                    Array.Sort(testHand.cardsOrdered);

                    HandType testType = testHand.GetHandType();
                    if (testType > maxType)
                    {
                        maxType = testType;
                    }
                }
                return maxType;
            }
            // no 'J', regular type determination
            return GetHandType();
        }
    }

    class HandBid
    {
        public Hand hand;
        public int bid;

        public HandBid(Hand hand, int bid)
        {
            this.hand = hand;
            this.bid = bid;
        }

        public HandBid(String line)
        {
            String[] words = line.Split();
            this.hand = new Hand(words[0]);
            this.bid = Int32.Parse(words[1]);
        }
    }

    class Day7
    {
        static String[] testInput =
        {
            "32T3K 765",
            "T55J5 684",
            "KK677 28",
            "KTJJT 220",
            "QQQJA 483"
        };

        static String[] actualInput =
        {
            "239A8 171",
            "8J456 629",
            "QKJ7Q 687",
            "67885 526",
            "24JT3 993",
            "63K64 692",
            "28K88 46",
            "KKK35 493",
            "AAAA8 78",
            "4K43T 199",
            "22A2J 212",
            "3J949 943",
            "96JQJ 546",
            "62666 243",
            "637J9 860",
            "9K9K9 632",
            "99494 375",
            "KKK8K 22",
            "AA8A8 780",
            "QQ422 715",
            "Q8QQQ 657",
            "9J955 241",
            "56Q55 194",
            "2KKQ4 69",
            "55552 956",
            "73TT7 808",
            "62T26 930",
            "44JAA 149",
            "85888 400",
            "T5KTK 854",
            "572K7 360",
            "22439 154",
            "7K777 746",
            "38333 90",
            "K8ATT 63",
            "5J4TJ 361",
            "9QJ33 97",
            "Q642J 487",
            "KTT5Q 173",
            "58365 936",
            "666J6 355",
            "55JK2 158",
            "KA572 33",
            "99899 952",
            "78T2T 913",
            "49429 123",
            "T755T 733",
            "J8767 308",
            "AAJTT 751",
            "44944 866",
            "2A53J 785",
            "K3434 914",
            "2KAK2 828",
            "J8279 658",
            "K7A32 162",
            "78962 844",
            "536AK 263",
            "J555J 777",
            "8888J 385",
            "94A4J 369",
            "82Q74 953",
            "K3KKK 17",
            "49266 756",
            "A7378 817",
            "44QT4 9",
            "99492 246",
            "A8484 329",
            "TT36A 972",
            "22Q2J 598",
            "J6K66 610",
            "4K73T 309",
            "6766K 709",
            "68QJ5 645",
            "J9599 183",
            "472Q6 395",
            "4JJ24 719",
            "3333K 429",
            "85587 743",
            "23323 579",
            "77755 297",
            "QQJT8 482",
            "53533 465",
            "6844T 980",
            "43596 845",
            "T2T22 731",
            "44KJK 504",
            "3TTQT 409",
            "5J554 410",
            "444Q4 328",
            "7JK77 153",
            "222QQ 111",
            "K4443 732",
            "54445 608",
            "76659 267",
            "25352 615",
            "3KKT3 262",
            "A22J9 694",
            "4JAT9 961",
            "TAAJA 282",
            "4J5T8 900",
            "87787 593",
            "KQ77K 318",
            "4TT49 182",
            "5K3Q2 962",
            "33K55 368",
            "T8297 839",
            "5T6AA 795",
            "37399 423",
            "5T5J5 466",
            "55565 664",
            "3Q233 807",
            "2885J 559",
            "Q4844 989",
            "84Q7T 873",
            "33273 675",
            "3Q943 317",
            "28868 740",
            "998J9 583",
            "Q5Q4Q 48",
            "444AT 320",
            "4JQJQ 578",
            "488Q8 392",
            "J4385 990",
            "TTTT5 75",
            "3K336 191",
            "J88AA 662",
            "Q66K6 278",
            "J9JJJ 543",
            "2AA2A 29",
            "Q7722 23",
            "892JT 393",
            "KKQJQ 449",
            "K68J7 569",
            "A3QJQ 450",
            "TKAKK 889",
            "266J4 932",
            "9JJ7A 757",
            "67667 144",
            "KTKA7 238",
            "QQ2Q2 342",
            "Q5T4T 890",
            "QQQ47 676",
            "88TTT 555",
            "QQ87Q 275",
            "J3T74 566",
            "84848 222",
            "J2J6Q 304",
            "9JTJ4 20",
            "4AAAA 706",
            "KKKK4 505",
            "56585 225",
            "TT5AA 745",
            "86KAQ 6",
            "A3T74 383",
            "K44K4 867",
            "44774 905",
            "84847 159",
            "Q446Q 378",
            "QT7TA 765",
            "63J3Q 671",
            "Q2Q7Q 653",
            "4J466 187",
            "4J554 782",
            "5224Q 942",
            "492A3 477",
            "4K84Q 335",
            "QJ4J7 659",
            "35232 14",
            "AKKK6 630",
            "378A6 290",
            "5TJTT 272",
            "AQJ48 576",
            "7T83Q 835",
            "97T65 476",
            "33J3J 647",
            "KQQK5 112",
            "QQTQT 133",
            "633KK 422",
            "872KT 708",
            "JKTAA 666",
            "439QK 160",
            "26626 992",
            "928K5 106",
            "22225 742",
            "AJJ94 351",
            "T3TKK 117",
            "9999T 34",
            "88666 524",
            "JTK86 580",
            "7KKKA 897",
            "K54K3 595",
            "J444Q 76",
            "J6363 997",
            "A6K8J 858",
            "9A444 738",
            "7TKT7 821",
            "44J99 768",
            "55554 934",
            "96KJK 987",
            "J9999 431",
            "63653 462",
            "AAJAJ 959",
            "59999 370",
            "835KJ 387",
            "T658Q 228",
            "85548 367",
            "5988Q 198",
            "33585 460",
            "AQKK9 321",
            "AA7K6 74",
            "3K9T9 750",
            "AA7AA 279",
            "2A2A2 195",
            "K85T6 958",
            "826T4 540",
            "888J2 294",
            "A65KK 977",
            "5QQ55 331",
            "QK3K2 652",
            "7KQ9Q 695",
            "52552 933",
            "KKK77 114",
            "88J33 584",
            "AQQQQ 507",
            "A5496 348",
            "TT744 979",
            "77744 539",
            "JJ828 607",
            "JTJ7T 886",
            "7767K 783",
            "KK2K7 377",
            "6Q6AA 353",
            "24K57 838",
            "4Q378 148",
            "2A333 725",
            "449T3 531",
            "J4464 1000",
            "4Q44A 623",
            "AA498 413",
            "JA5AA 345",
            "797A7 813",
            "462AT 498",
            "T4T7T 161",
            "82T82 683",
            "KK5KK 402",
            "73495 572",
            "44KAK 747",
            "88AJ6 735",
            "A8JAA 571",
            "64466 27",
            "TA76J 665",
            "27722 91",
            "J9JJ9 454",
            "5A5KK 558",
            "K52JQ 281",
            "J8JJJ 778",
            "Q49T6 113",
            "J9229 621",
            "66555 82",
            "K8236 826",
            "A5K32 398",
            "Q38A2 350",
            "356QQ 11",
            "66466 846",
            "55544 944",
            "35355 156",
            "5J577 224",
            "7Q952 143",
            "3TTTK 711",
            "63866 589",
            "949KK 960",
            "6J266 931",
            "25QK8 407",
            "9969T 982",
            "24JJ6 816",
            "J7A95 506",
            "8J88J 163",
            "47T48 135",
            "TTTA2 323",
            "J44TJ 13",
            "Q6KJ5 499",
            "8453Q 682",
            "JTTQT 718",
            "A424Q 693",
            "7Q25T 534",
            "668JJ 4",
            "75T55 461",
            "KTJTK 754",
            "8823Q 590",
            "6T234 791",
            "77K3J 180",
            "A9965 265",
            "TTTTK 849",
            "7Q873 291",
            "Q5Q7Q 874",
            "48888 641",
            "6986J 766",
            "4J8QT 442",
            "Q99K7 100",
            "KQKQ3 587",
            "22622 521",
            "Q49T9 322",
            "35Q82 554",
            "TJKA7 213",
            "2K22K 537",
            "J6K97 288",
            "3TT9T 32",
            "QQ5TQ 950",
            "K68J5 689",
            "525Q2 804",
            "QKAJ5 819",
            "434A6 920",
            "QQ6KK 928",
            "888K8 996",
            "KJJKK 427",
            "49J2T 883",
            "33QQJ 478",
            "QJTT7 475",
            "A9K84 790",
            "A759A 536",
            "QAT62 39",
            "5A96K 605",
            "575Q7 668",
            "83838 570",
            "T9K9K 438",
            "926K8 301",
            "A7AA9 202",
            "889J6 523",
            "Q56TK 128",
            "2J496 52",
            "TJ45K 434",
            "J8555 949",
            "4T797 927",
            "42JT2 439",
            "58AA8 915",
            "T543Q 532",
            "322J3 700",
            "QJ733 556",
            "89A88 420",
            "J863T 302",
            "9K354 730",
            "K88K8 710",
            "KK9JJ 376",
            "A3Q94 855",
            "KAJAA 797",
            "K6KKK 260",
            "AQAQA 880",
            "43AKA 231",
            "TK723 703",
            "A5988 312",
            "K3574 205",
            "6KKJK 157",
            "2Q2A7 364",
            "K8KK3 152",
            "233JJ 268",
            "8J967 809",
            "9JKKK 356",
            "T5TQ5 550",
            "96269 237",
            "J44J4 220",
            "84QQ4 81",
            "A555A 680",
            "228J8 853",
            "9K2K9 73",
            "4K242 998",
            "6676J 921",
            "J6K88 948",
            "77A77 975",
            "7TTT7 421",
            "3KA8Q 178",
            "T939J 349",
            "4K78T 336",
            "33433 164",
            "K4KK4 969",
            "9J737 179",
            "Q7Q54 515",
            "4K444 951",
            "8822A 729",
            "44J45 672",
            "9Q9Q9 203",
            "8257K 229",
            "2AAA3 221",
            "9329A 283",
            "A95QQ 66",
            "345KJ 923",
            "9J399 425",
            "TJ826 84",
            "A82KK 490",
            "533J3 141",
            "44434 528",
            "9647K 51",
            "ATAT3 310",
            "JT325 93",
            "95555 92",
            "J669Q 728",
            "2T32T 327",
            "44J44 764",
            "JA4JK 926",
            "2A779 41",
            "QKKKQ 602",
            "T4K68 102",
            "J8666 970",
            "7Q782 669",
            "TT2T5 771",
            "K6T82 758",
            "8AJAJ 266",
            "AAA93 287",
            "J22J2 372",
            "A643T 648",
            "84679 463",
            "6J555 432",
            "3J3A3 381",
            "54TJ3 707",
            "3Q8Q8 295",
            "57QQ5 397",
            "J985A 443",
            "TAT54 399",
            "6696T 799",
            "AA2AA 126",
            "Q55J5 189",
            "9A85K 37",
            "65JK3 674",
            "Q83Q3 338",
            "J9994 519",
            "T5K5T 352",
            "3393T 870",
            "T3KJ6 50",
            "999AQ 973",
            "28Q8A 872",
            "63TJA 734",
            "96446 430",
            "2TTTT 132",
            "222J2 249",
            "JAJ37 134",
            "QTK82 181",
            "89899 480",
            "7Q777 467",
            "24TT4 699",
            "J354A 910",
            "34AQ3 488",
            "KQ6AQ 373",
            "K7957 362",
            "5T53K 510",
            "6758J 762",
            "TTK9J 685",
            "4445K 592",
            "QK8TK 86",
            "J6649 170",
            "49399 142",
            "69486 563",
            "8888Q 527",
            "4839Q 65",
            "T4TTJ 859",
            "83J86 625",
            "3T33T 634",
            "66K6K 77",
            "QJJTQ 382",
            "7J6J6 727",
            "6T6TT 57",
            "T555Q 129",
            "77AAA 354",
            "9Q989 25",
            "33J33 881",
            "KA843 334",
            "AQ876 741",
            "TA5AK 2",
            "9QQQ4 285",
            "Q3QQ3 564",
            "62679 251",
            "25QKK 798",
            "T3785 343",
            "6Q836 547",
            "AK79T 964",
            "44426 869",
            "TTTT6 325",
            "TTJTJ 250",
            "K5558 210",
            "59569 508",
            "K9782 391",
            "75777 359",
            "8364A 892",
            "6K6KK 264",
            "KQ7AJ 206",
            "JT992 649",
            "48753 85",
            "A4A4A 643",
            "J6A5K 591",
            "6JQQ4 67",
            "94KAK 812",
            "A7677 613",
            "2K5T3 245",
            "J3JQJ 895",
            "T4TT3 293",
            "3JT3T 796",
            "84484 773",
            "QJA5J 885",
            "55A55 772",
            "6J495 21",
            "Q4T9J 705",
            "33QJJ 306",
            "K2KKK 284",
            "37KQ6 109",
            "742AA 424",
            "339J7 384",
            "4A44A 721",
            "Q6A82 98",
            "J233Q 789",
            "66865 899",
            "4Q44Q 903",
            "7AJ8J 646",
            "JQTT5 455",
            "366K9 968",
            "KK5TK 68",
            "963Q7 760",
            "75J7K 542",
            "J34AK 94",
            "JKKAA 319",
            "5Q495 938",
            "5JK55 286",
            "55888 509",
            "T7JT4 366",
            "88988 823",
            "JAA7A 802",
            "6JJ72 269",
            "76626 655",
            "J3KKK 879",
            "74J9J 642",
            "K623K 617",
            "9Q9QQ 26",
            "27K46 822",
            "6KKJJ 545",
            "656J7 88",
            "6TT9T 324",
            "28278 339",
            "8J885 696",
            "TKTTA 824",
            "84648 446",
            "JT2K2 585",
            "79J92 767",
            "TTTTA 314",
            "48688 535",
            "969AK 483",
            "QKKQ8 12",
            "7JQ4A 686",
            "33JJJ 240",
            "34343 850",
            "KKK55 759",
            "33886 530",
            "5K3KJ 898",
            "TQ34Q 125",
            "T59J6 624",
            "95949 232",
            "2T24T 836",
            "K8J98 514",
            "2QT22 581",
            "9779J 30",
            "355Q5 118",
            "QQQ6Q 146",
            "667J7 862",
            "34J2A 177",
            "77AQJ 631",
            "AAK88 105",
            "J5J99 912",
            "K6Q6J 71",
            "443J3 636",
            "AAATA 230",
            "JTKTT 45",
            "75755 981",
            "K8585 258",
            "7T47T 660",
            "43TK2 390",
            "K4Q6J 865",
            "6A666 702",
            "4A286 868",
            "77667 99",
            "T76T7 470",
            "58TTJ 888",
            "68648 418",
            "JJ655 47",
            "632Q5 248",
            "J64J7 313",
            "78234 169",
            "QQ5QJ 717",
            "7T777 38",
            "QQJQQ 622",
            "TKKKK 185",
            "T8JTJ 315",
            "4QJQ4 697",
            "Q99T8 503",
            "T3328 235",
            "QJA2T 244",
            "Q888T 841",
            "32434 107",
            "33739 954",
            "AKA4A 103",
            "99669 831",
            "7TKQJ 614",
            "6QQQJ 116",
            "34626 787",
            "88489 637",
            "26262 3",
            "3Q26Q 209",
            "QAQJT 96",
            "37232 929",
            "6Q934 986",
            "88886 814",
            "QJ352 253",
            "88QQ8 596",
            "67875 214",
            "7T4J4 562",
            "6JK86 776",
            "JJJAA 638",
            "TTTQ9 893",
            "64667 633",
            "83888 207",
            "KQQQ2 761",
            "52A55 61",
            "AQQJJ 978",
            "5559Q 698",
            "QQ88J 940",
            "99QQ5 337",
            "32222 233",
            "35375 340",
            "QA632 124",
            "QQ7JQ 901",
            "9KKK9 458",
            "28338 217",
            "8T699 984",
            "J92QQ 197",
            "KAAAA 919",
            "77477 917",
            "689Q2 852",
            "4JTJT 784",
            "7888J 403",
            "4Q4Q7 104",
            "J7283 502",
            "K96KK 172",
            "QA33Q 15",
            "J9962 415",
            "K4JA7 834",
            "Q4A4A 626",
            "JQAAA 218",
            "T6687 945",
            "KAKAT 781",
            "33A3A 516",
            "TQ968 620",
            "75547 175",
            "QQQJ8 793",
            "79245 955",
            "42422 825",
            "A6A46 603",
            "22663 501",
            "24J55 270",
            "54644 44",
            "Q33A5 806",
            "82888 1",
            "99498 832",
            "QQA66 991",
            "4T4J8 7",
            "5AJT4 586",
            "JA9AA 136",
            "66T76 805",
            "QQ8KT 788",
            "4A444 469",
            "J28K9 406",
            "KKQ6K 64",
            "AA355 946",
            "63333 358",
            "69968 252",
            "A5AAQ 837",
            "A5465 176",
            "4TQ83 820",
            "66656 573",
            "69652 887",
            "T46KQ 444",
            "8967A 491",
            "A7J7J 472",
            "92KJ6 441",
            "5A5J7 517",
            "645J6 371",
            "QA6JT 520",
            "88J98 486",
            "9488J 601",
            "555J5 277",
            "66QQ6 70",
            "9666K 875",
            "99559 677",
            "7K888 827",
            "QJ524 196",
            "9KQA9 357",
            "J69J2 165",
            "626T6 748",
            "66AK4 678",
            "K63JJ 101",
            "36336 909",
            "62J62 688",
            "77776 786",
            "TAA69 226",
            "KAJ22 346",
            "T4449 489",
            "TAT84 988",
            "KKKA3 737",
            "J5K4J 404",
            "6T666 552",
            "96853 553",
            "77A6A 426",
            "6J566 769",
            "8777T 298",
            "444AK 654",
            "555K5 437",
            "KJQT8 716",
            "4QQQK 457",
            "2QTK5 541",
            "TAA3A 863",
            "88A8A 848",
            "49366 681",
            "76A92 722",
            "AAKA8 908",
            "79JTT 31",
            "J246K 639",
            "97898 417",
            "KK8AK 568",
            "5AA5J 200",
            "95438 974",
            "J5KAA 211",
            "KQ955 433",
            "7J7J7 544",
            "T6TTA 712",
            "3AK34 522",
            "7747K 878",
            "244J2 95",
            "84822 333",
            "2TT33 611",
            "69T8T 408",
            "69999 19",
            "796QA 594",
            "A8AA9 495",
            "KT8Q3 597",
            "9J9J9 307",
            "Q85KQ 436",
            "87364 72",
            "JJ834 130",
            "6QJ66 965",
            "Q65T9 604",
            "9583K 447",
            "66696 10",
            "AAA42 5",
            "44Q9Q 80",
            "88748 60",
            "Q7Q37 533",
            "8T44T 619",
            "KQQKQ 967",
            "64TA7 273",
            "53359 882",
            "88T88 957",
            "T9TTT 435",
            "T642K 473",
            "28A85 456",
            "32475 896",
            "Q743J 560",
            "665Q5 829",
            "68T37 582",
            "63A7Q 440",
            "88887 723",
            "KA96T 941",
            "99587 925",
            "KK6AT 87",
            "45949 618",
            "45354 108",
            "AT3JT 884",
            "JJKQQ 296",
            "3JTQQ 513",
            "K86A7 299",
            "4KTTK 239",
            "94K7T 35",
            "96QT6 448",
            "J2A7A 924",
            "5J387 115",
            "Q22JQ 257",
            "22J2K 419",
            "K63AQ 774",
            "JT839 127",
            "JJ78Q 276",
            "36Q66 219",
            "T3Q7Q 983",
            "56445 56",
            "2A935 661",
            "AJ335 512",
            "QTQ6J 428",
            "QQQ7Q 259",
            "QQQ4Q 139",
            "44445 690",
            "85558 628",
            "TQTJQ 89",
            "KQ79K 59",
            "AKT36 137",
            "93763 810",
            "TT8T4 305",
            "4J562 204",
            "4J959 667",
            "7TTAK 811",
            "77Q7Q 459",
            "9KK27 40",
            "Q2KKK 606",
            "2QQQQ 234",
            "QAA9A 976",
            "844J8 650",
            "92252 166",
            "72648 8",
            "8293Q 966",
            "33439 701",
            "52AT2 999",
            "77333 916",
            "Q84TA 174",
            "K26J2 656",
            "99929 937",
            "5AQQJ 918",
            "J3993 609",
            "69AQ5 770",
            "TJJJQ 453",
            "7KK94 192",
            "3T7JT 704",
            "9966J 363",
            "65644 679",
            "93939 713",
            "9KQQ9 851",
            "773Q7 223",
            "274J6 496",
            "79999 548",
            "88T8T 749",
            "34QJQ 481",
            "64J4A 904",
            "4QQ54 485",
            "77JQQ 907",
            "A6QTA 877",
            "55575 247",
            "A7248 380",
            "Q2KA9 55",
            "QJ797 201",
            "8245Q 876",
            "82A32 468",
            "JQQTQ 818",
            "J73TA 119",
            "34J96 471",
            "77J77 150",
            "AJ7KA 464",
            "4AQ84 379",
            "293J7 289",
            "K55Q3 494",
            "99555 612",
            "44QQQ 451",
            "Q4J47 394",
            "5K5Q7 344",
            "A442A 411",
            "Q2KT9 151",
            "2J97T 497",
            "KJT4T 939",
            "7K598 271",
            "37838 168",
            "8558J 714",
            "Q88K8 120",
            "KKKKJ 18",
            "22862 330",
            "JA77A 995",
            "52J99 801",
            "7TTTT 292",
            "TKJ2A 902",
            "88938 388",
            "3544T 121",
            "43J63 24",
            "JQ559 138",
            "98A72 640",
            "5255J 389",
            "8Q47Q 891",
            "QTK53 54",
            "37QK5 167",
            "Q7J73 53",
            "TJ753 208",
            "486Q4 911",
            "33376 755",
            "5T586 193",
            "73447 236",
            "888J3 256",
            "26T22 574",
            "A5444 600",
            "QQJQJ 551",
            "JQQQ2 62",
            "95A4K 58",
            "366Q3 985",
            "66K66 752",
            "5K2J9 511",
            "AA554 479",
            "82TAJ 341",
            "JK82K 856",
            "AA3A6 311",
            "2K6A6 753",
            "A3JAA 49",
            "8J538 800",
            "8Q35J 840",
            "23T67 215",
            "22835 670",
            "949Q9 588",
            "TJ9KA 922",
            "94844 414",
            "J2KA3 492",
            "28878 567",
            "AAJ94 79",
            "669KJ 396",
            "Q5963 726",
            "T37JJ 830",
            "3QQQQ 792",
            "2Q95K 736",
            "7K8KK 963",
            "8QJ28 28",
            "5778A 906",
            "TT8T7 83",
            "44422 186",
            "TJTTT 857",
            "AJAAA 405",
            "KK82K 651",
            "TTTQQ 184",
            "AA254 227",
            "6Q666 525",
            "44428 452",
            "J5955 744",
            "965QJ 110",
            "846TT 779",
            "8JK8K 145",
            "224T4 935",
            "43245 864",
            "K66A3 261",
            "T6993 484",
            "9TJTT 365",
            "T5669 803",
            "63663 644",
            "K44JJ 332",
            "39369 122",
            "A57AT 724",
            "77A2A 561",
            "TA338 847",
            "66J6J 401",
            "7779Q 255",
            "A5AAA 720",
            "32223 500",
            "94744 763",
            "T444K 188",
            "3K3K3 635",
            "K492K 663",
            "29292 871",
            "TT44T 412",
            "JJJJJ 131",
            "86446 140",
            "68883 416",
            "456AT 16",
            "22595 518",
            "JT28T 386",
            "A7AJ6 994",
            "Q5333 565",
            "AA2A9 739",
            "4272Q 529",
            "J84T6 242",
            "AA4Q9 947",
            "55J35 303",
            "T4QA9 775",
            "7JJJA 894",
            "8T778 549",
            "K59A7 155",
            "7KK7J 280",
            "T44JT 971",
            "667Q7 347",
            "AKAKA 815",
            "3969J 300",
            "3TTT3 673",
            "463J2 316",
            "KT3KJ 538",
            "72T77 474",
            "3A9A3 842",
            "55JKK 627",
            "AQAKA 843",
            "K53A8 42",
            "82222 691",
            "44674 684",
            "Q8Q8T 216",
            "28828 274",
            "773J7 445",
            "TKJK7 147",
            "28499 861",
            "82877 43",
            "J7767 374",
            "TQA5T 577",
            "K7K77 599",
            "94QJ5 254",
            "T3269 575",
            "5Q776 190",
            "5AT92 326",
            "KT582 616",
            "285AA 36",
            "62AA7 557",
            "55K57 833",
            "9TT93 794"
        };

        static readonly Dictionary<char, int> cardValueMap = new Dictionary<char, int> {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'J', 11 },
            { 'T', 10 },
            { '9', 9 },
            { '8', 8 },
            { '7', 7 },
            { '6', 6 },
            { '5', 5 },
            { '4', 4 },
            { '3', 3 },
            { '2', 2 }
        };
        static readonly Dictionary<char, int> cardValueMapPart2 = new Dictionary<char, int> {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'T', 11 },
            { '9', 10 },
            { '8', 9 },
            { '7', 8 },
            { '6', 7 },
            { '5', 6 },
            { '4', 5 },
            { '3', 4 },
            { '2', 3 },
            { 'J', 2 }
        };

        public static int GetCardValue(char ch)
        {
            if (cardValueMap.ContainsKey(ch))
            {
                return cardValueMap[ch];
            }
            return 0;
        }

        public static int GetCardValuePart2(char ch)
        {
            if (cardValueMapPart2.ContainsKey(ch))
            {
                return cardValueMapPart2[ch];
            }
            return 0;
        }

        public static long GetTotalWinnings(String[] input)
        {
            List<HandBid> testHandBids = new List<HandBid>();
            foreach (String str in input)
            {
                testHandBids.Add(new HandBid(str));
            }
            testHandBids.Sort(delegate (HandBid hb1, HandBid hb2)
            {
                HandType h1Type = hb1.hand.GetHandType();
                HandType h2Type = hb2.hand.GetHandType();
                if (h1Type != h2Type)
                {
                    if (h1Type < h2Type)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    if (hb1.hand.cards[i] != hb2.hand.cards[i])
                    {
                        if (GetCardValue(hb1.hand.cards[i]) < GetCardValue(hb2.hand.cards[i]))
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                return 0;
            });
            long winningsSum = 0;
            for (int i = 0; i < testHandBids.Count; i++)
            {
                long val = (i + 1) * testHandBids[i].bid;
                winningsSum += val;
            }
            return winningsSum;
        }

        public static long GetTotalWinningsPart2(String[] input)
        {
            List<HandBid> testHandBids = new List<HandBid>();
            foreach (String str in input)
            {
                testHandBids.Add(new HandBid(str));
            }
            testHandBids.Sort(delegate (HandBid hb1, HandBid hb2)
            {
                HandType h1Type = hb1.hand.GetHandTypePart2();
                HandType h2Type = hb2.hand.GetHandTypePart2();
                if (h1Type != h2Type)
                {
                    if (h1Type < h2Type)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    if (hb1.hand.cards[i] != hb2.hand.cards[i])
                    {
                        if (GetCardValuePart2(hb1.hand.cards[i]) < GetCardValuePart2(hb2.hand.cards[i]))
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                return 0;
            });
            long winningsSum = 0;
            for (int i = 0; i < testHandBids.Count; i++)
            {
                long val = (i + 1) * testHandBids[i].bid;
                winningsSum += val;
            }
            return winningsSum;
        }

        public static void Part1()
        {
            if (GetTotalWinnings(testInput) != 6440)
            {
                Console.WriteLine("Part1 test failed");
            }
            Console.WriteLine("Part1 test passed");

            Console.WriteLine("Part1 winnings sum: " + GetTotalWinnings(actualInput));
        }

        public static void Part2()
        {
            if (GetTotalWinningsPart2(testInput) != 5905)
            {
                Console.WriteLine("Part2 test failed");
            }
            Console.WriteLine("Part2 test passed");

            Console.WriteLine("Part2 winnings sum: " + GetTotalWinningsPart2(actualInput));
        }
    }
}
