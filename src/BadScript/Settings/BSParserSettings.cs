namespace BadScript.Settings
{

    public class BSParserSettings
    {

        public bool AllowOptimization;

        public static BSParserSettings Default => new BSParserSettings { AllowOptimization = true };

        #region Public

        public BSParserSettings()
        {
        }

        #endregion

    }

}
