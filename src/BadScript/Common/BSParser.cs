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
using BadScript.Common.Types.Implementations;

namespace BadScript.Common
{

    public class BSParser
    {

        private readonly string m_OriginalSource;
        private int m_CurrentPosition;
        private readonly int m_SourcePositionOffset;
        private readonly string m_OffsetSource;
        private readonly bool m_AllowFunctionBaseInvocation;
        private readonly bool m_AllowFunctionGlobal;

        private char Current =>
            m_CurrentPosition < m_OriginalSource.Length ? m_OriginalSource[m_CurrentPosition] : '\0';

        private char[] CurrentArray => m_OriginalSource.ToCharArray();

        #region Public

        public BSParser(
            string script,
            string originalSource = null,
            int srcPosOffset = 0,
            bool allowFunctionBaseInvocation = false,
            bool allowFunctionGlobal = true )
        {
            m_AllowFunctionBaseInvocation = allowFunctionBaseInvocation;
            m_AllowFunctionGlobal = allowFunctionGlobal;
            m_OriginalSource = script;
            m_SourcePositionOffset = srcPosOffset;
            m_OffsetSource = originalSource ?? script;

            m_CurrentPosition = 0;
        }

        public SourcePosition CreateSourcePosition()
        {
            return CreateSourcePosition( m_CurrentPosition + m_SourcePositionOffset );
        }

        public int FindClosing( string open, string close )
        {
            int level = 1;

            while ( true )
            {
                if ( m_CurrentPosition >= m_OriginalSource.Length || Is( '\0' ) )
                {
                    throw new BSParserException( $"Expected '{close}'", this );
                }

                ReadWhitespaceAndNewLine();

                if ( IsStringQuotes() )
                {
                    BSExpression e = ParseString();

                    continue;
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

        public string GetNextWord()
        {
            if ( !TryReadNextWord( out string word ) )
            {
                throw new BSParserException( "Can not Parse Word", this );
            }

            return word;
        }

        public BSExpression Parse( int start )
        {
            //BSExpression expr = ParseWord();
            BSExpression expr = ParseValue();

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
                     key == "," ||
                     !BSOperatorPreceedenceTable.Has( start, key ) )
                {
                    m_CurrentPosition -= key.Length;

                    break;
                }

                if ( key != "" )
                {
                    BSOperator o = BSOperatorPreceedenceTable.Get( start, key );

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
            ReadWhitespaceAndNewLine();

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
            int pos = m_CurrentPosition;

            if ( Is( '[' ) )
            {
                m_CurrentPosition++;
                BSExpression i = ParseExpression( int.MaxValue );
                ReadWhitespaceAndNewLine();

                if ( !Is( ']' ) )
                {
                    throw new BSParserException( "Expected '['", this );
                }

                m_CurrentPosition++;

                return new BSArrayAccessExpression( CreateSourcePosition( pos ), expr, i );
            }

            throw new BSParserException( "Expected ']'", this );
        }

        public (BSExpression, BSExpression[]) ParseConditionalBlock()
        {
            if ( Is( '(' ) )
            {
                m_CurrentPosition++;

                BSExpression condition = ParseExpression( int.MaxValue );
                ReadWhitespaceAndNewLine();

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

        public BSExpression ParseEnumerableFunction( bool isGlobal )
        {
            StringBuilder sb = new StringBuilder();
            ReadWhitespaceAndNewLine();

            string funcName;
            int pos = m_CurrentPosition;

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

            ReadWhitespaceAndNewLine();
            BSFunctionParameter[] args = ParseArgumentList();

            ReadWhitespaceAndNewLine();

            if ( Is( '=' ) && Is( 1, '>' ) )
            {
                m_CurrentPosition += 2;
                ReadWhitespaceAndNewLine();

                return new BSEnumerableFunctionDefinitionExpression(
                                                                    CreateSourcePosition( pos ),
                                                                    funcName,
                                                                    isGlobal,
                                                                    args,
                                                                    new[] { Parse( int.MaxValue ) }
                                                                   );
            }

            string block = ParseBlock();

            BSParser p = new BSParser( block );

            BSExpression[] b = p.ParseToEnd();

            return new BSEnumerableFunctionDefinitionExpression(
                                                                CreateSourcePosition( pos ),
                                                                funcName,
                                                                isGlobal,
                                                                args,
                                                                b
                                                               );
        }

        public BSExpression ParseExpression( int start )
        {
            BSExpression expr = ParseValue();

            string preop = ParseKey();

            if ( BSOperatorPreceedenceTable.HasPostfix( start, preop ) )
            {
                expr = BSOperatorPreceedenceTable.GetPostfix( int.MaxValue, preop ).
                                                  Parse( expr, this );
            }
            else
            {
                m_CurrentPosition -= preop.Length;
            }

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
                 key == "," ||
                 !BSOperatorPreceedenceTable.Has( start, key ) )
            {
                m_CurrentPosition -= key.Length;
            }
            else if ( key != "" )
            {
                BSOperator o = BSOperatorPreceedenceTable.Get( start, key );

                BSExpression r = Parse( o.Parse( expr, this ), start );

                return r;
            }

            return expr;
        }

        public BSExpression ParseClass( bool isGlobal )
        {
            if ( !isGlobal )
            {
                throw new BSParserException( "Classes can only be defined 'global'" );
            }

            StringBuilder sb = new StringBuilder();
            ReadWhitespaceAndNewLine();

            SourcePosition pos = CreateSourcePosition();

            sb.Append( m_OriginalSource[m_CurrentPosition] );
            m_CurrentPosition++;

            while ( IsWordMiddle() )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            string className = sb.ToString();
            string baseClass = null;
            ReadWhitespaceAndNewLine();

            if ( Is( ':' ) )
            {
                m_CurrentPosition++;
                ReadWhitespaceAndNewLine();
                sb.Clear();
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;

                while ( IsWordMiddle() )
                {
                    sb.Append( m_OriginalSource[m_CurrentPosition] );
                    m_CurrentPosition++;
                }

                baseClass = sb.ToString();
            }

            ReadWhitespaceAndNewLine();
            int off = m_CurrentPosition + 1;
            string block = ParseBlock();
            BSParser p = new BSParser( block, m_OriginalSource, off, baseClass != null, false );
            BSExpression[] exprs = p.ParseToEnd();

            Dictionary < string, BSExpression > expressions = new Dictionary < string, BSExpression >();

            foreach ( BSExpression bsExpression in exprs )
            {
                if ( bsExpression is BSAssignExpression assign )
                {
                    expressions[( assign.Left as BSPropertyExpression ).Right] = assign.Right;
                }
                else if ( bsExpression is BSFunctionDefinitionExpression func )
                {
                    List < BSExpression > funcBlock = new List < BSExpression >( func.Block );
                    BSInvocationExpression baseInvocation = func.BaseInvocation;

                    if ( baseInvocation != null )
                    {
                        string baseFunc = func.Name == className ? baseClass : func.Name;

                        baseInvocation.Left = new BSPropertyExpression(
                                                                       SourcePosition.Unknown,
                                                                       new BSPropertyExpression(
                                                                            SourcePosition.Unknown,
                                                                            null,
                                                                            "base"
                                                                           ),
                                                                       baseFunc
                                                                      );

                        funcBlock.Insert( 0, baseInvocation );
                        func.Block = funcBlock.ToArray();
                    }

                    expressions[func.Name] = func;
                }
                else
                {
                    throw new BSParserException( $"Invalid Expression for Type: '{bsExpression}'" );
                }
            }

            return new BSClassExpression( pos, className, baseClass, expressions );
        }

        public BSExpression ParseFunction( bool isGlobal )
        {
            if ( isGlobal && !m_AllowFunctionGlobal )
            {
                throw new BSParserException( "'global' is invalid in this context" );
            }

            StringBuilder sb = new StringBuilder();
            ReadWhitespaceAndNewLine();

            int pos = m_CurrentPosition;

            string funcName;
            BSInvocationExpression baseInvocation = null;

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

                ReadWhitespaceAndNewLine();
            }
            else
            {
                funcName = "";

                if ( isGlobal )
                {
                    throw new BSParserException( "A global anonymous function is not allowed.", this );
                }
            }

            ReadWhitespaceAndNewLine();
            BSFunctionParameter[] args = ParseArgumentList();

            ReadWhitespaceAndNewLine();

            if ( Is( ':' ) )
            {
                m_CurrentPosition++;
                ReadWhitespaceAndNewLine();
                SourcePosition basePos = CreateSourcePosition();
                string word = GetNextWord();

                if ( word != "base" )
                {
                    throw new BSParserException( "Base Invocations have to Invoke 'base'", this );
                }

                if ( m_AllowFunctionBaseInvocation )
                {
                    baseInvocation = ParseInvocation( new BSPropertyExpression( basePos, null, "base" ) );
                }
                else
                {
                    throw new BSParserException( "Base Invocation is Invalid in this Context", this );
                }
            }

            ReadWhitespaceAndNewLine();

            if ( Is( '=' ) && Is( 1, '>' ) )
            {
                m_CurrentPosition += 2;
                ReadWhitespaceAndNewLine();

                return new BSFunctionDefinitionExpression(
                                                          CreateSourcePosition( pos ),
                                                          funcName,
                                                          args,
                                                          new[] { Parse( int.MaxValue ) },
                                                          isGlobal,
                                                          baseInvocation
                                                         );
            }

            string block = ParseBlock();

            BSParser p = new BSParser( block, m_OffsetSource, m_SourcePositionOffset + pos );

            BSExpression[] b = p.ParseToEnd();

            return new BSFunctionDefinitionExpression(
                                                      CreateSourcePosition( pos ),
                                                      funcName,
                                                      args,
                                                      b,
                                                      isGlobal,
                                                      baseInvocation
                                                     );
        }

        public BSInvocationExpression ParseInvocation( BSExpression expr )
        {
            int pos = m_CurrentPosition;

            if ( Is( '(' ) )
            {
                m_CurrentPosition++;

                List < BSExpression > exprs = new List < BSExpression >();

                ReadWhitespaceAndNewLine();

                if ( !Is( ')' ) )
                {
                    ReadWhitespaceAndNewLine();
                    exprs.Add( ParseExpression( int.MaxValue ) );
                    ReadWhitespaceAndNewLine();

                    while ( Is( ',' ) )
                    {
                        m_CurrentPosition++;
                        exprs.Add( ParseExpression( int.MaxValue ) );
                        ReadWhitespaceAndNewLine();
                    }

                    ReadWhitespaceAndNewLine();

                    if ( !Is( ')' ) )
                    {
                        throw new BSParserException( "Expected ')'", this );
                    }

                    m_CurrentPosition++;

                    return new BSInvocationExpression( CreateSourcePosition( pos ), expr, exprs.ToArray() );
                }
                else
                {
                    m_CurrentPosition++;

                    return new BSInvocationExpression( CreateSourcePosition( pos ), expr, exprs.ToArray() );
                }
            }

            throw new BSParserException( "Expected '('", this );
        }

        public string ParseKey()
        {
            ReadWhitespaceAndNewLine();
            StringBuilder sb = new StringBuilder();

            while ( m_OriginalSource.Length > m_CurrentPosition &&
                    !char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) &&
                    m_OriginalSource[m_CurrentPosition] != '_' &&
                    m_OriginalSource[m_CurrentPosition] != ')' )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            return sb.ToString();
        }

        public string ParseKeyword()
        {
            ReadWhitespaceAndNewLine();
            StringBuilder sb = new StringBuilder();

            while ( m_OriginalSource.Length > m_CurrentPosition &&
                    !char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) &&
                    !char.IsLetterOrDigit( m_OriginalSource, m_CurrentPosition ) &&
                    m_OriginalSource[m_CurrentPosition] != '_' &&
                    m_OriginalSource[m_CurrentPosition] != '\"' )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            return sb.ToString();
        }

        public BSExpression ParseNumber()
        {
            int negative = 1;
            int pos = m_CurrentPosition;

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

            return new BSValueExpression( CreateSourcePosition( pos ), negative * decimal.Parse( sb.ToString() ) );
        }

        public BSExpression ParseString()
        {
            ReadWhitespaceAndNewLine();

            if ( !IsStringQuotes() )
            {
                throw new BSParserException( "Expected '\"'", this );
            }

            int pos = m_CurrentPosition;
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
                    string s = Regex.Unescape( "\\" + m_OriginalSource[m_CurrentPosition] );
                    sb.Append( s );

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

            return new BSValueExpression( CreateSourcePosition( pos ), str );
        }

#if !DEBUG
        private class ParserException : Exception
        {
            public ParserException( Exception inner ) : base( inner.Message ){}

            public override string StackTrace => "";

    }
#endif
        public BSExpression[] ParseToEnd()
        {
            try
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
            catch ( Exception e )
            {
                throw
#if !DEBUG
                    new ParserException(e)
#endif
                    ;
            }
        }

        public BSExpression ParseValue()
        {
            int pos = m_CurrentPosition;
            ReadWhitespaceAndNewLine();

            if ( IsStringQuotes() )
            {
                return ParseString();
            }

            if ( IsWordStart() )
            {
                return ParseWord();
            }

            if ( IsDigit() || Is( '-' ) && !Is( 1, '-' ) || Is( '.' ) )
            {
                return ParseNumber();
            }

            if ( Is( '[' ) )
            {
                m_CurrentPosition += 1;
                List < BSExpression > es = new List < BSExpression >();
                ReadWhitespaceAndNewLine();

                if ( !Is( ']' ) )
                {
                    es.Add( ParseExpression( int.MaxValue ) );
                    ReadWhitespaceAndNewLine();

                    while ( Is( ',' ) )
                    {
                        m_CurrentPosition++;
                        ReadWhitespaceAndNewLine();

                        if ( Is( ']' ) )
                        {
                            break;
                        }

                        es.Add( ParseExpression( int.MaxValue ) );
                        ReadWhitespaceAndNewLine();
                    }
                }

                m_CurrentPosition += 1;

                return new BSArrayExpression( CreateSourcePosition( pos ), es.ToArray() );
            }

            if ( Is( '{' ) )
            {
                m_CurrentPosition += 1;
                Dictionary < string, BSExpression > es = new Dictionary < string, BSExpression >();
                ReadWhitespaceAndNewLine();

                if ( !Is( '}' ) )
                {
                    string key = GetNextWord();
                    ReadWhitespaceAndNewLine();
                    string equal = ParseKeyword();
                    ReadWhitespaceAndNewLine();
                    es[key] = ParseExpression( int.MaxValue );
                    ReadWhitespaceAndNewLine();

                    while ( Is( ',' ) )
                    {
                        m_CurrentPosition += 1;
                        ReadWhitespaceAndNewLine();
                        key = GetNextWord();
                        ReadWhitespaceAndNewLine();
                        equal = ParseKeyword();
                        ReadWhitespaceAndNewLine();
                        es[key] = ParseExpression( int.MaxValue );
                        ReadWhitespaceAndNewLine();

                        if ( Is( ']' ) )
                        {
                            break;
                        }
                    }
                }

                m_CurrentPosition += 1;

                return new BSTableExpression( CreateSourcePosition( pos ), es );
            }

            if ( Is( '(' ) )
            {
                m_CurrentPosition++;
                BSExpression expr = ParseExpression( int.MaxValue );
                m_CurrentPosition++;

                return expr;
            }

            string k = ParseKeyword();

            if ( BSOperatorPreceedenceTable.HasPrefix( int.MaxValue, k ) )
            {
                return BSOperatorPreceedenceTable.GetPrefix( int.MaxValue, k ).
                                                  Parse( ParseExpression( int.MaxValue ), this );
            }

            throw new BSParserException( "Can not Parse Value", this );
        }

        public BSExpression ParseWord()
        {
            return ParseWord( null );
        }

        public BSExpression ParseWord( BSExpression left )
        {
            ReadWhitespaceAndNewLine();
            int pos = m_CurrentPosition;
            string wordName = GetNextWord();

            bool isGlobal = wordName == "global";

            if ( isGlobal )
            {
                ReadWhitespaceAndNewLine();
                wordName = GetNextWord();
            }

            if ( wordName == "function" )
            {
                return ParseFunction( isGlobal );
            }

            if ( wordName == "class" )
            {
                return ParseClass( isGlobal );
            }

            if ( wordName == "enumerable" )
            {
                return ParseEnumerableFunction( isGlobal );
            }

            if ( isGlobal )
            {
                throw new BSParserException( "Expected 'function' or 'enumerable' after 'global'", this );
            }

            if ( wordName == "return" )
            {
                return new BSReturnExpression( CreateSourcePosition( pos ), ParseExpression( int.MaxValue ) );
            }

            if ( wordName == "new" )
            {
                ReadWhitespaceAndNewLine();
                SourcePosition p = CreateSourcePosition( pos );

                return new BSNewInstanceExpression(
                                                   p,
                                                   ParseInvocation( new BSPropertyExpression( p, null, GetNextWord() ) )
                                                  );
            }

            if ( wordName == "continue" )
            {
                return new BSContinueExpression( CreateSourcePosition( pos ) );
            }

            if ( wordName == "break" )
            {
                return new BSBreakExpression( CreateSourcePosition( pos ) );
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

                            if ( !TryReadNextWord( out wordName ) )
                            {
                                m_CurrentPosition = resetIndex;
                            }
                            else
                            {
                                ReadWhitespaceAndNewLine();
                            }
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

                return new BSIfExpression( CreateSourcePosition( pos ), cMap, elseBlock );
            }

            if ( wordName == "try" )
            {
                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] tryBlock = p.ParseToEnd();
                ReadWhitespaceAndNewLine();
                string catchStr = GetNextWord();

                if ( catchStr != "catch" )
                {
                    throw new BSParserException( "Expected 'catch' after 'try' block", this );
                }

                ReadWhitespaceAndNewLine();

                string exVar = null;

                if ( Is( '(' ) )
                {
                    m_CurrentPosition++;
                    exVar = ParseArgumentName();

                    if ( !Is( ')' ) )
                    {
                        throw new BSParserException( "Expected ')' after catch clause", this );
                    }

                    m_CurrentPosition++;
                }

                ReadWhitespaceAndNewLine();
                string cBlock = ParseBlock();

                BSParser cP = new BSParser( cBlock );

                BSExpression[] catchBlock = cP.ParseToEnd();
                ReadWhitespaceAndNewLine();

                return new BSTryExpression( CreateSourcePosition( pos ), tryBlock, catchBlock, exVar );
            }

            if ( wordName == "foreach" )
            {
                ReadWhitespaceAndNewLine();

                string[] vars;

                if ( Is( '(' ) )
                {
                    vars = ParseArgumentList().Select( x => x.Name ).ToArray();
                }
                else
                {
                    vars = new[] { GetNextWord() };
                }

                ReadWhitespaceAndNewLine();
                string inStr = GetNextWord();

                if ( inStr != "in" )
                {
                    throw new BSParserException( $"Expected 'in' got '{inStr}'", this );
                }

                ReadWhitespaceAndNewLine();
                BSExpression cDecl = ParseExpression( int.MaxValue );
                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return new BSForeachExpression( CreateSourcePosition( pos ), vars, cDecl, b );
            }

            if ( wordName == "while" )
            {
                (BSExpression, BSExpression[]) a = ParseConditionalBlock();

                return new BSWhileExpression( CreateSourcePosition( pos ), a.Item1, a.Item2 );
            }

            if ( wordName == "for" )
            {
                BSAssignExpression cDecl = ( BSAssignExpression )ParseExpression( int.MaxValue );
                BSPropertyExpression cProp = ( BSPropertyExpression )cDecl.Left;

                string untilWord = ParseKey();

                if ( !untilWord.StartsWith( "while" ) )
                {
                    throw new BSParserException( "Expected while", this );
                }

                ReadWhitespaceAndNewLine();

                BSExpression cCond;

                string op = untilWord.Substring( "while".Length, untilWord.Length - "while".Length );

                if ( string.IsNullOrEmpty( op ) )
                {
                    //Check condition to be true
                    cCond = ParseExpression( int.MaxValue );
                }
                else
                {
                    //Use OP to wrap the condition into an expression that checks for true/false
                    switch ( op )
                    {
                        case "=":
                            cCond = BSOperatorPreceedenceTable.Get( int.MaxValue, "==" ).
                                                               Parse( cProp, this );

                            break;

                        case "!":
                            cCond = BSOperatorPreceedenceTable.Get( int.MaxValue, "!=" ).
                                                               Parse( cProp, this );

                            break;

                        case "<":
                            cCond = BSOperatorPreceedenceTable.Get( int.MaxValue, "<" ).
                                                               Parse( cProp, this );

                            break;

                        case ">":
                            cCond = BSOperatorPreceedenceTable.Get( int.MaxValue, ">" ).
                                                               Parse( cProp, this );

                            break;

                        case "<=":
                            cCond = BSOperatorPreceedenceTable.Get( int.MaxValue, "<=" ).
                                                               Parse( cProp, this );

                            break;

                        case ">=":
                            cCond = BSOperatorPreceedenceTable.Get( int.MaxValue, ">=" ).
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
                    m_CurrentPosition -= step.Length;

                    cInc = new BSAssignExpression(
                                                  CreateSourcePosition( pos ),
                                                  cProp,
                                                  new BSInvocationExpression(
                                                                             CreateSourcePosition( pos ),
                                                                             new BSProxyExpression(
                                                                                  CreateSourcePosition( pos ),
                                                                                  new BSFunction(
                                                                                       CreateSourcePosition( pos ),
                                                                                       "function +(a, b)",
                                                                                       objects =>
                                                                                           BSOperatorImplementationResolver.
                                                                                               ResolveImplementation(
                                                                                                    "+",
                                                                                                    objects
                                                                                                   ).
                                                                                               ExecuteOperator(
                                                                                                    objects
                                                                                                   ),
                                                                                       2
                                                                                      ),
                                                                                  new BSBinaryOperatorMetaData(
                                                                                       "+",
                                                                                       "a, b",
                                                                                       2
                                                                                      )
                                                                                 ),
                                                                             new BSExpression[]
                                                                             {
                                                                                 cProp,
                                                                                 new BSValueExpression(
                                                                                      CreateSourcePosition( pos ),
                                                                                      ( decimal )1
                                                                                     )
                                                                             }
                                                                            )
                                                 );
                }
                else
                {
                    cInc = new BSAssignExpression(
                                                  CreateSourcePosition( pos ),
                                                  cProp,
                                                  BSOperatorPreceedenceTable.Get( int.MaxValue, "+" ).
                                                                             Parse( cProp, this )
                                                 );
                }

                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return new BSForExpression( CreateSourcePosition( pos ), cDecl, cCond, cInc, b );
            }

            if ( wordName == "null" && left == null )
            {
                return new BSValueExpression( CreateSourcePosition( pos ), null );
            }

            if ( wordName == "true" && left == null )
            {
                return new BSValueExpression( CreateSourcePosition( pos ), BSObject.True );
            }

            if ( wordName == "false" && left == null )
            {
                return new BSValueExpression( CreateSourcePosition( pos ), BSObject.False );
            }

            return new BSPropertyExpression( CreateSourcePosition( pos ), left, wordName );
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

        private SourcePosition CreateSourcePosition( int pos )
        {
            return SourcePosition.GetCurrentLineInfo( m_OffsetSource, pos );
        }

        private bool Is( string s )
        {
            bool isOpen = true;

            for ( int i = 0; i < s.Length; i++ )
            {
                isOpen &= Is( i, s[i] );
            }

            return isOpen;
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

        private BSFunctionParameter[] ParseArgumentList()
        {
            if ( !Is( '(' ) )
            {
                throw new BSParserException( "Expected '('", this );
            }

            m_CurrentPosition++;

            List < BSFunctionParameter > args = new List < BSFunctionParameter >();
            bool needsToBeOptional = false;

            if ( !Is( ')' ) )
            {
                bool notNull = false;
                bool optional = false;
                bool isArgArray = false;

                if ( Is( '*' ) )
                {
                    m_CurrentPosition++;
                    isArgArray = true;
                }
                else if ( Is( '!' ) )
                {
                    m_CurrentPosition++;
                    notNull = true;

                    if ( Is( '?' ) )
                    {
                        m_CurrentPosition++;
                        optional = true;
                        needsToBeOptional = true;
                    }
                }
                else if ( Is( "?" ) )
                {
                    m_CurrentPosition++;
                    optional = true;
                    needsToBeOptional = true;

                    if ( Is( '!' ) )
                    {
                        m_CurrentPosition++;
                        notNull = true;
                    }
                }

                if ( needsToBeOptional && !optional )
                {
                    throw new BSRuntimeException(
                                                 "Invalid Parameters. Optional parameters can not be followed by Non-Optional Parameters."
                                                );
                }

                args.Add( new BSFunctionParameter( ParseArgumentName(), notNull, optional, isArgArray ) );
                ReadWhitespaceAndNewLine();

                while ( Is( ',' ) )
                {
                    m_CurrentPosition++;
                    notNull = false;
                    optional = false;
                    ReadWhitespaceAndNewLine();

                    if ( Is( '!' ) )
                    {
                        m_CurrentPosition++;
                        notNull = true;

                        if ( Is( '?' ) )
                        {
                            m_CurrentPosition++;
                            optional = true;
                            needsToBeOptional = true;
                        }
                    }
                    else if ( Is( "?" ) )
                    {
                        m_CurrentPosition++;
                        optional = true;
                        needsToBeOptional = true;

                        if ( Is( '!' ) )
                        {
                            m_CurrentPosition++;
                            notNull = true;
                        }
                    }

                    if ( needsToBeOptional && !optional )
                    {
                        throw new BSRuntimeException(
                                                     "Invalid Parameters. Optional parameters can not be followed by Non-Optional Parameters."
                                                    );
                    }

                    args.Add( new BSFunctionParameter( ParseArgumentName(), notNull, optional, false ) );
                    ReadWhitespaceAndNewLine();
                }

                ReadWhitespaceAndNewLine();

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
            int end = FindClosing( "{", "}" );

            string s = m_OriginalSource.Substring( start, end - start );

            m_CurrentPosition = end + 1;

            return s;
        }

        private bool ReadComment()
        {
            if ( m_OriginalSource.Length > m_CurrentPosition + 1 &&
                 m_OriginalSource[m_CurrentPosition] == '/' &&
                 m_OriginalSource[m_CurrentPosition + 1] == '/' )
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

        private bool TryReadNextWord( out string word )
        {
            if ( !IsWordStart() )
            {
                word = null;

                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append( m_OriginalSource[m_CurrentPosition] );
            m_CurrentPosition++;

            while ( IsWordMiddle() )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            word = sb.ToString();

            return true;
        }

        #endregion

    }

}
