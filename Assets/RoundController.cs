using citdev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{

    [SerializeField] public int HitPoints;
    [SerializeField] public int HitPointsMax = 50;

    [SerializeField] public int Armor;
    [SerializeField] public int ArmorMax = 50;

    int tilesCleared = 0;

    void ApplyHpChange(int changeAmount)
    {
        HitPoints = Mathf.Clamp(HitPoints + changeAmount, 0, HitPointsMax);
    }

    void ApplyArmorChange(int changeAmount)
    {
        Armor = Mathf.Clamp(Armor + changeAmount, 0, ArmorMax);
    }

    public void PlayerCollectedTiles(List<GameTile> collected, BoardController board)
    {
        int healthGained = collected.FindAll((o) => o.tileType == TileType.Heart).Count;
        int armorGained = collected.FindAll((o) => o.tileType == TileType.Shield).Count;
        
        if (healthGained != 0) ApplyHpChange(healthGained);
        if (armorGained != 0) ApplyArmorChange(armorGained);

        board.ClearTiles(collected);
    }

    public TileType GetNextTile(bool setupTile = false)
    {
        tilesCleared += 1;

        if (tilesCleared % 15 == 0 && !setupTile)
        {
            return TileType.Monster;
        }

        return GetRandomTile();
    }

    TileType GetRandomTile()
    {
        int tileChoice = Random.Range(0, 4);
        return (TileType)tileChoice;
    }
}
