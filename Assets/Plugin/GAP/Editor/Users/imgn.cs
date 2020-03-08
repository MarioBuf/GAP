using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Plugins.GAP.Editor.Users
{
    class imgn: MonoBehaviour
    {
        public void Start(string url)
        {
            StartCoroutine(setImage(url));
        }

        IEnumerator setImage(string url)
        {
            byte[] imageBytes;
            using (var webClient = new WebClient())
            {
                imageBytes = webClient.DownloadData(url);
            }
            Texture2D texture=new Texture2D(25,25);
            texture.LoadImage(imageBytes);

            WWW www = new WWW(url);
            yield return www;

            // calling this function with StartCoroutine solves the problem
            Debug.Log("Texture2D è stata caricata");

            www.LoadImageIntoTexture(texture);
            www.Dispose();
            www = null;
        }
    }
}
