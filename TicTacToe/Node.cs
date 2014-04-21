using System;
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

        public delegate bool betterDelType(int? otherAlphaBeta);
        public static bool debugging = false;
        public static int nodesVisited = 0; // A counter for the number of nodes visited during a particular search.
        public static int depthLimit = -1; // -1 represents no limit.

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
        }

        public Node(char[,] currentBoard)
        {
            // Instantiate a node
            board = currentBoard;
            //I assume player controlling x is trying to maximize
        }

        // returns a delegate that can be used if a given value is better than the current alpha beta value.
        private betterDelType better
        {
            // Jason's delegates
            get
            {
                betterDelType b;
                if (xToMove()) b = x => (alphaBeta == null) ? true : x > alphaBeta;
                else b = x => (alphaBeta == null) ? true : x < alphaBeta;
                return b;
            }
        }

        // Display this Node's board state and heuristic value, if debugging is enabled.
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
                    //If this is the last layer to do stuff with then we should
                    //set the alpha/beta to the heuristic value
                    if(depth == 1 || gameOver()) c.updateMinMax(c.getHeuristic());

                    //otherwise recursively call setAlphaBetas on children with a decreased value of depth
                    else c.setAlphaBetas(depth - 1);

                    //Once the current node becomes an unfeasible choice for its parent then we no longer
                    //need to do update alpha/betas. This is the alpha beta pruning bit
                    if( !(parent.better(alphaBeta) || parent.alphaBeta == alphaBeta)  ) return;
                }
                if (parent.better(alphaBeta)) parent.alphaBeta = alphaBeta;
            }
            else updateMinMax(getHeuristic());
            
        }

        // This function accepts the player's move, and then calculates the computer's response.
        //updates nodes alpha/beta and recursively updates parent as necessary.
        private void updateMinMax(int val)
        {
            alphaBeta = val;
            if (parent != null && parent.better(alphaBeta))
            {
                parent.alphaBeta = val;
            }
            nodesVisited++;
        }

        public Node playerMove(int col, int row, int depth = 9)
        {
            nodesVisited = 0;
            generateChildren();
            Node newNode = children.Where(x => x.board[col, row] == (xToMove() ? 'x' : 'o')).First();
            if (newNode.gameOver()) return newNode;

            return newNode.computerMove(depthLimit);
        }

        // This function calculates the computer's move.
        public Node computerMove(int depth)
        {
            setAlphaBetas(depth);
            Node newNode = selectBestChild();
            showOptimalPath();
            return newNode;
        }

        // This funciton displays the trace. (The best path chosen)
        void showOptimalPath()
        {
            Node currentNode = this;
            debugDisplayNode();
            while(currentNode.children != null)
            {
                currentNode = currentNode.selectBestChild();
                currentNode.debugDisplayNode();
            }
            if (debugging)
            {
                Debug.WriteLine("***************");
                Debug.WriteLine("***************");
            }
        }

        // This function selects the Min or Max child, based upon the alpha/beta/heuristic values.
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

        // This checks to see if the game is over. (Cat's game or a win)
        public bool gameOver()
        {
            bool boardFilled =
                (from space in board.Cast<char>()
                 where (space != 'x' && space != 'o') //Where space unused
                 select space).Count() == 0;
            return boardFilled || justWon();
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
                        //newChild.alphaBeta = -1;
                        children.Add(newChild);
                    }
                }
            }   
        }

        // This function checks to see if this is an empty space.
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

        // Is it X's turn?
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

        // Heuristic is inspired by lexicographical orderings
        public int getHeuristic()
        {
            return
                2700 * (xWon() ? 1 : 0) - 2700 * (oWon() ? 1 : 0)
                + 900 * numberOfWinDirectionsWithExactly(2, true) - 900 * numberOfWinDirectionsWithExactly(2, false);
        }

        // Has X won the game?
        bool xWon()
        {
            return numberOfWinDirectionsWithExactly(3, true) > 0;
        }

        // Has Y won the game?
        bool oWon()
        {
            return numberOfWinDirectionsWithExactly(3, false) > 0;
        }

        // This function checks the vertical, horizantal, and diagonal directions for a specific number (num) of
        // Xs or Os in a row (or separated by a blank space)
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

        // Checks the diagonal lines for a specific number (num) of
        // Xs or Os in a row (or separated by a blank space)
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

        // Checks the vertical lines for a specific number (num) of
        // Xs or Os in a row (or separated by a blank space)
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

        // Checks the horizontal lines for a specific number (num) of
        // Xs or Os in a row (or separated by a blank space)
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
        
        // Get the last character representation of the player that just played.
        // Pass it false to reverse the character (x to o) (o to x)
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
