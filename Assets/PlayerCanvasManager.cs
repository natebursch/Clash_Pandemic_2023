using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerCanvasManager : MonoBehaviourPunCallbacks
{
    public PlayerManager playerManger;
    MouseLook mouseLook;
    PlayerMovement movement;
    public GameObject[] weapons;

    public PhotonView photonView;


    public GameObject missionAnnoucement_Screen;
    public TextMeshProUGUI missionAnnoucement_Text;
    public TextMeshProUGUI missionAnnoucement_ToolTip;
    public float show_missionAnnouncement_time = 3f;
    public float missionAnnoucement_timer = 0;


    //esc key settings / options settings so on
    public GameObject pauseScreen;
    //senstivity
    public Slider norm_sensitivitySlider;
    public TMP_InputField norm_sensitivityInputField;

    public Slider aim_sensitivitySlider;
    public TMP_InputField aim_sensitivityInputField;

    //volume
    //master volume
    public Slider masterVolume_slider;
    public TMP_InputField masterVolume_InputField;


    public void Start()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }

        mouseLook = GetComponentInChildren<MouseLook>();
        movement = GetComponent<PlayerMovement>();
        weapons = playerManger.weaponHolder;

        SetPlayerSettings();

        pauseScreen.SetActive(false);
        missionAnnoucement_Screen.SetActive(false);

        
    }
    public void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }


        // look for esc key being pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if it is already enabled deactivate
            if (pauseScreen.activeSelf)
            {
                DisableCharacter(false);
                pauseScreen.SetActive(false);
                PlayerPrefs.Save();
            }
            //if not active activate it
            else
            {
                DisableCharacter(true);
                pauseScreen.SetActive(true);
                PlayerPrefs.Save();

            }

        }
    }
    private void OnApplicationQuit()
    {
        SetPlayerSettings();
        PlayerPrefs.Save();
    }


    public void SetPlayerSettings()
    {

        //Debug.Log(PlayerPrefs.GetFloat("Normal_Sensitivity").ToString());
        //Debug.Log(PlayerPrefs.GetFloat("Aim_Sensitivity").ToString());
        //Debug.Log(PlayerPrefs.GetFloat("Master_Volume").ToString());

        // Senstivity

        ChangeNormalSensitivity(PlayerPrefs.GetFloat("Normal_Sensitivity"));
        norm_sensitivitySlider.value = PlayerPrefs.GetFloat("Normal_Sensitivity");
        norm_sensitivityInputField.text = PlayerPrefs.GetFloat("Normal_Sensitivity").ToString();

        ChangeAimSensitivity(PlayerPrefs.GetFloat("Aim_Sensitivity"));
        aim_sensitivitySlider.value = PlayerPrefs.GetFloat("Aim_Sensitivity");
        aim_sensitivityInputField.text = PlayerPrefs.GetFloat("Aim_Sensitivity").ToString();

        //Volume
        ChangeMasterVolume(PlayerPrefs.GetFloat("Master_Volume"));
        masterVolume_slider.value = PlayerPrefs.GetFloat("Master_Volume");
        masterVolume_InputField.text = PlayerPrefs.GetFloat("Master_Volume").ToString();

        //Debug.Log(PlayerPrefs.GetFloat("Normal_Sensitivity").ToString());
        //Debug.Log(PlayerPrefs.GetFloat("Aim_Sensitivity").ToString());
        //Debug.Log(PlayerPrefs.GetFloat("Master_Volume").ToString());

    }

    public void DisableCharacter(bool state)
    {
        mouseLook.enabled = !state;
        movement.enabled = !state;
        
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<WeaponManager>().enabled = !state;
        }

        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
            //AudioListener.volume = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            //AudioListener.volume = 1;
        }
        
        
    }



    #region Mission Announcement Text
    public void ShowMissionAnnouncement(string announcement,string tooltip, float timeToShow)
    {
        Debug.Log("HELLO???00");
        missionAnnoucement_Screen.SetActive(true);
        missionAnnoucement_Text.text = announcement;
        missionAnnoucement_ToolTip.text = tooltip;

        StartCoroutine(Hide_Annoucement_AfterDelay(timeToShow));
    }
    public IEnumerator Hide_Annoucement_AfterDelay(float timeToShow)
    {
        Debug.Log("Penis");
        yield return new WaitForSeconds(timeToShow);
        missionAnnoucement_Screen.SetActive(false);

    }


    #endregion

    #region Volume Settings
    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value / 100;
    }
    public void MasterVolume_Slider_Changed()
    {
        Debug.Log("Volume" + masterVolume_slider.value);
        if (masterVolume_InputField.text != masterVolume_slider.value.ToString())
        {
            masterVolume_InputField.text = "" + masterVolume_slider.value;
        }

        PlayerPrefs.SetFloat("Master_Volume", masterVolume_slider.value);
        ChangeMasterVolume(masterVolume_slider.value);

        
        
    }
    public void MasterVolume_InputField_Changed()
    {

        if (masterVolume_InputField.text != masterVolume_slider.value.ToString())
        {
            masterVolume_slider.value = float.Parse(masterVolume_InputField.text);
        }

        PlayerPrefs.SetFloat("Master_Volume", masterVolume_slider.value);
        ChangeMasterVolume(masterVolume_slider.value);

        
    }
    #endregion
    #region Sensitivity Settings
    public void ChangeNormalSensitivity(float sens)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<WeaponManager>().normalSensitivity = sens;
        }
    }
    public void ChangeAimSensitivity(float sens)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<WeaponManager>().aimSensitivity = sens;
        }
    }
    // Normal Sensitivity
    public void NormalSensitivity_SliderChanged()
    {
        //Debug.Log(norm_sensitivitySlider.value);
        norm_sensitivityInputField.text = "" + norm_sensitivitySlider.value;

        ChangeNormalSensitivity(norm_sensitivitySlider.value);

        PlayerPrefs.SetFloat("Normal_Sensitivity", norm_sensitivitySlider.value);
    }
    public void NormalSensitivity_InputFieldChanged()
    {
        //Debug.Log(sensitivityInputField.text);
        norm_sensitivitySlider.value = float.Parse(norm_sensitivityInputField.text);

        ChangeNormalSensitivity(norm_sensitivitySlider.value);

        PlayerPrefs.SetFloat("Normal_Sensitivity", norm_sensitivitySlider.value);
    }

    //Aim Sensitivity
    public void AimSensitivity_SliderChanged()
    {
        //Debug.Log(sensitivitySlider.value);
        aim_sensitivityInputField.text = "" + aim_sensitivitySlider.value;

        ChangeAimSensitivity(aim_sensitivitySlider.value);

        PlayerPrefs.SetFloat("Aim_Sensitivity", aim_sensitivitySlider.value);
    }
    public void AimSensitivity_InputFieldChanged()
    {
        //Debug.Log(sensitivityInputField.text);
        aim_sensitivitySlider.value = float.Parse(aim_sensitivityInputField.text);

        ChangeAimSensitivity(aim_sensitivitySlider.value);

        PlayerPrefs.SetFloat("Aim_Sensitivity", aim_sensitivitySlider.value);
    }


    #endregion

}
