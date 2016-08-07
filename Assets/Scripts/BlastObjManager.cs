using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlastObjManager : MonoBehaviour {

    Collider2D[] colList;
    public Texture2D explosionSprite;

    void Start()
    {
        StartCoroutine("DestroyThis");
        colList = Physics2D.OverlapCircleAll(transform.position, 30f);

        for(int i = 0; i < colList.Length; i++)
        {
            try 
            {
                EnemyDetails details = colList[i].gameObject.GetComponent<EnemyDetails>();
                if(details.isKillable)
                {
                    colList[i].gameObject.GetComponent<EnemyMovement>().Die();
                }
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }

        }
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().PlayOneShot("DeathSound", 1.4f);
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}
