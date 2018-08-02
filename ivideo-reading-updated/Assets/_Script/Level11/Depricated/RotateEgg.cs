using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEleven
{
    public class RotateEgg : MonoBehaviour
    {
        /// <summary>
        /// Rotation On This Axix .
        /// </summary>
        public enum DiretionOfRotation : int
        {
            Positive_X_Axix = 0,
            Negative_X_Axix = 1,
            Positve_Y_Axix = 2,
            Negative_Y_Axix = 3,
            Positive_Z_Axix = 4,
            Negative_Z_Axix = 5,
            None = 6 

        };

        /// <summary>
        /// Rotate over that axix .
        /// </summary>
        [Tooltip("Select the directio in which Egg will be rotate")]
        public DiretionOfRotation Direction;

        [Range(1f,200f)]
        public float RotationSpeed = 1f;


        public void Update()
        {
            

            //If animation is  required .
            if (Direction != DiretionOfRotation.None)
            {
                //Along X axis

                if (Direction == DiretionOfRotation.Positive_X_Axix)
                {
                   
                    transform.Rotate(Time.deltaTime * RotationSpeed , 0, 0, Space.World);
                    return;
                }
                if (Direction == DiretionOfRotation.Negative_X_Axix)
                {
                    transform.Rotate(-Time.deltaTime * RotationSpeed, 0, 0, Space.World);
                    return;
                }


                //Along Y axis

                if (Direction == DiretionOfRotation.Positve_Y_Axix)
                {
                    transform.Rotate(0, Time.deltaTime * RotationSpeed, 0, Space.World);
                    return;
                }
                if (Direction == DiretionOfRotation.Negative_Y_Axix)
                {
                    transform.Rotate(0, -Time.deltaTime * RotationSpeed, 0, Space.World);
                    return;
                }


                //Along z axis

                if (Direction == DiretionOfRotation.Positive_Z_Axix)
                {
                    transform.Rotate(0, 0 , Time.deltaTime * RotationSpeed, Space.World);
                    return;
                }
                if (Direction == DiretionOfRotation.Negative_Z_Axix)
                {
                    transform.Rotate(0, 0, -Time.deltaTime * RotationSpeed, Space.World);
                    return;
                }

            }


        }
        }

    }
