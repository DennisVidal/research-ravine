using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitChunk : MonoBehaviour
{
    Chunk chunk;
    float timer = 1.0f;
    int offset = 0;
    // Start is called before the first frame update
    void Start()
    {
        chunk = GetComponent<Chunk>();
    }

    void Update()
    {
        if (timer < 0.0f)
        {
            Vector3 pos = gameObject.transform.position;
            //chunk.Init(new Vector3Int((int)pos.x + offset % 3, (int)pos.y, (int)pos.z), 1);
            timer = 0.5f;
            offset = (offset+1)%3;
            gameObject.transform.position = pos;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }


}
