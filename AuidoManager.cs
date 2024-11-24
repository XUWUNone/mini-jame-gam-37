using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuidoManager : MonoBehaviour
{
    public static AuidoManager instance = null;
    public AudioSource BgmSource;
    public AudioSource SfxSource;

    public List<AudioClip> BGMList;
    public List<AudioClip> SFXList;
    public int BGMId = 0;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        BgmSource.clip = BGMList[0];
        BgmSource.Play();
        BgmSource.volume = 0.8f;
        BGMId = 0;
    }
    public void Update()
    {
        if(BgmSource.isPlaying == false)
        {
            if(BGMId == 0)
            {
                BgmSource.clip = BGMList[1];
                BgmSource.Play();
                BgmSource.volume = 0.4f;
                BGMId = 1;
            }
            else
            {
                BgmSource.clip = BGMList[0];
                BgmSource.Play();
                BgmSource.volume = 0.8f;
                BGMId = 0;
            }
        }
    }


    public void PlaySfxById(int id)
    {
        SfxSource.clip = SFXList[id];
        SfxSource.Play();
    }
}
