using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {
    [SerializeField] private Transform platformPrefab;
    [SerializeField] private Transform playerPos;
    [Space]
    [SerializeField] private int numOfPreLoadPlatform;
    [SerializeField] private float preLoadDis = 30;
    [Space]
    [SerializeField] private float maxHeight = 4;
    [SerializeField] private float minHeight = -6;
    [Space]
    [SerializeField] private float maxDis;
    [SerializeField] private float minDis;

    public List<Transform> platformList = new List<Transform>();
    private Transform lastEndPos;
    private bool nextIsUp;

    void Start() {
        // toạ độ cần đặt platform tiếp theo
        // "End Position" là 1 gameobject gắn ở cuối mỗi platform
        lastEndPos = platformPrefab.Find("End Position");

        CreateFirstPlatform();

        PreSpawnPlatform();
    }

    private void CreateFirstPlatform() {
        // nếu dùng trực tiếp platform có sẵn ở scence thì sẽ ko tìm được nó trong platformList
        Transform firstPlatform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
        platformList.Add(firstPlatform);
    }

    private void PreSpawnPlatform() {
        for (int i = 0; i < numOfPreLoadPlatform; i++) {
            SpawnSinglePlatform();
        }
    }

    void Update() {
        SpawnPlatformWhenPlayerNearEndPosition();

        RemovePlaformWhenExceedSpecificAmount();
    }

    private void SpawnPlatformWhenPlayerNearEndPosition() {
        if (playerPos != null) {
            if (IsPlayerNearEndPosition()) {
                for (int i = 0; i < numOfPreLoadPlatform; i++) {
                    SpawnSinglePlatform();
                }
            }
        }
    }

    private bool IsPlayerNearEndPosition() {
        return Mathf.Abs(lastEndPos.position.x - playerPos.position.x) < preLoadDis;
    }

    private void SpawnSinglePlatform() {
        float height;

        // platform sẽ nằm so le nhau (trên, dưới, trên, dưới,...)
        // 0 là độ cao của platform đầu tiên (platform player đứng lúc mới bắt đầu game)
        // các plaform sau đó sẽ chỉ nằm trong khoảng (2, maxHeight) cho platform bên trên
        // và (minHeight, -2) cho platform bên dưới
        if (nextIsUp) {
            height = Random.Range(2, maxHeight);
        } else {
            height = Random.Range(minHeight, -2);
        }
        nextIsUp = !nextIsUp;

        Vector3 pos = new Vector3(lastEndPos.position.x + Random.Range(minDis, maxDis), height, 0);
        Transform newPlatform = Instantiate(platformPrefab, pos, Quaternion.identity, gameObject.transform);
        platformList.Add(newPlatform);
        lastEndPos = newPlatform.Find("End Position");
    }

    private void RemovePlaformWhenExceedSpecificAmount() {
        if (platformList.Count >= numOfPreLoadPlatform * 3) {
            platformList.RemoveRange(0, numOfPreLoadPlatform * 2);
        }
    }
}
