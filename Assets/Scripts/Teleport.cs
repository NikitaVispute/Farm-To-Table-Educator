/*
 * 
 * Teleportation script for 5UDE VR simulator.
 * Author: Yash Mewada
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class Teleport : MonoBehaviour {

   
    // Inspector parameters
    [Tooltip("The tracking device used to determine absolute direction for steering.")]
    public CommonTracker tracker;

   

    [Tooltip("A button required to be pressed to activate steering.")]
    public CommonButton teleportButton;


    [Tooltip("The space that is translated by this interaction. Usually set to the physical tracking space.")]
    public CommonSpace space;

    [Tooltip("The median speed for movement expressed in meters per second.")]
    public float speed = 1.0f;


    ////Line Renderer for bezier Curve
    //public GameObject teleportPointersPrefab;
    //private GameObject linePointer;


    //creates a laser for teleporting
    public GameObject laserPrefab;
    private GameObject laser;
    public GameObject laserInvalidPrefab;
    private GameObject laserInvalid;
    private Transform laserInvalidTransform;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject hitObject;


    //creating the reticle for target to teleport to.
    [Tooltip("The target marker for the teleporting script.")]
    public GameObject teleportReticlePrefab;
    public GameObject teleportReticleInvalidPrefab;

    private GameObject reticleInvalid;
    private Transform teleportReticleInvalidTransform;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    public bool shouldTeleport;



    // Use this for initialization
    void Start () {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        laserInvalid = Instantiate(laserInvalidPrefab);
        laserInvalidTransform = laserInvalid.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        reticleInvalid = Instantiate(teleportReticleInvalidPrefab);
        teleportReticleInvalidTransform = reticleInvalid.transform;
       
        // myText = GameObject.Find("Text").GetComponent<Text>();
        // myText.color = Color.clear;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (!teleportButton.GetPress())
        {
            laser.SetActive(false);
            laserInvalid.SetActive(false);
            reticle.SetActive(false);
            reticleInvalid.SetActive(false);
           

        }

        if(teleportButton.GetPress()){
            RaycastHit hit;
          
            if (Physics.Raycast(tracker.transform.position, tracker.transform.forward, out hit, teleportMask))
            {
                hitPoint = hit.point;
                hitObject = hit.transform.gameObject;


                if (hitObject.tag.Contains("canTeleport"))
                {
                    showLaser(hit, true);
                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                    shouldTeleport = true;
                }
                else
                {
                    showLaser(hit, false);
                    reticleInvalid.SetActive(true);
                    shouldTeleport = false;
                    reticle.SetActive(false);
                    teleportReticleInvalidTransform.position = hitPoint + teleportReticleOffset;
                }
            }
        }

        if (shouldTeleport && !teleportButton.GetPress())
        {
            laserInvalid.SetActive(false);
            TeleportToTarget();
        }





    }

    private void showLaser(RaycastHit hit,bool valid){

        if (valid)
        {
            laserInvalid.SetActive(false);
            reticleInvalid.SetActive(false);
            laser.SetActive(true);
            laser.transform.position = Vector3.Lerp(tracker.transform.position, hitPoint, .5f);
            laserTransform.LookAt(hitPoint);
            laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
        }
        else
        {
            laser.SetActive(false);
            reticleInvalid.SetActive(true);
            laserInvalid.SetActive(true);
            laserInvalid.transform.position = Vector3.Lerp(tracker.transform.position, hitPoint, .5f);
            laserInvalidTransform.LookAt(hitPoint);
            laserInvalidTransform.localScale = new Vector3(laserInvalidTransform.localScale.x, laserInvalidTransform.localScale.y, hit.distance);
        }

            

    }

    public void TeleportToTarget(){

        shouldTeleport = false;

        reticle.SetActive(false);

      
        space.transform.position = hitPoint;

    }



    //calculating one point for bezier curve.
    public Vector3 calculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {


        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }


}
