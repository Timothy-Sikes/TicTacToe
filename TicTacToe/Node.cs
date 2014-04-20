using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TicTacToe
{
    public class Node : Heuristic
    {
        public char[,] board; // uses 'x' and 'o' chars to represent the board.
        public int level; // refers to the depth level
        public Node parent;
        public List<Node> children;
        public bool min; // Represents whether or not the node is for min or max.
        public int alphaBeta;
        public int numGeneratedDescendants = 0;
        private bool treatAsRoot;

        public delegate bool betterDelType(int otherAlphaBeta);

        public Node()
        {
            board = new char[3, 3];
            parent = null;
            treatAsRoot = true;
        }

        public Node(char[,] currentBoard, int currentLevel)
        {
            // Instantiate a node
            board = currentBoard;
            level = currentLevel;
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

        //Delves down to a depth of "depth" to set aplha/beta values
        public void setAlphaBetas(int depth)
        {
            if (!(depth == 0 || justWon())) generateChildren();
            foreach(Node c in children)
            {
                //Once the current node becomes an unfeasible choice for its parent then we no longer
                //need to do update alpha/betas. This is the alpha beta pruning bit
                if( !parent.better(alphaBeta) ) return;

                //If this is the last layer to do stuff with then we should
                //set the alpha/beta to the heuristic value
                if(depth == 0 || justWon()) updateAlphaBeta(getHeuristic());
                //otherwise recursively call setAlphaBetas on children with a decreased value of depth
                else setAlphaBetas(depth - 1);
            }
            
        }

        public Node playerMove(int col, int row)
        {
            generateChildren();
            Node newNode = children.Where(x => x.board[col, row] == (xToMove() ? 'x' : 'o')).First();
            newNode.treatAsRoot = true;
            return newNode.computerMove(5);
        }

        public Node computerMove(int depth)
        {
            setAlphaBetas(depth);
            Node newNode =
                (from b in children
                 where b.getHeuristic() == (children.Max(x => x.getHeuristic()))
                 select b).First();
            newNode.treatAsRoot = true;
            return newNode;
        }

        //updates nodes alpha/beta and recursively updates parent as necessary.
        private void updateAlphaBeta(int val)
        {
            alphaBeta = val;

            //Recursively update ancestors
            if(parent.better(alphaBeta)) updateAlphaBeta(alphaBeta);
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
                        newChild.level -= 1;
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

            return new Node(copy, level);
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
                2700 * (justWon() ? 1 : 0)
                - 900 * (numberOfDiagonalsWithExactly(2, false) + numberOfHorizontalsWithExactly(2, false) + numberOfVerticalsWithExactly(2, false))
                + 30 * (numberOfDiagonalsWithExactly(2) + numberOfHorizontalsWithExactly(2) + numberOfVerticalsWithExactly(2));
        }

        //Check to see if the player that just moved won
        public bool justWon()
        {
            return (numberOfDiagonalsWithExactly(3) >= 1) || (numberOfVerticalsWithExactly(3) >= 1) || (numberOfHorizontalsWithExactly(3) >= 1);
        }

        public int numberOfDiagonalsWithExactly(int num, bool forLastPlayerToMove = true)
        {
            char testChar = justMovedChar();
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
            char testChar = justMovedChar();

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

            char testChar = justMovedChar();

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
        
        private char justMovedChar(bool forLastPlayerToMove = true)
        {
            char testChar;
            if (!xToMove() && forLastPlayerToMove) testChar = 'x';
            else testChar = 'o';
            return testChar;
        }

    }
}
