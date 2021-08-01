using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController_DDOL : MonoBehaviour
{
    public int round = 1;
    public int totalKills = 0;
    public int coins = 0;
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
        round = 1;
        totalKills = 0;
        upgrades.Clear();
        upgrades.Add(_base);
        aggregate = _base;
    }

    public void CoinBalanceChange(int delta)
    {
        coins += delta;
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
                return acc;
            }
        );
        return aggregated;
    }
}
