using UnityEngine;
using TMPro;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance;
    [SerializeField] TextMeshProUGUI textBox;

    private void Start()
    {
        Instance = this;
        textBox.text = "";
    }

    public void DeleteLetter()
    {
        if(textBox.text.Length != 0) 
        {
            textBox.text = textBox.text.Remove(textBox.text.Length - 1, 1);
        }
    }

    public void AddLetter(string letter)
    {
        textBox.text = textBox.text + letter;
    }
}
