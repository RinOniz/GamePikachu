using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialCase
{
    public GameObject wholeSetup; 
    public GameObject lineAndResult; 
}

public class HowToPlayPanel : MonoBehaviour
{
    [SerializeField] private Button closePanelButton;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject closeMenuButton;

    [SerializeField] private TutorialCase[] validCases;
    [SerializeField] private TutorialCase[] invalidCases;

    private Coroutine tutorialRoutine;

    private void Start()
    {
        closePanelButton.onClick.AddListener(Hide);
    }

    public void Show()
    {
        panel.SetActive(true);

        title.SetActive(false);

        closeMenuButton.SetActive(false);

        tutorialRoutine = StartCoroutine(PlayTutorialAnimation());
    }

    public void Hide()
    {
        if (tutorialRoutine != null) { 
            StopCoroutine(tutorialRoutine);
        }

        panel.SetActive(false);

        title.SetActive(true);

        closeMenuButton.SetActive(true);
    }

    private IEnumerator PlayTutorialAnimation()
    {
        int step = 0; // Biến đếm số lượt đã chạy

        while (true)
        {
            // Dùng phép chia lấy dư (%) để nó tự quay vòng lại từ đầu nếu hết danh sách
            int vIndex = step % validCases.Length;
            int iIndex = step % invalidCases.Length;

            TutorialCase currentValid = validCases[vIndex];
            TutorialCase currentInvalid = invalidCases[iIndex];

            // --- BƯỚC 1: TẮT HẾT CÁC CASE KHÁC CŨ ĐI ---
            foreach (var c in validCases) c.wholeSetup.SetActive(false);
            foreach (var c in invalidCases) c.wholeSetup.SetActive(false);

            // --- BƯỚC 2: BẬT CASE HIỆN TẠI LÊN (Chưa có đường nối) ---
            currentValid.wholeSetup.SetActive(true);
            currentValid.lineAndResult.SetActive(false);

            currentInvalid.wholeSetup.SetActive(true);
            currentInvalid.lineAndResult.SetActive(false);

            yield return new WaitForSeconds(1.0f); // Dừng 1 giây để người chơi nhìn 2 con Pikachu

            // --- BƯỚC 3: BẬT ĐƯỜNG NỐI & DẤU CHÉO/TICK LÊN (Tạo hiệu ứng bùm) ---
            currentValid.lineAndResult.SetActive(true);
            currentInvalid.lineAndResult.SetActive(true);

            yield return new WaitForSeconds(2.0f); // Giữ nguyên hiện trường 2 giây để người chơi đọc Text

            // --- Tăng lượt chơi để vòng lặp sau chiếu Slide tiếp theo ---
            step++;
        }
    }
}
