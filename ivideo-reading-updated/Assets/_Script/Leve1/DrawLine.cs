using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine :MonoBehaviour {

    public void  LineDraw()
    {
        Plane planeObj = new Plane(Camera.main.transform.forward, this.transform.position);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (planeObj.Raycast(mouseRay, out rayDistance))
        {
            this.transform.position = mouseRay.GetPoint(rayDistance);
        }
    }
}
