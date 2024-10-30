import json
from tts_final import get_mp3, get_mp3_length
from faster_whisper_module import get_gesture_start_time

dominance_levels = [0, 1, 2]

for dominance_level in dominance_levels:
    json_path = 'speech/result_1minute/' + str(dominance_level) + "/json/1minute_9.json"
    mp3_path = 'speech/result_1minute/' + str(dominance_level) + "/mp3/1minute_9.mp3"

    with open(json_path, 'r', encoding='utf-8') as json_file:
        answer = json.load(json_file)

    # get_mp3(
    #     speech=answer['speech'],
    #     speaker="nihyun",
    #     dominance_level=dominance_level,
    #     mp3_path=mp3_path
    # )

    answer = get_gesture_start_time(answer, mp3_path)

    answer['speech_time'] = get_mp3_length(mp3_path)

    with open(json_path, 'w', encoding='utf-8') as json_file:
        json.dump(answer, json_file, ensure_ascii=False, indent=4)