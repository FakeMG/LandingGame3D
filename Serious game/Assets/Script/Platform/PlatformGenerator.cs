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
    [SerializeField] private float minHeight = -6;
    [Space]
    [SerializeField] private float maxDis;
    [SerializeField] private float minDis;

    public List<Transform> platformList = new List<Transform>();
    private Transform lastEndPos;
    private bool nextIsUp;

    // Start is called before the first frame update
    void Start()
    {
        // toạ độ cần đặt platform tiếp theo
        // "End Position" là 1 gameobject gắn ở cuối mỗi platform
        lastEndPos = platformPrefab.Find("End Position");

        // add platform đầu tiên vào list (platform player đứng lúc mới bắt đầu game)
        platformList.Add(platformPrefab);

        // spawn sẵn platform
        for (int i = 0; i < numOfPreLoadPlatform; i++)
        {
            spawnNewPlatform();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Nếu toạ độ x của người chơi tới lastEndPos nhỏ hơn 1 giá trị cho trước thì tạo thêm platform
        if (playerPos != null)
        {
            if (Mathf.Abs(lastEndPos.position.x - playerPos.position.x) < preLoadDis)
            {
                for (int i = 0; i < numOfPreLoadPlatform; i++)
                {
                    spawnNewPlatform();
                }
            }
        }

        // Nếu số lượng platform vượt quá số lượng quy định thì xoá bớt đi 2/3
        if(platformList.Count >= numOfPreLoadPlatform * 3)
        {
            platformList.RemoveRange(0, numOfPreLoadPlatform * 2);
        }
    }

    void spawnNewPlatform()
    {
        float height;

        // platform sẽ nằm so le nhau (trên, dưới, trên, dưới,...)
        // 0 là độ cao của platform đầu tiên (platform player đứng lúc mới bắt đầu game)
        // các plaform sau đó sẽ chỉ nằm trong khoảng (2, maxHeight) cho platform bên trên
        // và (minHeight, -2) cho platform bên dưới
        if (nextIsUp)
        {
            height = Random.Range(2, maxHeight);
        } 
        else
        {
            height = Random.Range(minHeight, -2);
        }
        nextIsUp = !nextIsUp;

        Vector3 pos = new Vector3(lastEndPos.position.x + Random.Range(minDis, maxDis), height ,0);
        Transform newPlatform = Instantiate(platformPrefab, pos, Quaternion.identity, gameObject.transform);
        platformList.Add(newPlatform);
        lastEndPos = newPlatform.Find("End Position");
    }
}
