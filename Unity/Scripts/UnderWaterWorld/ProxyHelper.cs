using UnityEngine;
using UnityEngine.SceneManagement;

public class ProxyHelper : MonoBehaviour
{
    public static ProxyHelper instance { get; private set; }

    private bool changedScene = false;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance);
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public int GetSceneIndex()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.buildIndex;
    }

    public GameObject FindMyGameObject(string tagName)
    {
        return GameObject.FindWithTag(tagName);
    }
}
