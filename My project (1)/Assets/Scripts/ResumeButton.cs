using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumeButton : MonoBehaviour
{
    public void ResumeGame()
    {
        string lastScene = PlayerPrefs.GetString("LastLevel", "");

        if (!string.IsNullOrEmpty(lastScene))
        {
            Time.timeScale = 1f; // Pastikan game tidak dalam keadaan pause
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("Belum ada progress tersimpan!");
            // Bisa dialihkan ke scene default
            SceneManager.LoadScene("Level1"); // ganti sesuai nama scene pertama
        }
    }
}
