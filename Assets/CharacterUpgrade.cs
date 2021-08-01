using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterUpgrade : ScriptableObject
{
    public int id;
    public string title;
    public string description;
    public string iconName;

    public int CoinValue = 0;
    public int ShieldInstanceMax = 0;
    public int HeartInstanceMax = 0;
    public int SwordInstanceMax = 0;
    public int SwordInstanceMin = 0;
    public int RoundStartShieldCount = 0;
    public int ShieldMax = 0;
    public int HitPointMax = 0;
    public int armorMaxPercentModifier = 0;
}
