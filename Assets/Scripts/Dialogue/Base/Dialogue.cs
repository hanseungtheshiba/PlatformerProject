[System.Serializable]
public struct Dialogue
{
    public enum FacePosition
    {
        NONE = 0,
        LEFT = 1,
        RIGHT = 2
    }
    public FacePosition facePosition;
    public string faceSpriteName;
    public string dialogue;
}