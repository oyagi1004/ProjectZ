//using System.Collections;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using UnityEngine;
//using System.IO;

//[System.Serializable]
//public class Dialogue
//{
//    public int Script_Id;
//    public string Script;
//    public int Actor;
//    public int Next_Id;
//}

//[System.Serializable]
//public class DataArray
//{
//    public List<Dialogue> DialogueList;
//}

//public class DialogueManager : MonoBehaviour {

//    public bool readNextScript = false;
//    public Dialogue ScriptData;

//    public delegate void DelegateAfterQuestion();
//    public DelegateAfterQuestion afterQuestion = null;

//    private static DialogueManager instance = null;


//    public static DialogueManager Instance{
//        get 
//        {
//            return instance;
//        }
//    }

//    void Awake()
//    {
//        if(instance)
//        {
//            DestroyImmediate(this);
//            return;
//        }
//        instance = this;

//        DontDestroyOnLoad(this);
//    }

//    DataArray list = new DataArray();


//	void Start () {
//        string data = File.ReadAllText(Application.dataPath + "/Resources/Data/TestJson.txt", System.Text.Encoding.UTF8);
//        //TextAsset data = Resources.Load("Data/" + "TestJson") as TextAsset;
//        list = JsonUtility.FromJson<DataArray>(data);
//    }
	
//    //scriptID로 Dialogue 구조체를 추출.
//    public Dialogue FindDialogueDataByID(int id)
//    {
//        foreach(Dialogue e in list.DialogueList)
//        {
//            if (e.Script_Id == id)
//                return e;
//        }

//        return null; 
//    }

//    // 해당 아이디의 scriptID로 대사추출(삭제 예정.)
//    public string FindScriptByID(int id)
//    {
//        return FindDialogueDataByID(id).Script;
//    }

//    public void SetDialogue(int scriptId)
//    {
//        StartCoroutine(DoNextScript(scriptId));
//    }

//    public void SetDialogue(int scriptId, NPCController.NPCDIALOGUETYPE type)
//    {
//        if (type == NPCController.NPCDIALOGUETYPE.DEFALUT)
//            StartCoroutine(DoNextScript(scriptId));
//        else if (type == NPCController.NPCDIALOGUETYPE.COMMERCE)
//        {
//            StartCoroutine(AnswerQuestion(scriptId, UIManager.Instance.ShowShopMenuPopUpWnd));
//        }
//    }

//    IEnumerator DoNextScript(int scriptID)
//    {
//        ScriptData = FindDialogueDataByID(scriptID);
//        UIManager.Instance.SetDialogueFont(ScriptData.Script);
//        yield return new WaitForSeconds(0.1f);
//        if (ScriptData.Next_Id != 0)
//        {
//            PlayerManager.Instance.bReadNextScript = true;
//            PlayerManager.Instance.bEndDialog = false;
//        }
//        else
//        {
//            PlayerManager.Instance.bReadNextScript = false;
//        }
//            yield return new WaitForSeconds(0.2f);
//    }
//    IEnumerator AnswerQuestion( int scriptID, DelegateAfterQuestion _delegate = null)
//    {
//        ScriptData = FindDialogueDataByID(scriptID);
//        UIManager.Instance.SetDialogueFont(ScriptData.Script);
//        yield return new WaitForSeconds(0.1f);
//        if(ScriptData.Next_Id != 0)
//        {
//            PlayerManager.Instance.bReadNextScript = true;
//            PlayerManager.Instance.bEndDialog = false;
//        }
//        else
//        {
//            //PlayerManager.Instance.bReadNextScript = false;
//            if (_delegate != null)
//                _delegate();
//        }
//        yield return new WaitForSeconds(0.2f);
//    }


    
//}
