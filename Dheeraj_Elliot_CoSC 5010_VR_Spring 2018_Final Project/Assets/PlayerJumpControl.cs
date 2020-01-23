using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpControl : MonoBehaviour {
    float gravity = 0;
    Vector3 moveDirection = Vector3.zero;
    bool onGuiShow = false;
    bool isParachuteDeployed = false;
    bool onParachuteDeployedGuiShow = false;
    bool hasJumped = false;
    private SteamVR_TrackedController VRcontroller;

    // Use this for initialization
    void Start () {
        VRcontroller = GetComponent<SteamVR_TrackedController>();
    }
	
	// Update is called once per frame
	void Update () {
        float drop_height = GameObject.Find("Capsule").transform.position.y;
        if (moveDirection.y > gravity * -1)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        //Project a message to user to deploy parachute and save his life
        if (drop_height <= 2500.00 && !isParachuteDeployed)
        {
            onGuiShow = true;
        }
        else
        {
            onGuiShow = false;
            onParachuteDeployedGuiShow = false;

        }

        if (isParachuteDeployed)
        {
            //steer parachute.
            float leftY = GameObject.Find("Controller (left)").transform.position.y;
            float rightY = GameObject.Find("Controller (right)").transform.position.y;

            float leftRight = leftY - rightY;
            float scale = Mathf.Abs(leftRight * .01f);

            if (leftRight < 0)
            {
                //GameObject.Find("[CameraRig]").transform.rotation = new Vector3(0.0f, 0.0f, 30f);
                if (GameObject.Find("[CameraRig]").transform.rotation.z < 30f)
                {
                    GameObject.Find("[CameraRig]").transform.Rotate(0.0f, 0.0f, 30f);
                }
                GameObject.Find("[CameraRig]").transform.Translate(1f, 0.0f, 0f);
            }
            else
            {

                GameObject.Find("[CameraRig]").transform.Rotate(0.0f, 0.0f, 30f);
                GameObject.Find("[CameraRig]").transform.Translate(1f, 0.0f, 0);
            }
        }

        if (VRcontroller.padPressed)
        {
            Debug.Log("Pad Clicked");
            if (!hasJumped)
            {
                GameObject.Find("[CameraRig]").transform.Rotate(90.0f, 0.0f, 0.0f);
                hasJumped = true;
                gravity = 100;
            }
        }

        if (VRcontroller.triggerPressed)
        {
            Debug.Log("Trigger Clicked");
            if (!isParachuteDeployed)
            {
                gravity = 0;
                GameObject.Find("[CameraRig]").transform.Rotate(-90.0f, 0.0f, 0.0f);
                onGuiShow = false;
                isParachuteDeployed = true;
            }
            else
            {
                onGuiShow = false;
                onParachuteDeployedGuiShow = true;
            }
        }
    }

    void OnGUI()
    {
        if (!hasJumped)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 400, 25), "Press any pad to jump out");
        else if (onGuiShow)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 400, 25), "Press any trigger to deploy the parachute");
        else if (onParachuteDeployedGuiShow)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 400, 25), "Parachute already deployed");
    }
}