using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;

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

    private const int NB_ZONES = 30;
    private static char[] FORREST_STATE = new string(' ', NB_ZONES).ToArray();
    private static int currentIndex = 0;
    private static StringBuilder finalPhrase = new StringBuilder();
    private static StringBuilder commands = new StringBuilder();
    private static StringBuilder playerErrorOutput = new StringBuilder();

    static void Main(string[] args)
    {
        try
        {
            string playerExePath = args[0];

            string magicPhrase = "NONONONONONONONONONONONONONONONONONONONO";

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = playerExePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            processStartInfo.Environment.Add("DEBUGGER", "");

            using (Process playerProcess = new Process())
            {
                playerProcess.StartInfo = processStartInfo;
                playerProcess.OutputDataReceived += PlayerProcess_OutputDataReceived;
                playerProcess.ErrorDataReceived += PlayerProcess_ErrorDataReceived;

                if (playerProcess.Start())
                {
                    playerProcess.StandardInput.WriteLine(magicPhrase);
                    playerProcess.BeginOutputReadLine();
                    playerProcess.BeginErrorReadLine();

                    playerProcess.WaitForExit();
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
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            //Console.Error.WriteLine(playerErrorOutput);
            Console.WriteLine(magicPhrase);
            Console.WriteLine(commands);
            Console.WriteLine(finalPhrase.ToString());
            Console.WriteLine();
            Console.WriteLine((finalPhrase.ToString() == magicPhrase) ? "SUCCESSS !!!" : "FAIL !");

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

    private static void MoveNext()
    {
        if (currentIndex >= NB_ZONES - 1)
            currentIndex = 0;
        else
            currentIndex++;
    }

    private static void MovePrevious()
    {
        if (currentIndex <= 0)
            currentIndex = NB_ZONES - 1;
        else
            currentIndex--;
    }

    private static void MoveLetterNext()
    {
        char currentLetter = FORREST_STATE[currentIndex];
        char newLetter;
        bool isSpace = currentLetter == ' ';
        bool isLastLetter = currentLetter == 'Z';
        if (isSpace)
            newLetter = 'A';
        else if (isLastLetter)
            newLetter = ' ';
        else
            newLetter = (char)(((int)currentLetter) + 1 );

        FORREST_STATE[currentIndex] = newLetter;
    }

    private static void MoveLetterPrevious()
    {
        char currentLetter = FORREST_STATE[currentIndex];
        char newLetter;
        bool isSpace = currentLetter == ' ';
        bool isFirstLetter = currentLetter == 'A';
        if (isSpace)
            newLetter = 'Z';
        else if (isFirstLetter)
            newLetter = ' ';
        else
            newLetter = (char)(((int)currentLetter) - 1);

        FORREST_STATE[currentIndex] = newLetter;
    }

    private static void Take()
    {
        finalPhrase.Append(FORREST_STATE[currentIndex]);
    }
}