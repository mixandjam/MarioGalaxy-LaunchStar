using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class StarLauncher : MonoBehaviour
{
    Animator animator;
    MovementInput movement;

    public AnimationCurve pathCurve;

    [Range(1,10)]
    public float speed = 10;

    [Space]
    [Header("Booleans")]
    public bool insideLaunchStar;
    public bool flying;
    public bool almostFinished;

    private Transform launchObject;

    [Space]
    [Header("Public References")]
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineDollyCart dollyCart;
    float cameraRotation;
    public Transform playerParent;

    [Space]
    [Header("Launch Preparation Sequence")]
    public float prepMoveDuration = .15f;
    public float prepRotateDuration = .15f;
    public float launchInterval = .5f;


    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<MovementInput>();
    }

    void Update()
    {
        if (insideLaunchStar)
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(CenterLaunch());


        if (flying)
            animator.SetFloat("Path", dollyCart.m_Position);

        if(dollyCart.m_Position > .7f && !almostFinished && flying)
        {
            almostFinished = true;
            thirdPersonCamera.m_XAxis.Value = cameraRotation;
            playerParent.DORotate(new Vector3(360 + (250), 0, 0), .8f, RotateMode.LocalAxisAdd)
                .OnComplete(()=>playerParent.DORotate(new Vector3(0,playerParent.eulerAngles.y,playerParent.eulerAngles.z),.2f));
            transform.DOLocalRotate(Vector3.zero, 1);
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.O))
            Time.timeScale = .2f;
        if (Input.GetKeyDown(KeyCode.P))
            Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    IEnumerator CenterLaunch()
    {
        movement.enabled = false;
        DOTween.KillAll();

        //Checks to see if there is a Camera Trigger at the DollyTrack object - if there is activate its camera
        if (launchObject.GetComponent<CameraTrigger>() != null)
            launchObject.GetComponent<CameraTrigger>().SetCamera();

        dollyCart.m_Position = 0;
        dollyCart.m_Path = null;
        dollyCart.m_Path = launchObject.GetComponent<CinemachineSmoothPath>();
        dollyCart.enabled = true;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Sequence CenterLaunch = DOTween.Sequence();
        CenterLaunch.Append(transform.DOMove(dollyCart.transform.position, prepMoveDuration));
        CenterLaunch.Join(transform.DORotate(dollyCart.transform.eulerAngles + new Vector3(90, 0, 0), prepRotateDuration));
        CenterLaunch.OnComplete(() => LaunchSequence());
    }

    private void OnTriggerEnter(Collider other)
    {
        print("enter");
        if (other.CompareTag("Launch"))
        {
            insideLaunchStar = true;
            launchObject = other.transform;
        }

        if (other.CompareTag("CameraTrigger"))
            other.GetComponent<CameraTrigger>().SetCamera();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Launch"))
        {
            insideLaunchStar = false;
            launchObject = null;
        }
    }

    Sequence LaunchSequence()
    {
        cameraRotation = transform.eulerAngles.y;
        GetComponentInChildren<ParticleSystem>().Play();

        Transform originalLaunch = launchObject;

        flying = true;
        animator.SetBool("flying", true);
        Sequence s = DOTween.Sequence();

        playerParent.transform.position = launchObject.position;
        playerParent.transform.rotation = transform.rotation;

        s.AppendCallback(() => transform.parent = playerParent.transform);                              // Attatch the player to the empty gameObject
        s.AppendCallback(() => playerParent.parent = dollyCart.transform);                              // Attach the empty GO to the DollyCart
        s.Append(transform.DOMove(transform.localPosition - transform.up, launchInterval/1.5f));        // Pull player a little bit back
        s.AppendInterval(launchInterval);                                                               // Wait for a while before the launch
        s.Append(DOVirtual.Float(dollyCart.m_Position, 1, 10/speed, PathSpeed).SetEase(pathCurve));  // Lerp the value of the Dolly Cart position from 0 to 1
        s.Join(transform.DOLocalMove(new Vector3(0,-.5f,0), .5f));                                               // Return player's Y position
        s.Join(transform.DOLocalRotate(new Vector3(0, 360, 0),                                          // Slow rotation for when player is flying
            (10 / speed) / 1.8f, RotateMode.LocalAxisAdd)).SetEase(Ease.InOutSine); 
        s.AppendCallback(() => Land());                                                                 // Call Land Function

        return s;
    }

    void Land()
    {
        //transform.DORotate(new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z), .2f);
        dollyCart.enabled = false;
        dollyCart.m_Position = 0;
        movement.enabled = true;
        transform.parent = null;

        flying = false;
        almostFinished = false;
        animator.SetBool("flying", false);

        GetComponentInChildren<ParticleSystem>().Stop();
    }

    public void PathSpeed(float x)
    {
        dollyCart.m_Position = x;
    }
}
