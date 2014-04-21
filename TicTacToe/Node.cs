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
        public int? alphaBeta;
        public int numGeneratedDescendants = 0;
        private bool treatAsRoot;

        public delegate bool betterDelType(int? otherAlphaBeta);
        public static bool debugging = false;
        public static int nodesVisited = 0;

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
                betterDelType b;
                if (xToMove()) b = x => (alphaBeta == null)? true : x > alphaBeta;
                else b = x => (alphaBeta == null) ? true : x < alphaBeta;
                return b;
            }
        }

        void debugDisplayNode()
        {
            if (debugging)
            {
                Debug.WriteLine("------------------");
                for (int col = 0; col < 3; col++)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        Debug.Write(board[row, col]);
                    }
                    Debug.WriteLine("");
                }
                Debug.WriteLine("Value: " + alphaBeta);
                Debug.WriteLine("**************************");
            }
        }

        //Delves down to a depth of "depth" to set aplha/beta values
        public void setAlphaBetas(int depth)
        {
            if (!(depth == 0 || gameOver()))
            {
                generateChildren();
                foreach(Node c in children)
                {
                    //Once the current node becomes an unfeasible choice for its parent then we no longer
                    //need to do update alpha/betas. This is the alpha beta pruning bit
                    //if( !parent.better(alphaBeta) ) return;

                    //If this is the last layer to do stuff with then we should
                    //set the alpha/beta to the heuristic value
                    if(depth == 1 || gameOver()) c.updateMinMax(c.getHeuristic());
                    //otherwise recursively call setAlphaBetas on children with a decreased value of depth
                    else c.setAlphaBetas(depth - 1);
                }
                alphaBeta = selectBestChild().alphaBeta;
            }
            else updateMinMax(getHeuristic());
            
        }

        public Node playerMove(int col, int row, int depth = 9)
        {
            nodesVisited = 0;
            generateChildren();
            Node newNode = children.Where(x => x.board[col, row] == (xToMove() ? 'x' : 'o')).First();
            newNode.treatAsRoot = true;
            if (newNode.gameOver()) return newNode;

            return newNode.computerMove(depth);
        }

        public Node computerMove(int depth)
        {
            setAlphaBetas(depth);
            Node newNode = selectBestChild();
            newNode.treatAsRoot = true;
            showOptimalPath();
            return newNode;
        }

        void showOptimalPath()
        {
            Node currentNode = this;
            debugDisplayNode();
            while(currentNode.children != null)
            {
                currentNode = currentNode.selectBestChild();
                currentNode.debugDisplayNode();
            }
        }

        private Node selectBestChild()
        {
            Node newNode;
            if (!xToMove())
            {
                newNode =
                    (from b in children
                     where b.alphaBeta == (children.Min(x => x.alphaBeta))
                     select b).First();
            }
            else
            {
                newNode =
                    (from b in children
                     where b.alphaBeta == (children.Max(x => x.alphaBeta))
                     select b).First();
            }
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
        private void updateMinMax(int val)
        {
            alphaBeta = val;
            nodesVisited++;
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
                        //newChild.alphaBeta = -1;
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
                2700 * (xWon() ? 1 : 0) - 2700 * (oWon() ? 1 : 0)
                + 900 * numberOfWinDirectionsWithExactly(2, true) - 900 * numberOfWinDirectionsWithExactly(2, false);
        }

        bool xWon()
        {
            return numberOfWinDirectionsWithExactly(3, true) > 0;
        }

        bool oWon()
        {
            return numberOfWinDirectionsWithExactly(3, false) > 0;
        }

        public int numberOfWinDirectionsWithExactly(int num, bool playerX)
        {
            int returnVal = 0;
            bool lastPlayerToMove = playerX && !xToMove() || !playerX && xToMove();
            returnVal = returnVal + numberOfDiagonalsWithExactly(num, lastPlayerToMove) +
                numberOfHorizontalsWithExactly(num, lastPlayerToMove) + numberOfVerticalsWithExactly(num, lastPlayerToMove);

            return returnVal;
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
                else if ( !spaceUnused(i, i) ) inARow1--;
                if (board[i, 2 - i] == testChar) inARow2++;
                else if (!spaceUnused(i, 2 - i)) inARow2--;
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
                    else if (!spaceUnused(i, j)) inARow--;
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
                    else if (!spaceUnused(j, i)) inARow--;
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
