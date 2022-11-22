using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class JermaChase : MonoBehaviour
{


    public Transform jerma;
    public SpriteRenderer jermaRenderer;
    public AudioSource jermaVoice;
    public AudioInfo clip;
    public Sprite chaseSprite;
    public Sprite normalSprite;
    public float chaseSpeed = 3;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Racer")
        {
            if (other.transform.parent.GetComponent<Racer>() is Racer_Player)
                Chase(other.transform);
        }
    }

    private void Chase(Transform target)
    {
        float vol = Game_Manager._Instance.audioManager.sfxVol;
        jermaVoice.PlayOneShot(clip.clip,clip.volume * vol);
        Vector3 oldPos = jerma.position;
        Quaternion oldRot = jerma.rotation;
        Vector3 targetPos = target.position;
        targetPos.y -= 5f;
        jerma.LookAt(target);
        jermaRenderer.sprite = chaseSprite;
        jerma.DOMove(targetPos, chaseSpeed).SetEase(Ease.InBounce)
            .OnComplete(() => {
                jerma.DOMove(oldPos, .5f).OnComplete(() => jermaRenderer.sprite = normalSprite) ;
            });
    }
}
