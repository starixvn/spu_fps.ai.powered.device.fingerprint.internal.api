namespace Device.Fingerprint.Model
{
    public enum Flagged
    {
        Human = 1,
        EmptyToken,
        InvalidToken,
        EmptyAnalysis,
        InvalidAnalysis,
        EmptyDevice,
        InvalidDevice,
        EmptyUserAgent,
        Headless,
    }
}
