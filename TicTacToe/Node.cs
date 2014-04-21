﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using System.Diagnostics;

namespace TicTacToe
{
    public class Node : Heuristic
    {
        public char[,] board; // uses 'x' and 'o' chars to represent the board.
        public Node parent;
        public List<Node> children;
        public bool min; // Represents whether or not the node is for min or max.
        public int alphaBeta;
        public int numGeneratedDescendants = 0;
        private bool treatAsRoot;

        public delegate bool betterDelType(int otherAlphaBeta);
        public bool debugging = true;

        public Node()
        {
            board = new char[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    board[i, j] = '-';
                }
            }
            parent = null;
            treatAsRoot = true;
        }

        public Node(char[,] currentBoard)
        {
            // Instantiate a node
            board = currentBoard;
            //I assume player  controlling x is trying to maximize
            
        }

        private betterDelType better
        {
            get
            {
                if (treatAsRoot) return x => true;

                betterDelType b = x => false;
                if(better == null)
                {
                    if (xToMove()) b = x => x > alphaBeta;
                    else b = x => x < alphaBeta;
                }
                return b;
            }
        }

        void debugDisplayNode()
        {
            Debug.WriteLine("------------------");
            for(int col = 0; col < 3; col++)
            {
                for(int row = 0; row < 3; row++)
                {
                    Debug.Write(board[row, col]);
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("Value: " + alphaBeta);
        }

        //Delves down to a depth of "depth" to set aplha/beta values
        public void setAlphaBetas(int depth)
        {
            if (!(depth == 0 || justWon())) generateChildren();
            foreach(Node c in children)
            {
                //Once the current node becomes an unfeasible choice for its parent then we no longer
                //need to do update alpha/betas. This is the alpha beta pruning bit
                //if( !parent.better(alphaBeta) ) return;

                //If this is the last layer to do stuff with then we should
                //set the alpha/beta to the heuristic value
                if(depth == 1 || justWon()) c.updateAlphaBeta(c.getHeuristic());
                //otherwise recursively call setAlphaBetas on children with a decreased value of depth
                else setAlphaBetas(depth - 1);
            }
            
        }

        public Node playerMove(int col, int row, int depth = 5)
        {
            
            generateChildren();
            Node newNode = children.Where(x => x.board[col, row] == (xToMove() ? 'x' : 'o')).First();
            newNode.treatAsRoot = true;
            if (newNode.gameOver()) return newNode;

            return newNode.computerMove(depth);
        }

        public Node computerMove(int depth)
        {
            setAlphaBetas(depth);
            Node newNode =
                (from b in children
                 where b.getHeuristic() == (children.Max(x => x.getHeuristic()))
                 select b).First();
            foreach(Node c in children)
            {
                c.debugDisplayNode();
            }
            newNode.treatAsRoot = true;
            return newNode;
        }

        public bool gameOver()
        {
            bool boardFilled =
                (from space in board.Cast<char>()
                 where (space != 'x' && space != 'o') //Where space unused
                 select space).Count() == 0;
            return boardFilled || justWon();
        }

        //updates nodes alpha/beta and recursively updates parent as necessary.
        private void updateAlphaBeta(int val)
        {
            alphaBeta = val;

            //Recursively update ancestors
            if (parent != null && parent.better(alphaBeta))
            {
                parent.alphaBeta = alphaBeta;
                parent.updateAlphaBeta(alphaBeta);
            }
        }

        private void generateChildren()
        {
            children = new List<Node>();
            // generates all the possible children of this node.
            for (int i = 0; i < Math.Sqrt(board.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(board.Length); j++)
                {
                    if(spaceUnused(i, j))
                    {
                        Node newChild = CreateCopy();
                        newChild.board[i, j] = newChild.xToMove() ? 'x' : 'o';
                        newChild.parent = this;
                        newChild.treatAsRoot = false;
                        children.Add(newChild);
                    }
                }
            }   
        }

        private bool spaceUnused(int col, int row)
        {
            return !(board[col, row] == 'x' || board[col, row] == 'o');
        }

        //Returns a copy of this node
        private Node CreateCopy()
        {
            //Create a copy of the current board
            char[,] copy = new char[3, 3];
            Array.Copy(board, copy, board.Length);

            return new Node(copy);
        }

        public bool xToMove()
        {
            int numX = 0;
            int numO = 0;
            foreach(char c in board)
            {
                if (c == 'x') numX++;
                else if (c == 'o') numO++;
            }

            return !(numX > numO);
        }

        //Heuristic is inspired by lexicographical orderings
        public int getHeuristic()
        {
            return
                (xToMove() ? -1 : 1) *
                (2700 * (justWon() ? 1 : 0)
                - 900 * (numberOfDiagonalsWithExactly(2, false) + numberOfHorizontalsWithExactly(2, false) + numberOfVerticalsWithExactly(2, false))
                + 30 * (numberOfDiagonalsWithExactly(2) + numberOfHorizontalsWithExactly(2) + numberOfVerticalsWithExactly(2)));
        }

        //Check to see if the player that just moved won
        public bool justWon(bool forLastPlayerToMove = true)
        {
            return (numberOfDiagonalsWithExactly(3, forLastPlayerToMove) >= 1) || 
                (numberOfVerticalsWithExactly(3, forLastPlayerToMove) >= 1) || 
                (numberOfHorizontalsWithExactly(3, forLastPlayerToMove) >= 1);
        }

        public int numberOfDiagonalsWithExactly(int num, bool forLastPlayerToMove = true)
        {
            char testChar = justMovedChar(forLastPlayerToMove);
            
            int inARow1, inARow2; inARow1 = inARow2 = 0;
            for(int i = 0; i < 3; i++)
            {
                if (board[i, i] == testChar) inARow1++;
                if (board[2 - i, 2 - i] == testChar) inARow2++;
            }
            return
                ((inARow1 == num) ? 1 : 0) +
                ((inARow2 == num) ? 1 : 0);
        }

        public int numberOfVerticalsWithExactly(int num, bool forLastPlayerToMove = true)
        {
            int returnVal = 0;
            char testChar = justMovedChar(forLastPlayerToMove);

            int inARow;
            for(int i = 0; i < 3; i++)
            {
                inARow = 0;
                for(int j = 0; j < 3; j++)
                {
                    if (board[i, j] == testChar) inARow++;
                }
                if (inARow == num) returnVal++;
            }
            return returnVal;
        }

        public int numberOfHorizontalsWithExactly(int num, bool forLastPlayerToMove = true)
        {
            int returnVal = 0;
            char testChar = justMovedChar(forLastPlayerToMove);

            int inARow;
            for (int i = 0; i < 3; i++)
            {
                inARow = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (board[j, i] == testChar) inARow++;
                }
                if (inARow == num) returnVal++;
            }
            return returnVal;
        }
        
        public char justMovedChar(bool forLastPlayerToMove = true)
        {
            bool x;
            if(!forLastPlayerToMove)
                x =  xToMove();
            char testChar;
            if ( (xToMove() && !forLastPlayerToMove) || (!xToMove() && forLastPlayerToMove) ) testChar = 'x';
            else testChar = 'o';
            return testChar;
        }

    }
}
