using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    private AssetReference assetTextReference = null;
    [SerializeField]
    private DialogueData dialogueData = null;

    private void Start()
    {
        LoadDialogueData();
    }

    private void LoadDialogueData()
    {
        string jsonString = "";
        assetTextReference.LoadAssetAsync<TextAsset>().Completed += handle => {
            jsonString = handle.Result.text;
            dialogueData = JsonUtility.FromJson<DialogueData>(jsonString);
            Addressables.Release(handle);
        };        
    }

    public DialogueGroup GetDialogueGroup(string id)
    {
        return dialogueData.dialogueGroupList.Find((dialogueGroup) => id.Equals(dialogueGroup.id));
    }
}
