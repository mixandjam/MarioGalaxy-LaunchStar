using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class randomWalk : MonoBehaviour
{

    public float speed = 10;
    public bool insideLaunch;
    public Transform launchObject;

    private CinemachineDollyCart dollyCart;

    // Start is called before the first frame update
    void Start()
    {
        dollyCart = GetComponent<CinemachineDollyCart>();   
    }

    // Update is called once per frame
    void Update()
    {

        if (insideLaunch)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dollyCart.m_Path = launchObject.GetComponent<CinemachineSmoothPath>();
                DOTween.KillAll();
                dollyCart.m_Position = 0;
                dollyCart.enabled = true;
                LaunchSequence();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("enter");
        if (other.CompareTag("Launch"))
        {
            insideLaunch = true;
            launchObject = other.transform;
        }

        if (other.CompareTag("CameraTrigger"))
        {
            other.GetComponent<CameraTrigger>().SetCamera();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Launch"))
        {
            insideLaunch = false;
            launchObject = null;
        }
    }

    Sequence LaunchSequence()
    {
        Sequence s = DOTween.Sequence();

        Transform originalLaunch = launchObject;

        s.AppendCallback(() => print("wait a second"));
        s.AppendInterval(1);
        s.AppendCallback(() => print("launch!"));
        s.Append(DOVirtual.Float(dollyCart.m_Position, 1, .5f, PathSpeed).SetEase(Ease.Linear));
        s.AppendCallback(()=>dollyCart.enabled = false);
        s.Append(transform.DORotate(new Vector3(-90, 0, 0), .3f));
        return s;
    }

    public void PathSpeed(float x)
    {
        dollyCart.m_Position = x;
    }
   
}
