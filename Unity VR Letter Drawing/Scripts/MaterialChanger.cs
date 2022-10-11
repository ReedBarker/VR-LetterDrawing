using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Material[] material;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    // Update is called once per frame
    void Update()
    {
        using (StreamReader reader = new StreamReader("letter.data"))
        {
            char value = (char)reader.Read();
            if (value == 'C')
            {
                rend.sharedMaterial = material[1];
            }
            else if (value == 'O')
            {
                rend.sharedMaterial = material[2];
            }
            else if (value == 'S')
            {
                rend.sharedMaterial = material[3];
            }
        }
    }
}
