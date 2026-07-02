public static class GameProgress
{
    public static bool HasTalkedToArchivist { get; private set; }
    public static bool HasActivatedPatchCrystal { get; private set; }

    public static void MarkTalkedToArchivist()
    {
        HasTalkedToArchivist = true;
    }

    public static void MarkPatchCrystalActivated()
    {
        HasActivatedPatchCrystal = true;
    }

    public static void ResetProgress()
    {
        HasTalkedToArchivist = false;
        HasActivatedPatchCrystal = false;
    }
}