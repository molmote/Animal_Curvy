using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BaseWidgetEntity : MonoBehaviour 
{
    protected     List<UIButton>                          buttonList;
    protected     Dictionary<string, UIButton>            buttonDict;
    private       Dictionary<string, UIEventListener>     buttonListenerDict;


    protected virtual void Awake() 
    {
        this.buttonList             = new List<UIButton>();
        this.buttonDict             = new Dictionary<string, UIButton>();
        this.buttonListenerDict     = new Dictionary<string, UIEventListener>();
    }
     //<<-----------------------------------------------------------------------
    public static GameObject FindChild(GameObject rootObject, string name)
    {
        Queue<Transform> queueTransform = new Queue<Transform>();

        queueTransform.Enqueue(rootObject.transform);

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

    /*protected virtual void Awake() 
    {
        this.enabled                = false;

        // gameObject.GetComponent<UIPanel>().enabled = true;
        this.transform.parent       = this.FindChild(GameController.instance.gameObject, this.parentName).transform;

        gameObject.SetActive(false);

        // this.SetLayer(0);
    }*/

   //<<-----------------------------------------------------------------------
    public GameObject FindChild(string name)
    {
        Queue<Transform> queueTransform = new Queue<Transform>();

        queueTransform.Enqueue(gameObject.transform);

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
        
        queueTransform.Clear();
        queueTransform = null;

        return null;
    }

    //<<-----------------------------------------------------------------------
    protected GameObject FindChild(string tag, 
                                   string name)
    {
        GameObject rootObject = this.FindChild(tag);

        Queue<Transform> queueTransform = new Queue<Transform>();
        
        queueTransform.Enqueue(rootObject.transform);

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
    protected T GetComponent<T>(string tag, string name) where T : UnityEngine.Component
    {
        GameObject go = this.FindChild(tag, name);
        if ( go != null )
        return go.GetComponent<T>();
        else 
            return null;
    }

    protected T AddComponent<T>(string tag, string name) where T : UnityEngine.Component
    {
        return this.FindChild(tag, name).AddComponent<T>();
    }
    
    //<<-----------------------------------------------------------------------
    protected T GetComponent<T>(string name) where T : UnityEngine.Component
    {
        GameObject go = this.FindChild(name);
        if ( go != null )
        return go.GetComponent<T>();
        else 
            return null;
    }

    public static T GetComponent<T>(GameObject tagObj, string name) where T : UnityEngine.Component
    {
        GameObject go = FindChild(tagObj, name);
        if ( go != null )
        return go.GetComponent<T>();
        else 
            return null;
    }

    //<<-----------------------------------------------------------------------
    protected T AddComponent<T>(string name) where T : UnityEngine.Component
    {
        return this.FindChild(name).AddComponent<T>();
    }

    protected GameObject AddOnClickListener(GameObject btn, UIEventListener.VoidDelegate OnClick)
    {
        return this.AddOnClickListener(btn, btn.name, OnClick);
    }

    protected GameObject AddOnDragListener( GameObject btn,
                                            UIEventListener.VectorDelegate OnDrag)
    {
        return this.AddOnDragListener( btn, btn.name, OnDrag);
    }

    protected GameObject AddOnDragListener( string tag, 
                                            string name, 
                                            UIEventListener.VectorDelegate OnDrag)
    {
        GameObject btn = this.FindChild(tag, name);
        return this.AddOnDragListener( btn, tag+name, OnDrag);
    }

    protected GameObject AddOnDragListener( string name, 
                                            UIEventListener.VectorDelegate OnDrag)
    {
        GameObject btn = this.FindChild(name);
        return this.AddOnDragListener( btn, name, OnDrag);
    }

    protected GameObject AddOnDragListener( GameObject btn, string _name, UIEventListener.VectorDelegate OnDrag)
    {
        if(btn == null)
        {
            MyLogger.LogError("BUTTON NOT FOUND","BUTTON("+name+") NOT FOUND ERROR");
        }
        else
        {
            UIButton uiButton       = btn.GetComponent<UIButton>();
            UIEventListener events  = btn.GetComponent<UIEventListener>();
            if(events == null)
            {
                events = btn.AddComponent<UIEventListener>();
            }
            events.onDrag = OnDrag;

            if(this.buttonDict.ContainsKey(_name) == false)
            {
                if(uiButton != null)
                {
                    this.buttonDict.Add(_name, uiButton);
                    this.buttonList.Add(uiButton);
                }
                
                this.buttonListenerDict.Add(_name, events);
            }
        }
        return btn;
    }

    protected GameObject AddOnClickListener(GameObject btn, string _name, UIEventListener.VoidDelegate OnClick)
    {
        if(btn == null)
        {
            MyLogger.LogError("BUTTON NOT FOUND","BUTTON("+name+") NOT FOUND ERROR");
        }
        else
        {
            if(this.buttonDict.ContainsKey(_name))
            {

            }
            else
            {
                UIButton uiButton       = btn.GetComponent<UIButton>();
                UIEventListener events  = btn.GetComponent<UIEventListener>();

                if(events == null)
                {
                    events = btn.AddComponent<UIEventListener>();
                }

                events.onClick = OnClick;

                if(uiButton != null)
                {
                    this.buttonDict.Add(_name, uiButton);
                    this.buttonList.Add(uiButton);
                }
                
                this.buttonListenerDict.Add(_name, events);
            }
        }
        return btn;
    }    

    protected GameObject AddOnClickListener(string tag, 
                                            string name, 
                                            UIEventListener.VoidDelegate OnClick)
    {
        GameObject btn = this.FindChild(tag, name);
        this.AddOnClickListener(btn, tag+name, OnClick);
        return btn;
    }

    protected GameObject AddOnClickListener(string name, UIEventListener.VoidDelegate OnClick)
    {
        GameObject btn = this.FindChild(name);
        this.AddOnClickListener(btn, OnClick);
        return btn;
    }
    
    protected UIButton GetButton(string name)
    {
        if(this.buttonDict.ContainsKey(name))
        {
            return this.buttonDict[name];
        }
        else
        {
            return null;
        }
    } 

    protected void SetEnableAll(bool isEnabled)
    {
        foreach(UIButton button in this.buttonList)
        {
            button.isEnabled = isEnabled;
        }
    }

    public void SetLayerInChildren(int layerNumber) 
    {
        //foreach (Transform trans in this.GetComponentsInChildren<Transform>(true)) 
        this.gameObject.layer = layerNumber;
        //foreach( Transform trans in this.transform )
        foreach (Transform trans in this.GetComponentsInChildren<Transform>(true)) 
        {
            trans.gameObject.layer = layerNumber;
        }
    }
}
