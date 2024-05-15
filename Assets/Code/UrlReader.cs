using System.Runtime.InteropServices;
using UnityEngine;

namespace Code
{
    public class UrlReader : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern string GetURLFromPage();
   
        [DllImport("__Internal")]
        private static extern string GetQueryParam(string paramId);
 
        public string ReadQueryParam(string paramId)
        {
            return GetQueryParam(paramId);
        }
   
        public string ReadURL()
        {
            return GetURLFromPage();
        }
    }
}