using UnityEditor;
[InitializeOnLoad]
class StartUp
{
    static StartUp()
    {
        PlayerSettings.keystorePass = "gold1234";
        PlayerSettings.keyaliasPass = "gold1234";
    }
}