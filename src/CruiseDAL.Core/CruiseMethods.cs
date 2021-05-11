namespace CruiseDAL.Schema
{
    public static class CruiseMethods
    {
        public const string H_PCT = "100";
        public const string STR = "STR";
        public const string S3P = "S3P";
        public const string THREEP = "3P";
        public const string FIX = "FIX";
        public const string F3P = "F3P";
        public const string FCM = "FCM";
        public const string PCM = "PCM";
        public const string PNT = "PNT";
        public const string P3P = "P3P";
        public const string THREEPPNT = "3PPNT";
        public const string FIXCNT = "FIXCNT";

        public const string SYSTEMATIC_SAMPLER_TYPE = "SystematicSelecter";
        public const string BLOCK_SAMPLER_TYPE = "BlockSelecter";
        public const string SRS_SAMPLER_TYPE = "SRSSelecter";
        public const string THREEP_SAMPLER_TYPE = "ThreePSelecter";
        public const string CLICKER_SAMPLER_TYPE = "ClickerSelecter";

        public static readonly string[] RECON_METHODS = { FIX, PNT };
        public static readonly string[] SUPPORTED_METHODS = { H_PCT, STR, S3P, THREEP, FIX, F3P, FCM, PCM, PNT, P3P, THREEPPNT, FIXCNT };
        public static readonly string[] PLOT_METHODS = { FIX, F3P, FCM, PNT, PCM, P3P };
        public static readonly string[] THREE_P_METHODS = { THREEP, S3P, F3P, P3P };
        public static readonly string[] VARIABLE_RADIUS_METHODS = { PCM, PNT, P3P, THREEPPNT };
        public static readonly string[] FIXED_SIZE_PLOT_METHODS = { FIX, FCM, F3P, FIXCNT };
        public static readonly string[] FREQUENCY_SAMPLED_METHODS = { STR, FCM, PCM, S3P };
        public static readonly string[] TALLY_METHODS = { STR, THREEP, S3P, F3P, P3P, PCM, FCM };
        public static readonly string[] MANDITORY_TALLY_METHODS = { STR, THREEP, S3P, PCM, FCM };
        public static readonly string[] UNSUPPORTED_METHODS = { };
    }
}