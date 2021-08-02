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
    [SerializeField] public int RoundScore = 0;

    GameController_DDOL _gc;

    bool roundEnded = false;
    bool WaitingForUpgrade = false;

    [SerializeField] List<UI_UpgradeOption> upgradeSlots;

    int enemyHp = 4;
    int enemyDmg = 2;

    int turn = 1;
    int round = 1;
    public bool CanCastHelm = false;
    public int KillRequirement = 15;
    public int tilesCleared = 0;
    public int RoundMoves = 0;
    [SerializeField] GameObject dmgPrefab;
    [SerializeField] GameObject hpPrefab;
    [SerializeField] GameObject apPrefab;
    [SerializeField] GameObject gpPrefab;
    [SerializeField] GameObject floaterParent;

    void FloatDamage(int dmg)
    {
        var go = Instantiate(dmgPrefab, floaterParent.transform);
        go.GetComponent<TextMeshProUGUI>().text = "-" + dmg + " HP";
    }
    void FloatHeal(int dmg)
    {
        var go = Instantiate(hpPrefab, floaterParent.transform);
        go.GetComponent<TextMeshProUGUI>().text = "+" + dmg + " HP";
    }
    void FloatArmor(int dmg)
    {
        var go = Instantiate(apPrefab, floaterParent.transform);
        go.GetComponent<TextMeshProUGUI>().text = "+" + dmg + " AP";
    }
    void FloatGold(int dmg)
    {
        var go = Instantiate(gpPrefab, floaterParent.transform);
        go.GetComponent<TextMeshProUGUI>().text = "+" + dmg + " GP";
    }
    public void AttemptToCastHelm()
    {
        if (!CanCastHelm) return;

        _gc.PlaySound("Special_Ability_Use");
        FindObjectOfType<BoardController>()?.ConvertHeartsToSwords(5);
        CanCastHelm = false;
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
        ApplyHpChange(_gc.GetUpgradeValues().HitPointMax);
        if (!roundEnded)
        {
            FindObjectOfType<BoardController>()?.ToggleTileFreeze(false);

        }
    }

    private void Start()
    {
        nextRoundPanel.SetActive(false);
        losePanel.SetActive(false);
        levelupPanel.SetActive(false);
        _gc = FindObjectOfType<GameController_DDOL>();

        _gc.round += 1;
        round = _gc.round;
        DoCharacterProgressionCheck();
        SetEnemyStatsByRound();
        SetupCharacterForRound();
    }

    void SetupCharacterForRound()
    {
        if (_gc.hasHelmet)
        {
            CanCastHelm = true;
        }
        CharacterUpgrade cu = _gc.GetUpgradeValues();
        int startingShields = cu.RoundStartShieldCount;
        Armor += startingShields;
        HitPoints = cu.HitPointMax;
    }

    void SetEnemyStatsByRound()
    {
        enemyHp = round + 1;
        enemyDmg = Mathf.Max(round - (round % 2), 1);
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
        Armor = Mathf.Clamp(Armor + changeAmount, 0, stats.ShieldMax + (int) (stats.ShieldMax * (stats.armorMaxPercentModifier / 100)));
    }

    public void PlayerCollectedTiles(List<GameTile> collected, BoardController board)
    {
        RoundMoves++;
        int chainLength = collected.Count;
        int pointMultiplier = 1;
        if (chainLength >= 5) pointMultiplier++;
        if (chainLength >= 6) pointMultiplier++;
        if (chainLength >= 8) pointMultiplier++;
        if (chainLength >= 10) pointMultiplier++;

        int pointsEarned = chainLength * pointMultiplier * 10;
        _gc.score += pointsEarned;
        RoundScore += pointsEarned;

        int healthGained = collected
            .Where((o) => o.tileType == TileType.Heart)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        int armorGained = collected
            .Where((o) => o.tileType == TileType.Shield)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        int coinMultiplier = 1;

        List<GameTile> coinsCollected = collected.Where((o) => o.tileType == TileType.Coin).ToList();

        if (_gc.hasBelt && coinsCollected.Count >= 5)
        {
            coinMultiplier = 2;
            // (e) : STOLE DOUBLE COINS
            _gc.PlaySound("Special_Ability_Use");
        } else if (coinsCollected.Count > 0)
        {
            _gc.PlaySound("Coin_Collect");
        }

        int coinGained = collected
            .Where((o) => o.tileType == TileType.Coin)
            .Aggregate(0, (acc, cur) => acc + cur.Power) * coinMultiplier;

        if (coinsCollected.Count > 0)
        {
            _gc.PlaySound("Coin_Collect");
            FloatGold(coinGained);
        }

        int damageDealt = collected
            .Where((o) => o.tileType == TileType.Sword)
            .Aggregate(0, (acc, cur) => acc + cur.Power);

        List<GameTile> enemies = collected
            .Where((o) => o.tileType == TileType.Monster).ToList();

        List<GameTile> clearableTiles = collected
            .Where((o) => o.tileType != TileType.Monster).ToList();

        if (healthGained != 0)
        {
            ApplyHpChange(healthGained);
            _gc.PlaySound("Heart_Use");
            FloatHeal(healthGained);
        }
        if (armorGained != 0)
        {
            ApplyArmorChange(armorGained);
            _gc.PlaySound("Shield_Use");
            FloatArmor(armorGained);
        }
        
        _gc.CoinBalanceChange(coinGained);

        if (enemies.Count > 0)
        {
            _gc.PlaySound("Sword_Hit");
        }

        foreach (GameTile monster in enemies)
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
        FindObjectOfType<BoardController>()?.ToggleTileFreeze(true);
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
        StartCoroutine("LoseRoutine");
    }

    IEnumerator LoseRoutine()
    {
        FindObjectOfType<BoardController>()?.ToggleTileFreeze(true);
        _gc.PreviousRoundMoves = RoundMoves;
        _gc.PreviousRoundScore = RoundScore;
        FindObjectOfType<UI_SlidingStartText>().GoGoStartText("YOU LOSE", "YOU LOSE");
        yield return new WaitForSeconds(3.0f);
        _gc.ChangeScene("GameOver");
    }

    void DoVictory()
    {
        roundEnded = true;
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
        FindObjectOfType<UI_SlidingStartText>().GoGoStartText("WAVE COMPLETE", "WAVE COMPLETE");

        _gc.PreviousRoundScore = RoundScore;
        _gc.PreviousRoundMoves = RoundMoves;
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

        if (monsters.Count > 0)
        {
            int random = Random.Range(1, 4);
            _gc.PlaySound("Monster_Hit_" + random);
            FloatDamage(damageReceived);
        }
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

    public void ConvertTileToSword(GameTile tile)
    {
        PrepNewTile(tile);
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
