# https://shengyu7697.github.io/blog/2019/11/29/Python-OpenCV-camera/
#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import cv2
import socket

cap = cv2.VideoCapture(0)
if not cap.isOpened():
    print("Cannot open camera")
    exit()

# socket init
HOST = 'localhost'
PORT = 2020
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

# https://blog.maxkit.com.tw/2017/07/socket-opencv-client.html
while(True):
    # Capture frame-by-frame
    ret, frame = cap.read()

    # if frame is read correctly ret is True
    if not ret:
        print("Can't receive frame (stream end?). Exiting ...")
        break

    # color to gray
    # gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    # s.send(b'test')

    s.send(bytes(cv2.imencode('.jpg', frame)[1]))
    # 顯示圖片
    # cv2.imshow('frame', frame)
    # 按下 q 鍵離開迴圈
    if cv2.waitKey(1) == ord('q'):
        break

# 釋放該攝影機裝置
cap.release()
cv2.destroyAllWindows()
