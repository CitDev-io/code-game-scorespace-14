using citdev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public delegate void NoParamDelegate();

public class RoundController : MonoBehaviour
{
    public NoParamDelegate OnRoundEnd;

    [SerializeField] GameObject nextRoundPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject levelupPanel;

    [SerializeField] public int HitPoints;
    [SerializeField] public int Armor;
    [SerializeField] public int Kills = 0;
    [SerializeField] public int CharacterLevel = 0;
    [SerializeField] public int NextLevelAt = 5;
    GameController_DDOL _gc;

    bool roundEnded = false;
    bool WaitingForUpgrade = false;

    [SerializeField] List<UI_UpgradeOption> upgradeSlots;

    int enemyHp = 4;
    int enemyDmg = 2;

    int turn = 1;
    int round = 1;
    public int KillRequirement = 15;
    int tilesCleared = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DoLevelUp(1, 2);
        }
    }
    public int TotalKills()
    {
        return _gc.totalKills;
    }

    public int CoinBalance()
    {
        return _gc.coins;
    }

    public CharacterUpgrade StatSheet()
    {
        return _gc.GetUpgradeValues();
    }

    public void UpgradeSelected()
    {
        WaitingForUpgrade = false;
        levelupPanel.SetActive(false);
    }

    private void Start()
    {
        nextRoundPanel.SetActive(false);
        losePanel.SetActive(false);
        levelupPanel.SetActive(false);
        _gc = FindObjectOfType<GameController_DDOL>();

        round = _gc.round;
        DoCharacterProgressionCheck();
        SetEnemyStatsByRound();
        SetupCharacterForRound();
    }

    void SetupCharacterForRound()
    {
        CharacterUpgrade cu = _gc.GetUpgradeValues();
        int startingShields = cu.RoundStartShieldCount;
        Armor += startingShields;
        HitPoints = cu.HitPointMax;
    }

    void SetEnemyStatsByRound()
    {
        switch(round)
        {
            case 1:
                enemyHp = 3;
                enemyDmg = 1;
                break;
            case 2:
                enemyHp = 4;
                enemyDmg = 2;
                break;
            case 3:
                enemyHp = 6;
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
        CharacterUpgrade stats = _gc.GetUpgradeValues();
        HitPoints = Mathf.Clamp(HitPoints + changeAmount, 0, stats.HitPointMax);

        if (HitPoints == 0)
        {
            // (e) PLAYER DIED
            DoLose();
        }
    }

    void ApplyArmorChange(int changeAmount)
    {
        CharacterUpgrade stats = _gc.GetUpgradeValues();
        Armor = Mathf.Clamp(Armor + changeAmount, 0, stats.ShieldMax);
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
        
        _gc.CoinBalanceChange(coinGained);

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
        _gc.OnMonsterKilled();
        DoCharacterProgressionCheck();
      
        if (Kills >= KillRequirement)
        {
            DoVictory();
        }
    }

    void DoCharacterProgressionCheck()
    {
        int totalKills = _gc.totalKills;
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
        if (from == 0) return;
        WaitingForUpgrade = true;

        levelupPanel.SetActive(true);

        List<int> pickedUpgrades = new List<int>();
        CharacterUpgrade currentUps = _gc.GetUpgradeValues();

        if (currentUps.SwordInstanceMin == currentUps.SwordInstanceMax)
        {
            pickedUpgrades.Add(5); // Make Minimum Sword +1 unavilable
        }

        foreach (UI_UpgradeOption upgradeSlot in upgradeSlots)
        {
            int randomId = Random.Range(1, 9);
            while (pickedUpgrades.Contains(randomId))
            {
                randomId = Random.Range(1, 9);
            }
            pickedUpgrades.Add(randomId);
            CharacterUpgrade rando = Resources.Load<CharacterUpgrade>("CharacterUpgrade/" + randomId);

            upgradeSlot.SetupButtonWithValues(rando);
        }
        
    }

    void DoLose()
    {
        _gc.Reset();
        losePanel.SetActive(true);
    }
    void DoVictory()
    {
        roundEnded = true;
        _gc.round += 1;
        OnRoundEnd?.Invoke();
        StartCoroutine("RoundVictory");
    }

    IEnumerator RoundVictory()
    {
        while(WaitingForUpgrade)
        {
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<UI_SlidingStartText>().GoGoStartText("ROUND COMPLETE", "ROUND COMPLETE");


        yield return new WaitForSeconds(3f);
        _gc.ChangeScene("RoundScore");    
    }

    void DoEnemiesTurn()
    {
        if (roundEnded) return;

        // deal damage to player for existing monsters
        var monsters = GameObject.FindObjectOfType<BoardController>().GetMonsters()
            .Where((o) => o.TurnAppeared < turn).ToList();

        int damageReceived = monsters
                .Aggregate(0, (acc, cur) => acc + cur.Power);

        AssessAttack(damageReceived);
        FindObjectOfType<BoardController>()?.EnemyIconsTaunt();
        turn += 1;
    }

    TileType GetNextTile()
    {
        tilesCleared += 1;

        if (tilesCleared > 40 && tilesCleared % 8 == 0)
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

    void PrepNewTile(GameTile tile)
    {
        CharacterUpgrade stats = _gc.GetUpgradeValues();

        switch (tile.tileType) {
            case TileType.Coin:
                tile.Power = stats.CoinValue;
                tile.HitPoints = 0;
                break;
            case TileType.Heart:
                tile.Power = Random.Range(1, stats.HeartInstanceMax + 1);
                tile.HitPoints = 0;
                break;
            case TileType.Shield:
                tile.Power = Random.Range(1, stats.ShieldInstanceMax + 1);
                tile.HitPoints = 0;
                break;
            case TileType.Sword:
                tile.Power = Random.Range(stats.SwordInstanceMin, stats.SwordInstanceMax + 1);
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
        tile.label2.text = tile.Power > 0 && tile.tileType != TileType.Coin ? tile.Power + "" : "";
        tile.TurnAppeared = turn;

    }

    // should not be called locally! board needs to cascade guys above
    public void RecycleTileForPosition(GameTile tile, Vector2 position)
    {
        tile.SetTileType(GetNextTile());
        tile.SnapToPosition(position.x, 7);
        tile.AssignPosition(position);
        PrepNewTile(tile);
    }
}
