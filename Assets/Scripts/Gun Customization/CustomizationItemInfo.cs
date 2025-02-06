using UnityEngine;

[CreateAssetMenu]

public class CustomizationItemInfo : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public int price;
    public bool Disabled;


    [ContextMenu("Update Name")]
    public void SetName()
    {
        Name = name;
    }
}