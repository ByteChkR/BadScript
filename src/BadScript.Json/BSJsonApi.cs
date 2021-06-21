using BadScript.Common.Types;

namespace BadScript.Json
{

    public static class BSJsonApi
    {

        #region Public

        public static void AddApi( BSJsonApiSettings settings )
        {
            if ( ( settings & BSJsonApiSettings.BS2Json ) != 0 )
            {
                BSFunction f = new BSFunction( "function bs2json(jsonStr)", BS2Json.Convert );
                BSEngine.AddStatic( "bs2json", f );
            }

            if ( ( settings & BSJsonApiSettings.Json2BS ) != 0 )
            {
                BSFunction f = new BSFunction( "function json2bs(jsonStr)", Json2BS.Convert );
                BSEngine.AddStatic( "json2bs", f );
            }
        }

        #endregion

    }

}
