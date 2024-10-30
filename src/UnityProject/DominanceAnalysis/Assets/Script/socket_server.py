import socket
import threading
from llm_module import get_answer

def handle_client_connection(client_socket):
    with client_socket:
        while True:
            data = client_socket.recv(4096).decode('utf-8')
            if not data:
                break
            print(f"Received data: {data}")

            # 유니티로부터 받은 데이터를 LLM 모델에 전달
            answer = get_answer(data)
            print(answer)
            client_socket.sendall(answer.encode('utf-8'))

def start_server(host='127.0.0.1', port=25001):
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)  # 소켓 재사용 옵션 설정
    server.bind((host, port))
    server.listen(5)
    print(f"Server listening on {host}:{port}")
    while True:
        client_socket, addr = server.accept()
        print(f"Accepted connection from {addr}")
        client_handler = threading.Thread(
            target=handle_client_connection,
            args=(client_socket,)
        )
        client_handler.start()

if __name__ == "__main__":
    start_server()