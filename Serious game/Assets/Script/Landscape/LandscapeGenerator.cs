using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour
{
    [SerializeField] private Transform landscapePrefab;
    [SerializeField] private Transform playerPos;
    [Space]
    [SerializeField] private int numOfPreLoadLandscape = 4;
    [SerializeField] private float preLoadDis = 10;

    public List<Transform> landscapeList = new List<Transform>();
    private Transform lastEndPos;

    // Start is called before the first frame update
    void Start()
    {
        // to? ?? c?n ??t landscape ti?p theo
        // "End Position" là 1 gameobject g?n ? cu?i m?i landscape
        lastEndPos = landscapePrefab.Find("End Position");

        // add landscape ???c ??t s?n ??u tiên vào list
        landscapeList.Add(landscapePrefab);

        // spawn s?n platform
        for (int i = 0; i < numOfPreLoadLandscape; i++)
        {
            spawnNewLandscape();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // N?u to? ?? x ng??i ch?i t?i lastEndPos nh? h?n 1 giá tr? cho tr??c thì t?o thêm landscape
        if (playerPos != null)
        {
            if (Mathf.Abs(lastEndPos.position.x - playerPos.position.x) < preLoadDis)
            {
                for (int i = 0; i < numOfPreLoadLandscape; i++)
                {
                    spawnNewLandscape();
                }
            }
        }
    }

    void spawnNewLandscape()
    {
        Vector3 pos = new Vector3(lastEndPos.position.x, -7, 14.3f);
        Transform newLandspace = Instantiate(landscapePrefab, pos, Quaternion.identity, gameObject.transform);
        landscapeList.Add(newLandspace);
        lastEndPos = newLandspace.Find("End Position");
    }
}
