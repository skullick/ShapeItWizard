import cv2
import mediapipe as mp
import numpy as np
import socket
import json
import screeninfo

screen = screeninfo.get_monitors()[0]


sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5053)

class HandPen:
    def __init__(self):
        self.mp_hands = mp.solutions.hands
        self.hands = self.mp_hands.Hands(min_detection_confidence=0.7, min_tracking_confidence=0.7, max_num_hands=1)
        self.mp_draw = mp.solutions.drawing_utils
        self.drawing_points = []  # Store drawn points
        self.text = ""  # Stores recognized text
        self.shape = ""  # Stores recognized shape

    def count_fingers(self, hand_landmarks):
        """Counts the number of extended fingers (1-5)."""
        finger_tips = [4, 8, 12, 16, 20]  # Thumb, Index, Middle, Ring, Pinky
        fingers_up = sum(hand_landmarks.landmark[tip].y < hand_landmarks.landmark[tip - 2].y for tip in finger_tips)
        return fingers_up

    def track_hand(self, frame):
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = self.hands.process(rgb_frame)

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                self.mp_draw.draw_landmarks(frame, hand_landmarks, self.mp_hands.HAND_CONNECTIONS)

                # Check if all 5 fingers are up → Erase the drawing
                fingers_up = self.count_fingers(hand_landmarks)
                if fingers_up == 5:
                    self.recognize_shape()  # Recognize shape if needed
                    if self.shape != "":
                        data = {"command": "erase", "shape": self.shape}
                        sock.sendto(json.dumps(data).encode(), serverAddressPort)
                    else:
                        data = {"command": "erase", "shape": ""}
                        sock.sendto(json.dumps(data).encode(), serverAddressPort)
                    self.drawing_points.clear()  # Clear all drawing points
                    self.text = ""  # Clear recognized text
                    self.shape = ""  # Clear recognized shape
                    print("🧼 Eraser Mode: Clearing the screen!")

                else:
                    # Get index finger tip (landmark 8)
                    h, w, _ = frame.shape
                    index_tip = hand_landmarks.landmark[8]
                    x, y = int(index_tip.x * w), int(index_tip.y * h)

                    # Append point to list
                    self.drawing_points.append((x, y))
                    data = {"command": "draw", "x": index_tip.x, "y": index_tip.y}
                    sock.sendto(json.dumps(data).encode(), serverAddressPort)

                # Draw all points as a connected line
                for i in range(1, len(self.drawing_points)):
                    cv2.line(frame, self.drawing_points[i - 1], self.drawing_points[i], (255, 0, 255), 7)  # Thick line

        # ✅ Display recognized text & shape on screen

        return frame


    def recognize_shape(self):
        """Detects basic shapes (circle, triangle, square) from drawn points."""
        if len(self.drawing_points) < 20:  # Ignore if too few points
            return

        canvas = np.ones((720, 1280, 3), dtype=np.uint8) * 255  # White background
        for i in range(1, len(self.drawing_points)):
            cv2.line(canvas, self.drawing_points[i - 1], self.drawing_points[i], (0, 0, 0), 8)

        # ✅ Convert to grayscale and detect contours
        gray = cv2.cvtColor(canvas, cv2.COLOR_BGR2GRAY)
        _, binary = cv2.threshold(gray, 120, 255, cv2.THRESH_BINARY_INV)
        contours, _ = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

        for cnt in contours:
            approx = cv2.approxPolyDP(cnt, 0.04 * cv2.arcLength(cnt, True), True)
            sides = len(approx)
            print(f"🔺 Detected Sides: {sides}")

            if sides == 3:
                self.shape = "Triangle"
            elif sides == 4:
                self.shape = "Square" 
                
            elif sides == 5:
                self.shape = "Pentagon"
            elif sides > 5:
                self.shape = "Circle"
            else:
                self.shape = ""

        print(f"🔵 Recognized Shape: {self.shape}")

# ✅ Initialize Hand Tracker
hand_pen = HandPen()

# ✅ Start Webcam Capture
cap = cv2.VideoCapture(0)

WINDOW_NAME = "ShapeItWizard"
cv2.namedWindow(WINDOW_NAME, cv2.WINDOW_GUI_NORMAL)  # No toolbar  # Small window (320x240 pixels)
cv2.moveWindow(WINDOW_NAME, screen.width - 320, 0)  # Top-left corner

while cap.isOpened():
    success, frame = cap.read()
    if not success:
        break

    frame = cv2.flip(frame, 1)  # Flip for mirror effect
    data = []
    tracked_frame = hand_pen.track_hand(frame)
    cv2.resizeWindow(WINDOW_NAME, 320, 180)  # Small window (320x180 pixels)
    cv2.imshow(WINDOW_NAME, tracked_frame)
    key = cv2.waitKey(1) & 0xFF
    if key == ord('s'):
        hand_pen.recognize_shape()  # Detect shapes
    if key == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()