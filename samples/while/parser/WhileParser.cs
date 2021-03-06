﻿using System;
using System.Collections.Generic;
using System.Linq;
using sly.lexer;
using sly.parser.generator;
using csly.whileLang.model;

namespace csly.whileLang.parser
{
    public class WhileParser
    {

        #region statements

        [Production("statement :  LPAREN [d] statement RPAREN [d]")]
        public WhileAST block( Statement statement)
        {
            return statement;
        }

        [Production("statement : sequence")]
        public WhileAST statementSequence(WhileAST sequence)
        {
            return sequence;
        }

        [Production("sequence : statementPrim additionalStatements*")]
        public WhileAST sequenceStatements(WhileAST first, List<WhileAST> next)
        {
            SequenceStatement seq = new SequenceStatement(first as Statement);
            seq.AddRange(next.Cast<Statement>().ToList<Statement>());
            return seq;
        }

        [Production("additionalStatements : SEMICOLON [d] statementPrim")]
        public WhileAST additional(WhileAST statement)
        {
            return statement;
        }

        [Production("statementPrim: IF [d] WhileParser_expressions THEN [d] statement ELSE [d] statement")]
        public WhileAST ifStmt(WhileAST cond, WhileAST thenStmt, Statement elseStmt)
        {
            IfStatement stmt = new IfStatement(cond as Expression, thenStmt as Statement, elseStmt);
            return stmt;
        }

        [Production("statementPrim: WHILE [d] WhileParser_expressions DO [d] statement")]
        public WhileAST whileStmt(WhileAST cond,  WhileAST blockStmt)
        {
            WhileStatement stmt = new WhileStatement(cond as Expression, blockStmt as Statement);
            return stmt;
        }

        [Production("statementPrim: IDENTIFIER ASSIGN [d] WhileParser_expressions")]
        public WhileAST assignStmt(Token<WhileToken> variable, Expression value)
        {
            AssignStatement assign = new AssignStatement(variable.StringWithoutQuotes, value);
            return assign;
        }

        [Production("statementPrim: SKIP [d]")]
        public WhileAST skipStmt()
        {
            return new SkipStatement();
        }

        [Production("statementPrim: RETURN [d] WhileParser_expressions")]
        public WhileAST returnStmt( WhileAST expression)
        {
            return new ReturnStatement(expression as Expression);
        }

        [Production("statementPrim: PRINT [d] WhileParser_expressions")]
        public WhileAST skipStmt(WhileAST expression)
        {
            return new PrintStatement(expression as Expression);
        }





        #endregion


        #region OPERANDS

        [Production("primary: INT")]
        public WhileAST PrimaryInt(Token<WhileToken> intToken)
        {
            return new IntegerConstant(intToken.IntValue);
        }

        [Production("primary: TRUE")]
        [Production("primary: FALSE")]
        public WhileAST PrimaryBool(Token<WhileToken> boolToken)
        {
            return new BoolConstant(bool.Parse(boolToken.StringWithoutQuotes));
        }

        [Production("primary: STRING")]
        public WhileAST PrimaryString(Token<WhileToken> stringToken)
        {
            return new StringConstant(stringToken.StringWithoutQuotes);
        }

        [Production("primary: IDENTIFIER")]
        public WhileAST PrimaryId(Token<WhileToken> varToken)
        {
            return new Variable(varToken.StringWithoutQuotes);
        }

        [Operand]
        [Production("operand: primary")]
        public WhileAST Operand(WhileAST prim)
        {
            return prim;
        }

        #endregion

        #region NUMERIC OPERATIONS
        [Operation((int)WhileToken.PLUS, 2, Associativity.Right, 10)]
        [Operation((int)WhileToken.MINUS, 2, Associativity.Right, 10)]
        public WhileAST binaryTermNumericExpression(WhileAST left, Token<WhileToken> operatorToken, WhileAST right)
        {
            BinaryOperator oper = BinaryOperator.ADD;

            switch (operatorToken.TokenID)
            {
                case WhileToken.PLUS:
                    {
                        oper = BinaryOperator.ADD;
                        break;
                    }
                case WhileToken.MINUS:
                    {
                        oper = BinaryOperator.SUB;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            BinaryOperation operation = new BinaryOperation(left as Expression, oper, right as Expression);
            return operation;
        }

        [Operation((int)WhileToken.TIMES, 2, Associativity.Right, 50)]
        [Operation((int)WhileToken.DIVIDE, 2, Associativity.Right, 50)]
        public WhileAST binaryFactorNumericExpression(WhileAST left, Token<WhileToken> operatorToken, WhileAST right)
        {
            BinaryOperator oper = BinaryOperator.MULTIPLY;

            switch (operatorToken.TokenID)
            {
                case WhileToken.TIMES:
                    {
                        oper = BinaryOperator.MULTIPLY;
                        break;
                    }
                case WhileToken.DIVIDE:
                    {
                        oper = BinaryOperator.DIVIDE;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            BinaryOperation operation = new BinaryOperation(left as Expression, oper, right as Expression);
            return operation;
        }

        [Operation((int)WhileToken.MINUS, 1, Associativity.Right, 100)]
        public WhileAST unaryNumericExpression(Token<WhileToken> operation, WhileAST value)
        {
            return new Neg(value as Expression);
        }


        #endregion



        #region BOOLEAN OPERATIONS

        [Operation((int)WhileToken.OR, 2, Associativity.Right, 10)]
        public WhileAST binaryOrExpression(WhileAST left, Token<WhileToken> operatorToken, WhileAST right)
        {
            BinaryOperator oper = BinaryOperator.OR;

            
            BinaryOperation operation = new BinaryOperation(left as Expression, oper, right as Expression);
            return operation;
        }

        [Operation((int)WhileToken.AND, 2, Associativity.Right, 50)]
        public WhileAST binaryAndExpression(WhileAST left, Token<WhileToken> operatorToken, WhileAST right)
        {
            BinaryOperator oper = BinaryOperator.AND;


            BinaryOperation operation = new BinaryOperation(left as Expression, oper, right as Expression);
            return operation;
        }

        [Operation((int)WhileToken.NOT, 1, Associativity.Right, 100)]
        public WhileAST binaryOrExpression(Token<WhileToken> operatorToken, WhileAST value)
        {
            
            return new Not(value as Expression);
        }
        #endregion

        #region COMPARISON OPERATIONS

        [Operation((int)WhileToken.LESSER, 2, Associativity.Right, 50)]
        [Operation((int)WhileToken.GREATER, 2, Associativity.Right, 50)]
        [Operation((int)WhileToken.EQUALS, 2, Associativity.Right, 50)]
        [Operation((int)WhileToken.DIFFERENT, 2, Associativity.Right, 50)]
        public WhileAST binaryComparisonExpression(WhileAST left, Token<WhileToken> operatorToken, WhileAST right)
        {
            BinaryOperator oper = BinaryOperator.ADD;

            switch (operatorToken.TokenID)
            {
                case WhileToken.LESSER:
                    {
                        oper = BinaryOperator.LESSER;
                        break;
                    }
                case WhileToken.GREATER:
                    {
                        oper = BinaryOperator.GREATER;
                        break;
                    }
                case WhileToken.EQUALS:
                    {
                        oper = BinaryOperator.EQUALS;
                        break;
                    }
                case WhileToken.DIFFERENT:
                    {
                        oper = BinaryOperator.DIFFERENT;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            BinaryOperation operation = new BinaryOperation(left as Expression, oper, right as Expression);
            return operation;
        }


        #endregion

        #region STRING OPERATIONS

        [Operation((int)WhileToken.CONCAT, 2, Associativity.Right, 10)]
        public WhileAST binaryStringExpression(WhileAST left, Token<WhileToken> operatorToken, WhileAST right)
        {
            BinaryOperator oper = BinaryOperator.CONCAT;            
            BinaryOperation operation = new BinaryOperation(left as Expression, oper, right as Expression);
            return operation;
        }
        #endregion











    }
}


