using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;
using BadScript.Common.Expressions.Implementations.Binary;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Operators;
using BadScript.Common.Operators.Implementations;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript
{

    public static class BSOperatorPreceedenceTable
    {
        private static readonly List<BSOperator> s_Operators =
            new List<BSOperator>
            {
                new BSBinaryOperator(
                    6,
                    "+",
                    new BSFunction(
                        "function +(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "+", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    6,
                    "-",
                    new BSFunction(
                        "function -(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "-", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    5,
                    "*",
                    new BSFunction(
                        "function *(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "*", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    5,
                    "/",
                    new BSFunction(
                        "function /(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "/", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    5,
                    "%",
                    new BSFunction(
                        "function %(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "%", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    15,
                    "??",
                    new BSFunction(
                        "function ??(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "??", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    9,
                    "==",
                    new BSFunction(
                        "function ==(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "==", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    9,
                    "!=",
                    new BSFunction(
                        "function !=(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "!=", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    13,
                    "&&",
                    new BSFunction(
                        "function &&(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "&&", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    14,
                    "||",
                    new BSFunction(
                        "function ||(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "||", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    11,
                    "^",
                    new BSFunction(
                        "function ^(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "^", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    "<",
                    new BSFunction(
                        "function <(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "<", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    ">",
                    new BSFunction(
                        "function >(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( ">", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    "<=",
                    new BSFunction(
                        "function <=(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "<=", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    ">=",
                    new BSFunction(
                        "function >=(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( ">=", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSAssignmentOperator(),
                new BSMemberAccessOperator(),
            };

        private static readonly List < BSOperator > s_PrefixOperators =
            new List < BSOperator >
            {
                new BSUnaryOperator(
                    "!",
                    new BSFunction(
                        "function !(a)",
                        objects =>
                            BSOperatorImplementationResolver.
                                ResolveImplementation( "!", objects ).
                                ExecuteOperator( objects ),
                        1
                    )
                )
            };

        public static bool Has( int p, string k ) =>
            s_Operators.Any( x => x.Preceedence <= p && x.OperatorKey == k );

        private static BSOperator Get( List < BSOperator > ops, int p, string key )
        {
            foreach (BSOperator bsOperator in ops)
            {
                if (bsOperator.Preceedence <= p && bsOperator.OperatorKey == key)
                    return bsOperator;
            }

            throw new BSParserException( $"Can not Resolve Operator: {key} with precedence <= {p}" );
        }

        public static BSOperator Get( int p, string key ) => Get( s_Operators, p, key );

        public static BSOperator GetPrefix( int p, string key ) => Get( s_PrefixOperators, p, key );


    }
    public class BSParser
    {

        
        

        

        private readonly string m_OriginalSource;
        private int m_CurrentPosition;
        private char Current =>
            m_CurrentPosition < m_OriginalSource.Length ? m_OriginalSource[m_CurrentPosition] : '\0';

        private char[] CurrentArray => m_OriginalSource.ToCharArray();

        #region Public

        public BSParser( string script )
        {
            m_OriginalSource = script;

            m_CurrentPosition = 0;
        }

        public int FindClosing( char open, char close )
        {
            int level = 1;

            while ( true )
            {
                if ( Is( '\0' ) )
                {
                    throw new BSParserException( $"Expected '{close}'", this );
                }

                if ( Is( open ) )
                {
                    level++;
                }
                else if ( Is( close ) )
                {
                    level--;
                }

                if ( level == 0 )
                {
                    break;
                }

                m_CurrentPosition++;
            }

            int r = m_CurrentPosition;
            m_CurrentPosition++;

            return r;
        }

        public (string line, int lineCount, int col) GetCurrentLineInfo()
        {
            char[] text = m_OriginalSource.ToCharArray();

            if ( m_CurrentPosition < m_OriginalSource.Length )
            {
                text = text.Take( m_CurrentPosition ).ToArray();
            }

            int lineCount = 0;
            int lastNewLine = 0;

            for ( int i = 0; i < text.Length; i++ )
            {
                if ( text[i] == '\n' )
                {
                    lastNewLine = i;
                    lineCount++;
                }
            }

            int textStart = lastNewLine + 1;
            int nextNewLine = m_OriginalSource.IndexOf( '\n', textStart );

            if ( nextNewLine == -1 )
            {
                nextNewLine = m_OriginalSource.Length - textStart;
            }

            return ( m_OriginalSource.Substring( textStart, (nextNewLine - 1)-textStart).Trim(), lineCount, m_CurrentPosition - lastNewLine );
        }

        public BSExpression Parse( int start )
        {
            BSExpression expr = ParseWord();

            return Parse( expr, start );
        }

        public BSExpression Parse( BSExpression expr, int start )
        {
            while ( m_CurrentPosition < m_OriginalSource.Length )
            {
                while ( Is( '(' ) || Is( '[' ) )
                {
                    if ( Is( '(' ) )
                    {
                        expr = ParseInvocation( expr );
                    }
                    else
                    {
                        expr = ParseArrayAccess( expr );
                    }
                }

                string key = ParseKeyword();

                if ( key.StartsWith( ")" ) ||
                     key.StartsWith( "]" ) ||
                     key == "," || !BSOperatorPreceedenceTable.Has(start, key))
                {
                    m_CurrentPosition -= key.Length;

                    break;
                }

                if ( key != "")
                {
                    BSOperator o = BSOperatorPreceedenceTable.Get(start, key);

                    expr = Parse( o.Parse( expr, this ), o.Preceedence );
                }

                if ( m_CurrentPosition >= m_OriginalSource.Length || m_OriginalSource[m_CurrentPosition] == '\n' )
                {
                    break;
                }
            }

            return expr;
        }

        public string ParseArgumentName()
        {
            ReadWhitespace();

            if ( !IsWordStart() )
            {
                throw new BSParserException( "Can not Parse Word", this );
            }

            StringBuilder sb = new StringBuilder();
            sb.Append( m_OriginalSource[m_CurrentPosition] );
            m_CurrentPosition++;

            while ( IsWordMiddle() )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            string wordName = sb.ToString();

            return wordName;
        }

        public BSExpression ParseArrayAccess( BSExpression expr )
        {
            if ( Is( '[' ) )
            {
                m_CurrentPosition++;
                BSExpression i = ParseExpression( int.MaxValue );
                ReadWhitespace();

                if ( !Is( ']' ) )
                {
                    throw new BSParserException( "Expected '['", this );
                }

                m_CurrentPosition++;

                return new BSArrayAccessExpression( expr, i );
            }

            throw new BSParserException( "Expected ']'", this );
        }

        public (BSExpression, BSExpression[]) ParseConditionalBlock()
        {
            if ( Is( '(' ) )
            {
                m_CurrentPosition++;

                BSExpression condition = ParseExpression(int.MaxValue);
                ReadWhitespace();

                if ( !Is( ')' ) )
                {
                    throw new BSParserException( "Expected ')'", this );
                }

                m_CurrentPosition++;
                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return ( condition, b );
            }

            throw new BSParserException( "Expected '('", this );
        }

        public BSExpression ParseExpression( int start )
        {
            BSExpression expr = ParseValue();

            while ( Is( '(' ) || Is( '[' ) )
            {
                if ( Is( '(' ) )
                {
                    expr = ParseInvocation( expr );
                }
                else
                {
                    expr = ParseArrayAccess( expr );
                }
            }

            string key = ParseKeyword();

            if ( key.StartsWith( ")" ) ||
                 key.StartsWith( "]" ) ||
                 key == "," || !BSOperatorPreceedenceTable.Has(start, key))
            {
                m_CurrentPosition -= key.Length;
            }
            else if ( key != "" )
            {
                BSOperator o = BSOperatorPreceedenceTable.Get(start, key);

                BSExpression r = Parse(o.Parse(expr, this), start);

                return r;
            }

            return expr;
        }

        public BSExpression ParseFunction( bool isGlobal )
        {
            StringBuilder sb = new StringBuilder();
            ReadWhitespace();

            string funcName;

            if ( IsWordStart() )
            {

                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;

                while ( IsWordMiddle() )
                {
                    sb.Append( m_OriginalSource[m_CurrentPosition] );
                    m_CurrentPosition++;
                }

                funcName = sb.ToString();
            }
            else
            {
                funcName = "";

                if ( isGlobal )
                {
                    throw new BSParserException( "A global anonymous function is not allowed.", this );
                }
            }

            ReadWhitespace();
            (bool, string)[] args = ParseArgumentList();

            ReadWhitespaceAndNewLine();

            string block = ParseBlock();

            BSParser p = new BSParser( block );

            BSExpression[] b = p.ParseToEnd();

            return new BSFunctionDefinitionExpression( funcName, args, b, isGlobal );
        }

        public BSExpression ParseInvocation( BSExpression expr )
        {
            if ( Is( '(' ) )
            {
                m_CurrentPosition++;

                List < BSExpression > exprs = new List < BSExpression >();

                if ( !Is( ')' ) )
                {
                    exprs.Add( ParseExpression(int.MaxValue) );
                    ReadWhitespace();

                    while ( Is( ',' ) )
                    {
                        m_CurrentPosition++;
                        exprs.Add( ParseExpression(int.MaxValue) );
                        ReadWhitespace();
                    }

                    ReadWhitespace();

                    if ( !Is( ')' ) )
                    {
                        throw new BSParserException( "Expected ')'", this );
                    }

                    m_CurrentPosition++;

                    return new BSInvocationExpression( expr, exprs.ToArray() );
                }
                else
                {
                    m_CurrentPosition++;

                    return new BSInvocationExpression( expr, exprs.ToArray() );
                }
            }

            throw new BSParserException( "Expected '('", this );
        }

        public string ParseKey()
        {
            ReadWhitespace();
            StringBuilder sb = new StringBuilder();

            while ( m_OriginalSource.Length > m_CurrentPosition &&
                    !char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) &&
                    m_OriginalSource[m_CurrentPosition] != '_' )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            return sb.ToString();
        }

        public string ParseKeyword()
        {
            ReadWhitespace();
            StringBuilder sb = new StringBuilder();

            while ( m_OriginalSource.Length > m_CurrentPosition &&
                    !char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) &&
                    !char.IsLetterOrDigit( m_OriginalSource, m_CurrentPosition ) &&
                    m_OriginalSource[m_CurrentPosition] != '_' )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            return sb.ToString();
        }

        public BSExpression ParseNumber()
        {
            int negative = 1;

            if ( Is( '-' ) )
            {
                m_CurrentPosition++;
                negative = -1;
            }

            StringBuilder sb = new StringBuilder();
            bool isFraction = false;

            while ( IsDigit() || !isFraction && Is( '.' ) )
            {
                if ( Is( '.' ) )
                {
                    isFraction = true;
                }

                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            if ( sb.Length == 0 )
            {
                throw new BSParserException( "Can not parse Number", this );
            }

            return new BSValueExpression( negative * decimal.Parse( sb.ToString() ) );
        }

        public BSExpression ParseString()
        {
            ReadWhitespace();

            if ( !IsStringQuotes() )
            {
                throw new BSParserException( "Expected '\"'", this );
            }

            m_CurrentPosition++;
            StringBuilder sb = new StringBuilder();
            bool isEscaped = false;

            while ( !IsStringQuotes() && !IsNewLine() )
            {
                if ( m_OriginalSource[m_CurrentPosition] == '\\' )
                {
                    isEscaped = !isEscaped;
                }
                else
                {
                    sb.Append( m_OriginalSource[m_CurrentPosition] );
                }

                m_CurrentPosition++;

                if ( isEscaped )
                {
                    string s = Regex.Unescape("\\" + m_OriginalSource[m_CurrentPosition]);
                    sb.Append(s);

                    m_CurrentPosition++;
                    isEscaped = false;
                }
            }

            if ( !IsStringQuotes() )
            {
                throw new BSParserException( "Expected '\"'", this );
            }

            m_CurrentPosition++; //Skip over string quotes
            string str = sb.ToString();

            return new BSValueExpression( str );
        }

        public BSExpression[] ParseToEnd()
        {
            ReadWhitespaceAndNewLine();
            List < BSExpression > ret = new List < BSExpression >();

            while ( m_CurrentPosition < m_OriginalSource.Length )
            {
                ReadWhitespaceAndNewLine();

                if ( m_CurrentPosition < m_OriginalSource.Length )
                {
                    ret.Add( Parse( int.MaxValue ) );
                }
            }

            return ret.ToArray();
        }

        public BSExpression ParseValue()
        {
            ReadWhitespace();

            if ( IsStringQuotes() )
            {
                return ParseString();
            }

            if ( IsWordStart() )
            {
                return ParseWord();
            }

            if ( IsDigit() || Is( '-' ) || Is( '.' ) )
            {
                return ParseNumber();
            }

            if ( Is( '[' ) && Is( 1, ']' ) )
            {
                m_CurrentPosition += 2;

                return new BSArrayExpression();
            }

            if ( Is( '{' ) && Is( 1, '}' ) )
            {
                m_CurrentPosition += 2;

                return new BSTableExpression();
            }

            if ( Is( '(' ) )
            {
                m_CurrentPosition++;
                BSExpression expr = ParseExpression(int.MaxValue);
                m_CurrentPosition++;

                return expr;
            }

            if ( Is( '!' ) )
            {
                m_CurrentPosition++;

                return BSOperatorPreceedenceTable.GetPrefix(int.MaxValue, "!").Parse( ParseExpression(int.MaxValue), this );
            }

            throw new BSParserException( "Can not Parse Value", this );
        }

        public BSExpression ParseWord()
        {
            return ParseWord( null );
        }

        public BSExpression ParseWord( BSExpression left )
        {
            ReadWhitespace();
            string wordName = GetNextWord();

            bool isGlobal = false;

            if ( wordName == "function" || ( isGlobal = wordName == "global" ) )
            {
                if ( isGlobal )
                {
                    ReadWhitespace();
                    wordName = GetNextWord();

                    if ( wordName != "function" )
                    {
                        throw new BSParserException( "Expected 'function' after 'global'", this );
                    }
                }

                return ParseFunction( isGlobal );
            }

            if (wordName == "return")
            {
                return new BSReturnExpression(ParseExpression(int.MaxValue));
            }
            if (wordName == "continue")
            {
                return new BSContinueExpression();
            }
            if (wordName == "break")
            {
                return new BSBreakExpression();
            }

            if ( wordName == "if" )
            {
                Dictionary < BSExpression, BSExpression[] > cMap =
                    new Dictionary < BSExpression, BSExpression[] >();

                BSExpression[] elseBlock = null;

                (BSExpression, BSExpression[]) a = ParseConditionalBlock();
                cMap.Add( a.Item1, a.Item2 );
                int resetIndex = ReadWhitespaceAndNewLine();

                if ( Current != '\0' )
                {
                    wordName = GetNextWord();

                    while ( wordName == "else" )
                    {
                        ReadWhitespaceAndNewLine();

                        if ( Is( '{' ) )
                        {
                            string block = ParseBlock();

                            BSParser p = new BSParser( block );

                            elseBlock = p.ParseToEnd();

                            break;
                        }

                        resetIndex = ReadWhitespaceAndNewLine();
                        wordName = GetNextWord();
                        ReadWhitespaceAndNewLine();

                        if ( wordName == "if" )
                        {
                            (BSExpression, BSExpression[]) sA = ParseConditionalBlock();
                            cMap.Add( sA.Item1, sA.Item2 );
                            resetIndex = ReadWhitespaceAndNewLine();
                            wordName = GetNextWord();
                            ReadWhitespaceAndNewLine();
                        }
                        else
                        {
                            m_CurrentPosition = resetIndex;
                        }
                    }

                    if ( wordName != "else" )
                    {
                        m_CurrentPosition = resetIndex;
                    }
                }

                return new BSIfExpression( cMap, elseBlock );
            }

            if ( wordName == "foreach" )
            {
                ReadWhitespace();

                string[] vars;

                if ( Is( '(' ) )
                {
                    vars = ParseArgumentList().Select( x => x.Item2 ).ToArray();
                }
                else
                {
                    vars = new[] { GetNextWord() };
                }

                ReadWhitespace();
                string inStr = GetNextWord();

                if ( inStr != "in" )
                {
                    throw new BSParserException( $"Expected 'in' got '{inStr}'", this );
                }

                ReadWhitespace();
                BSExpression cDecl = ParseExpression(int.MaxValue);
                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return new BSForeachExpression( vars, cDecl, b );
            }

            if ( wordName == "while" )
            {
                (BSExpression, BSExpression[]) a = ParseConditionalBlock();

                return new BSWhileExpression( a.Item1, a.Item2 );
            }

            if ( wordName == "for" )
            {
                BSAssignExpression cDecl = ( BSAssignExpression ) ParseExpression(int.MaxValue);
                BSPropertyExpression cProp = ( BSPropertyExpression ) cDecl.GetLeft();

                string untilWord = ParseKey();

                if ( !untilWord.StartsWith( "while" ) )
                {
                    throw new BSParserException( "Expected while", this );
                }

                ReadWhitespace();

                BSExpression cCond;

                string op = untilWord.Substring( "while".Length, untilWord.Length - "while".Length );

                if ( string.IsNullOrEmpty( op ) )
                {
                    //Check condition to be true
                    cCond = ParseExpression(int.MaxValue);
                }
                else
                {
                    //Use OP to wrap the condition into an expression that checks for true/false
                    switch ( op )
                    {
                        case "=":
                            cCond = BSOperatorPreceedenceTable.Get(int.MaxValue, "==").
                                                Parse( cProp, this );

                            break;

                        case "!":
                            cCond = BSOperatorPreceedenceTable.Get(int.MaxValue, "!=").
                                                               Parse( cProp, this );

                            break;

                        case "<":
                            cCond = BSOperatorPreceedenceTable.Get(int.MaxValue, "<").
                                                               Parse( cProp, this );

                            break;

                        case ">":
                            cCond = BSOperatorPreceedenceTable.Get(int.MaxValue, ">").
                                                               Parse( cProp, this );

                            break;

                        case "<=":
                            cCond = BSOperatorPreceedenceTable.Get(int.MaxValue, "<=").
                                                               Parse( cProp, this );

                            break;

                        case ">=":
                            cCond = BSOperatorPreceedenceTable.Get(int.MaxValue, ">=").
                                                               Parse( cProp, this );

                            break;

                        default:
                            throw new BSParserException( "Invalid Operator: " + op, this );
                    }
                }

                string step = ParseKey();

                BSExpression cInc;

                if ( step != "step" )
                {
                    cInc = new BSAssignExpression(
                        cProp,
                        new BSInvocationExpression(
                            new BSProxyExpression(
                                new BSFunction(
                                    "function +(a, b)",
                                    objects => BSOperatorImplementationResolver.
                                               ResolveImplementation( "+", objects ).
                                               ExecuteOperator( objects ),
                                    2 ) ),
                            new BSExpression[] { cProp, new BSValueExpression( ( decimal ) 1 ) } ) );
                }
                else
                {
                    cInc = new BSAssignExpression(
                        cProp,
                        BSOperatorPreceedenceTable.Get(int.MaxValue, "+").
                                                   Parse( cProp, this ) );
                }

                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return new BSForExpression( cDecl, cCond, cInc, b );

            }

            if ( wordName == "null" && left == null )
            {
                return new BSValueExpression( null );
            }

            return new BSPropertyExpression( left, wordName );
        }

        public int ReadWhitespace()
        {
            int r = m_CurrentPosition;

            while ( m_OriginalSource.Length > m_CurrentPosition &&
                    m_OriginalSource[m_CurrentPosition] != '\n' &&
                    char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) )
            {
                m_CurrentPosition++;
            }

            if ( ReadComment() )
            {
                ReadWhitespaceAndNewLine();
            }

            return r;
        }

        public int ReadWhitespaceAndNewLine()
        {
            int r = m_CurrentPosition;

            while ( m_OriginalSource.Length > m_CurrentPosition &&
                    char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) )
            {
                m_CurrentPosition++;
            }

            if ( ReadComment() )
            {
                ReadWhitespaceAndNewLine();
            }

            return r;
        }

        #endregion

        #region Private

        private string GetNextWord()
        {
            if ( !IsWordStart() )
            {
                throw new BSParserException( "Can not Parse Word", this );
            }

            StringBuilder sb = new StringBuilder();
            sb.Append( m_OriginalSource[m_CurrentPosition] );
            m_CurrentPosition++;

            while ( IsWordMiddle() )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            string wordName = sb.ToString();

            return wordName;
        }

        private bool Is( char c )
        {
            return m_OriginalSource.Length > m_CurrentPosition && m_OriginalSource[m_CurrentPosition] == c;
        }

        private bool Is( int off, char c )
        {
            return m_OriginalSource.Length > m_CurrentPosition + off && m_OriginalSource[m_CurrentPosition + off] == c;
        }

        private bool IsDigit()
        {
            return m_OriginalSource.Length > m_CurrentPosition && char.IsDigit( m_OriginalSource[m_CurrentPosition] );
        }

        private bool IsNewLine()
        {
            return m_OriginalSource.Length > m_CurrentPosition && m_OriginalSource[m_CurrentPosition] == '\n';
        }

        private bool IsStringQuotes()
        {
            return m_OriginalSource.Length > m_CurrentPosition && m_OriginalSource[m_CurrentPosition] == '\"';
        }

        private bool IsWordMiddle()
        {
            return m_OriginalSource.Length > m_CurrentPosition &&
                   ( char.IsLetterOrDigit( m_OriginalSource[m_CurrentPosition] ) ||
                     m_OriginalSource[m_CurrentPosition] == '_' );
        }

        private bool IsWordStart()
        {
            return m_OriginalSource.Length > m_CurrentPosition &&
                   ( char.IsLetter( m_OriginalSource[m_CurrentPosition] ) ||
                     m_OriginalSource[m_CurrentPosition] == '_' );
        }

        private (bool, string)[] ParseArgumentList()
        {
            if ( !Is( '(' ) )
            {
                throw new BSParserException( "Expected '('", this );
            }

            m_CurrentPosition++;

            List < (bool, string) > args = new List < (bool, string) >();

            if ( !Is( ')' ) )
            {
                bool allowNull = true;

                if ( Is( '!' ) )
                {
                    m_CurrentPosition++;
                    allowNull = false;
                }

                args.Add( ( allowNull, ParseArgumentName() ) );
                ReadWhitespace();

                while ( Is( ',' ) )
                {
                    m_CurrentPosition++;
                    allowNull = true;
                    ReadWhitespace();

                    if ( Is( '!' ) )
                    {
                        m_CurrentPosition++;
                        allowNull = false;
                    }

                    args.Add( ( allowNull, ParseArgumentName() ) );
                    ReadWhitespace();
                }

                ReadWhitespace();

                if ( !Is( ')' ) )
                {
                    throw new BSParserException( "Expected ')'", this );
                }

                m_CurrentPosition++;

                return args.ToArray();
            }
            else
            {
                m_CurrentPosition++;

                return args.ToArray();
            }
        }

        private string ParseBlock()
        {
            if ( !Is( '{' ) )
            {
                throw new BSParserException( "Expected '{'", this );
            }

            m_CurrentPosition++;

            //Find Closing
            int start = m_CurrentPosition;
            int end = FindClosing( '{', '}' );

            string s = m_OriginalSource.Substring( start, end - start );

            m_CurrentPosition = end + 1;

            return s;
        }

        private bool ReadComment()
        {
            if ( m_OriginalSource.Length > m_CurrentPosition + 1 &&
                 m_OriginalSource[m_CurrentPosition] == '/' &&
                 m_OriginalSource[m_CurrentPosition] == '/' )
            {
                while ( Current != '\n' && Current != '\0' )
                {
                    m_CurrentPosition++;
                }

                m_CurrentPosition++;

                return true;
            }

            return false;
        }

        #endregion
    }

}
