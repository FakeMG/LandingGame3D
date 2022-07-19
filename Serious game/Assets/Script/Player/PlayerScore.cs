using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {
    public Text textScore;
    [SerializeField] private int score;

    private static int prePlatformID;
    private static int currentPlatformID;
    private GameObject currentPlatform;
    private int skipOverMultiplier = 2;

    private void OnCollisionEnter(Collision collision) {
        if (IsAPlatform(collision)) {
            GetPlatformIDAndGameObject(collision);

            UpdateScore();

            UpdateScoreWhenSkipOver();

            UpdatePrePlatformID();

            UpdateScoreUI();
        }
    }

    private bool IsAPlatform(Collision collision) {
        PlatformBehaviour platform = collision.transform.GetComponent<PlatformBehaviour>();
        return platform;
    }

    private void GetPlatformIDAndGameObject(Collision collision) {
        PlatformBehaviour platform = collision.transform.GetComponent<PlatformBehaviour>();
        currentPlatform = collision.gameObject;
        currentPlatformID = platform.GetID();
    }

    private void UpdateScore() {
        if (currentPlatformID - prePlatformID == 1) {
            score++;
            skipOverMultiplier = 2; //reset
        }
    }

    private void UpdateScoreWhenSkipOver() {
        if (currentPlatformID - prePlatformID > 1) {
            score += (currentPlatformID - prePlatformID) * skipOverMultiplier;
            skipOverMultiplier += 2;
        }
    }

    private void UpdatePrePlatformID() {
        prePlatformID = currentPlatformID;
    }

    private void UpdateScoreUI() {
        textScore.text = score.ToString();
    }

    public GameObject GetCurrentPlatform() {
        return currentPlatform;
    }
}
