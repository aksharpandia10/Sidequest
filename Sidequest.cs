using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UIElements;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Unity.EditorCoroutines.Editor;


public class Sidequest : EditorWindow
{
    private string textPrompt = "";
    private int selectedStyle = 0;
    private string[] styleOptions = { "Realism (Red Dead)", "Fantasy realism (Elden Ring)", "Low poly (Borderlands)", "Cartoon (Minecraft)",};
    private string path;
    private string base64Content;
    private List<string> imagesResponse = new List<string>();
    private string testAPI;
    private string image_string = "";
    private HttpResponseMessage response;
    private bool isGenerating = false;
    private bool hasGeneratedImage = false;
    //private var form = new MultipartFormDataContent();
    //private byte[] bytes;
    
    [MenuItem("Window/Sidequest")]
    static void OpenWindow()
    {
        Sidequest window = (Sidequest)GetWindow(typeof(Sidequest));
        window.minSize = new Vector2(600,300);
        window.Show();
    }

    void OnGUI() {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Upload Image:");
            GUILayout.EndHorizontal();
            Texture2D uploadedImage = new Texture2D(512,512);
            if (GUILayout.Button("Browse"))
                    {
                       path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");  
                    }   
            if (!string.IsNullOrEmpty(path)) {
                byte[] bytes = File.ReadAllBytes(@path);
                uploadedImage.LoadImage(bytes);
                GUILayout.Label(uploadedImage);
                base64Content = Convert.ToBase64String(bytes);                
            }

            textPrompt = GUILayout.TextField("Prompt",textPrompt);
            GUILayout.Label("Select Style:");
            selectedStyle = EditorGUILayout.Popup(selectedStyle, styleOptions);
            if (GUILayout.Button("Generate")) {
                //Debug.Log(textPrompt);
                Debug.Log(selectedStyle);
                //EditorCoroutineUtility.StartCoroutine(Upload());
                GenerateImages();
                //generateImagesTest();
            }
            //CONVERTS IMAGE STRING TO IMAGE, UNCOMMENT IF FOR LOOP DOESNT WORK//
            byte[] data = System.Convert.FromBase64String(image_string);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(data);
             
            if (texture != null && hasGeneratedImage) {
                float windowWidth = EditorGUIUtility.currentViewWidth-20;
                float thumbnailWidth = (windowWidth / 5);
                //GUILayout.Label(texture, GUILayout.Width(128), GUILayout.Height(128));
                //Debug.Log(image_string);  
                GUILayout.BeginHorizontal("box");
                foreach (string base64Image in imagesResponse)
                {
                    GUILayout.BeginVertical();
                    Texture2D tex = Base64toTexture2D(base64Image);
                    GUILayout.Label(tex, GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailWidth));
                    if (GUILayout.Button(("Save"), GUILayout.Width(thumbnailWidth), GUILayout.Height(20)))
                    {
                        Debug.Log("hello");
                    }
                    if (GUILayout.Button(("Remove background"), GUILayout.Width(thumbnailWidth), GUILayout.Height(20)))
                    {
                        Debug.Log("hello");
                    }
                    GUILayout.EndVertical();
                //     float imageWidth = texture.xwidth;
                //     float imageHeight = texture.height;
                //     float screenWidth = Screen.width;
                //     float screenHeight = Screen.height;
                //     Rect imageRect = new Rect((screenWidth - imageWidth) / 2, (screenHeight - imageHeight) / 2 + i * (imageHeight + buttonHeight + buttonMargin), imageWidth, imageHeight);

                // // Draw the texture
                //     GUI.DrawTexture(imageRect, texture);

                    // // Draw the texture
                    // GUI.DrawTexture(new Rect(xPosition, 10f, rowHeight, rowHeight), texture);

                    // // Increment x position for next image
                    // xPosition += rowHeight + 10f;
                }
                GUILayout.EndHorizontal();
            }
            //Debug.Log(imagesResponse);
            //hasGeneratedImage = false;
    }

    public Texture2D Base64toTexture2D(string image_string) {
        byte[] data = System.Convert.FromBase64String(image_string);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(data);
        return texture;
    } 
    public async Task GenerateImagesAsync() {
        //isGenerating = true;
        using (var httpClient = new HttpClient()) 
        {
        // Set the base address of your Flask API
        httpClient.BaseAddress = new Uri("http://localhost:5000");
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(textPrompt), "textPrompt");
        // form.Add(new StringContent(path), "imagePath");
        
        //form.Add(new IntegerContent(selectedStyle), "selectedStyle");
        //Send a POST request to a specific endpoint
        response = await httpClient.PostAsync("/text_image", form);
        
        // Check if the request was successful
        if (response.IsSuccessStatusCode)
            {
                
                // Read the response content as string
                testAPI = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(testAPI);

                // iterating through each image in the response and adding to a list of images 
                foreach (JProperty property in jsonObject.Properties()) {
                    string imageString = property.Value.ToString();
                    imagesResponse.Add(imageString);
                    //**Debug statements checking if each image added is different**//
                    //Debug.Log(imageString.Substring(imageString.Length - 4));
                }
                image_string = jsonObject["0"].Value<string>();
                //Debug.Log(image_string);
                hasGeneratedImage = true;
                Debug.Log("image gen successful");
                //Console.WriteLine("Response from Flask API: " + responseData);
            }
            else
            {
                // If request failed, print the status code
                Debug.Log("Request failed with status code: " + response.StatusCode);
            }
        }
        //isGenerating = false;
    }

    // public IEnumerator Upload() {
    //     using UnityWebRequest www = UnityWebRequest.Post("https://httpbin.org/post", form);
    //     yield return www.SendWebRequest();
    // } 

    public async void generateImagesTest() {
        using var httpClient = new HttpClient();
        var form = new MultipartFormDataContent();
        // Set the base address of your Flask API
        httpClient.BaseAddress = new Uri("http://localhost:5000");
        //form.Add(new StringContent(path), "imagePath");
        form.Add(new StringContent(textPrompt), "textPrompt");

        try
        {
            // Send a GET request to a specific endpoint
            response = await httpClient.PostAsync("/test",form);
            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                
                // Read the response content as string
                testAPI = await response.Content.ReadAsStringAsync();
                // var result = JsonConvert.DeserializeObject(testAPI);
                JObject jsonObject = JObject.Parse(testAPI);
                string result = jsonObject["image1"].Value<string>();
                Debug.Log(result);
                //Console.WriteLine("Response from Flask API: " + responseData);
            }
            else
            {
                // If request failed, print the status code
                Debug.Log("Request failed with status code: " + response.StatusCode);
            }
        }
        catch (HttpRequestException e)
        {
            // If an error occurred during the request, print the error
            Debug.Log("Request failed: " + e.Message);
        }
    }

    private async void GenerateImages(){
        Debug.Log("call to image gen start");
        await GenerateImagesAsync();
        //Debug.Log("call to image gen complete");
    }
}

