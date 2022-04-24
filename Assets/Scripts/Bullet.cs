using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PhotonView PV;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        StartCoroutine(DestroyBullet(4));
    }

    IEnumerator DestroyBullet(float time)
    {
        yield return new WaitForSeconds(time);
        if (PV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.up * 50, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Spawnar um efeito de explosao ou hit
        if(collision.gameObject.GetComponent<Zombie>() != null)
        {
            PhotonView photonViewZombie = collision.gameObject.GetComponent<PhotonView>();
            photonViewZombie.RPC("PlaySplashSound", RpcTarget.All);
            photonViewZombie.RPC("SpawnBloodEffect", RpcTarget.All);

            if (GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(collision.gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }

            if(Random.value >= 0.7f)
            {
                AlertManager.Instance.RandomPhrases();
            }

            //Ao encostar no zumbi, remover instantaneamente a bala evitando problemas no rigidbody
            StartCoroutine(DestroyBullet(0));
        }
    }

}
