using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnhancer
{
    // might want to change this because it cant have custom data, all of them have to behave the same which is ok for some cases, not ok for others
    public static Dictionary<char, MovementEnhancer> enhancers = new Dictionary<char, MovementEnhancer>()
    {
        { '+', new DirectionChanger() }
    };

    public virtual void ApplyEffect(Player player)
    {

    }
}
