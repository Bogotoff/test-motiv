using UnityEngine;
using System.Collections;

public class BirdGameCompleteTrigger : MonoBehaviour
{
    public BirdGame game;

	public void OnTriggerEnter(Collider other)
    {
        if (game != null) {
            game.onTrigger(other);
        }
    }
}
