using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public PlayerMovement play;
    public float mouseSensitivity = 5;
    float xRotation = 0f;
    public Transform playerBody;
    public GameObject cam;

    //alpha values
    public float damage;
    public float reloadTime;
    public float dist;
    public float currAmmoP;
    public bool shootReady;
    public TextMeshProUGUI pistolAmmoText;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        shootReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, play.tilt);
        playerBody.Rotate(Vector3.up * mouseX);

        //alpha shooting controls
        damage = 2;
        reloadTime = 0.2f;
        dist = 1000f;
        pistolAmmoText.text = "Ammo: " + currAmmoP;

        if (Input.GetKey(KeyCode.Mouse0) && shootReady){
            ShootPistol();
        }
    }

    void ShootPistol(){
        RaycastHit hit;
        StartCoroutine(reload());
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dist)){
            Debug.Log(hit.transform.name);
            currAmmoP--;
        }
    }

    IEnumerator reload()
    {
        shootReady = false;
        yield return new WaitForSeconds(reloadTime);
        shootReady = true;
    }
}
