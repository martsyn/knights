namespace Knights;

record MoveDesc(int Piece, int Target);

static class Program
{
    static readonly int[][] Moves = [[3, 6], [7, 9], [8], [0, 5, 9], [6], [3], [0, 4], [1, 8], [2, 7], [1, 3]];

    static bool IsWin(int[] state)
    {
        return (state[0] == 4 || state[0] == 6)
               && (state[1] == 4 || state[1] == 6)
               && (state[2] == 0 || state[2] == 5)
               && (state[3] == 0 || state[3] == 5);
    }

    static int GetHash(int[] state)
    {
        return state[0] * 1000 + state[1] * 100 + state[2] * 10 + state[3];
    }

    static List<MoveDesc>? Move(int[] state, int depth, int maxDepth, HashSet<int> seen)
    {
        seen.Add(GetHash(state));
        for (int piece = 0; piece < state.Length; piece++)
        {
            int current = state[piece];
            for (int moveIndex = 0; moveIndex < Moves[current].Length; moveIndex++)
            {
                int target = Moves[state[piece]][moveIndex];
                bool conflict = false;
                for (int other = 0; other < state.Length; other++)
                {
                    if (piece == other) continue;
                    if (state[other] == target)
                    {
//                        Console.WriteLine($"Conflict {piece} from {current} -> {target} conflicts with {other}");
                        conflict = true;
                        break;
                    }
                }

                if (conflict)
                    continue;

                state[piece] = target;
                int hash = GetHash(state);
                if (!seen.Contains(hash))
                {
//                    Console.WriteLine($"{new string(' ', depth)} Move {piece} from {current} -> {target} state: {hash:D4} (depth={depth})");
                    if (IsWin(state)) return [new MoveDesc(piece, target)];
                    if (depth < maxDepth)
                    {
                        var res = Move(state, depth + 1, maxDepth, seen);
                        if (res != null)
                        {
                            res.Add(new MoveDesc(piece, target));
                            return res;
                        }
                    }
                }
                // else
                // {
                //     Console.WriteLine($"Seen {hash:D4}");   
                // }

                state[piece] = current;
            }
        }

        return null;
    }

    static void Main()
    {
        int[] initialState = [0, 5, 4, 6];
        for (int maxDepth = 5; maxDepth < 300; maxDepth++)
        {
            int[] state = (int[]) initialState.Clone();
            Console.WriteLine($"----------------------------------------\nDepth {maxDepth}");
            var seen = new HashSet<int>();
            var res = Move(state, 0, maxDepth, seen);
            if (res != null)
            {
                Console.WriteLine($"Solution ({res.Count} moves):");
                res.Reverse();
                state = (int[]) initialState.Clone();
                Console.WriteLine($"Initial state: {GetHash(state):D4}");
                foreach (var move in res)
                {
                    int current = state[move.Piece];
                    state[move.Piece] = move.Target;
                    Console.WriteLine($"{move.Piece}: {current} -> {move.Target} state: {GetHash(state):D4}");
                }

                break;
            }

            Console.WriteLine($"No solution, tried={seen.Count}");
        }
    }
}