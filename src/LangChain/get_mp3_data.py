from tts_final import get_mp3
import os

dominance_levels = [0, 1, 2]
name = ["nihyun", "nihyun", "nihyun"]
backchannel_list = ["음", "으음", "어"]
greeting_list = [
    "안녕하세요! 반가워요",
    "안녕하세요! 반가워요",
    "안녕하세요! 반가워요"
]
farewell_list = [
    "오늘 즐거웠어요. 조심히 가세요, 또 봬요.",
    "오늘 즐거웠어요. 다음에 또 봐요!",
    "오늘 재미있었어요. 다음에 또 만나요!"
]

def get_data(list, data_name):
    folder_path = 'data/' + data_name

    if not os.path.exists(folder_path):
        os.makedirs(folder_path)
        for dominance_level in dominance_levels:
            os.makedirs(folder_path + "/" + str(dominance_level))

    for dominance_level in dominance_levels:
        for idx, speech in enumerate(list):
            mp3_path = folder_path + "/" + str(dominance_level) + "/" + data_name + str(idx + 1) + ".mp3"
            get_mp3(
                speech=speech,
                speaker=name[idx],
                dominance_level=dominance_level,
                mp3_path=mp3_path
            )


if __name__ == "__main__":
    get_data(greeting_list, "greeting")
    get_data(farewell_list, "farewell")
    # get_data(backchannel_list, "backchannel")