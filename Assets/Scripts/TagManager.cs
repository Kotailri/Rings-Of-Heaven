using System.Collections.Generic;
using UnityEngine;

public enum Tags
{
    DamagePlayer,
    DamagedByPlayer,
    BouncesRing
}

public class TagManager : MonoBehaviour
{
    public List<Tags> tags = new List<Tags>();
    
    public bool IsOfTag(Tags tag)
    {
        return tags.Contains(tag);
    }
}
