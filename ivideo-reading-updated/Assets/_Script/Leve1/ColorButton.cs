using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof( Button))]
public class ColorButton : MonoBehaviour {

    public Color ColorOfButton;

    public delegate void ColorChangeDelegate(Color pressedButtonColor);

    public static event ColorChangeDelegate colorchanged;

    public void OnEnable()
    {
        //Set the color of the button 
        gameObject.GetComponent<Image>().color = ColorOfButton;
        //Remove all listener
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        //Attach listener
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
       {
           //Change the color of the trail renderer
           if (colorchanged != null)
               colorchanged(pressedButtonColor: ColorOfButton);

       }
        );
    }

}
