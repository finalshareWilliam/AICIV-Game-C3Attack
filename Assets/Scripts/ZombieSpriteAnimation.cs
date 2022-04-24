using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpriteAnimation : MonoBehaviour, IPunObservable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;

    private float timeToChangeSprite = 0.05f;
    private float currentTimeToChangeSprite = 0f;
    private int currentIndexIdleSprite = 0;
    private int currentIndexWalkSprite = 0;
    private Rigidbody2D rb;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentIndexIdleSprite);
            stream.SendNext(currentIndexWalkSprite);
        }
        else if (stream.IsReading)
        {
            currentIndexIdleSprite = (int)stream.ReceiveNext();
            currentIndexWalkSprite = (int)stream.ReceiveNext();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        currentTimeToChangeSprite += Time.deltaTime;
        if(currentTimeToChangeSprite >= timeToChangeSprite)
        {
            if(rb.velocity.x > 0 || rb.velocity.x < 0 || rb.velocity.y > 0 || rb.velocity.y < 0)
            {
                spriteRenderer.sprite = walkSprites[currentIndexWalkSprite];
                ++currentIndexWalkSprite;
                if (currentIndexWalkSprite > walkSprites.Length - 1)
                    currentIndexWalkSprite = 0;
            }
            else
            {
                spriteRenderer.sprite = idleSprites[currentIndexIdleSprite];
                ++currentIndexIdleSprite;
                if (currentIndexIdleSprite > idleSprites.Length - 1)
                    currentIndexIdleSprite = 0;
            }
            currentTimeToChangeSprite = 0;
        }
    }
}
