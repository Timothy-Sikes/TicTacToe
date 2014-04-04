using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public static class MaxMin
    {
        public static void orderChildren(bool iAmX, List<Node> children)
        {
            children = children.OrderBy(x => x.getHeuristic()).ToList();
        }
    }
}
