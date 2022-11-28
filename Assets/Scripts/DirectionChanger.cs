using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionChanger : MovementEnhancer
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect(Player player)
    {
        player.currentState = PlayerState.IDLE;
    }
}
