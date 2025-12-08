using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowsFileReader : MonoBehaviour
{

    public AnimationFileReader Reader;

    public Button RArrow;
    public Button LArrow;
    public Button TArrow;
    public Button BArrow;
    
    // Start is called before the first frame update
    void Start()
    {
        RArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(0,1,0)));
        LArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(0,-1,0)));
        TArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(-1,0,0)));
        BArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(1,0,0)));
    }

 

    // Update is called once per frame
    void Update()
    {
    }
}
