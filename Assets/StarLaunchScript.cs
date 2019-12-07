using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class StarLaunchScript : MonoBehaviour
{
    public bool flying;
    public bool insideLaunchStar;
    Transform launchObject;
    MovementInput movement;

    public CinemachineDollyCart dollyCart;
    public Transform parentObject;

    void Start()
    {
        movement = GetComponent<MovementInput>();  
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CenterSequence();
        }

        if (flying)
        {
            parentObject.position = dollyCart.transform.position;
            parentObject.rotation = dollyCart.transform.rotation;
        }

    }

    Sequence CenterSequence()
    {
        movement.enabled = false;
        parentObject.position = dollyCart.transform.position;
        parentObject.rotation = dollyCart.transform.rotation;
        transform.parent = parentObject;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(new Vector3(0, 0, -.5f), .5f));
        s.Join(transform.DOLocalRotate(new Vector3(90, 0, 0), .5f));
        s.Append(LaunchSequence());
        return s;
    }

    Sequence LaunchSequence()
    {
        flying = true;

        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => DOVirtual.Float(0, 1, 3, PathPosition).SetEase(Ease.InOutSine));
        return s;
    }

    void PathPosition(float x)
    {
        dollyCart.m_Position = x;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Launch"))
            insideLaunchStar = true; launchObject = other.transform;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Launch"))
            insideLaunchStar = false;
    }
}
