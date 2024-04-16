from flask import Flask, request
app = Flask(__name__)
import requests
import base64
import os
import json

@app.route("/test", methods=['POST','GET'])
def test():
    prompt = request.form['textPrompt']
    images_test = {"image1":prompt}
    return json.dumps(images_test)

@app.route("/image_image", methods=['POST','GET'])
def image_to_image():
    prompt = request.form['textPrompt']
    engine_id = "stable-diffusion-v1-6"
    api_host = os.getenv("API_HOST", "https://api.stability.ai")
    api_key = "sk-gJiIjm1MBKfaRDcORngLnGFqbXhufEZSMTKL4Xd1zdczKU45"
    if api_key is None:
        raise Exception("Missing Stability API key.")

    response = requests.post(
    f"{api_host}/v1/generation/{engine_id}/image-to-image",
    headers={
        "Accept": "application/json",
        "Authorization": f"Bearer {api_key}"
    },
    files={
        "init_image": open(r"C:\Users\aksha\OneDrive\Pictures\treasure chest.png", "rb")
    },
    data={
        "image_strength": 0.35,
        "init_image_mode": "IMAGE_STRENGTH",
        "text_prompts[0][text]": prompt,
        "cfg_scale": 7,
        "samples": 2,   
        "steps": 80,
    }
    )
    if response.status_code != 200:
        raise Exception("Non-200 response: " + str(response.text))

    data = response.json()
    images = {}
    for i, image in enumerate(data["artifacts"]):
        images[i] = image["base64"]
        #images.append(image["base64"])
        # with open(f"./out/v1_img2img_{i}.png", "wb") as f:
        #     f.write(base64.b64decode(image["base64"]))
    # enumerated_images = enumerate(images)
    return json.dumps(images)

@app.route("/text_image", methods=['POST','GET'])
def text_to_image():
    prompt = request.form['textPrompt']
    engine_id = "stable-diffusion-v1-6"
    api_host = os.getenv("API_HOST", "https://api.stability.ai")
    api_key = "sk-gJiIjm1MBKfaRDcORngLnGFqbXhufEZSMTKL4Xd1zdczKU45"
    if api_key is None:
        raise Exception("Missing Stability API key.")

    response = requests.post(
    f"{api_host}/v1/generation/{engine_id}/text-to-image",
    headers={
        "Content-Type": "application/json",
        "Accept": "application/json",
        "Authorization": f"Bearer {api_key}"
    },
    json={
        "text_prompts": [
            {
                "text": prompt
            }
        ],
        "cfg_scale": 7,
        "height": 320,
        "width": 320,
        "samples": 2,
        "steps": 30,
    },
    )
    if response.status_code != 200:
        raise Exception("Non-200 response: " + str(response.text))

    data = response.json()
    images = {}
    for i, image in enumerate(data["artifacts"]):
        images[i] = image["base64"]
        #images.append(image["base64"])
        # with open(f"./out/v1_img2img_{i}.png", "wb") as f:
        #     f.write(base64.b64decode(image["base64"]))
    # enumerated_images = enumerate(images)
    return images


# url = "https://api.imgur.com/3/image"
#     payload={'type': 'image',
#         'title': 'Simple upload',
#         'description': 'This is a simple image upload in Imgur'}
#     files=[
#         ('image',('2021-class-grads.jpg',open(r"C:\Users\aksha\OneDrive\Pictures\2021-class-grads.jpg",'rb'),'image/jpg'))
#         ]
#     headers = {
#     'Authorization': 'Client-ID {{a9bbc71cefc6912}}'
#     }
#     response = requests.request("POST", url, headers=headers, data=payload, files=files)
#     return response.text

