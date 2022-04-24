using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour, IPunObservable
{
    [SerializeField] private Transform[] spawnPoints;
    private int maxEnemiesPerWave = 2;
    private float timeToSpawn = 1.5f;
    private float currentTimeToSpawn = 0;
    private int currentEnemies = 0;
    private int currentEnemiesSpawned = 0;
    private int wave = 1;
    private const int amountToIncreaseMaxEnemiesPerWave = 3;

    private bool startSpawn = true;

    private TMP_Text waveText;
    private TMP_Text zombiesCountText;

    void Start()
    {
        waveText = GameObject.Find("WaveText").GetComponent<TMP_Text>();
        zombiesCountText = GameObject.Find("ZombiesCountText").GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentEnemies = FindObjectOfType<Zombie>() != null ? FindObjectsOfType<Zombie>().Length : 0;

            if (startSpawn)
            {
                currentTimeToSpawn += Time.deltaTime;
                if (currentTimeToSpawn >= timeToSpawn && currentEnemies < maxEnemiesPerWave)
                {
                    PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Zombie"), spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
                    ++currentEnemiesSpawned;
                    currentTimeToSpawn = 0;
                }

                if (currentEnemiesSpawned >= maxEnemiesPerWave)
                {
                    startSpawn = false;
                }
            }
            else
            {
                if(currentEnemies == 0)
                {
                    ++wave;
                    currentEnemiesSpawned = 0;
                    maxEnemiesPerWave += amountToIncreaseMaxEnemiesPerWave;
                    currentTimeToSpawn = 0;
                    startSpawn = true;
                }
            }

            //PV.RPC("WaveStats", RpcTarget.All, wave, currentEnemiesSpawned, currentEnemies, maxEnemiesPerWave);

        }

        waveText.text = "WAVE " + wave;
        zombiesCountText.text = string.Format("{0}/{1} ({2})", currentEnemiesSpawned, maxEnemiesPerWave, currentEnemies);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(wave);
            stream.SendNext(currentEnemies);
            stream.SendNext(currentEnemiesSpawned);
            stream.SendNext(maxEnemiesPerWave);
        }
        else if (stream.IsReading)
        {
            wave = (int)stream.ReceiveNext();
            currentEnemies = (int)stream.ReceiveNext();
            currentEnemiesSpawned = (int)stream.ReceiveNext();
            maxEnemiesPerWave = (int)stream.ReceiveNext();
        }
    }
}
