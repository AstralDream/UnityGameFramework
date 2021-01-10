using UnityEngine;

// 模块化封装，引入命名空间
namespace MobaGame
{
    // 不继承Mono的类可以使用New的方式
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;
        static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
    }

    public class UnitySingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        // 在新的场景中对象会保存
                        //obj.hideFlags = HideFlags.DontSave;
                        // 在新的场景中对象会保存，但是不会显示在Hierarchy面板中
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        _instance = (T)obj.AddComponent(typeof(T));
                    }
                }
                return _instance;
            }
        }
        public virtual void Awake()
        {
            // 进入新场景时，物体会被保留及其子物体；
            // 如果其不为根节点则无法保留，要保留的话得节点分离
            DontDestroyOnLoad(this.gameObject);
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
