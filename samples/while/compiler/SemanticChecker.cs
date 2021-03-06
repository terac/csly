﻿using System;
using System.Collections.Generic;
using System.Text;
using csly.whileLang.model;

namespace csly.whileLang.compiler
{
    public class SemanticChecker
    {

        private ExpressionTyper expressionTyper;

        public CompilerContext SemanticCheck(WhileAST ast)
        {
            expressionTyper = new ExpressionTyper();
            return SemanticCheck(ast, new CompilerContext());
        }

        private CompilerContext SemanticCheck(WhileAST ast, CompilerContext context)
        {
            if (ast is AssignStatement assign)
            {
                SemanticCheck(assign, context);
            }
            if (ast is SequenceStatement seq)
            {
                SemanticCheck(seq, context);
            }
            if (ast is SkipStatement skip)
            {
                //SemanticCheck(skip, context);
            }
            if (ast is PrintStatement print)
            {
                SemanticCheck(print, context);
            }
            if (ast is ReturnStatement ret)
            {
                SemanticCheck(ret, context);
            }
            if (ast is IfStatement si)
            {
                SemanticCheck(si, context);
            }
            if (ast is WhileStatement tantque)
            {
                SemanticCheck(tantque, context);
            }
            return context;
        }

        private void SemanticCheck(AssignStatement ast, CompilerContext context)
        {
            WhileType valType = expressionTyper.TypeExpression(ast.Value, context);
            bool varExists = context.VariableExists(ast.VariableName);
            //ast.CompilerScope = context.CurrentScope;
            if (varExists)
            {
                WhileType varType = context.GetVariableType(ast.VariableName);
                if (varType != valType)
                {
                    throw new TypingException($"bad assignment : {valType} expecting {varType}");
                }
            }
            else
            {
                bool creation = context.SetVariableType(ast.VariableName, valType);
                ast.IsVariableCreation = creation;
                ast.CompilerScope = context.CurrentScope;
            }
        }

        private void SemanticCheck(PrintStatement ast, CompilerContext context)
        {
            WhileType val = expressionTyper.TypeExpression(ast.Value, context);
        }

        private void SemanticCheck(ReturnStatement ast, CompilerContext context)
        {
            WhileType valType = expressionTyper.TypeExpression(ast.Value, context);
            if (valType != WhileType.INT)
            {
                throw new TypingException($"bad return type : {valType} expecting INT");
            }
        }

        private void SemanticCheck(SequenceStatement ast, CompilerContext context)
        {
            context.OpenNewScope();
            ast.CompilerScope = context.CurrentScope;
            for (int i = 0; i < ast.Count; i++)
            {
                Statement stmt = ast.Get(i);
                SemanticCheck(stmt, context);
            }
            context.CloseScope();
        }

        private void SemanticCheck(IfStatement ast, CompilerContext context)
        {
            WhileType val = expressionTyper.TypeExpression(ast.Condition, context);
            if (val != WhileType.BOOL)
            {
                throw new SignatureException($"invalid condition type {ast.Condition.Dump("")} at {ast.Position.ToString()}");
            }
            ast.CompilerScope = context.CurrentScope;

            context.OpenNewScope();
            SemanticCheck(ast.ThenStmt);
            context.CloseScope();

            context.OpenNewScope();
            SemanticCheck(ast.ElseStmt);
            context.CloseScope();
        }

        private void SemanticCheck(WhileStatement ast, CompilerContext context)
        {
            WhileType cond = expressionTyper.TypeExpression(ast.Condition, context);
            if (cond != WhileType.BOOL)
            {
                throw new SignatureException($"invalid condition type {ast.Condition.Dump("")} at {ast.Position.ToString()}");
            }
            ast.CompilerScope = context.CurrentScope;

            context.OpenNewScope();
            SemanticCheck(ast.BlockStmt,context);
            context.CloseScope();
        }




    }
}
