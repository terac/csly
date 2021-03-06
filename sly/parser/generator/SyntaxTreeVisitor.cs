using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using sly.parser.syntax;
using sly.lexer;
using System.Reflection;
using sly.parser.parser;
using static sly.parser.parser.ValueOptionConstructors;

namespace sly.parser.generator
{

    public class SyntaxVisitorResult<IN,OUT> where IN : struct
    {
        
        public Token<IN> TokenResult;

        public OUT ValueResult;
        
        public List<OUT> ValueListResult;
        
        public List<Token<IN>> TokenListResult;

        public ValueOption<OUT> OptionResult;

        private bool isTok;

        public bool IsOption => OptionResult != null;

        public bool IsToken => isTok;

        public bool Discarded => IsToken && TokenResult != null && TokenResult.Discarded;

        private bool isVal;
        public bool IsValue => isVal;

        private bool isValueList = false;

        public bool IsValueList => isValueList;
        
        private bool isTokenList = false;

        public bool IsTokenList => isTokenList;

        public bool IsNone => !IsToken && !IsValue && !IsTokenList && ! IsValueList; 

        public static SyntaxVisitorResult<IN,OUT> NewToken(Token<IN> tok)
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();
            res.TokenResult = tok;
            res.isTok = true;
            return res;
        }

        public static SyntaxVisitorResult<IN, OUT> NewValue(OUT val)
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();
            res.ValueResult = val;
            res.isVal = true;
            return res;
        }

        public static SyntaxVisitorResult<IN, OUT> NewValueList(List<OUT> values)
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();
            res.ValueListResult = values;
            res.isValueList = true;
            return res;
        }
        
        public static SyntaxVisitorResult<IN, OUT> NewTokenList(List<Token<IN>> tokens)
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();
            res.TokenListResult = tokens;
            res.isTokenList = true;
            return res;
        }

        public static SyntaxVisitorResult<IN, OUT> NewOptionSome(OUT value)
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();
            res.OptionResult = Some<OUT>(value);
            return res;
        }

        public static SyntaxVisitorResult<IN, OUT> NewOptionNone()
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();
            res.OptionResult = None<OUT>();
            return res;
        }



        public static SyntaxVisitorResult<IN, OUT> NoneResult()
        {
            SyntaxVisitorResult<IN, OUT> res = new SyntaxVisitorResult<IN, OUT>();            
            return res;
        }


    }

    public class SyntaxTreeVisitor<IN,OUT> where IN : struct
    {
        public Type ParserClass { get; set; }

        public object ParserVsisitorInstance { get; set; }

        public ParserConfiguration<IN,OUT> Configuration { get; set; }
              

        public SyntaxTreeVisitor(ParserConfiguration<IN,OUT> conf, object parserInstance)
        {
            this.ParserClass = ParserClass;
            this.Configuration = conf;
            this.ParserVsisitorInstance = parserInstance;
        }

        public OUT VisitSyntaxTree(ISyntaxNode<IN> root)
        {
            SyntaxVisitorResult<IN, OUT> result = Visit(root);
            return result.ValueResult;
        }

        protected virtual SyntaxVisitorResult<IN,OUT> Visit(ISyntaxNode<IN> n)
        {
            if (n is SyntaxLeaf<IN>)
            {
                return Visit(n as SyntaxLeaf<IN>);
            }            
            else if (n is SyntaxNode<IN>)
            {
                return Visit(n as SyntaxNode<IN>);
            }
            else
            {
                return null;
            }
        }

        private SyntaxVisitorResult<IN, OUT> Visit(SyntaxNode<IN> node)
        {
            
            
            SyntaxVisitorResult<IN, OUT> result = SyntaxVisitorResult<IN, OUT>.NoneResult();
            if (node.Visitor != null || node.IsByPassNode)
            {                
                List<object> args = new List<object>();
                int i = 0;
                foreach (ISyntaxNode<IN> n in node.Children)
                {
                    SyntaxVisitorResult<IN,OUT> v = Visit(n);


                    if (v.IsToken)
                    {
                        if (!v.Discarded)
                        {
                            args.Add(v.TokenResult);
                        }
                    }
                    else if (v.IsValue)
                    {
                        args.Add(v.ValueResult);
                    }

                    i++;
                }
                if (node.IsByPassNode)
                {   
                    result = SyntaxVisitorResult<IN, OUT>.NewValue((OUT)args[0]);                    
                }
                else
                {
                    MethodInfo method = null;
                    try
                    {
                        method = node.Visitor;
                        object t = (method.Invoke(ParserVsisitorInstance, args.ToArray()));
                        OUT res = (OUT)t;
                        result = SyntaxVisitorResult<IN, OUT>.NewValue(res);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OUTCH {e.Message} calling {node.Name} =>  {method?.Name}");
                    }
                }

            }
            
            return result;
        }

        private SyntaxVisitorResult<IN, OUT> Visit(SyntaxLeaf<IN> leaf)
        {
            return SyntaxVisitorResult<IN, OUT>.NewToken(leaf.Token);
        }
    }
}