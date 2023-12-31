using System.Collections.Generic;
using UnityEngine;

public enum Tags
{
    DamagesPlayer,
    DamagedByPlayer,
    BouncesRing,
    BrokeByRing
}

public class TagManager : MonoBehaviour
{
    public List<Tags> tags = new List<Tags>();
    
    public bool IsOfTag(Tags tag)
    {
        return tags.Contains(tag);
    }
}
