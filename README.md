# Shape It Wizard

ShapeItWizard là một tựa game tương tác độc đáo, đưa bạn vào vai một phù thủy, sử dụng đũa phép để vẽ nên những câu thần chú, tiêu diệt các quái vật hình khối. Trò chơi kết hợp công nghệ theo dõi chuyển động tay hiện đại qua **Mediapipe** với giao diện game sống động được phát triển bằng **Unity**.

[](https://github.com/skullick/ShapeItWizard/blob/main/icon.png)
## 🎮 Lối chơi
Trong trò chơi, bạn sẽ đối mặt với làn sóng kẻ địch liên tục xuất hiện từ trên trời và lao về phía mình. Nhiệm vụ của bạn là vẽ các hình 2D tương ứng với loại kẻ địch bằng chuyển động tay. Mục tiêu là tiêu diệt càng nhiều kẻ địch càng tốt bằng cách vẽ đúng hình dạng.
Nếu bạn vẽ đúng hình, tất cả kẻ địch cùng loại đang xuất hiện trên màn hình sẽ bị tiêu diệt ngay lập tức. Mỗi kẻ địch bị tiêu diệt sẽ mang lại cho bạn 1 điểm.
Bạn bắt đầu với 3 mạng. Nếu kẻ địch tiếp cận bạn mà không bị tiêu diệt, bạn sẽ mất 1 mạng. Khi hết mạng, trò chơi kết thúc.

Chỉ giơ và di chuyển ngón trỏ trước webcam để vẽ các hình. Mở cả 5 ngón tay để xác nhận hình đang vẽ. Hiện tại, trò chơi gồm các hình: Tròn, Tam giác, Vuông, Ngũ Giác.

## 🚀 Công nghệ sử dụng
- **Nhận diện & Theo dõi chuyển động tay**: Phần này được viết bằng Python, sử dụng mô hình ```hand_landmark``` từ thư viện Mediapipe để nhận diện và theo dõi cử chỉ tay của người chơi.
- **Giao diện Game**: Toàn bộ phần game được xây dựng và cài đặt bằng Unity3D.

Phần mô hình Mediapipe và Unity giao tiếp với nhau qua socket. Trước đó, ```main.py``` đã được chuyển đổi và đóng gói môi trường sử dụng **Pyinstaller** rồi copy toàn bộ thư mục này vào trong thư mục Contents của file .app (xây dựng bằng Unity)
## 💻 Hướng dẫn cài đặt và chạy (MacOS)
Hiện tại, Shape It Wizard đã được kiểm nghiệm trên hệ điều hành MacOS.
Tải [file](https://drive.google.com/drive/folders/1z2jqvYtLrQwm8UZiarky7YyFJrNm_DKu?usp=drive_link) và giải nén để thử nghiệm.

**Lưu ý**: Ở lượt chạy đầu tiên, kiểm tra xem game có hiện ô hội thoại yêu cầu quyền truy cập camera. Nếu có, hãy xác nhận và chạy lại trò chơi, chờ camera hiện ra để bắt đầu. Nếu không xuất hiện, hay kiểm tra quyền trong cài đặt của máy.

## 📝 Đóng góp
Dự án này vẫn đang trong quá trình phát triển và luôn hoan nghênh mọi sự đóng góp từ cộng đồng! Nếu bạn có ý tưởng cải tiến, phát hiện lỗi, hoặc muốn bổ sung tính năng mới, đừng ngần ngại tạo một pull request hoặc liên hệ trực tiếp.
