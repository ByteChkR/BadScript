using BadScript.Runtime.Implementations;

namespace BadScript.Json
{

    public static class BSJsonApi
    {

        #region Public

        public static void AddApi( BSJsonApiSettings settings )
        {
            if ( ( settings & BSJsonApiSettings.BS2Json ) != 0 )
            {
                BSRuntimeFunction f = new BSRuntimeFunction( "function bs2json(jsonStr)", BS2Json.Convert );
                BSEngine.AddStatic( "bs2json", f );
            }

            if ( ( settings & BSJsonApiSettings.Json2BS ) != 0 )
            {
                BSRuntimeFunction f = new BSRuntimeFunction( "function json2bs(jsonStr)", Json2BS.Convert );
                BSEngine.AddStatic( "json2bs", f );
            }
        }

        #endregion

    }

}