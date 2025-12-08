using System.Collections.Generic;
using UnityEngine;

namespace AMVCC
{
    public class AppElement : MonoBehaviour
    {
        public Application App => Assert<Application>(_application, true);
        private Application _application;

        private Dictionary<string, object> _cacheStorage;

        /// <summary>
        /// Finds an instance of 'T' if 'obj' is null. Returns 'obj' otherwise.
        /// If 'global' is 'true' searches in all scope, otherwise, searches in children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        public T Assert<T>(T obj, bool global = false) where T : Object
        {
            return obj ?? (global ? GameObject.FindObjectOfType<T>() : transform.GetComponentInChildren<T>());
        }
       

       
        /// <summary>
        /// Logs a message using this element information.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="verbose"></param>
        public void Log(object msg, int verbose = 0)
        {
            // Only outputs logs equal or bigger than the application 'verbose' level.
            if (verbose <= App.Verbose)
                Debug.Log(GetType().Name + "> " + msg);
        }
    }
}
