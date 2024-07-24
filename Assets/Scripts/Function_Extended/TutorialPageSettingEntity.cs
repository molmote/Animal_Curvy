using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TutorialPageSettingEntity : MonoBehaviour//BaseWidgetEntity
{
    public List<TutorialObject> PageList = new List<TutorialObject>();

    [System.Serializable]
    public class TutorialObject
    {
        //[Tooltip("구분용 이름", "구분용 이름입니다.\n게임상에서 사용되지는 않습니다.")]
        public string Name;

        //[Tooltip("페이지", "페이지입니다.", typeof(GameObject))]
        public GameObject page;

        //[Tooltip("이벤트 Collider", "설정된 Collider를 누르면 다음페이지로 넘어갑니다.", typeof(GameObject))]
        public GameObject Collider = null;

        //[Tooltip("타겟 UI", "선택한 UI에 맞게 UI 경로로 찾아들어가서 다음페이지로 넘어가는 이벤트를 등록합니다.", typeof(E_UIList))]
        public E_UIList targetUI = E_UIList.NONE;

        //[Tooltip("타겟 경로", "UI 경로입니다. (Camera/Anchor/Panel/Button)")]
        public string targetPath;
        
        //[Tooltip("자동 다음페이지", "설정된 시간 뒤에 자동으로 다음페이지로 넘어갑니다.")]
        public float AutoNextDelay = 0;

        //[Tooltip("타겟 UI", "선택한 UI에 맞게 UI 경로로 찾아들어가서 다음페이지로 넘어가는 이벤트를 등록합니다.", typeof(E_UIList))]
        public TUTORIAL_EVENT pageEvent = TUTORIAL_EVENT.NONE;
    }

    public enum TUTORIAL_EVENT
    {
        NONE,
        LOADING_BEFOREHAND,     
    }

    private int index = 0;
    private int count = 0;
    private float elapsed = 0.0f;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            foreach (UISprite sprite in GetComponentsInChildren<UISprite>())
            {
                if (sprite.atlas.spriteMaterial.shader.name.Contains("Transparent Colored"))
                {
                    sprite.atlas.spriteMaterial.shader = Shader.Find("Unlit/Transparent Colored");
                }
            }
        }

        foreach (var item in PageList)
        {
            item.page.SetActive(false);
        }

        NextPage();
    }

    /// <summary>
    /// 다음 페이지로 넘김
    /// </summary>
    public void NextPage()
    {
        count = 0;

        if (PageList.Count > index)
        {
            //StopCoroutine("AutoNextDelay");
            // 이전 페이지 끔
            if (index - 1 >= 0)
            {
                PageList[index - 1].page.SetActive(false);
            }

            // 페이지 켜줌
            PageList[index].page.SetActive(true);
            if (PageList[index].Collider != null)
            {
                AddOnClickListener(PageList[index].Collider);
                //UIEventListener.Get(PageList[index].Collider).onClick += nextPage;
            }
            else
            {
                MyLogger.Log("DEBUG", "collider is null!!!");
            }

            if (PageList[index].AutoNextDelay > 0)
            {
                //StartCoroutine("AutoNextDelay", PageList[index].AutoNextDelay);
            }

            if (PageList[index].pageEvent == TUTORIAL_EVENT.LOADING_BEFOREHAND)
            {
                TutorialManager.instance.OnDestroyTutorial();
            }

            //region Next Page로 넘길때 붙여질 이벤트 설정

            switch (PageList[index].targetUI)
            {
                case E_UIList.CharacterPopup :
                    //this.SetFindPathGO(PageList[index].targetPath, MainUIEntityManager.instance.GetTutorialTarget(PageList[index].targetUI));
                    break;

                case E_UIList.BuddyPopup :
                    //this.SetFindPathGO(PageList[index].targetPath, MainUIEntityManager.instance.GetTutorialTarget(PageList[index].targetUI));
                    break;

                case E_UIList.GameMenuUI:
                    //this.SetFindPathGO(PageList[index].targetPath, GameUIManager.instance.transform);
                    break; 

                default:
                        MyLogger.Red( "Toturial->NextPage", "INVALID TUTORIAL SELECTED!!");
                    break;
            }

            //endregion

            ++index;
        }
        else
        {
            // 꺼짐

            TutorialManager.instance.OnDestroyTutorial();
            Destroy(gameObject);
            //SetActive(false);
        }
    }

    /*IEnumerator AutoNextDelay(float time)
    {
        yield return new WaitForSeconds(time);
        NextPage();
    }*/

    void Update()
    {
        if(PageList[index-1].AutoNextDelay > 0 )        
        {
            MyLogger.Red("DEBUG", "elpased : " + elapsed);
            elapsed += Time.deltaTime;
            if (elapsed >= PageList[index-1].AutoNextDelay)
            {
                elapsed = 0.0f;
                //index++;
                NextPage();
            }
        }
    }

    void AddOnClickListener(GameObject go)
    {
        UIEventListener events = go.GetComponent<UIEventListener>();
        if(events == null)
            events = go.AddComponent<UIEventListener>();

        events.onClick += nextPage;
        //NextPage();
    }

    void nextPage(GameObject go)
    {
        UIEventListener.Get(go).onClick -= nextPage;
        NextPage();
    }

    void SetFindPathGO(string path, Transform tf)
    {
        if(path == "")
            return;

        Transform found = this.GetComponent<Transform>(tf,path);

        AddOnClickListener(found.gameObject);
        //UIEventListener.Get(found.gameObject).onClick += nextPage;
    }

    //<<-----------------------------------------------------------------------
    protected GameObject FindChild(Transform t, string name)
    {
        Queue<Transform> queueTransform = new Queue<Transform>();
        
        queueTransform.Enqueue(t);

        while(queueTransform.Count > 0)
        {
            Transform childForm = queueTransform.Dequeue();
            foreach(Transform childTransform in childForm)
            {
                if(childTransform.name == name)
                {
                    return childTransform.gameObject;
                }
                else
                {
                    queueTransform.Enqueue(childTransform);
                }
            }
        }
        return null;
    }

    //<<-----------------------------------------------------------------------
    public T GetComponent<T>(Transform t, string name) where T : UnityEngine.Component
    {
        return this.FindChild(t, name).GetComponent<T>();
    }
}
