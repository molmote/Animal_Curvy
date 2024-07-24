using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BaseEntityManager  : BaseWidgetEntity
{
    //protected BaseController                        controller;

	//<<-----------------------------------------------------------------------
    public GameObject FindChild(GameObject gameObject, string name)
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
        return null;
    }

    //<<-----------------------------------------------------------------------
    protected GameObject FindChild(GameObject gameObject,
    							   string tag, 
    							   string name)
    {
        GameObject rootObject = this.FindChild(gameObject, tag);

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
        return this.FindChild(this.gameObject, tag, name).GetComponent<T>();
    }

    protected T AddComponent<T>(string tag, string name) where T : UnityEngine.Component
    {
        return this.FindChild(this.gameObject, tag, name).AddComponent<T>();
    }
    
	//<<-----------------------------------------------------------------------
    protected T GetComponent<T>(string name) where T : UnityEngine.Component
    {
        return this.FindChild(this.gameObject, name).GetComponent<T>();
    }

    public T GetComponent<T>(GameObject obj, string name) where T : UnityEngine.Component
    {
        return this.FindChild(obj, name).GetComponent<T>();
    }

    //<<-----------------------------------------------------------------------
    protected T AddComponent<T>(string name) where T : UnityEngine.Component
    {
        return this.FindChild(this.gameObject, name).AddComponent<T>();
    }
}
