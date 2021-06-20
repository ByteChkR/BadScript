﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Value;
using BadScript.Parser.Operators;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser
{

    public class BSParser
    {

        private static readonly List < BSOperator > s_Operators =
            new List < BSOperator >
            {
                new BSRuntimeBinaryOperator(
                                            0,
                                            "+",
                                            new BSRuntimeFunction( "function +(a, b)", BSDefaultOperators.OperatorAdd )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "-",
                                            new BSRuntimeFunction(
                                                                  "function -(a, b)",
                                                                  BSDefaultOperators.OperatorMinus
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            2,
                                            "*",
                                            new BSRuntimeFunction(
                                                                  "function *(a, b)",
                                                                  BSDefaultOperators.OperatorMultiply
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            2,
                                            "/",
                                            new BSRuntimeFunction(
                                                                  "function /(a, b)",
                                                                  BSDefaultOperators.OperatorDivide
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            4,
                                            "%",
                                            new BSRuntimeFunction( "function %(a, b)", BSDefaultOperators.OperatorMod )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "==",
                                            new BSRuntimeFunction(
                                                                  "function ==(a, b)",
                                                                  BSDefaultOperators.OperatorEquality
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "!=",
                                            new BSRuntimeFunction(
                                                                  "function !=(a, b)",
                                                                  BSDefaultOperators.OperatorInEquality
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "&&",
                                            new BSRuntimeFunction( "function &&(a, b)", BSDefaultOperators.OperatorAnd )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "||",
                                            new BSRuntimeFunction( "function ||(a, b)", BSDefaultOperators.OperatorOr )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "^",
                                            new BSRuntimeFunction( "function ^(a, b)", BSDefaultOperators.OperatorXOr )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "<",
                                            new BSRuntimeFunction(
                                                                  "function <(a, b)",
                                                                  BSDefaultOperators.OperatorLessThan
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            ">",
                                            new BSRuntimeFunction(
                                                                  "function >(a, b)",
                                                                  BSDefaultOperators.OperatorGreaterThan
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            "<=",
                                            new BSRuntimeFunction(
                                                                  "function <=(a, b)",
                                                                  BSDefaultOperators.OperatorLessOrEqual
                                                                 )
                                           ),
                new BSRuntimeBinaryOperator(
                                            0,
                                            ">=",
                                            new BSRuntimeFunction(
                                                                  "function >=(a, b)",
                                                                  BSDefaultOperators.OperatorGreaterOrEqual
                                                                 )
                                           ),
                new BSAssignmentOperator(),
                new BSMemberAccessOperator(),
            };

        private static readonly Dictionary < string, BSOperator > m_PrefixOperators =
            new Dictionary < string, BSOperator >
            {
                {
                    "!", new BSRuntimeUnaryOperator(
                                                    "!",
                                                    new BSRuntimeFunction(
                                                                          "function !(a)",
                                                                          BSDefaultOperators.OperatorNot
                                                                         )
                                                   )
                }
            };

        private readonly string m_OriginalSource;
        private int m_CurrentPosition;

        private char Current =>
            m_CurrentPosition < m_OriginalSource.Length ? m_OriginalSource[m_CurrentPosition] : '\0';

        private char[] CurrentArray => m_OriginalSource.ToCharArray();

        #region Public

        public BSParser( string script )
        {
            m_OriginalSource = script;
            //StringBuilder sb = new StringBuilder();

            //char last = '\0';

            //for ( int i = 0; i < script.Length; i++ )
            //{
            //    if ( script[i] == '/' && last == '/' )
            //    {
            //        sb.Remove( sb.Length - 1, 1 );
            //        int end = script.IndexOf( '\n', i );

            //        if ( end == -1 )
            //        {
            //            end = script.Length;
            //        }

            //        sb.Append( ' ', end - i );
            //        i = end - 1;
            //    }
            //    else
            //    {
            //        sb.Append( script[i] );
            //    }

            //    last = script[i];
            //}

            //m_OriginalSource = sb.ToString();

            m_CurrentPosition = 0;
        }

        public int FindClosing( char open, char close )
        {
            int level = 1;

            while ( true )
            {
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
                     key == "," ||
                     key == "" ||
                     s_Operators.Take( start ).Any( x => x.OperatorKey == key ) )
                {
                    m_CurrentPosition -= key.Length;

                    break;
                }

                if ( key != "" || s_Operators.Take( start ).Any( x => x.OperatorKey == key ) )
                {
                    BSOperator o = s_Operators.Skip( start ).First( x => x.OperatorKey == key );

                    expr = Parse( o.Parse( expr, this ), 0 );
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
                throw new Exception( "Can not Parse Word" );
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
                BSExpression i = ParseExpression( 0 );
                ReadWhitespace();

                if ( !Is( ']' ) )
                {
                    throw new Exception();
                }

                m_CurrentPosition++;

                return new BSArrayAccessExpression( expr, i );
            }

            throw new Exception();
        }

        public (BSExpression, BSExpression[]) ParseConditionalBlock()
        {
            if ( Is( '(' ) )
            {
                m_CurrentPosition++;

                BSExpression condition = ParseExpression( 0 );
                ReadWhitespace();

                if ( !Is( ')' ) )
                {
                    throw new Exception();
                }

                m_CurrentPosition++;
                ReadWhitespaceAndNewLine();
                string block = ParseBlock();

                BSParser p = new BSParser( block );

                BSExpression[] b = p.ParseToEnd();

                return ( condition, b );
            }

            throw new Exception();
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
                 key == "," ||
                 s_Operators.Take( start ).Any( x => x.OperatorKey == key ) )
            {
                m_CurrentPosition -= key.Length;
            }
            else if ( key != "" )
            {
                BSOperator o = s_Operators.Skip( start ).First( x => x.OperatorKey == key );

                BSExpression r = Parse( o.Parse( expr, this ), 0 );

                return r;
            }

            return expr;
        }

        public BSExpression ParseFunction()
        {
            StringBuilder sb = new StringBuilder();
            ReadWhitespace();

            if ( !IsWordStart() )
            {
                throw new Exception( "Can not Parse Word" );
            }

            sb.Append( m_OriginalSource[m_CurrentPosition] );
            m_CurrentPosition++;

            while ( IsWordMiddle() )
            {
                sb.Append( m_OriginalSource[m_CurrentPosition] );
                m_CurrentPosition++;
            }

            string funcName = sb.ToString();

            ReadWhitespace();
            string[] args = ParseArgumentList();

            ReadWhitespaceAndNewLine();

            string block = ParseBlock();

            BSParser p = new BSParser( block );

            BSExpression[] b = p.ParseToEnd();

            return new BSFunctionDefinitionExpression( funcName, args, b, false );
        }

        public BSExpression ParseInvocation( BSExpression expr )
        {
            if ( Is( '(' ) )
            {
                m_CurrentPosition++;

                List < BSExpression > exprs = new List < BSExpression >();

                if ( !Is( ')' ) )
                {
                    exprs.Add( ParseExpression( 0 ) );
                    ReadWhitespace();

                    while ( Is( ',' ) )
                    {
                        m_CurrentPosition++;
                        exprs.Add( ParseExpression( 0 ) );
                        ReadWhitespace();
                    }

                    ReadWhitespace();

                    if ( !Is( ')' ) )
                    {
                        throw new Exception();
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

            throw new Exception();
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
                throw new Exception( "Can not parse Number" );
            }

            return new BSValueExpression( negative * decimal.Parse( sb.ToString() ) );
        }

        public BSExpression ParseString()
        {
            ReadWhitespace();

            if ( !IsStringQuotes() )
            {
                throw new Exception( "Can not Parse String" );
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
                    if ( m_OriginalSource[m_CurrentPosition] == 'n' )
                    {
                        sb.Append( "\n" );
                    }
                    else
                    {
                        sb.Append( m_OriginalSource[m_CurrentPosition] );
                    }

                    m_CurrentPosition++;
                    isEscaped = false;
                }
            }

            if ( !IsStringQuotes() )
            {
                throw new Exception( "Can not Parse String" );
            }

            m_CurrentPosition++; //Skip over string quotes
            string str = sb.ToString();

            return new BSValueExpression(  str );
        }

        public BSExpression[] ParseToEnd()
        {
            ReadWhitespaceAndNewLine();
            List < BSExpression > ret = new List < BSExpression >();

            while ( m_CurrentPosition < m_OriginalSource.Length )
            {
                while ( m_CurrentPosition < m_OriginalSource.Length &&
                        char.IsWhiteSpace( m_OriginalSource, m_CurrentPosition ) )
                {
                    m_CurrentPosition++;
                }

                if ( m_CurrentPosition < m_OriginalSource.Length )
                {
                    ret.Add( Parse( 0 ) );
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
                BSExpression expr = ParseExpression( 0 );
                m_CurrentPosition++;

                return expr;
            }

            if ( Is( '!' ) )
            {
                m_CurrentPosition++;

                return m_PrefixOperators["!"].Parse( ParseExpression( 0 ), this );
            }

            throw new Exception();
        }

        public BSExpression ParseWord()
        {
            return ParseWord( null );
        }

        public BSExpression ParseWord( BSExpression left )
        {
            ReadWhitespace();

            string wordName = GetNextWord();

            if ( wordName == "function" )
            {
                return ParseFunction();
            }

            if ( wordName == "return" )
            {
                return new BSReturnExpression( ParseExpression( 0 ) );
            }

            if ( wordName == "if" )
            {
                Dictionary < BSExpression, BSExpression[] > cMap =
                    new Dictionary < BSExpression, BSExpression[] >();

                BSExpression[] elseBlock = null;

                (BSExpression, BSExpression[]) a = ParseConditionalBlock();
                cMap.Add( a.Item1, a.Item2 );
                int resetIndex = ReadWhitespaceAndNewLine();
                if(Current != '\0')
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

            if ( wordName == "while" )
            {
                (BSExpression, BSExpression[]) a = ParseConditionalBlock();

                return new BSWhileExpression( a.Item1, a.Item2 );
            }

            if ( wordName == "for" )
            {
                throw new Exception( "For loops are not supported yet" );
            }

            if ( wordName == "null" && left == null )
            {
                return new BSValueExpression(null );
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

            if(ReadComment())ReadWhitespace();

                return r;
        }

        private bool ReadComment()
        {
            if ( m_OriginalSource.Length > m_CurrentPosition + 1 &&
                 m_OriginalSource[m_CurrentPosition] == '/' &&
                 m_OriginalSource[m_CurrentPosition] == '/' )
            {
                while ( Current != '\n' && Current != '\0' )
                    m_CurrentPosition++;

                m_CurrentPosition++;
                return true;
            }

            return false;
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
                ReadWhitespaceAndNewLine();
            return r;
        }

        #endregion

        #region Private

        private string GetNextWord()
        {

            if ( !IsWordStart() )
            {
                throw new Exception( "Can not Parse Word" );
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

        private string[] ParseArgumentList()
        {
            if ( !Is( '(' ) )
            {
                throw new Exception();
            }

            m_CurrentPosition++;

            List < string > args = new List < string >();

            if ( !Is( ')' ) )
            {
                args.Add( ParseArgumentName() );
                ReadWhitespace();

                while ( Is( ',' ) )
                {
                    m_CurrentPosition++;
                    args.Add( ParseArgumentName() );
                    ReadWhitespace();
                }

                ReadWhitespace();

                if ( !Is( ')' ) )
                {
                    throw new Exception();
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
                throw new Exception();
            }

            m_CurrentPosition++;

            //Find Closing
            int start = m_CurrentPosition;
            int end = FindClosing( '{', '}' );

            string s = m_OriginalSource.Substring( start, end - start );

            m_CurrentPosition = end + 1;

            return s;
        }

        #endregion

    }

}
