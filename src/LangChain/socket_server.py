import socket
import threading
import json
import os
from llm_module import get_answer, get_backchannel
from tts_final import get_mp3_volume_pitch, get_mp3_length
from faster_whisper_module import get_gesture_start_time

def handle_client_connection(client_socket):
    with client_socket:
        while True:
            data = client_socket.recv(4096).decode('utf-8')
            if not data:
                break
            print(f"Received data: {data}")

            data_to_dict = json.loads(data)
            setup = data_to_dict['setup']

            if setup:
                answer = get_backchannel(data)
                answer = json.dumps(answer, ensure_ascii=False)
            else:
                answer = get_answer(data)

                script_dir = os.path.dirname(os.path.abspath(__file__))  # 현재 파일의 디렉토리
                wav_path = os.path.join(script_dir, "mp3", "answer.mp3")  # 절대 경로로 변환

                get_mp3_volume_pitch(
                    speech=answer['speech'],
                    speaker=data_to_dict['speaker'],
                    volume=data_to_dict['volume'],
                    pitch=data_to_dict['pitch'],
                    mp3_path=wav_path
                )

                answer['speech_time'] = get_mp3_length(wav_path)

                answer = get_gesture_start_time(answer, wav_path)

                answer = json.dumps(answer, ensure_ascii=False)

            client_socket.sendall(answer.encode('utf-8'))

def start_server(host='127.0.0.1', port=25001):
    
    print(f"Starting server")
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)  # 소켓 재사용 옵션 설정
    server.bind((host, port))
    server.listen(5)
    print(f"Server listening on {host}:{port}")

    while True:
        try:
            client_socket, addr = server.accept()
            print(f"Accepted connection from {addr}")
            client_handler = threading.Thread(
                target=handle_client_connection,
                args=(client_socket,)
            )
            client_handler.start()
        except Exception as e:
            print(f"Error accepting connection: {e}")


if __name__ == "__main__":
    start_server()