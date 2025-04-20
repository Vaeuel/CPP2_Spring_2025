using System;
using UnityEngine;

[DefaultExecutionOrder(-1)] //Specifies earlier loads
public abstract class Singleton<T> : MonoBehaviour where T : Component //Abstract prevents the script from being instantiated **'T' is a generic Type **Components are more flexible.
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            try
            {
                instance = FindAnyObjectByType<T>();
                /*if (instance == null) throw new NullReferenceException();
            }
                **This code isn't needed but is used as examples of possible use cases**
            catch (NullReferenceException e)
            {
                Debug.LogException(e);
                GameObject obj = new GameObject($"{typeof(T).Name}");//points to the name of the child class and renames
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);*/
            }

            catch (Exception e)//Exception class is a super class. NullReferenceExcaption is a sub class of this.
            {
                Debug.LogException(e);
                GameObject obj = new GameObject($"{typeof(T).Name}");//points to the name of the child class and renames
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }

            finally
            {
                //catch all code.
            }

            return instance;
        }
    }
    
    protected virtual void Awake() //Virtual allows children to over ride this function
    {
        if (!instance)
        {
            instance = this as T; //Casting?
            DontDestroyOnLoad(instance);
            return;
        }

        Destroy(gameObject);
    }
}
