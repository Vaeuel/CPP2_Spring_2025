using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrapper : Singleton<BootStrapper>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void BootStrapGame()
    {
        CheckScene("Bootstrap");
    }
    private void Start()
    {
        //CheckScene("Game");
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public static void CheckScene(string sceneName)
    {
        // traverse the currently loaded scene if it's loaded otherwise exit
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName)
                return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}

