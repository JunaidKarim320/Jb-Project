
using UnityEngine;
[CreateAssetMenu]

public class InventoryItemInfo : ScriptableObject
{

    public string Name;

    public Sprite Icon;
    public float Damage;
    public float Range;
    public float Recoil;
    public float Magazine;
    public float RateOfFire;
    public int price;
    public bool AdUnlock;
    public bool Customization;
    public int id;
}

