using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
    private static readonly char[] FORREST_STATE = new string(' ', NB_ZONES).ToArray();

    static void Main(string[] args)
    {
        string? debuggerEnvVar = Environment.GetEnvironmentVariable("DEBUGGER");
        if (debuggerEnvVar != null)
            Debugger.Launch();

        try
        {
            Console.Error.WriteLine($"A_INT_VALUE = {A_INT_VALUE}");
            Console.Error.WriteLine($"Z_INT_VALUE = {Z_INT_VALUE}");
            Console.Error.WriteLine($"SPACE_INT_VALUE_HAUT = {SPACE_INT_VALUE_HAUT}");
            Console.Error.WriteLine($"SPACE_INT_VALUE_BAS = {SPACE_INT_VALUE_BAS}");

            string magicPhrase = Console.ReadLine()!;
            StringBuilder actionCommands = new StringBuilder();

            char[] magicPhraseArray = magicPhrase.ToArray();
            int currentPosition = 0;
            int currentLetterIndex = 0;
            foreach (char letter in magicPhraseArray)
            {
                char currentLetter = FORREST_STATE[currentPosition];
                Console.Error.Write($"l='{letter}'");
                Console.Error.Write(";");
                Console.Error.Write($"cP={currentPosition}");
                Console.Error.Write(";");
                Console.Error.Write($"cL='{currentLetter}'");
                Console.Error.Write(";");
                Console.Error.Write($"F_S=[{string.Join(',', FORREST_STATE)}]");
                Console.Error.Write(";");

                List<ActionSequence> actionSequences = new List<ActionSequence>();

                bool letterSpace = letter == ' ';
                bool currentLetterSpace = currentLetter == ' ';
                bool letterEqualCurrentLetter = letter == currentLetter;
                bool moveNext = !letterEqualCurrentLetter && (letterSpace || (!currentLetterSpace && !letterEqualCurrentLetter));
                Console.Error.Write($"mN={moveNext}");
                Console.Error.Write(";");

                // Move Action
                if (moveNext)
                {
                    actionSequences.Add(GetActionMoveNext());
                    bool endForrest = currentPosition == NB_ZONES - 1;
                    currentPosition = (!endForrest) ? currentPosition + 1 : 0;
                    currentLetter = FORREST_STATE[currentPosition];
                    currentLetterSpace = currentLetter == ' ';
                    // Reset if necessary
                    if (endForrest)
                    {
                        Console.Error.Write($"eF={endForrest}");
                        Console.Error.Write(";");
                        actionSequences.Add(GetActionReset(currentLetter));
                    }
                }

                // Select letter action
                bool hasToSelectLetter = !letterSpace && (currentLetterSpace || moveNext);
                Console.Error.Write($"sL={hasToSelectLetter}");
                Console.Error.Write(";");
                if (hasToSelectLetter)
                {
                    actionSequences.Add(GetActionLetter(letter));
                }

                actionSequences.Add(GetActionTake());

                // Save State
                if (!letterSpace)
                {
                    FORREST_STATE[currentPosition] = letter;
                }

                // Transorm action to string
                foreach (ActionSequence actionSequence in actionSequences)
                {
                    string actionSequenceStr = new string(actionSequence.ActionName, actionSequence.ActionCount);
                    Console.Error.Write(actionSequenceStr);
                    actionCommands.Append(actionSequenceStr);
                }

                currentLetterIndex++;
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

    private static ActionSequence GetActionLetter(char letter)
    {
        int charIntValue = (int)letter;
        int nbPlus = 1 + (charIntValue - A_INT_VALUE);
        int nbMoins = 1 + (Z_INT_VALUE - charIntValue);

        char action = (nbPlus <= nbMoins) ? ACTION_LETTER_NEXT : ACTION_LETTER_PREV;
        int nbAction = Math.Min(nbPlus, nbMoins);

        if (nbAction < 0)
            Console.Error.WriteLine($"Error GetActionLetter nbAction < 0 {nbAction}");
        return new ActionSequence(action, nbAction);
    }

    private static ActionSequence GetActionReset(char currentLetter)
    {
        int charIntValue = (int)currentLetter;
        int nbPlus = DistanceInt(SPACE_INT_VALUE_HAUT, charIntValue);
        int nbMoins = DistanceInt(charIntValue, SPACE_INT_VALUE_BAS);

        char action = (nbPlus <= nbMoins) ? ACTION_LETTER_NEXT : ACTION_LETTER_PREV;
        int nbAction = Math.Min(nbPlus, nbMoins);

        return new ActionSequence(action, nbAction);
    }

    private static ActionSequence GetActionTake()
    {
        return new ActionSequence(ACTION_TAKE, 1);
    }

    private static ActionSequence GetActionMoveNext()
    {
        return new ActionSequence(ACTION_MOVE_NEXT, 1);
    }

    private static int DistanceInt(int a, int b)
    {
        return Math.Abs(a - b);
    }

    private static int DistanceLetters(char a, char b)
    {
        int charAIntValue = (int)a;
        int charBIntValue = (int)b;

        return Math.Abs(charAIntValue - charBIntValue);
    }

    class ActionSequence
    {
        public char ActionName { get; }
        public int ActionCount { get; }

        public ActionSequence(char action, int count)
        {
            ActionName = action;
            ActionCount = count;
        }
    }
}