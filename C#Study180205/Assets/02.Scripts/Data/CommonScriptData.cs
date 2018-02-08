using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonScript
{
    public int Id;
    public string Script_KOR;
    public string Script_ENG;

    public CommonScript(int id, string script_kor, string script_eng)
    {
        Id = id;
        Script_KOR = script_kor;
        Script_ENG = script_eng;
    }
}

public class CommonScriptData : MonoBehaviour {
    List<CommonScript> list = new List<CommonScript>();

    private static CommonScriptData instance = null;

    public static CommonScriptData Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);

        List<Dictionary<string, object>> data = CSVParser.Read("CommonScript");

        foreach(Dictionary<string, object> d in data)
        {
            CommonScript c = new CommonScript(int.Parse(d["SCRIPT_ID"].ToString()),
                                              d["SCRIPT_KOR"].ToString(),
                                              d["SCRIPT_ENG"].ToString());

            

            list.Add(c);
        }
    }

    public CommonScript FindCommonScriptDataByID(int scriptID)
    {
        foreach(CommonScript c in list)
        {
            if (scriptID == c.Id)
                return c;
        }

        return null;
    }

    public string FindCommonScriptByID(int scriptID)
    {
        foreach(CommonScript c in list)
        {
            if(scriptID == c.Id)
            {
                //여기서 옵션값에 따라 해당 국가 언어 출력!
                return c.Script_KOR;
            }
        }
        return "null";
    }

}
