using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour {
    public GameObject player;
    public PlatformGenerator platformGenerator;
    [SerializeField][Min(0)] private float changingSpeed = 0.2f;
    [SerializeField][Min(0)] private float zoomSpeed = 50f;
    [SerializeField][Range(3f, 5f)] private float insideDistance = 5f;
    [SerializeField][Range(0f, 3f)] private float outsideDistance = 3f;

    private MovementController playerMovement;
    private PlayerScore playerScore;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineComponentBase componentBase;

    private bool zoomIn, zoomOut;

    void Awake() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        playerMovement = player.GetComponent<MovementController>();
        playerScore = player.GetComponent<PlayerScore>();
        componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    void LateUpdate() {
        //Source: https://stackoverflow.com/questions/59346229/change-camera-distance-of-cinemachine-in-script

        if (componentBase is CinemachineFramingTransposer) {
            if (playerMovement.isLanded()) {
                FollowPlayer();

                ControlZoomToFitPlatformOnScreen();
            } else {
                StopFollowingPlayer();
            }
        }
    }

    private void FollowPlayer() {
        (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = 0;
        (componentBase as CinemachineFramingTransposer).m_XDamping = 2;
    }

    private void ControlZoomToFitPlatformOnScreen() {
        int index = GetIndexOfCurrentStandingPlatform();

        if (index >= 0) {
            SetZoomCondition(index);

            if (zoomIn) {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance -= zoomSpeed * Time.deltaTime;
            }

            if (zoomOut) {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance += zoomSpeed * Time.deltaTime;
            }
        }
    }

    private int GetIndexOfCurrentStandingPlatform() {
        GameObject currentPlatform = playerScore.GetCurrentPlatform();
        int index = -1;
        if (currentPlatform != null) {
            index = platformGenerator.platformList.IndexOf(currentPlatform.transform);
        }

        return index;
    }

    private void SetZoomCondition(int index) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        zoomIn = true;
        zoomOut = false;

        /* index of frustum plane
         *      3
         * 0   5,4    1
         *      2
         * inside is positive
         */
        for (int i = 1; i <= 3; i++) {
            float disToEachPlane = planes[i].GetDistanceToPoint(platformGenerator.platformList[index + 1].position);

            // zoomIn phụ thuộc vào khoảng cách giữa platform tới cả 3 cạnh
            if (disToEachPlane < insideDistance) {
                zoomIn = false;
            }

            // zoomOut phụ thuộc vào khoảng cách giữa platform tới cạnh gần nhất
            if (disToEachPlane < outsideDistance) {
                zoomOut = true;
            }
        }
    }

    private void StopFollowingPlayer() {
        if ((componentBase as CinemachineFramingTransposer).m_DeadZoneWidth <= 0.7f) {
            (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth += changingSpeed * Time.deltaTime;
        }
        if ((componentBase as CinemachineFramingTransposer).m_XDamping <= 5) {
            (componentBase as CinemachineFramingTransposer).m_XDamping += changingSpeed * Time.deltaTime;
        }
    }
}
