//partially adapted from CharacterMovement script in Island Asset. Don't move the mouse while wearing the Vive or it will
//alter your view of the world. Gravity was implemented in this script previously.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
	float speed = 7;
	float gravity = 0;
	CharacterController controller;
	Vector3 moveDirection = Vector3.zero;
    bool onGuiShow = false;
    bool isParachuteDeployed = false;
    bool onParachuteDeployedGuiShow = false;
    bool hasJumped = false;
    private SteamVR_TrackedController LVRcontroller, RVRcontroller;

    // Use this for initialization
    void Start () {
		controller = transform.GetComponent<CharacterController>();
        
    }
	
	// Update is called once per frame
	void Update () {
        GameObject lCont = GameObject.Find("Controller (left)");
        GameObject rCont = GameObject.Find("Controller (right)");
        LVRcontroller = lCont.GetComponent<SteamVR_TrackedController>();
        RVRcontroller = rCont.GetComponent<SteamVR_TrackedController>();

        //APPLY GRAVITY
        float drop_height = GameObject.Find("Capsule").transform.position.y;
		if(moveDirection.y > gravity * -1) {
			moveDirection.y -= gravity * Time.deltaTime;
		}
		controller.Move(moveDirection * Time.deltaTime);
		var left = transform.TransformDirection(Vector3.left);

		/*if(controller.isGrounded) {
			if(Input.GetKeyDown(KeyCode.Space)) {
				moveDirection.y = speed;
			}
			else if(Input.GetKey("w")) {
				if(Input.GetKey(KeyCode.LeftShift)) {
					controller.SimpleMove(transform.forward * speed * 2);
				}
				else {
					controller.SimpleMove(transform.forward * speed);
				}
			}
			else if(Input.GetKey("s")) {
				if(Input.GetKey(KeyCode.LeftShift)) {
					controller.SimpleMove(transform.forward * -speed * 2);
				}
				else {
					controller.SimpleMove(transform.forward * -speed);
				}
			}
			else if(Input.GetKey("a")) {
				if(Input.GetKey(KeyCode.LeftShift)) {
					controller.SimpleMove(left * speed * 2);
				}
				else {
					controller.SimpleMove(left * speed);
				}
			}
			else if(Input.GetKey("d")) {
				if(Input.GetKey(KeyCode.LeftShift)) {
					controller.SimpleMove(left * -speed * 2);
				}
				else {
					controller.SimpleMove(left * -speed);
				}
			}
		}
		else {
			if(Input.GetKey("w")) {
				Vector3 relative = new Vector3();
				relative = transform.TransformDirection(0, 0, 1);
				if(Input.GetKey(KeyCode.LeftShift)) {
					controller.Move(relative * Time.deltaTime * speed * 2);
				}
				else {
					controller.Move(relative * Time.deltaTime * speed);
				}
			}
		}*///Commented out some of previous code due to steering implementation replacing it.

        //Project a message to person at computer to deploy parachute and save his life
        if(drop_height <= 2500.00 && !isParachuteDeployed)
        {
            onGuiShow = true;//Displays message
        }
        else
        {
            onGuiShow = false;
            onParachuteDeployedGuiShow = false;

        }

        if (isParachuteDeployed&&hasJumped)
        {
            //steer parachute.
            float leftY = GameObject.Find("Controller (left)").transform.position.y;
            float rightY = GameObject.Find("Controller (right)").transform.position.y;

            float leftRight = leftY - rightY;
            float scale = Mathf.Abs(leftRight * .5f);
            if (controller.isGrounded)//If you're on the ground, walk around by pressing either pad.
                //Steering implemented with vive headset.
            {
                if (LVRcontroller.padPressed||RVRcontroller.padPressed)
                {
                    GameObject.Find("[CameraRig]").transform.localPosition -= new Vector3(GameObject.Find("Camera (eye)").transform.forward.x * .05f, 0, GameObject.Find("Camera (eye)").transform.forward.z * .05f);
                }
            }
            else
            {
                if (leftRight < 0)
                {
                    //transform around forward's x-axis and forward.
                    GameObject.Find("[CameraRig]").transform.localRotation = Quaternion.Euler(30f * scale * GameObject.Find("Camera (eye)").transform.forward.x, GameObject.Find("Camera (eye)").transform.forward.y + 300f * scale, 30f * scale *GameObject.Find("Camera (eye)").transform.forward.z);
                    GameObject.Find("[CameraRig]").transform.localPosition += new Vector3(-GameObject.Find("Camera (eye)").transform.forward.x * .2f, 0.0f, -GameObject.Find("Camera (eye)").transform.forward.z * .1f);
                }
                else if (leftRight > 0)
                {
                    
                    
                    GameObject.Find("[CameraRig]").transform.localRotation = Quaternion.Euler(-30f * scale * GameObject.Find("Camera (eye)").transform.forward.x, GameObject.Find("Camera (eye)").transform.forward.y - 300f*scale, -30f * scale * GameObject.Find("Camera (eye)").transform.forward.z);
                        //GameObject.Find("[CameraRig]").transform.Rotate(0f, scale * -300f, 0f);
                    GameObject.Find("[CameraRig]").transform.localPosition += new Vector3(-GameObject.Find("Camera (eye)").transform.forward.x * .2f, 0.0f, -GameObject.Find("Camera (eye)").transform.forward.z * .1f);

                }
            }
        }

        if (LVRcontroller.padPressed || RVRcontroller.padPressed)
        {
            //Debug.Log("Pad Clicked");abarber3@uwyo.edu
            if (!hasJumped)//JUMP OUT OF HELICOPTER!!! (Press pad on vive controller)
            {
                GameObject.Find("[CameraRig]").transform.Rotate(90.0f, 0.0f, 0.0f);
                hasJumped = true;
                gravity = 100;
            }
        }

        if (hasJumped && (LVRcontroller.triggerPressed || RVRcontroller.triggerPressed))
        {
            //Debug.Log("Trigger Clicked");
            if (!isParachuteDeployed)//DEPLOY PARACHUTE!!! (Press trigger on vive controller)
            {
                moveDirection = Vector3.zero;
                gravity = 15;
                GameObject.Find("[CameraRig]").transform.Rotate(90.0f, 0.0f, 0.0f);
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
        if(!hasJumped)
             GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 400, 25), "Press pad to jump out!!");
        else if(onGuiShow)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 400, 25), "Press trigger to deploy the parachute!!");
        else if (onParachuteDeployedGuiShow)
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 400, 25), "Parachute already deployed you blockhead!!");
    }

}
