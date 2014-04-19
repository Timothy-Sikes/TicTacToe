using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TicTacToe
{
    public class Node : Heuristic
    {
        private char[,] board; // uses 'x' and 'o' chars to represent the board.
        public int level; // refers to the depth level
        public Node parent;
        public List<Node> children;
        public bool min; // Represents whether or not the node is for min or max.
        public int alphaBeta;
        public delegate bool del(int otherAlphaBeta);
        del better;

        public Node(char[,] currentBoard, int currentLevel)
        {
            // Instantiate a node
            board = currentBoard;
            level = currentLevel;
            //I assume player  controlling x is trying to maximize
            if(xToMove()) better = x => x > alphaBeta;
            generateChildren();
        }

        //Delves down to a depth of "depth" to set aplha/beta values
        public void setAlphaBetas(int depth)
        {
            foreach(Node c in children)
            {
                //Once the current node becomes an unfeasible choice for its parent then we no longer
                //need to do update alpha/betas. This is the alpha beta pruning bit
                if( !parent.better(alphaBeta) ) return;

                //If this is the last layer to do stuff with then we should
                //set the alpha/beta to the heuristic value
                if(depth == 0) updateAlphaBeta(getHeuristic());
                //otherwise recursively call setAlphaBetas on children with a decreased value of depth
                else setAlphaBetas(depth - 1);
            }
            
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
                        children.Add(newChild);
                    }
                }
            }   
        }

        private bool spaceUnused(int col, int row)
        {
            return !(board[col, row] != 'x' || board[col, row] != 'o');
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

        public int getHeuristic()
        {
            return 0;
        }
 
    }
}
