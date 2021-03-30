using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    public BlockView[] blockViews;


    public void ChangeBlockView(int select)
    {
        for(int i = 0; i < blockViews.Length; i++)
        {
            if(i != select)
            {
                blockViews[i].selectButton.SetActive(true);
                blockViews[i].transform.SetSiblingIndex(0);
            }
            else
            {
                blockViews[i].selectButton.SetActive(false);
            }
        }
    }
}
