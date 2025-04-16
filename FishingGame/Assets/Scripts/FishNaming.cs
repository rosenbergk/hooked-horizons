// FishNaming.cs
using UnityEngine;

public static class FishNaming
{
    public static string GetFishName(int fishType)
    {
        switch (fishType)
        {
            case 1:
            {
                string[] type1Names = { "Bluegill", "Crappie", "Yellow Perch" };
                return type1Names[Random.Range(0, type1Names.Length)];
            }
            case 2:
            {
                string[] type2Names = { "Largemouth Bass", "Smallmouth Bass", "Walleye" };
                return type2Names[Random.Range(0, type2Names.Length)];
            }
            case 3:
            {
                string[] type3Names = { "Northern Pike", "Chinook Salmon", "Striped Bass" };
                return type3Names[Random.Range(0, type3Names.Length)];
            }
            case 4:
            {
                string[] type4Names = { "Yellowfin Tuna", "Tarpon", "Sturgeon" };
                return type4Names[Random.Range(0, type4Names.Length)];
            }
            case 5:
                return "Shark";
            default:
                return "Unknown Fish";
        }
    }
}
