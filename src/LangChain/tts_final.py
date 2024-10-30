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

volume = [-5, 0, 5]
pitch = [0, -1, -2]

def get_mp3(speech, speaker, dominance_level, mp3_path):
    global CLIENT_ID
    global CLIENT_SECRET
    global url

    speech = re.sub(r'\[.*?\]', '', speech)

    data = "speaker=" + speaker + "&volume=" + str(volume[dominance_level]) + "&speed=0&pitch=" + str(pitch[dominance_level]) + "&alpha=-1&format=mp3&text=" + speech

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

def get_mp3_volume_pitch(speech, speaker, volume, pitch, mp3_path):
    global CLIENT_ID
    global CLIENT_SECRET
    global url

    speech = re.sub(r'\[.*?\]', '', speech)

    data = "speaker=" + speaker + "&volume=" + str(volume) + "&speed=0&pitch=" + str(pitch) + "&alpha=-1&format=mp3&text=" + speech

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

def get_mp3_length(mp3_path):
    audio = MP3(mp3_path)
    return audio.info.length

# name = "nihyun"
# dominance_level = 1
# text = '''
# GMO 식품의 상용화는 여러 측면에서 필요합니다. 첫째, 식량 문제 해결에 기여합니다. 세계 인구가 증가함에 따라 식량 수요도 급격히 증가하고 있습니다. GMO 작물은 생산성을 높여 같은 면적에서 더 많은 식량을 생산할 수 있도록 설계되어 있어 기후 변화와 자연재해에도 강한 특성을 지니고 있습니다. 둘째, 영양 강화에 도움을 줄 수 있습니다. 예를 들어, 비타민 A 결핍 문제를 해결하기 위해 개발된 황금쌀은 특히 영양 결핍이 심각한 지역에서 큰 도움이 될 수 있습니다. 셋째, 환경적인 장점도 있습니다. GMO 작물은 해충 저항성을 갖추고 있어 농약 사용을 줄일 수 있으며, 제초제에 내성이 있는 작물은 토양 침식을 방지하는 경작 방식을 가능하게 합니다. 이로 인해 장기적으로 환경 보전에 긍정적인 영향을 미칠 수 있습니다. 현재까지의 과학적 연구는 GMO 식품이 기존 식품과 동일한 안전성을 가진다고 입증하고 있습니다. 따라서, GMO 식품의 상용화는 지속적인 안전성 연구와 규제를 바탕으로 더욱 확대되어야 합니다.
# '''
#
# mp3_path = "test/output.mp3"
#
# # tts mp3 파일 생성
# get_mp3(text, name, volume[dominance_level], pitch[dominance_level], mp3_path)
# print(get_mp3_length(mp3_path))

