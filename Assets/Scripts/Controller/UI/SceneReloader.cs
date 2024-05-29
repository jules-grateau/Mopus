using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Controller.UI
{
    public class SceneReloader : MonoBehaviour
    {
        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}