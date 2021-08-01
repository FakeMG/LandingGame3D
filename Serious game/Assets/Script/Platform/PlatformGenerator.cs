using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private Transform platformPrefab;
    [SerializeField] private Transform playerPos;
    [Space]
    [SerializeField] private int numOfPreLoadPlatform;
    [SerializeField] private float preLoadDis = 30;
    [Space]
    [SerializeField] private float maxHeight = 4;
    [SerializeField] private float minHeight = -8;
    [Space]
    [SerializeField] private float maxDis;
    [SerializeField] private float minDis;

    private List<Transform> platformList = new List<Transform>();
    private Transform lastEndPos;
    private bool nextIsUp;

    // Start is called before the first frame update
    void Start()
    {
        lastEndPos = platformPrefab.Find("End Position");

        for (int i = 0; i < numOfPreLoadPlatform; i++)
        {
            spawnNewPlatform();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPos != null)
        {
            if (Vector3.Distance(lastEndPos.position, playerPos.position) < preLoadDis)
            {
                for (int i = 0; i < numOfPreLoadPlatform; i++)
                {
                    spawnNewPlatform();
                }
            }
        }

        if(platformList.Count >= numOfPreLoadPlatform * 3)
        {
            for (int i = 0; i < numOfPreLoadPlatform; i++)
            {
                Destroy(platformList[i].gameObject);
            }

            platformList.RemoveRange(0, numOfPreLoadPlatform);
        }
    }

    void spawnNewPlatform()
    {
        float height;
        if (nextIsUp)
        {
            height = Random.Range(2, maxHeight);
        } 
        else
        {
            height = Random.Range(-2, minHeight);
        }
        nextIsUp = !nextIsUp;

        Vector3 dis = new Vector3(lastEndPos.position.x + Random.Range(minDis, maxDis), height ,0);
        Transform newPlatform = Instantiate(platformPrefab, dis, Quaternion.identity, gameObject.transform);
        platformList.Add(newPlatform);
        lastEndPos = newPlatform.Find("End Position");
    }

    
}
