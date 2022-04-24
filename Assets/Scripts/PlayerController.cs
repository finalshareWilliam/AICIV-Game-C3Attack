using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private float speed;
    [SerializeField] private Light2D fireLight;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.75f; //0.13f
    private float nextFire = 0.0f;
    private PhotonView PV;
    private AudioSource audioSource;

    private CircleCollider2D playerCollider;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private bool isAlive = true;
    private float health = 1;
    private Image healthBarImage;
    private float cameraShakeTime = 0;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            healthBarImage = GameObject.Find("Healthbar").GetComponent<Image>();
        }

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CircleCollider2D>();
    }

    void Axis()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Shoot()
    {
        if(Input.GetMouseButton(0) && Time.time > nextFire)
        {
            PV.RPC("ShootRpc", RpcTarget.All);
            ShakeCamera(4f, .1f);
        }

    }

    [PunRPC]
    void ShootRpc()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet"), firePoint.position, firePoint.rotation);
        
        audioSource.PlayOneShot(fireSound);
        nextFire = Time.time + fireRate;

        StartCoroutine(ShotFireLight());
    }

    IEnumerator ShotFireLight()
    {
        fireLight.enabled = true;
        yield return new WaitForSeconds(fireRate / 5);
        fireLight.enabled = false;
    }

    void Update()
    {
        if (PV.IsMine)
        {
            Axis();

            if(isAlive)
                Shoot();

            cinemachineVirtualCamera.Follow = transform;

            if(cameraShakeTime > 0)
            {
                cameraShakeTime -= Time.deltaTime;
                if (cameraShakeTime <= 0)
                {
                    CinemachineBasicMultiChannelPerlin noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    noise.m_AmplitudeGain = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            //Evita problema de aumentar velocidade na diagonal
            if (movement.magnitude > 1)
                movement = movement.normalized;

            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);


            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        cameraShakeTime = time;
    }
    public bool IsAlive() { 
        return isAlive; 
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Zombie>() != null)
        {
            if (!PV.IsMine)
                return;

            health -= 0.34f;
           
            if(healthBarImage != null)
                healthBarImage.fillAmount = health;

            if (health <= 0)
            {
                PV.RPC("HideDeadPlayer", RpcTarget.All, PV.ViewID, PV.Owner.NickName);
            }
            else
            {
                PV.RPC("LostLife", RpcTarget.All, PV.ViewID, PV.Owner.NickName);
            }
        }
    }

    [PunRPC]
    public void HideDeadPlayer(int playerID, string playerName)
    {
        isAlive = false;
        spriteObject.SetActive(isAlive);
        playerCollider.enabled = isAlive;
        AlertManager.Instance.ShowText(string.Format("{0} morreu =(", playerName), Color.yellow, 0.5f);
        if (PV.IsMine)
        {
            GameOverManager.Instance.ShowPanel();
        }
    }

    [PunRPC]
    public void LostLife(int playerID, string playerName)
    {
        StartCoroutine(Respawn(playerName));
    }

    [PunRPC]
    IEnumerator Respawn(string playerName)
    {
        AlertManager.Instance.ShowText(string.Format("{0} perdeu 1 vida", playerName), Color.yellow, 0.5f);
        isAlive = false;
        spriteObject.SetActive(isAlive);
        playerCollider.enabled = isAlive;
        yield return new WaitForSeconds(5);
        isAlive = true;
        transform.position = new Vector3(20, 10, 0);
        spriteObject.SetActive(isAlive);
        playerCollider.enabled = isAlive;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(isAlive);
        }
        else if (stream.IsReading)
        {
            health = (float)stream.ReceiveNext();
            isAlive = (bool)stream.ReceiveNext();
        }
    }
}
