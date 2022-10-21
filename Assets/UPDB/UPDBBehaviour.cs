using UnityEngine;

namespace UPDB
{
    ///<summary>
    /// UPDB methods that does not use extensions, callable in every classes that derives from monoBehaviour
    ///</summary>
    public class UPDBBehaviour : MonoBehaviour
    {
        /// <summary>
        /// try to find Object, and, if not, let an exception parameter
        /// </summary>
        /// <param name="variable">variable that will assign the TryFindObjectOfType value</param>
        /// <returns></returns>
        public static bool TryFindObjectOfType<T>(out T variable) where T : Object
        {
            variable = FindObjectOfType<T>();
            return variable != null;
        }

    } 
}
