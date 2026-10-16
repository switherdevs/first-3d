using UnityEngine;

public class CodeInputUI : MonoBehaviour
{
    [Header("CODE LOCK")]
    [SerializeField]
    private CodeLock codeLock;

    public void CheckCode()
    {
        if (codeLock == null)
        {
            Debug.LogError(
                "[CodeInputUI] Chưa gắn CodeLock!"
            );

            return;
        }

        codeLock.CheckCode();
    }
}