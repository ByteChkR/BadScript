﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Access;
using BadScript.Parser.Expressions.Implementations.Binary;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Parser.Expressions.Implementations.Types;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Parser.OperatorImplementations;
using BadScript.Parser.Operators;
using BadScript.Parser.Operators.Implementations;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser
{

    public class BSParser
    {

#if !DEBUG
        public class ParserException : Exception
        {

            public override string StackTrace => "";

            #region Public

            public ParserException( Exception inner ) : base( inner.Message )
            {
            }

            #endregion

        }
#endif

        private readonly string m_OriginalSource;
        private int m_CurrentPosition;
        private readonly int m_SourcePositionOffset;
        private readonly string m_OffsetSource;
        private readonly bool m_AllowFunctionBaseInvocation;
        private readonly bool m_AllowFunctionGlobal;

        public char Current => m_CurrentPosition < m_OriginalSource.Length ? m_OriginalSource[m_CurrentPosition] : '\0';

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

        public SourcePosition CreateSourcePosition( int pos )
        {
            return SourcePosition.GetSourcePosition( m_OffsetSource, m_SourcePositionOffset + pos );
        }

        public void DecrementPosition()
        {
            m_CurrentPosition--;
        }

        public void DecrementPosition( int c )
        {
            for ( int i = 0; i < c; i++ )
            {
                DecrementPosition();
            }
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
                    ParseString();

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

        public int GetPosition()
        {
            return m_CurrentPosition;
        }

        public int GetTotalPosition()
        {
            return m_SourcePositionOffset + m_CurrentPosition;
        }

        public void IncrementPosition()
        {
            m_CurrentPosition++;
        }

        public void IncrementPosition( int c )
        {
            for ( int i = 0; i < c; i++ )
            {
                IncrementPosition();
            }
        }

        public bool Is( string s )
        {
            bool isOpen = true;

            for ( int i = 0; i < s.Length; i++ )
            {
                isOpen &= Is( i, s[i] );
            }

            return isOpen;
        }

        public bool Is( char c )
        {
            return m_OriginalSource.Length > m_CurrentPosition && m_OriginalSource[m_CurrentPosition] == c;
        }

        public bool Is( int off, char c )
        {
            return m_OriginalSource.Length > m_CurrentPosition + off && m_OriginalSource[m_CurrentPosition + off] == c;
        }

        public bool IsDigit()
        {
            return m_OriginalSource.Length > m_CurrentPosition && char.IsDigit( m_OriginalSource[m_CurrentPosition] );
        }

        public bool IsNewLine()
        {
            return m_OriginalSource.Length > m_CurrentPosition && m_OriginalSource[m_CurrentPosition] == '\n';
        }

        public bool IsStringQuotes()
        {
            return m_OriginalSource.Length > m_CurrentPosition && m_OriginalSource[m_CurrentPosition] == '\"';
        }

        public bool IsWordMiddle()
        {
            return m_OriginalSource.Length > m_CurrentPosition &&
                   ( char.IsLetterOrDigit( m_OriginalSource[m_CurrentPosition] ) ||
                     m_OriginalSource[m_CurrentPosition] == '_' );
        }

        public bool IsWordStart()
        {
            return m_OriginalSource.Length > m_CurrentPosition &&
                   ( char.IsLetter( m_OriginalSource[m_CurrentPosition] ) ||
                     m_OriginalSource[m_CurrentPosition] == '_' );
        }

        public BSExpression Parse( int start )
        {
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

        public BSFunctionParameter[] ParseArgumentList()
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

        public string ParseBlock()
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

        public BSExpression ParseClass( bool isGlobal )
        {
            StringBuilder sb = new StringBuilder();
            ReadWhitespaceAndNewLine();

            SourcePosition pos = CreateSourcePosition( m_CurrentPosition );

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
            BSParser p = new BSParser( block, m_OffsetSource, m_SourcePositionOffset + off, baseClass != null, false );
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

            return new BSClassExpression( pos, className, baseClass, isGlobal, expressions );
        }

        public (BSExpression, BSExpression[]) ParseConditionalBlock( Func < string, string > blockModifier = null )
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
                block = blockModifier?.Invoke( block ) ?? block;

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return ( condition, b );
            }

            throw new BSParserException( "Expected '('", this );
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

        public BSForeachExpression ParseForeachExpression( int pos )
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

        public BSForExpression ParseForExpression( int pos, Func < string, string > blockModifier = null )
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

            block = blockModifier?.Invoke( block ) ?? block;

            BSParser p = new BSParser( block );

            BSExpression[] b = p.ParseToEnd();

            return new BSForExpression( CreateSourcePosition( pos ), cDecl, cCond, cInc, b );
        }

        public BSFunctionDefinitionExpression ParseFunction(
            int pos,
            bool isGlobal,
            Func < string, string > blockModifier = null )
        {
            if ( isGlobal && !m_AllowFunctionGlobal )
            {
                throw new BSParserException( "'global' is invalid in this context" );
            }

            StringBuilder sb = new StringBuilder();
            ReadWhitespaceAndNewLine();

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
                SourcePosition basePos = CreateSourcePosition( m_CurrentPosition );
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

            ReadWhitespaceAndNewLine();
            int posOffset = m_CurrentPosition + 1;
            string block = ParseBlock();

            block = blockModifier?.Invoke( block ) ?? block;

            BSParser p = new BSParser( block, m_OffsetSource, m_SourcePositionOffset + posOffset );

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

        public BSIfExpression ParseIfExpression( int pos, Func < string, string > blockModifier = null )
        {
            string wordName;

            Dictionary < BSExpression, BSExpression[] > cMap =
                new Dictionary < BSExpression, BSExpression[] >();

            BSExpression[] elseBlock = null;

            (BSExpression, BSExpression[]) a = ParseConditionalBlock();
            cMap.Add( a.Item1, a.Item2 );
            int resetIndex = ReadWhitespaceAndNewLine();

            if ( Current != '\0' && IsWordStart() )
            {
                wordName = GetNextWord();

                while ( wordName == "else" )
                {
                    ReadWhitespaceAndNewLine();

                    if ( Is( '{' ) )
                    {
                        string block = ParseBlock();
                        block = blockModifier?.Invoke( block ) ?? block;

                        BSParser p = new BSParser( block );

                        elseBlock = p.ParseToEnd();

                        break;
                    }

                    resetIndex = ReadWhitespaceAndNewLine();
                    wordName = GetNextWord();
                    ReadWhitespaceAndNewLine();

                    if ( wordName == "if" )
                    {
                        (BSExpression, BSExpression[]) sA = ParseConditionalBlock( blockModifier );
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

        public BSNamespaceExpression ParseNamespace()
        {
            SourcePosition sp = CreateSourcePosition( m_CurrentPosition );

            ReadWhitespaceAndNewLine();
            List < string > fn = new();
            bool restart = true;

            while ( restart )
            {
                ReadWhitespaceAndNewLine();
                fn.Add( GetNextWord() );
                ReadWhitespaceAndNewLine();

                if ( Is( '{' ) )
                {
                    restart = false;
                }

                if ( Is( '.' ) )
                {
                    restart = true;
                    m_CurrentPosition++;
                }
            }

            ReadWhitespaceAndNewLine();
            int off = m_CurrentPosition + 1;
            string block = ParseBlock();
            BSParser p = new BSParser( block, m_OffsetSource, m_SourcePositionOffset + off );
            BSExpression[] exprs = p.ParseToEnd();

            return new BSNamespaceExpression( sp, fn.ToArray(), exprs );
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

            if ( sb[sb.Length - 1] == '.' )
            {
                m_CurrentPosition--;           //Rewind to the dot we accidentially added.
                sb.Remove( sb.Length - 1, 1 ); //Remove the dot from the number
            }

            if ( sb.Length == 0 )
            {
                throw new BSParserException( "Can not parse Number", this );
            }

            return new BSValueExpression( CreateSourcePosition( pos ), negative * decimal.Parse( sb.ToString() ) );
        }

        public BSExpression ParseString( bool isFormatString = false )
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
            List < BSExpression > exprs = new List < BSExpression >();

            while ( !IsStringQuotes() && !IsNewLine() )
            {
                if ( isFormatString && Is( '{' ) )
                {
                    IncrementPosition();

                    if ( !Is( '{' ) )
                    {
                        BSExpression expr = Parse( int.MaxValue );

                        if ( !Is( '}' ) )
                        {
                            throw new BSParserException( "Expected '}'", this );
                        }

                        sb.Append( $"{{{exprs.Count}}}" );

                        exprs.Add( expr );
                    }
                    else
                    {
                        sb.Append( "{" );
                    }
                }
                else if ( m_OriginalSource[m_CurrentPosition] == '\\' )
                {
                    isEscaped = true;
                }
                else if ( isFormatString && Is( '}' ) && Is( 1, '}' ) )
                {
                    IncrementPosition();
                    sb.Append( '}' );
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

            if ( exprs.Count != 0 )
            {
                return new BSFormattedStringExpression( CreateSourcePosition( pos ), str, exprs.ToArray() );
            }

            return new BSValueExpression( CreateSourcePosition( pos ), str );
        }

        public BSExpression[] ParseToEnd()
        {
#if !DEBUG
            try
            {
#endif
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

#if !DEBUG
            }
            catch ( Exception e )
            {
                throw
                    new ParserException( e )
                    ;
            }
#endif
        }

        public BSTryExpression ParseTryExpression( int pos )
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

        public BSUsingExpression ParseUsing()
        {
            SourcePosition sp = CreateSourcePosition( m_CurrentPosition );

            ReadWhitespaceAndNewLine();
            List < string > fn = new();
            bool restart = true;

            while ( restart )
            {
                ReadWhitespaceAndNewLine();
                fn.Add( GetNextWord() );
                ReadWhitespaceAndNewLine();

                if ( Is( '.' ) )
                {
                    restart = true;
                    m_CurrentPosition++;
                }
                else
                {
                    restart = false;
                }
            }

            return new BSUsingExpression( sp, fn.ToArray() );
        }

        public BSExpression ParseValue()
        {
            int pos = m_CurrentPosition;
            ReadWhitespaceAndNewLine();

            if ( Is( '$' ) )
            {
                m_CurrentPosition++;

                if ( !IsStringQuotes() )
                {
                    m_CurrentPosition--;
                }
                else
                {
                    return ParseString( true );
                }
            }

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

                    if ( equal != "=" )
                    {
                        throw new BSParserException( $"Expected '=' after Property Name '{key}'" );
                    }

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

                        if ( equal != "=" )
                        {
                            throw new BSParserException( $"Expected '=' after Property Name '{key}'" );
                        }

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
                return ParseFunction( pos, isGlobal );
            }

            if ( wordName == "class" )
            {
                return ParseClass( isGlobal );
            }

            if ( isGlobal )
            {
                throw new BSParserException( "Expected 'function', 'class' or 'enumerable' after 'global'", this );
            }

            if ( wordName == "namespace" )
            {
                return ParseNamespace();
            }

            if ( wordName == "using" )
            {
                return ParseUsing();
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
                return ParseIfExpression( pos );
            }

            if ( wordName == "try" )
            {
                return ParseTryExpression( pos );
            }

            if ( wordName == "foreach" )
            {
                return ParseForeachExpression( pos );
            }

            if ( wordName == "while" )
            {
                (BSExpression, BSExpression[]) a = ParseConditionalBlock();

                return new BSWhileExpression( CreateSourcePosition( pos ), a.Item1, a.Item2 );
            }

            if ( wordName == "for" )
            {
                return ParseForExpression( pos );
            }

            if ( wordName == "null" && left == null )
            {
                return new BSValueExpression( CreateSourcePosition( pos ), null );
            }

            if ( wordName == "true" && left == null )
            {
                SourcePosition po = CreateSourcePosition( pos );

                return new BSValueExpression( po, BSObject.CreateTrue( po ) );
            }

            if ( wordName == "false" && left == null )
            {
                SourcePosition po = CreateSourcePosition( pos );

                return new BSValueExpression( po, BSObject.CreateFalse( po ) );
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

        public void SetPosition( int pos )
        {
            m_CurrentPosition = pos;
        }

        #endregion

        #region Private

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
