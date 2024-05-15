using UnityEngine;
using UnityEngine.UI;

public class QREncodeAdapter : MonoBehaviour
{
    [SerializeField]
    private QRCodeEncodeController m_qrController;
    [SerializeField]
    private RawImage m_qrCodeImage;

    [SerializeField]
    private Texture2D m_codeTex;

    #region Public Methods

    public void Encode(string value)
    {
        m_qrController.Encode(value);
    }
    
    public void QrEncodeFinished(Texture2D tex)
    {
        if (tex != null && tex != null) {
            int width = tex.width;
            int height = tex.height;
            float aspect = width * 1.0f / height;
            m_qrCodeImage.GetComponent<RectTransform> ().sizeDelta = new Vector2 (170, 170.0f / aspect);
            m_qrCodeImage.texture = tex;
            m_codeTex = tex;
        } else {
        }
    }

    [ContextMenu("Encode")]
    public void Test()
    {
        Encode("ScoreRoom123");
    }

    #endregion
}