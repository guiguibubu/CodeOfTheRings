using System.Text;
using System.Diagnostics;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    private const int A_INT_VALUE = (int)'A';
    private const int Z_INT_VALUE = (int)'Z';
    private const int SPACE_INT_VALUE_HAUT = Z_INT_VALUE + 1;
    private const int SPACE_INT_VALUE_BAS = A_INT_VALUE - 1;

    private const char ACTION_TAKE = '.';
    private const char ACTION_MOVE_NEXT = '>';
    private const char ACTION_MOVE_PREV = '<';
    private const char ACTION_LETTER_NEXT = '+';
    private const char ACTION_LETTER_PREV = '-';

    private const int NB_ZONES = 30;

    static void Main(string[] args)
    {
        string? debuggerEnvVar = Environment.GetEnvironmentVariable("DEBUGGER");
        if (debuggerEnvVar != null)
            Debugger.Launch();

        try
        {
            string magicPhrase = Console.ReadLine()!;
            StringBuilder actionCommands = new StringBuilder();

            Queue<char> remainingMagicPhrase = new Queue<char>(magicPhrase);

            Foret foret = new Foret(NB_ZONES);
            NbActionAnalyzer nbActionAnalyzer = new NbActionAnalyzer(foret, remainingMagicPhrase);

            while (remainingMagicPhrase.Any())
            {
                Console.Error.Write($"pos='{foret.CurrentIndex}'");
                Console.Error.Write(";");
                Console.Error.Write($"F_S={foret}");
                Console.Error.Write(";");

                List<ActionSequence> actionSequences = nbActionAnalyzer.GetListeMinimumActionToDo();
                foret.Update(actionSequences);

                actionSequences.Add(GetActionTake());

                // Transorm action to string
                foreach (ActionSequence actionSequence in actionSequences)
                {
                    string actionSequenceStr = actionSequence.ToString();
                    Console.Error.Write(actionSequenceStr);
                    actionCommands.Append(actionSequenceStr);
                }

                Console.Error.WriteLine();
            }
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.Error.WriteLine(actionCommands.ToString());
            Console.WriteLine(actionCommands.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw;
        }
    }

    private static ActionSequence GetActionLetter(char source, char target)
    {
        int nbPlus = DistanceLettersAscendant(source, target);
        int nbMoins = DistanceLettersDescendant(source, target);

        char action = (nbPlus <= nbMoins) ? ACTION_LETTER_NEXT : ACTION_LETTER_PREV;
        int nbAction = Math.Min(nbPlus, nbMoins);

        if (nbAction < 0)
            throw new ApplicationException($"Error GetActionLetter nbAction < 0 {nbAction}");
        return new ActionSequence(action, nbAction);
    }

    private static ActionSequence GetActionTake()
    {
        return new ActionSequence(ACTION_TAKE, 1);
    }

    private static ActionSequence GetActionMove(int source, int target)
    {
        char action = (target >= source) ? ACTION_MOVE_NEXT : ACTION_MOVE_PREV;
        return new ActionSequence(action, DistanceInt(source, target));
    }

    private static int DistanceInt(int a, int b)
    {
        return Math.Abs(a - b);
    }

    private static int DistanceLettersAscendant(char source, char target)
    {
        int sourceIntValue = (source == ' ') ? SPACE_INT_VALUE_BAS : (int)source;
        int targetIntValue = (target == ' ') ? SPACE_INT_VALUE_HAUT : (int)target;

        bool lettreOrdreAscendant = targetIntValue >= sourceIntValue;

        if (lettreOrdreAscendant)
            return DistanceInt(sourceIntValue, targetIntValue);
        else
            return DistanceInt(sourceIntValue, SPACE_INT_VALUE_HAUT) + DistanceInt(SPACE_INT_VALUE_BAS, targetIntValue);
    }

    private static int DistanceLettersDescendant(char source, char target)
    {
        int sourceIntValue = (source == ' ') ? SPACE_INT_VALUE_BAS : (int)source;
        int targetIntValue = (target == ' ') ? SPACE_INT_VALUE_HAUT : (int)target;

        bool lettreOrdreDescendant = targetIntValue <= sourceIntValue;

        if (lettreOrdreDescendant)
            return DistanceInt(sourceIntValue, targetIntValue);
        else
            return DistanceInt(sourceIntValue, SPACE_INT_VALUE_BAS) + DistanceInt(SPACE_INT_VALUE_HAUT, targetIntValue);
    }

    class ActionSequence
    {
        public char ActionName { get; }
        public int ActionCount { get; }

        public ActionSequence(char action, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Must be positive");
            ActionName = action;
            ActionCount = count;
        }

        public override string ToString()
        {
            return new string(ActionName, ActionCount);
        }
    }

    class NbActionAnalyzer
    {
        private readonly Foret _foret;
        private readonly Queue<char> _remainingMagicPhrase;

        private int nbLetterConsumned;

        private List<ActionSequence> _listeMinimumActionToDo { get; set; }

        public NbActionAnalyzer(Foret foret, Queue<char> remainingMagicPhrase)
        {
            _foret = foret;
            _remainingMagicPhrase = remainingMagicPhrase;
            _listeMinimumActionToDo = new List<ActionSequence>();
            nbLetterConsumned = 0;
        }

        private void Update(char letter)
        {
            int minTotalAction = int.MaxValue;
            for (int i = 0; i < _foret.Size; i++)
            {
                ActionSequence actionMove = GetActionMove(_foret.CurrentIndex, i);
                ActionSequence actionLetter = GetActionLetter(_foret[i], letter);

                List<ActionSequence> listeAction = new List<ActionSequence>();
                if (actionMove.ActionCount > 0)
                    listeAction.Add(actionMove);
                if(actionLetter.ActionCount > 0)
                    listeAction.Add(actionLetter);

                int totalAction = listeAction.Sum(action => action.ActionCount);
                if (totalAction < minTotalAction)
                {
                    minTotalAction = totalAction;
                    _listeMinimumActionToDo = listeAction;
                }
            }
        }

        public List<ActionSequence> GetListeMinimumActionToDo()
        {
            _listeMinimumActionToDo.Clear();

            Update(_remainingMagicPhrase.Dequeue());
            return _listeMinimumActionToDo;
        }
    }

    class Foret
    {
        public readonly int Size;
        private readonly char[] STATE;

        public int CurrentIndex { get; private set; }
        public char CurrentLetter { get { return this[CurrentIndex]; } }

        public Foret(int size)
        {
            CurrentIndex = 0;
            Size = size;
            STATE = new string(' ', size).ToArray();
        }

        public char this[int index]
        {
            get { return STATE[index]; }
        }

        public void Update(IEnumerable<ActionSequence> actionSequences)
        {
            foreach(ActionSequence actionSequence in actionSequences)
                Update(actionSequence);
        }

        public void Update(ActionSequence actionSequence)
        {
            string commands = actionSequence.ToString();

            foreach (char command in commands)
            {
                switch (command)
                {
                    case ACTION_MOVE_NEXT:
                        MoveNext(); break;
                    case ACTION_MOVE_PREV:
                        MovePrevious(); break;
                    case ACTION_LETTER_NEXT:
                        MoveLetterNext(); break;
                    case ACTION_LETTER_PREV:
                        MoveLetterPrevious(); break;
                    default:
                        throw new ApplicationException($"Unknown action '{command}'");
                }
            }
        }

        private void MoveNext()
        {
            if (CurrentIndex >= Size - 1)
                CurrentIndex = 0;
            else
                CurrentIndex++;
        }

        private void MovePrevious()
        {
            if (CurrentIndex <= 0)
                CurrentIndex = Size - 1;
            else
                CurrentIndex--;
        }

        private void MoveLetterNext()
        {
            char currentLetter = STATE[CurrentIndex];
            char newLetter;
            bool isSpace = currentLetter == ' ';
            bool isLastLetter = currentLetter == 'Z';
            if (isSpace)
                newLetter = 'A';
            else if (isLastLetter)
                newLetter = ' ';
            else
                newLetter = (char)(((int)currentLetter) + 1);

            STATE[CurrentIndex] = newLetter;
        }

        private void MoveLetterPrevious()
        {
            char currentLetter = STATE[CurrentIndex];
            char newLetter;
            bool isSpace = currentLetter == ' ';
            bool isFirstLetter = currentLetter == 'A';
            if (isSpace)
                newLetter = 'Z';
            else if (isFirstLetter)
                newLetter = ' ';
            else
                newLetter = (char)(((int)currentLetter) - 1);

            STATE[CurrentIndex] = newLetter;
        }

        public override string ToString()
        {
            return $"[{string.Join(',', STATE)}]";
        }
    }
}