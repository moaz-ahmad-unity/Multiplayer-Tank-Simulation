using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace ChobiAssets.PTM
{

    public class Static_Wheel_CS : MonoBehaviour, IPunObservable
    {
        /*
		 * This script rotates the Static_Wheel in combination with "Static_Wheel_Parent_CS" in the parent object.
		*/


        // User options >>
        public bool Is_Left;
        public Static_Wheel_Parent_CS Parent_Script;
        // << User optionsss

        Transform thisTransform;
        Vector3 currentAng;
        bool isRepairing;


        //mz
        PhotonView parentPhotonView;

        void Start()
        {
            parentPhotonView = GetComponentInParent<PhotonView>();

            thisTransform = transform;
        }


        void Update()
        {
            if (parentPhotonView != null && !parentPhotonView.IsMine) return;

            // Check the tank is visible by any camera.
            if (Parent_Script.Is_Visible)
            {
                Rotate();
            }
        }


        void Rotate()
        {
            currentAng = thisTransform.localEulerAngles;

            if (Is_Left)
            { // Left
                currentAng.y = Parent_Script.Left_Angle_Y;
            }
            else
            { // Right
                currentAng.y = Parent_Script.Right_Angle_Y;
            }

            thisTransform.localEulerAngles = currentAng;
        }


        void Track_Destroyed_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Disable this script.
            this.enabled = false;

            // Switch the flag.
            isRepairing = true;
        }


        void Track_Repaired_Linkage(bool isLeft)
        { // Called from "Damage_Control_Center_CS".
            if (isLeft != Is_Left)
            { // The direction is different.
                return;
            }

            // Enable this script.
            this.enabled = true;

            // Switch the flag.
            isRepairing = false;
        }


        void Pause(bool isPaused)
        { // Called from "Game_Controller_CS".

            // Check the track is being repaired.
            if (isRepairing)
            {
                return;
            }

            this.enabled = !isPaused;
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (thisTransform)
                    stream.SendNext(thisTransform.localEulerAngles);
            }
            else
            {
                if (thisTransform)
                    thisTransform.localEulerAngles = (Vector3)stream.ReceiveNext();
            }
        }
    }

}