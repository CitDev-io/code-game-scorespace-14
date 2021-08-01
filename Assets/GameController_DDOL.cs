using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using citdev;

public class GameController_DDOL : MonoBehaviour
{
    public int round = 1;
    public int totalKills = 0;
    public int coins = 0;
    public int score = 0;

    public int PreviousRoundScore = 0;
    public int PreviousRoundMoves = 0;

    public List<CharacterUpgrade> upgrades = new List<CharacterUpgrade>();
    CharacterUpgrade _base;
    CharacterUpgrade aggregate;
    ChangeScene _sceneChanger;

    public bool hasHelmet = false;
    public bool hasChestplate = false;
    public bool hasBelt = false;

    void Awake()
    {
        _base = Resources.Load<CharacterUpgrade>("CharacterUpgrade/Base");
        DontDestroyOnLoad(this.gameObject);
        Reset();
    }

    private void Start()
    {
        _sceneChanger = GetComponent<ChangeScene>();
    }

    public void ChangeScene(string sceneName)
    {
        _sceneChanger.SwapToScene(sceneName);
    }

    public void Reset()
    {
        round = 0;
        totalKills = 0;
        coins = 0;
        score = 0;
        PreviousRoundScore = 0;
        PreviousRoundMoves = 0;
        hasBelt = false;
        hasHelmet = false;
        hasChestplate = false;
        upgrades.Clear();
        upgrades.Add(_base);
        aggregate = _base;
    }

    public void CoinBalanceChange(int delta)
    {
        coins += delta;
    }

    public void ObtainChestplate()
    {
        hasChestplate = true;
        RequestCharacterUpgrade(Resources.Load<CharacterUpgrade>("CharacterUpgrade/COW"));
    }
    public void OnMonsterKilled()
    {
        totalKills += 1;
    }

    public void RequestCharacterUpgrade(CharacterUpgrade upgrade)
    {
        if (upgrade == null) return;

        upgrades.Add(upgrade);
        aggregate = GenerateAggregateSheet();
        if (upgrade.id == 5)
        {
            FindObjectOfType<BoardController>()?.MinimumSwordRaiseTo(aggregate.SwordInstanceMin);
        }

        if (upgrade.id == 8)
        {
            FindObjectOfType<BoardController>()?.RaiseCoinValueTo(aggregate.CoinValue);
        }
    }

    public CharacterUpgrade GetUpgradeValues()
    {
        if (upgrades.Count == 0) return new CharacterUpgrade();
        if (upgrades.Count == 1)
        {
            return upgrades[0];
        }

        return aggregate;
    }

    CharacterUpgrade GenerateAggregateSheet()
    {
        CharacterUpgrade aggregated = upgrades.Aggregate(
            ScriptableObject.CreateInstance<CharacterUpgrade>(),
            (acc, cur) =>
            {
                acc.CoinValue += cur.CoinValue;
                acc.ShieldInstanceMax += cur.ShieldInstanceMax;
                acc.HeartInstanceMax += cur.HeartInstanceMax;
                acc.SwordInstanceMax += cur.SwordInstanceMax;
                acc.SwordInstanceMin += cur.SwordInstanceMin;
                acc.RoundStartShieldCount += cur.RoundStartShieldCount;
                acc.ShieldMax += cur.ShieldMax;
                acc.HitPointMax += cur.HitPointMax;
                acc.armorMaxPercentModifier += cur.armorMaxPercentModifier;
                return acc;
            }
        );
        return aggregated;
    }
}
