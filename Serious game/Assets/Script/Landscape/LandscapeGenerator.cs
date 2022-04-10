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
        // to? ?? c?n ??t landscape ti?p theo
        // "End Position" l� 1 gameobject g?n ? cu?i m?i landscape
        lastEndPos = landscapePrefab.Find("End Position");

        // add landscape ??u ti�n v�o list (landscape player ??ng l�c m?i b?t ??u game)
        landscapeList.Add(landscapePrefab);

        // spawn s?n landscape
        for (int i = 0; i < numOfPreLoadLandscape; i++) {
            spawnNewLandscape();
        }
    }

    // Update is called once per frame
    void Update() {
        // N?u to? ?? x c?a ng??i ch?i t?i lastEndPos nh? h?n 1 gi� tr? cho tr??c th� t?o th�m landscape
        if (playerPos != null) {
            if (Mathf.Abs(lastEndPos.position.x - playerPos.position.x) < preLoadDis) {
                for (int i = 0; i < numOfPreLoadLandscape; i++) {
                    spawnNewLandscape();
                }
            }
        }
    }

    void spawnNewLandscape() {
        Vector3 pos = new Vector3(lastEndPos.position.x, landscapePrefab.position.y, landscapePrefab.position.z);
        Transform newLandspace = Instantiate(landscapePrefab, pos, Quaternion.identity, gameObject.transform);
        landscapeList.Add(newLandspace);
        lastEndPos = newLandspace.Find("End Position");
    }
}
