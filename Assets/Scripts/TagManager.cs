using System.Collections.Generic;
using UnityEngine;

public enum Tags
{
    DamagesPlayer,
    DamagesEnemy,
    DamagedByPlayer,
    BouncesRing,
    BrokeByRing
}

public class TagManager : MonoBehaviour
{
    public List<Tags> tags = new();
    
    public bool IsOfTag(Tags tag)
    {
        return tags.Contains(tag);
    }
}
