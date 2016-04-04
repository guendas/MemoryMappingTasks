﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TestTrial : MonoBehaviour
{

    public GameObject player, oculus_player, treasure, instruction_menu, feedbackEndTrial,tornadoFeedback, tornado;
    public GameObject[] tornados;
    int menu_score, score_ui;
    int score, dummy = 0;
    bool showFeedback, tilted,startRot = false;
    float time_anim;
    public Text score_gained, gems_info;
    public static GameObject collectObj;
    public static Vector3 goal_position, last_position;
    public static float pause_t, trial_t, collision_t, distance = 0.0f;
    public static string _FileName, score_text = "";

    //Called before any other function
    void Awake()
    {
        UsefulFunctions.old_trial = UsefulFunctions.current_trial;
        //If status is ok activates the right player GameObject, checking if Oculus toggle was on or off
        switch (PlayerPrefs.GetInt("Oculus"))//PlayerPrefs.GetInt("Oculus")
        {
            //Default case
            case 0:
                player.SetActive(true);
                oculus_player.SetActive(false);
                player.GetComponent<CharacterController>().enabled = false;
                player.GetComponent<MouseLook>().enabled = false;
                Debug.Log("Activate player prefab");
                break;
            //Oculus case
            case 1:
                //player.SetActive(false);
                oculus_player.SetActive(true);
                Debug.Log("Activate Oculus OVR Player");
                break;
        }
        //Randomize gem position and assign it to the variable
        collectObj = UsefulFunctions.ChooseGem(collectObj);
        //Randomize Player and Treasure position
        UsefulFunctions.RndPositionObj(player); //Randomize player and treasure chest position
        //Randomize number of collectable obj
        menu_score = 3;
        //Update score and gems UI
        gems_info.text = "GEMS: " + score.ToString() + "/" + menu_score.ToString();
        score_gained.text = "0";
        treasure.GetComponent<Animator>().enabled = false;
        feedbackEndTrial.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        //Show exp instructions
        instruction_menu.SetActive(true);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        trial_t += Time.deltaTime; //updates time
        if (score == menu_score)
        {
            if(time_anim == (trial_t + 5))
            {
                foreach (GameObject g in tornados)
                    g.GetComponent<ParticleSystem>().enableEmission = false;
            }
            feedbackEndTrial.GetComponent<Animator>().SetBool("hasToGoBack", showFeedback);
            if (UsefulFunctions.OnButtonPression() == true)
            {
                score_text = score_gained.text;
                Application.LoadLevel(6); //Feedback level
            }
        }
        if (treasure.GetComponent<Animator>().isActiveAndEnabled)
        {
            dummy++;
            if (dummy == 80)
            {
                treasure.SetActive(false);
                player.GetComponent<MouseLook>().enabled = true;
                tilted = true;
                player.GetComponent<Animator>().SetBool("Tilted", tilted);
                startRot = true;
            }
        }
        if (player.transform.rotation.eulerAngles.x == 0.0f && startRot)
        {
            player.GetComponent<Animator>().enabled = false;
            startRot = false;
        }
    }

    //Object collision
    public void OnTriggerEnter(Collider col)
    {
        AudioSource audio = player.GetComponent<AudioSource>();
        audio.Play();
        score++;
        gems_info.text = "GEMS: " + score.ToString() + "/" + menu_score.ToString();
        collision_t = trial_t;
        collectObj.SetActive(false);
        if (score != menu_score)
        {
            collectObj = UsefulFunctions.ChooseGem(collectObj);
        }
        else
        {
            player.GetComponent<CharacterController>().enabled = false;
            tornado.SetActive(true);
            foreach (GameObject g in tornados)
                g.SetActive(true);
            tornadoFeedback.SetActive(true);
            StartCoroutine(WaitForIt(5f));
            //feedbackEndTrial.SetActive(true);
            //showFeedback = true;
        }
    }

    IEnumerator WaitForIt(float seconds)
    {
        Debug.Log("Waiting START..." + Time.time);
        yield return new WaitForSeconds(seconds);
        UsefulFunctions.RndPositionObj(player);
        Debug.Log("Waiting END..." + Time.time);
        tornadoFeedback.SetActive(false);
        tornado.SetActive(false);
        foreach (GameObject g in tornados)
            g.SetActive(false);  
    }

}



