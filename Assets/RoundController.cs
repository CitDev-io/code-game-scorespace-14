using citdev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class RoundController : MonoBehaviour
{
    [SerializeField] GameObject nextRoundPanel;
    [SerializeField] GameObject losePanel;

    [SerializeField] public int HitPoints;
    [SerializeField] public int HitPointsMax = 50;

    [SerializeField] public int Armor;
    [SerializeField] public int ArmorMax = 50;
    [SerializeField] public int Coins = 0;
    [SerializeField] public int Kills = 0;
    [SerializeField] public int CharacterLevel = 1;
    [SerializeField] public int NextLevelAt = 5;
    GameController_DDOL _gc;

    int enemyHp = 4;
    int enemyDmg = 2;
    int coinValue = 10;
    int swordValue = 1;
    int heartValue = 1;
    int armorValue = 1;

    int turn = 1;
    int round = 1;
    public int KillRequirement = 30;
    int tilesCleared = 0;
    
    public int TotalKills()
    {
        return _gc.totalKills;
    }

    private void Start()
    {
        nextRoundPanel.SetActive(false);
        losePanel.SetActive(false);
        _gc = GameObject.FindObjectOfType<GameController_DDOL>();

        round = _gc.round;
        DoCharacterProgressionCheck();
        SetEnemyStatsByRound();
    }

    void SetEnemyStatsByRound()
    {
        switch(round)
        {
            case 1:
                enemyHp = 1;
                enemyDmg = 1;
                break;
            case 2:
                enemyHp = 4;
                enemyDmg = 2;
                break;
            default:
                enemyHp = 10;
                enemyDmg = 3;
                break;
        }
    }

    void AssessAttack(int damage)
    {
        if (Armor >= damage)
        {
            Armor -= damage;
            return;
        }

        int remainingDmg = damage - Armor;
        if (Armor > 0)
        {
            ApplyArmorChange(-Armor);
        }

        ApplyHpChange(-remainingDmg);
    }

    void ApplyHpChange(int changeAmount)
    {
        HitPoints = Mathf.Clamp(HitPoints + changeAmount, 0, HitPointsMax);

        if (HitPoints == 0)
        {
            // (e) PLAYER DIED
            DoLose();
        }
    }

    void ApplyArmorChange(int changeAmount)
    {
        Armor = Mathf.Clamp(Armor + changeAmount, 0, ArmorMax);
    }

    public void PlayerCollectedTiles(List<GameTile> collected, BoardController board)
    {
        int healthGained = collected
            .Where((o) => o.tileType == TileType.Heart)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        int armorGained = collected
            .Where((o) => o.tileType == TileType.Shield)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        int coinGained = collected
            .Where((o) => o.tileType == TileType.Coin)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        int damageDealt = collected
            .Where((o) => o.tileType == TileType.Sword)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        List<GameTile> enemies = collected
            .Where((o) => o.tileType == TileType.Monster).ToList();

        List<GameTile> clearableTiles = collected
            .Where((o) => o.tileType != TileType.Monster).ToList();

        if (healthGained != 0) ApplyHpChange(healthGained);
        if (armorGained != 0) ApplyArmorChange(armorGained);
        
        Coins += coinGained;

        foreach(GameTile monster in enemies)
        {
            // (e) : HITTING A MONSTER
            monster.HitPoints -= damageDealt;
            if (monster.HitPoints <= 0)
            {
                // (e) : MONSTER DIED
                clearableTiles.Add(monster);
                OnMonsterKill();
            } else
            {
                // (e) : MONSTER SURVIVED ATTACK
                monster.label1.text = monster.HitPoints + "";
            }
        }

        board.ClearTiles(clearableTiles);
        // (e) : FINISHED RESOLVING USER COLLECTION
        DoEnemiesTurn();
    }

    void OnMonsterKill()
    {
        Kills += 1;
        GameObject.FindObjectOfType<GameController_DDOL>().OnMonsterKilled();
        DoCharacterProgressionCheck();
      
        if (Kills >= KillRequirement)
        {
            DoVictory();
        }
    }

    void DoCharacterProgressionCheck()
    {
        int totalKills = GameObject.FindObjectOfType<GameController_DDOL>().totalKills;
        int prevLevel = CharacterLevel;
        int achievedLevel = 1;
        int costOfEntry = 5;
        while (totalKills >= costOfEntry)
        {
            achievedLevel += 1;
            costOfEntry += achievedLevel * 5;
        }

        CharacterLevel = achievedLevel;
        NextLevelAt = costOfEntry;

        if (achievedLevel > prevLevel)
        {
            DoLevelUp(prevLevel, achievedLevel);
        }
    }

    void DoLevelUp(int from, int to)
    {
        Debug.Log("DING From " + from + " to " + to);
    }

    void DoLose()
    {
        _gc.Reset();
        losePanel.SetActive(true);
    }
    void DoVictory()
    {
        GameObject.FindObjectOfType<GameController_DDOL>().round += 1;
        nextRoundPanel.SetActive(true);
    }

    void DoEnemiesTurn()
    {
        // deal damage to player for existing monsters
        var monsters = GameObject.FindObjectOfType<BoardController>().GetMonsters()
            .Where((o) => o.TurnAppeared < turn).ToList();

        int damageReceived = monsters
                .Aggregate(0, (acc, cur) => acc + cur.Power);

        AssessAttack(damageReceived);
        turn += 1;
    }

    TileType GetNextTile()
    {
        tilesCleared += 1;

        if (tilesCleared > 70 && tilesCleared % 15 == 0)
        {
            Debug.Log(tilesCleared);
            return TileType.Monster;
        }

        return GetRandomTile();
    }

    TileType GetRandomTile()
    {
        int tileChoice = Random.Range(0, 4);
        return (TileType)tileChoice;
    }

    void PrepNewTile(GameTile tile)
    {
        switch (tile.tileType) {
            case TileType.Coin:
                tile.Power = coinValue;
                tile.HitPoints = 0;
                break;
            case TileType.Heart:
                tile.Power = heartValue;
                tile.HitPoints = 0;
                break;
            case TileType.Shield:
                tile.Power = armorValue;
                tile.HitPoints = 0;
                break;
            case TileType.Sword:
                tile.Power = swordValue;
                tile.HitPoints = 0;
                break;
            case TileType.Monster:
                tile.Power = enemyDmg;
                tile.HitPoints = enemyHp;
                break;
            default:
                tile.Power = 0;
                tile.HitPoints = 0;
                break;
        }
        tile.label1.text = tile.HitPoints > 0 ? tile.HitPoints + "" : "";
        tile.label2.text = tile.Power > 0 ? tile.Power + "" : "";
        tile.TurnAppeared = turn;

    }

    // should not be called locally! board needs to cascade guys above
    public void RecycleTileForPosition(GameTile tile, Vector2 position)
    {
        tile.SetTileType(GetNextTile());
        tile.SnapToPosition(position.x, 10);
        tile.AssignPosition(position);
        PrepNewTile(tile);
    }
}
