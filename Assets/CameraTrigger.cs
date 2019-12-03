using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    Transform camerasGroup;

    [Header("Camera Settings")]
    public bool activatesCamera = false;
    public CinemachineVirtualCamera camera;

    private void Start()
    {
        camerasGroup = GameObject.Find("Cameras").transform;
    }

    public void SetCamera()
    {
        for (int i = 0; i < camerasGroup.childCount; i++)
        {
            camerasGroup.GetChild(i).gameObject.SetActive(false);
        }
        camera.gameObject.SetActive(activatesCamera);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .1f);
    }
}
