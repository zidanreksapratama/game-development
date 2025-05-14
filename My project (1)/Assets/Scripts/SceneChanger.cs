using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void GantiScene(string namaScene)
    {
        SceneManager.LoadScene(namaScene);
    }
}
