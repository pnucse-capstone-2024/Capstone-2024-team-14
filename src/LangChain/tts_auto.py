import os
import re
import urllib.request
from dotenv import load_dotenv
from mutagen.mp3 import MP3

load_dotenv()

# 네이버 클라우드 플랫폼에서 발급받은 Client ID와 Client Secret 입력
CLIENT_ID = os.getenv('CLIENT_ID')
CLIENT_SECRET = os.getenv('CLIENT_SECRET')

# 음성 합성 요청을 보낼 URL
url = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts"

def get_mp3_length(speech, speaker, volume, pitch, mp3_path):
    global CLIENT_ID
    global CLIENT_SECRET
    global url

    speech = re.sub(r'\[.*?\]', '', speech)

    data = "speaker=" + speaker + "&volume=" + str(volume) + "&speed=0&pitch=" + str(pitch) + "&format=mp3&text=" + speech

    request = urllib.request.Request(url)
    request.add_header("X-NCP-APIGW-API-KEY-ID", CLIENT_ID)
    request.add_header("X-NCP-APIGW-API-KEY", CLIENT_SECRET)

    response = urllib.request.urlopen(request, data=data.encode('utf-8'))
    rescode = response.getcode()
    if (rescode == 200):
        print("TTS mp3 저장")
        response_body = response.read()
        with open(mp3_path, 'wb') as f:
            f.write(response_body)
    else:
        print("Error code:" + rescode)

    audio = MP3(mp3_path)
    return audio.info.length

def get_backchannel(text, speaker, volume, pitch, mp3_path):
    global CLIENT_ID
    global CLIENT_SECRET
    global url

    data = "speaker=" + speaker + "&volume=" + str(volume) + "&speed=0&pitch=" + str(pitch) + "&format=mp3&text=" + text

    request = urllib.request.Request(url)
    request.add_header("X-NCP-APIGW-API-KEY-ID", CLIENT_ID)
    request.add_header("X-NCP-APIGW-API-KEY", CLIENT_SECRET)

    response = urllib.request.urlopen(request, data=data.encode('utf-8'))
    rescode = response.getcode()
    if (rescode == 200):
        print("TTS backchannel mp3 저장")
        response_body = response.read()
        with open(mp3_path, 'wb') as f:
            f.write(response_body)
    else:
        print("Error code:" + rescode)
#
# name = ["nihyun", "nminjeong", "njihwan"]
# volume = [0.5, 0.7, 1.0]
# pitch = [0.8, 1.0, 1.2]
# text = ["음", "으음", "어"]
# hello1 = [
#     "안녕하세요, 만나서 반가워요. 도움이 필요하면 언제든 말씀해주세요.",
#     "안녕하세요! 만나서 반가워요. 오늘 잘 부탁드려요.",
#     "안녕하세요! 반가워요, 기대되네요."
# ]
# hello2 = [
#     "오늘 즐거웠어요. 조심히 가세요, 또 봬요.",
#     "오늘 좋았어요. 다음에 또 봐요!",
#     "오늘 재미있었어요. 다음에 또 만나요!"
# ]
#
# # for idx in range(0, 3):
# #     mp3_path = "backchannel/" + name[idx] + ".mp3"
# #
# #     get_backchannel(text[idx], name[idx], volume[idx], pitch[idx], mp3_path)
# #
# #     mp3_path = "backchannel/" + name[idx] + "-hello1.mp3"
# #
# #     get_backchannel(hello1[idx], name[idx], volume[idx], pitch[idx], mp3_path)
# #
# #     mp3_path = "backchannel/" + name[idx] + "-hello2.mp3"
# #
# #     get_backchannel(hello2[idx], name[idx], volume[idx], pitch[idx], mp3_path)
#
# for idx in range(0, 3):
#     for text_idx in range(0, 3):
#         mp3_path = "backchannel2/" + name[idx] + "-" + str(text_idx + 1) + ".mp3"
#
#         get_backchannel(text[text_idx], name[idx], volume[idx], pitch[idx], mp3_path)