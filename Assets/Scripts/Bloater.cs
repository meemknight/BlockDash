using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BloaterState
{
    HIDDEN,
    ACTIVE,
    TRANS_HIDDEN,
    TRANS_ACTIVE,
}

public class Bloater : MonoBehaviour
{
    // Start is called before the first frame update
    float activeTime = .5f;
    float hiddenTime = 1.0f;
    float transitionTimeActive = 0.1f;
    float transitionTimeHidden = 0.5f;

    float currentTime = 0.0f;

    Vector3 activeScale = new Vector3(3.0f, 3.0f, 3.0f);
    Vector3 hiddenScale = new Vector3(1.0f, 1.0f, 1.0f);

    BloaterState currentState = BloaterState.HIDDEN;

    Transform mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        switch (currentState)
        {
            case BloaterState.ACTIVE:
                if (currentTime >= activeTime)
                {
                    currentState = BloaterState.TRANS_HIDDEN;
                    currentTime -= activeTime;
                }
                break;
            case BloaterState.HIDDEN:
                if (currentTime >= hiddenTime)
                {
                    currentState = BloaterState.TRANS_ACTIVE;
                    currentTime -= hiddenTime;
                }
                break;
            case BloaterState.TRANS_ACTIVE:
                mesh.transform.localScale = Vector3.Lerp(hiddenScale, activeScale, currentTime / transitionTimeActive);
                if (currentTime >= transitionTimeActive)
                {
                    currentState = BloaterState.ACTIVE;
                    currentTime -= transitionTimeActive;
                }
                break;
            case BloaterState.TRANS_HIDDEN:
                mesh.transform.localScale = Vector3.Lerp(activeScale, hiddenScale, currentTime / transitionTimeHidden);
                if (currentTime >= transitionTimeHidden)
                {
                    currentState = BloaterState.HIDDEN;
                    currentTime -= transitionTimeHidden;
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent.GetComponent<Player>().hurt(1);
        }
    }
}
