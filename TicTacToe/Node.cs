using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TicTacToe
{
    public class Node : Heuristic
    {
        private char[][] board; // uses 'x' and 'o' chars to represent the board.
        public int level; // refers to the depth level
        public List<Node> children;
        public bool min; // Represents whether or not the node is for min or max.

        public Node(char[][] currentBoard, int currentLevel)
        {
            // Instantiate a node
            board = currentBoard;
            level = currentLevel;
            generateChildren();
        }

        public void generateChildren()
        {
            // generates all the possible children of this node.
        }

        public int getHeuristic()
        {
            return 0;
        }
 
    }
}
