using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionChanger : MovementEnhancer
{
    public override void ApplyEffect(Player player)
    {
        player.CurrentState = PlayerState.IDLE;
        player.freezeInput(.1f);
    }
}
