using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Engine
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
    private const char ACTION_BOUCLE_BEGIN = '[';
    private const char ACTION_BOUCLE_END = ']';

    private const int NB_ZONES = 30;
    private const int NB_MAX_ACTION = 4000;
    private static readonly double NB_MAX_MILLISEC = TimeSpan.FromSeconds(2).TotalMilliseconds;
    private static readonly Foret foret = new Foret(NB_ZONES);

    private static StringBuilder finalPhrase = new StringBuilder();
    private static StringBuilder commands = new StringBuilder();
    private static StringBuilder playerErrorOutput = new StringBuilder();

    static void Main(string[] args)
    {
        try
        {
            string playerExePath = args[0];

            string magicPhrase = "ABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZA";

            Stopwatch stopwatch = new Stopwatch();

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = playerExePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            //processStartInfo.Environment.Add("DEBUGGER", "");

            using (Process playerProcess = new Process())
            {
                playerProcess.StartInfo = processStartInfo;
                playerProcess.OutputDataReceived += PlayerProcess_OutputDataReceived;
                playerProcess.ErrorDataReceived += PlayerProcess_ErrorDataReceived;

                if (playerProcess.Start())
                {
                    stopwatch.Start();
                    playerProcess.StandardInput.WriteLine(magicPhrase);
                    playerProcess.BeginOutputReadLine();
                    playerProcess.BeginErrorReadLine();

                    playerProcess.WaitForExit();
                    stopwatch.Stop();
                    playerProcess.Close();
                }
            }

            foreach (char command in commands.ToString())
            {
                switch (command)
                {
                    case ACTION_TAKE:
                        Take(); break;
                    case ACTION_MOVE_NEXT:
                    case ACTION_MOVE_PREV:
                    case ACTION_LETTER_NEXT:
                    case ACTION_LETTER_PREV:
                    case ACTION_BOUCLE_BEGIN:
                    case ACTION_BOUCLE_END:
                        foret.Update(command); break;
                    default:
                        throw new ApplicationException($"Unknown action '{command}'");
                }
            }
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            //Console.Error.WriteLine(playerErrorOutput);
            Console.WriteLine(magicPhrase);
            Console.WriteLine(commands);
            Console.WriteLine(finalPhrase.ToString());
            Console.WriteLine($"Chrono (max. {NB_MAX_MILLISEC} ms) = {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"NbAction (max. {NB_MAX_ACTION}) = {commands.Length}");
            Console.WriteLine(finalPhrase.ToString());
            Console.WriteLine();
            bool successPhrase = finalPhrase.ToString() == magicPhrase;
            bool successNbAction = commands.Length <= NB_MAX_ACTION;
            bool successTemps = stopwatch.ElapsedMilliseconds <= NB_MAX_MILLISEC;
            bool success = successPhrase && successNbAction && successTemps;
            Console.WriteLine(success ? "SUCCESSS !!!" : "FAIL !");
            if (!successPhrase)
                Console.WriteLine("Mauvaise phrase a la fin");
            if (!successNbAction)
                Console.WriteLine($"Trop d'actions (max. {NB_MAX_ACTION}) : {commands.Length}");
            if (!successTemps)
                Console.WriteLine($"Trop long (max. {NB_MAX_MILLISEC} ms) : {stopwatch.ElapsedMilliseconds}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw;
        }

        Console.ReadKey();
    }

    private static void PlayerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        string? line = e.Data;
        // Collect the sort command output.
        if (!string.IsNullOrEmpty(line))
        {
            line = line.Replace(Environment.NewLine, "");
            commands.Append(line);
        }
    }

    private static void PlayerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        string? line = e.Data;
        // Collect the sort command output.
        if (!string.IsNullOrEmpty(line))
        {
            playerErrorOutput.Append(line);
        }
    }

    private static void Take()
    {
        finalPhrase.Append(foret.CurrentLetter);
    }

    class Foret
    {
        private int currentIndex = 0;
        private readonly int Size;
        private readonly char[] STATE;

        private bool isInBoucle = false;

        public char CurrentLetter { get { return STATE[currentIndex]; } }

        public Foret(int size)
        {
            Size = size;
            STATE = new string(' ', size).ToArray();
        }

        public void Update(char command)
        {
            bool skipAction = isInBoucle && CurrentLetter == ' ';
            switch (command)
            {
                case ACTION_MOVE_NEXT:
                    if(!skipAction) 
                        MoveNext(); 
                    break;
                case ACTION_MOVE_PREV:
                    if (!skipAction) 
                        MovePrevious(); 
                    break;
                case ACTION_LETTER_NEXT:
                    if (!skipAction) 
                        MoveLetterNext();
                    break;
                case ACTION_LETTER_PREV:
                    if (!skipAction)
                        MoveLetterPrevious(); 
                    break;
                case ACTION_BOUCLE_BEGIN:
                    isInBoucle = true;
                    break;
                case ACTION_BOUCLE_END:
                    isInBoucle = false;
                    break;
                default:
                    throw new ApplicationException($"Unknown action '{command}'");
            }
        }

        private void MoveNext()
        {
            if (currentIndex >= Size - 1)
                currentIndex = 0;
            else
                currentIndex++;
        }

        private void MovePrevious()
        {
            if (currentIndex <= 0)
                currentIndex = Size - 1;
            else
                currentIndex--;
        }

        private void MoveLetterNext()
        {
            char currentLetter = STATE[currentIndex];
            char newLetter;
            bool isSpace = currentLetter == ' ';
            bool isLastLetter = currentLetter == 'Z';
            if (isSpace)
                newLetter = 'A';
            else if (isLastLetter)
                newLetter = ' ';
            else
                newLetter = (char)(((int)currentLetter) + 1);

            STATE[currentIndex] = newLetter;
        }

        private void MoveLetterPrevious()
        {
            char currentLetter = STATE[currentIndex];
            char newLetter;
            bool isSpace = currentLetter == ' ';
            bool isFirstLetter = currentLetter == 'A';
            if (isSpace)
                newLetter = 'Z';
            else if (isFirstLetter)
                newLetter = ' ';
            else
                newLetter = (char)(((int)currentLetter) - 1);

            STATE[currentIndex] = newLetter;
        }

        public override string ToString()
        {
            return $"[{string.Join(',', STATE)}]";
        }
    }
}