using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIControl : MonoBehaviour {

	public void TouchStartBtn()
    {
        SceneManager.LoadScene("TestScene");


    }
}
