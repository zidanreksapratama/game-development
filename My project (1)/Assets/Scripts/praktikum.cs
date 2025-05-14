using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class praktikum : MonoBehaviour
{
    public int health = 100;
    float speed = 5.5f;
    bool isAlive = true;
    string namaPlayer = "sadako";
    char grade = 'A';
    // Start is called before the first frame update
    void Start()
    {
        // // Debug.Log("Healt Player : " + health);

        // int damage = 20;
        // health = health - damage;
        // Debug.Log("Darah Sekarang " + health);

        // bool isDead = (health <= 0);
        // Debug.Log("Apakah Player Mati? " + isDead);

        // if(isAlive && health > 0)
        // {
        //     Debug.Log("Pemain masih hidup");
        // }
        // else
        // {
        //     Debug.Log("Pemain sudah mati");
        // }



        // for (int i = 1; i <=5; i++)
        // {
        //     Debug.Log("Hit ke - " + i);
        // }


        // GetPlayerName();

        // StartCoroutine(contohCorotine());

        string status = GetPlayerStatus(80);
        Debug.Log("Status Pemain " + status);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void GetPlayerName()
    // {
    //     Debug.Log(namaPlayer);
    // }



    // IEnumerator contohCorotine()
    // {
    //     Debug.Log("Mulai Corotine");
    //     yield return new WaitForSeconds(2f);
    //     Debug.Log("Corotine Selesai");
    // }



    string GetPlayerStatus(int darah)
    {
        if (darah > 50)
            return "Hidup";
        else if (darah > 0)
            return "Lemah";
        else
            return "Mati";
    }
}
