using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Imaging
{

    public class ImagingApi : ABSScriptInterface
    {
        #region Public

        public ImagingApi() : base( "drawing" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                new BSObject( "loadImage" ),
                new BSFunction( "function loadImage(data)", LoadImageApi, 1, 1 ) );

            root.InsertElement(
                new BSObject( "createImage" ),
                new BSFunction( "function createImage(width, height)", CreateEmptyBitmapApi, 2, 2 ) );

            root.InsertElement(
                new BSObject( "color" ),
                new BSFunction( "function color(a, r, g, b)/color(r, g, b)", CreateColorApi, 3, 4 ) );

            root.InsertElement( new BSObject( "rotateFlipType" ), BSTable.FromEnum < RotateFlipType >() );
        }

        #endregion

        #region Private

        private ABSObject BitmapDisposeApi( Bitmap bmp )
        {
            bmp.Dispose();

            return new BSObject( null );
        }

        private ABSObject BitmapGetPixelApi( Bitmap bmp, ABSObject[] arg )
        {
            int x = ( int ) arg[0].ConvertDecimal();
            int y = ( int ) arg[1].ConvertDecimal();

            return ConvertColor( bmp.GetPixel( x, y ) );
        }

        private ABSObject BitmapHeightApi( Bitmap bmp )
        {
            return new BSObject( ( decimal ) bmp.Height );
        }

        private ABSObject BitmapRotateFlip( Bitmap bmp, ABSObject[] arg )
        {
            RotateFlipType t = ( RotateFlipType ) arg[0].ConvertDecimal();
            bmp.RotateFlip( t );

            return new BSObject( null );
        }

        private ABSObject BitmapSerializeApi( Bitmap bmp, ABSObject[] arg )
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save( ms, ImageFormat.Png );
            BSArray a = new BSArray( ms.ToArray().Select( x => new BSObject( ( decimal ) x ) ) );

            return a;
        }

        private ABSObject BitmapSetPixelApi( Bitmap bmp, ABSObject[] arg )
        {
            int x = ( int ) arg[0].ConvertDecimal();
            int y = ( int ) arg[1].ConvertDecimal();
            bmp.SetPixel( x, y, ConvertColor( arg[2] ) );

            return new BSObject( null );
        }

        private ABSObject BitmapWidthApi( Bitmap bmp )
        {
            return new BSObject( ( decimal ) bmp.Width );
        }

        private Color ConvertColor( ABSObject o )
        {
            if ( o is ABSTable t )
            {
                int a = ( int ) t.GetRawElement( new BSObject( "a" ) ).ConvertDecimal();
                int r = ( int ) t.GetRawElement( new BSObject( "r" ) ).ConvertDecimal();
                int g = ( int ) t.GetRawElement( new BSObject( "g" ) ).ConvertDecimal();
                int b = ( int ) t.GetRawElement( new BSObject( "b" ) ).ConvertDecimal();

                return Color.FromArgb( a, r, g, b );
            }

            throw new BSInvalidTypeException( o.Position, "Expected Color Table", o, "Color Table" );
        }

        private ABSTable ConvertColor( Color c )
        {
            BSTable t = new BSTable( SourcePosition.Unknown );
            t.InsertElement( new BSObject( "a" ), new BSObject( ( decimal ) c.A ) );
            t.InsertElement( new BSObject( "r" ), new BSObject( ( decimal ) c.R ) );
            t.InsertElement( new BSObject( "g" ), new BSObject( ( decimal ) c.G ) );
            t.InsertElement( new BSObject( "b" ), new BSObject( ( decimal ) c.B ) );

            return t;
        }

        private ABSObject CreateBitmapApi( Bitmap bmp )
        {
            BSTable table = new BSTable( SourcePosition.Unknown );

            table.InsertElement(
                new BSObject( "getWidth" ),
                new BSFunction( "function getWidth()", x => BitmapWidthApi( bmp ), 0 ) );

            table.InsertElement(
                new BSObject( "getHeight" ),
                new BSFunction( "function getHeight()", x => BitmapHeightApi( bmp ), 0 ) );

            table.InsertElement(
                new BSObject( "setPixel" ),
                new BSFunction( "function setPixel(x, y, value)", x => BitmapSetPixelApi( bmp, x ), 3 ) );

            table.InsertElement(
                new BSObject( "getPixel" ),
                new BSFunction( "function getPixel(x, y)", x => BitmapGetPixelApi( bmp, x ), 2 ) );

            table.InsertElement(
                new BSObject( "rotateFlip" ),
                new BSFunction( "function rotateFlip()", x => BitmapRotateFlip( bmp, x ), 1 ) );

            table.InsertElement(
                new BSObject( "serialize" ),
                new BSFunction( "function serialize()", x => BitmapSerializeApi( bmp, x ), 0, 0 ) );

            table.InsertElement(
                new BSObject( "dispose" ),
                new BSFunction( "function dispose()", x => BitmapDisposeApi( bmp ), 0, 0 ) );

            return table;
        }

        private ABSObject CreateColorApi( ABSObject[] arg )
        {
            if ( arg.Length == 3 )
            {
                int r = ( int ) arg[0].ConvertDecimal();
                int g = ( int ) arg[1].ConvertDecimal();
                int b = ( int ) arg[2].ConvertDecimal();

                return ConvertColor( Color.FromArgb( r, g, b ) );
            }
            else
            {
                int a = ( int ) arg[0].ConvertDecimal();
                int r = ( int ) arg[1].ConvertDecimal();
                int g = ( int ) arg[2].ConvertDecimal();
                int b = ( int ) arg[3].ConvertDecimal();

                return ConvertColor( Color.FromArgb( a, r, g, b ) );
            }

            return null;
        }

        private ABSObject CreateEmptyBitmapApi( ABSObject[] arg )
        {
            Bitmap bmp = new Bitmap( ( int ) arg[0].ConvertDecimal(), ( int ) arg[1].ConvertDecimal() );

            return CreateBitmapApi( bmp );
        }

        private ABSObject LoadImageApi( ABSObject[] arg )
        {
            if ( arg[0] is ABSArray arr )
            {
                byte[] b = arr.ForEach( x => ( byte ) x.ConvertDecimal() ).ToArray();
                MemoryStream ms = new MemoryStream( b );
                Bitmap bmp = ( Bitmap ) Image.FromStream( ms );

                return CreateBitmapApi( bmp );
            }

            throw new BSInvalidTypeException( arg[0].Position, "Expected Array", arg[0], "Array" );
        }

        #endregion
    }

}
