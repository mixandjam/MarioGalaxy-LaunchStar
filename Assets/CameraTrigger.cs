using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    CinemachineBrain brain;
    Transform camerasGroup;

    [Header("Camera Settings")]
    public bool activatesCamera = false;
    public CinemachineVirtualCamera camera;
    public bool cut;

    private void Start()
    {
        camerasGroup = GameObject.Find("Cameras").transform;
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public void SetCamera()
    {
        brain.m_DefaultBlend.m_Style = cut ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseOut;

        if (camerasGroup.childCount <= 0)
            return;
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
