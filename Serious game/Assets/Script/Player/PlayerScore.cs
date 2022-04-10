using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {
    public Text textScore;
    [SerializeField] private int score;

    private static int prePlatformID;
    private int skipOverMultiply = 2;
    private GameObject currentPlatform;


    private void OnCollisionEnter(Collision collision) {
        currentPlatform = collision.gameObject;
        PlatformBehaviour temp = collision.transform.GetComponent<PlatformBehaviour>();
        if (temp && gameObject.activeInHierarchy) {
            if (temp.id - prePlatformID == 1) {
                prePlatformID = temp.id;
                score++;
                skipOverMultiply = 2;
            }
            if (temp.id - prePlatformID > 1) {
                score += (temp.id - prePlatformID) * skipOverMultiply;
                prePlatformID = temp.id;
                skipOverMultiply += 2;
            }

            textScore.text = score.ToString();
        }
    }

    public GameObject getCurrentPlatform() {
        return currentPlatform;
    }
}
