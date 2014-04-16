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
        public List<Node> children;
        public bool min; // Represents whether or not the node is for min or max.

        public Node(char[,] currentBoard, int currentLevel)
        {
            // Instantiate a node
            board = currentBoard;
            level = currentLevel;
            generateChildren();
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
