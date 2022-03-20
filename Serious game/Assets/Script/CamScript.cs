using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamScript : MonoBehaviour
{
    public GameObject player;
    public PlatformGenerator platformGenerator;
    [SerializeField] float zoomSpeed = 50f;

    private PlayerMovement playerMovement;
    private PlayerScore playerScore;
    private CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerScore = player.GetComponent<PlayerScore>();
    }

    // Update is called once per frame
    void Update()
    {
        //Control camera
        //https://stackoverflow.com/questions/59346229/change-camera-distance-of-cinemachine-in-script
        CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is CinemachineFramingTransposer)
        {
            if (playerMovement.isLanded())
            {
                //Lấy index trong "platformGenerator.platformList" của platform mà người chơi đang đứng lên
                GameObject currentPlatform = playerScore.getCurrentPlatform();
                int index = 0;
                if (currentPlatform != null)
                {
                    index = platformGenerator.platformList.IndexOf(playerScore.getCurrentPlatform().transform);
                }

                (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = 0;
                (componentBase as CinemachineFramingTransposer).m_XDamping = 3;

                if (index >= 0)
                {
                    // Kiểm tra xem có zoom in được không
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                    bool zoomIn = true;
                    foreach (Plane plane in planes)
                    {
                        //khi nào mà màn hình chỉ còn platform đang đứng và platform tiếp theo
                        //và platform tiếp theo sẽ ở sát bên phải màn hình thì ngừng zoom
                        if (plane.GetDistanceToPoint(platformGenerator.platformList[index + 1].position) < 3)
                        {
                            zoomIn = false;
                            break;
                        }
                    }

                    if (zoomIn)
                    {
                        (componentBase as CinemachineFramingTransposer).m_CameraDistance -= zoomSpeed * Time.deltaTime;
                    }
                }
            }
            else
            {
                (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = .7f;
                (componentBase as CinemachineFramingTransposer).m_XDamping = 5;
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = 18;
            }
        }
    }
}
