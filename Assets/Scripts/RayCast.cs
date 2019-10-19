/*
 * 
 * Teleportation script for 5UDE VR simulator.
 * Author: Yash Mewada
 * 
 */











using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class RayCast : MonoBehaviour {


    // Inspector parameters
    [Tooltip("The tracking device used to determine absolute direction for steering.")]
    public CommonTracker tracker;

 
    [Tooltip("A button required to be pressed to activate steering.")]
    public CommonButton raycastButton;

    


    [Tooltip("The space that is translated by this interaction. Usually set to the physical tracking space.")]
    public CommonSpace space;


    //[Header("Bounds for raycasting")]
    ////LineRenderer Prefab Useful for drawing outline 
    //public GameObject lineRendererPrefab;
    //private GameObject lineGenerator;

    //[Header("Raycast Boundary indicator")]
    //public GameObject raycastBoundary;
    //private ParticleSystem raycastParticle;

    [Header("Laser prefab and teleport reticle prefab")]
    //creates a laser for teleporting
    public GameObject laserPrefab;
    public GameObject raycastReticlePrefab;

    //enable invalid raycast
    public GameObject invalidLaserPrefab;
    private GameObject invalidLaser;
    private Transform invalidLaserTransform;
    


    //creating the reticle for target to teleport to.
    [Tooltip("The target marker for the teleporting script.")]

    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    private GameObject reticle;
    private Transform raycastReticleTransform;
    public Vector3 raycastReticleOffset;
    public LayerMask rayCastMask;


    [Header("Bounds for validity of raycast")]
    //Radius bounds.
    public float validRaycastRadius = 2.0f;
    public LayerMask detectableObjectLayer;


    [Header("CanvasUI Prefab to show the food info")]
    //CanvasUIPrefab
    //public GameObject canvasUIPrefab;
    public GameObject canvasUI;
    


    [Header("Image Data for information about food")]
    //Image for data
    public Sprite canvasPigImage;
    public Sprite canvasCornImage;
    public Sprite canvasWheatImage;
    public Sprite canvasChickenImage;
    public Sprite canvasCowImage;

    protected Image canvasUIimage;

    [Header("Sounds for the animal information")]
    public AudioClip pigInfoSound;
    public AudioClip cowInfoSound;
    public AudioClip wheatInfoSound;
    public AudioClip cornInfoSound;
    public AudioClip chickenInfoSound;
    public bool isAudioPlaying = false;
    private AudioSource infoAudioSource;

   

    // Use this for initialization
    void Start () {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        reticle = Instantiate(raycastReticlePrefab);
        raycastReticleTransform = reticle.transform;
        invalidLaser = Instantiate(invalidLaserPrefab);
        invalidLaserTransform = invalidLaser.transform;
        canvasUIimage = canvasUI.GetComponentInChildren<Image>();
        //lineGenerator = Instantiate(lineRendererPrefab);
        //raycastParticle = raycastBoundary.GetComponent<ParticleSystem>();
        
        

        
	}

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(tracker.transform.position, validRaycastRadius);
    }
    // Update is called once per frame
    void FixedUpdate () {

        if (!raycastButton.GetPress())
        {
            //setRaycastBoundParams();
            //raycastBoundary.SetActive(false);
            laser.SetActive(false);
            reticle.SetActive(false);
            canvasUI.SetActive(false);
            invalidLaser.SetActive(false);
          
        }

        if(raycastButton.GetPress()){
            //setRaycastBoundParams();
            //raycastBoundary.SetActive(true);
            RaycastHit hit;
           
            if (Physics.Raycast(tracker.transform.position, tracker.transform.forward, out hit, rayCastMask))
            {
                
                hitPoint = hit.point;
               
                if (insideObjectRaycastBounds(hitPoint))
                {
                    showLaser(hit,true);
                    raycastReticleTransform.position = hitPoint + raycastReticleOffset;
                    GameObject hitObject = hit.transform.gameObject;
                    Debug.Log(hitObject.name);
                    showUIAndGenerateCanvas(hitObject);
                }
                else
                {
                    showLaser(hit, false);
                    canvasUI.SetActive(false);
                }
              
                
                
                


            }
        }

      

    }



    //private void Update()
    //{
    //    //if (infoAudioSource != null)
    //    //{
    //    //    // Set the audiosource's loop setting
    //    //    infoAudioSource.loop = false;
    //    //    // Set the audiosource's volume
    //    //    infoAudioSource.volume = 2f;
    //    //    // Set the audiosource's minimum distance
    //    //    infoAudioSource.minDistance = 5;
    //    //    // Set the audiosource's maximum distance
    //    //    infoAudioSource.maxDistance = 100;
    //    //    // Set the audiosource's blend setting to 3D
    //    //    infoAudioSource.spatialBlend = 1.0f;
    //    //    Debug.Log("CLIP : " + infoAudioSource.clip.name);
    //    //    infoAudioSource.Play();
    //    //}
    //}




    //set raycast boundary particle system params;
    //private void setRaycastBoundParams()
    //{
    //    Vector3 particlePosition = new Vector3(tracker.transform.position.x, 0, tracker.transform.position.z);
    //    raycastBoundary.transform.position = particlePosition;
    //    raycastParticle.transform.position = particlePosition;

    //    return;

    //}

    private void showUIAndGenerateCanvas(GameObject hitObject)
    {
 
        if (hitObject.CompareTag("animal"))
        {
            //Debug.Log(hitObject.name);
            string selectedObjectName = hitObject.name.ToLower();
            Vector3 selectedObjectPosition = hitObject.transform.position;
            string animalName = getObjectName(selectedObjectName);

            //getting collider of animal and then setting the canvas ui above the height of collider
            Collider animalCollider = hitObject.GetComponent<Collider>();

           infoAudioSource = hitObject.GetComponent<AudioSource>();
           
            canvasUI.transform.eulerAngles = new Vector3(0, tracker.transform.eulerAngles.y, 0);
            canvasUI.transform.position = new Vector3(selectedObjectPosition.x, animalCollider.bounds.size.y+0.8f, selectedObjectPosition.z);
           
           // Debug.Log("Canvas OBJECT LOCATION : " + canvasUI.transform.position);
            canvasUIimage.transform.position = canvasUI.transform.position;
            if (animalName != "")
            {
                Sprite infoAboutAnimal = getFoodSources(animalName);
               
                if (infoAboutAnimal!= null)
                {

                    setUIDetails(animalName, infoAboutAnimal);
                  
                    
                }
                //myText.color = Color.Lerp(myText.color, Color.white, 6f * Time.deltaTime);
            }
        }
        else
        {
            //myText.color = Color.Lerp(myText.color, Color.clear, 5f * Time.deltaTime);
            canvasUI.SetActive(false);
        }
    }

    private AudioClip getAudioInfo(string animalName)
    {
        if (name == "pig")
        {
            return pigInfoSound;
        }
        else if (name == "cow")
        {

            Debug.Log("returning cow " );
            return cowInfoSound;
        }
        else if (name == "chicken")
        {
            return chickenInfoSound;
        }
        else if (name == "wheat")
        {
            return wheatInfoSound;
        }
        else if (name == "corn")
        {
            return cornInfoSound;
        }
        else
        {
            return null;
        }
    }

    private Sprite getFoodSources(string name){
        if(name == "pig"){
            return canvasPigImage;
        }else if(name == "cow"){
            return canvasCowImage;
        }else if(name == "chicken"){
            return canvasChickenImage;
        }else if(name == "wheat"){
            return canvasWheatImage;
        }else if( name == "corn"){
            return canvasCornImage;
        }else{
            return null;
        }


    }

    //Bound for raycast
   private bool insideObjectRaycastBounds(Vector3 hitPoint)
    {
       
        Collider[] hitColliders = Physics.OverlapSphere(tracker.transform.position, validRaycastRadius,detectableObjectLayer);
        //Debug.Log("Checking inside radius : " + (hitPoint - tracker.transform.position).sqrMagnitude);
        
        //Debug.Log("in sphere? :  " + ((hitPoint - tracker.transform.position).sqrMagnitude < validRaycastRadius * validRaycastRadius));
        if((hitPoint - tracker.transform.position).sqrMagnitude < validRaycastRadius * validRaycastRadius)
        {
            
            return true;
        }
        return false;

    }

    private void showLaser(RaycastHit hit, bool valid)
    {

        if (valid)
        {
            invalidLaser.SetActive(false);
            reticle.SetActive(true);
            //reticleInvalid.SetActive(false);
            laser.SetActive(true);
            laser.transform.position = Vector3.Lerp(tracker.transform.position, hitPoint, .5f);
            laserTransform.LookAt(hitPoint);
            laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
            invalidLaser.SetActive(true);
            invalidLaser.transform.position = Vector3.Lerp(tracker.transform.position, hitPoint, .5f);
            invalidLaserTransform.LookAt(hitPoint);
            invalidLaserTransform.localScale = new Vector3(invalidLaserTransform.localScale.x, invalidLaserTransform.localScale.y, hit.distance);
        }



    }
    private void setUIDetails(string name,  Sprite image){
        
        canvasUIimage.sprite = image;

        canvasUI.SetActive(true);

    }

    private string getObjectName(string selectedObjectName){
        string textResult = "";
        if(selectedObjectName.Contains("pig")){
            textResult = "pig";
        }else if(selectedObjectName.Contains("chicken")){
            textResult = "chicken";

        }
        else if(selectedObjectName.Contains("cow")){
            textResult = "cow";

        }
        else if(selectedObjectName.Contains("wheat")){
            textResult = "wheat";

        }
        else if(selectedObjectName.Contains("corn")){
            textResult = "corn";

        }


        return textResult;
    }


    


  
    

    




}
