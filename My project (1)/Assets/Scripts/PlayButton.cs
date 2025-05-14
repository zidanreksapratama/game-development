using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    // Nama scene yang ingin dimuat saat tombol Play ditekan
    public string sceneName = "Level1";

    public void PlayGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
