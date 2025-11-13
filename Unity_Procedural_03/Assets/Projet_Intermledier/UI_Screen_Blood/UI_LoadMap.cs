using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LoadMap : MonoBehaviour
{
    private string REF_MAP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        REF_MAP = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    public void Relance_La_Game()
    {
        print(REF_MAP);
        SceneManager.LoadScene(REF_MAP, LoadSceneMode.Single);
    }
    public void QuitGame_Level()
    {
        Debug.Log("Quit Game called");
        Application.Quit();
    }
}
