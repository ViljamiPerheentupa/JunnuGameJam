using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Animator _anim;
    public void Awake()
    {
        GameObject.Find("MenuAmbiance").GetComponent<MenuAmbiance>().MenuAmbianceState(true);
    }
    public void StartGame()
    {
        _anim.Play("menu_blackout");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
