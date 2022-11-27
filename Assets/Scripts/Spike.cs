using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpikeState
{
    HIDDEN,
    ACTIVE,
    TRANS_HIDDEN,
    TRANS_ACTIVE
}

public class Spike : MonoBehaviour
{
    float activeTime = .5f;
    float hiddenTime = 1.0f;
    float transitionTimeActive = 0.1f;
    float transitionTimeHidden = 0.5f;

    float currentTime = 0.0f;

    Vector3 activePosition = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 hiddenPosition = new Vector3(0.0f, -1.4f, 0.0f);

    SpikeState currentState = SpikeState.ACTIVE;

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
            case SpikeState.ACTIVE:
                if (currentTime >= activeTime)
                {
                    currentState = SpikeState.TRANS_HIDDEN;
                    currentTime -= activeTime;
                }
                break;
            case SpikeState.HIDDEN:
                if (currentTime >= hiddenTime)
                {
                    currentState = SpikeState.TRANS_ACTIVE;
                    currentTime -= hiddenTime;
                }
                break;
            case SpikeState.TRANS_ACTIVE:
                mesh.transform.localPosition = Vector3.Lerp(hiddenPosition, activePosition, currentTime / transitionTimeActive);
                if (currentTime >= transitionTimeActive)
                {
                    currentState = SpikeState.ACTIVE;
                    currentTime -= transitionTimeActive;
                }
                break;
            case SpikeState.TRANS_HIDDEN:
                mesh.transform.localPosition = Vector3.Lerp(activePosition, hiddenPosition, currentTime / transitionTimeHidden);
                if (currentTime >= transitionTimeHidden)
                {
                    currentState = SpikeState.HIDDEN;
                    currentTime -= transitionTimeHidden;
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent.GetComponent<Player>().kill();
        }
    }
}
