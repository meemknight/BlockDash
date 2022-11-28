using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnhancer : MonoBehaviour
{
    public static Dictionary<char, MovementEnhancer> enhancers = new Dictionary<char, MovementEnhancer>()
    {
        { '+', new DirectionChanger() }
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ApplyEffect(Player player)
    {

    }
}
