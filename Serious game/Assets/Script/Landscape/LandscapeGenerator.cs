using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour {
    [SerializeField] private Transform landscapePrefab;
    [SerializeField] private Transform playerPos;
    [Space]
    [SerializeField] private int numOfPreLoadLandscape = 4;
    [SerializeField] private float preLoadDis = 100;

    public List<Transform> landscapeList = new List<Transform>();
    private Transform lastEndPos;

    // Start is called before the first frame update
    void Start() {
        // toạ độ cần đặt landscape tiếp theo
        // "End Position" là 1 gameobject gắn ở cuối mỗi platform
        lastEndPos = landscapePrefab.Find("End Position");

        // add landscape đầu tiên vào list (landscape player đứng lúc mới bắt đầu game)
        landscapeList.Add(landscapePrefab);

        PreSpawnLandscape();
    }

    private void PreSpawnLandscape() {
        for (int i = 0; i < numOfPreLoadLandscape; i++) {
            SpawnSingleLandscape();
        }
    }

    // Update is called once per frame
    void Update() {
        SpawnLandscapeWhenPlayerNearEndPosition();

        RemoveLandscapeWhenExceedSpecificAmount();
    }

    private void SpawnLandscapeWhenPlayerNearEndPosition() {
        if (playerPos != null) {
            if (IsPlayerNearEndPosition()) {
                for (int i = 0; i < numOfPreLoadLandscape; i++) {
                    SpawnSingleLandscape();
                }
            }
        }
    }

    private bool IsPlayerNearEndPosition() {
        return Mathf.Abs(lastEndPos.position.x - playerPos.position.x) < preLoadDis;
    }

    private void SpawnSingleLandscape() {
        Vector3 pos = new Vector3(lastEndPos.position.x, landscapePrefab.position.y, landscapePrefab.position.z);
        Transform newLandspace = Instantiate(landscapePrefab, pos, Quaternion.identity, gameObject.transform);
        landscapeList.Add(newLandspace);
        lastEndPos = newLandspace.Find("End Position");
    }

    private void RemoveLandscapeWhenExceedSpecificAmount() {
        if (landscapeList.Count >= numOfPreLoadLandscape * 3) {
            landscapeList.RemoveRange(0, numOfPreLoadLandscape * 2);
        }
    }
}
