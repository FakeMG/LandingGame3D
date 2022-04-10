using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour {
    public GameObject player;
    public PlatformGenerator platformGenerator;
    [SerializeField] float zoomSpeed = 50f;

    private PlayerMovement playerMovement;
    private PlayerScore playerScore;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineComponentBase componentBase;

    void Awake() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerScore = player.GetComponent<PlayerScore>();
        componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    // Update is called once per frame
    void Update() {
        //Control camera
        //https://stackoverflow.com/questions/59346229/change-camera-distance-of-cinemachine-in-script
        if (componentBase is CinemachineFramingTransposer) {
            if (playerMovement.isLanded()) {
                // Độ zoom default
                (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = 0;
                (componentBase as CinemachineFramingTransposer).m_XDamping = 3;

                //Lấy index trong "platformGenerator.platformList" của platform mà người chơi đang đứng lên
                GameObject currentPlatform = playerScore.getCurrentPlatform();
                int index = 0;
                if (currentPlatform != null) {
                    index = platformGenerator.platformList.IndexOf(currentPlatform.transform);
                }

                if (index >= 0) {
                    // Kiểm tra xem có zoom in được không
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                    bool zoomIn = true;
                    bool zoomOut = false;

                    foreach (Plane plane in planes) {
                        //khi nào mà màn hình chỉ còn platform đang đứng và platform tiếp theo
                        //và platform tiếp theo sẽ ở sát bên phải màn hình thì ngừng zoom
                        float disToEachPlane = plane.GetDistanceToPoint(platformGenerator.platformList[index + 1].position);

                        if (disToEachPlane < 3) {
                            zoomIn = false;
                        }

                        if (disToEachPlane < 2) {
                            zoomOut = true;
                        }
                    }

                    if (zoomIn) {
                        (componentBase as CinemachineFramingTransposer).m_CameraDistance -= zoomSpeed * Time.deltaTime;
                    }

                    if (zoomOut) {
                        (componentBase as CinemachineFramingTransposer).m_CameraDistance += zoomSpeed * Time.deltaTime;
                    }
                }
            } else {
                (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = .7f;
                (componentBase as CinemachineFramingTransposer).m_XDamping = 5;
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = 18;
            }
        }
    }
}
