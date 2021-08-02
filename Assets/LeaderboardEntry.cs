using System;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
    public int rank;
}

[Serializable]
public class InfamyResult
{
    public LeaderboardEntry[] result;
}